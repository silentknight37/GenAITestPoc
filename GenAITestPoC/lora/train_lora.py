"""
Train a LoRA adapter that teaches an open instruct model to generate the GenAI Test Framework's
test-case JSON, distilling the behaviour captured from the base (teacher) model.

Dataset (either format is accepted):
  • instruction JSONL — {"instruction": "...", "input": "...", "output": "..."}   (Alpaca-style)
  • chat JSONL        — {"messages": [{"role": "system"...}, {"role": "user"...}, {"role": "assistant"...}]}
The framework captures the chat form when OPENAI_TRAINING_DATA is set; convert_dataset.py turns
that into the instruction form documented in the thesis.

Example:
    python train_lora.py \
        --data dataset.instruction.jsonl \
        --base-model meta-llama/Llama-3.2-3B-Instruct \
        --output ./genai-lora-adapter \
        --epochs 3 \
        --report-to wandb \
        --push-to-hub --hub-model-id your-org/genai-lora
"""
import argparse
import torch
from datasets import load_dataset
from peft import LoraConfig
from transformers import AutoModelForCausalLM, AutoTokenizer, BitsAndBytesConfig
from trl import SFTConfig, SFTTrainer


def parse_args():
    p = argparse.ArgumentParser(description="Train a LoRA adapter for GenAI test generation.")
    p.add_argument("--data", required=True, help="Training JSONL (instruction or chat format).")
    p.add_argument("--base-model", default="meta-llama/Llama-3.2-3B-Instruct",
                   help="Open instruct base model from HuggingFace.")
    p.add_argument("--output", default="./genai-lora-adapter", help="Where to save the adapter.")
    p.add_argument("--epochs", type=float, default=3.0)
    p.add_argument("--batch-size", type=int, default=1)
    p.add_argument("--grad-accum", type=int, default=8)
    p.add_argument("--lr", type=float, default=2e-4)
    p.add_argument("--max-seq-len", type=int, default=4096)
    p.add_argument("--val-frac", type=float, default=0.1,
                   help="Fraction held out for validation (reported as eval loss).")
    p.add_argument("--seed", type=int, default=42)
    p.add_argument("--lora-r", type=int, default=16)
    p.add_argument("--lora-alpha", type=int, default=32)
    p.add_argument("--no-4bit", action="store_true", help="Disable 4-bit (QLoRA) loading.")
    # Experiment tracking (Weights & Biases / MLflow)
    p.add_argument("--report-to", default="none", choices=["none", "wandb", "mlflow", "tensorboard"],
                   help="Experiment tracker for training metrics.")
    p.add_argument("--run-name", default="genai-lora", help="Run name for the experiment tracker.")
    # Model storage on the HuggingFace Hub
    p.add_argument("--push-to-hub", action="store_true", help="Push the adapter to the HuggingFace Hub.")
    p.add_argument("--hub-model-id", default=None, help="Hub repo id, e.g. your-org/genai-lora.")
    return p.parse_args()


def to_messages(example):
    """Normalise either dataset format into a chat message list."""
    msgs = example.get("messages")
    if msgs:
        return msgs
    instruction = (example.get("instruction") or "").strip()
    user_input  = (example.get("input") or "").strip()
    output      = (example.get("output") or "").strip()
    user = instruction + (f"\n\n{user_input}" if user_input else "")
    return [
        {"role": "user", "content": user},
        {"role": "assistant", "content": output},
    ]


def main():
    args = parse_args()

    tokenizer = AutoTokenizer.from_pretrained(args.base_model)
    if tokenizer.pad_token is None:
        tokenizer.pad_token = tokenizer.eos_token

    quant = None
    if not args.no_4bit:
        quant = BitsAndBytesConfig(
            load_in_4bit=True,
            bnb_4bit_quant_type="nf4",
            bnb_4bit_compute_dtype=torch.bfloat16,
            bnb_4bit_use_double_quant=True,
        )

    model = AutoModelForCausalLM.from_pretrained(
        args.base_model,
        quantization_config=quant,
        torch_dtype=torch.bfloat16,
        device_map="auto",
    )

    raw = load_dataset("json", data_files=args.data, split="train")
    print(f"Loaded {len(raw)} training examples from {args.data}")

    # Render each example to a single training string via the model's chat template.
    def render(example):
        return {"text": tokenizer.apply_chat_template(to_messages(example), tokenize=False)}

    dataset = raw.map(render, remove_columns=raw.column_names)

    # Deterministic held-out split so the thesis can report eval loss on unseen examples.
    split = dataset.train_test_split(test_size=args.val_frac, seed=args.seed)
    train_dataset, eval_dataset = split["train"], split["test"]
    print(f"Train: {len(train_dataset)}   Val: {len(eval_dataset)}")

    lora = LoraConfig(
        r=args.lora_r,
        lora_alpha=args.lora_alpha,
        lora_dropout=0.05,
        bias="none",
        task_type="CAUSAL_LM",
        target_modules=["q_proj", "k_proj", "v_proj", "o_proj",
                        "gate_proj", "up_proj", "down_proj"],
    )

    sft_config = SFTConfig(
        output_dir=args.output,
        num_train_epochs=args.epochs,
        per_device_train_batch_size=args.batch_size,
        gradient_accumulation_steps=args.grad_accum,
        learning_rate=args.lr,
        max_seq_length=args.max_seq_len,
        dataset_text_field="text",
        logging_steps=10,
        save_strategy="epoch",
        eval_strategy="epoch",
        seed=args.seed,
        bf16=True,
        gradient_checkpointing=True,
        packing=False,
        report_to=args.report_to,
        run_name=args.run_name,
        push_to_hub=args.push_to_hub,
        hub_model_id=args.hub_model_id,
    )

    trainer = SFTTrainer(
        model=model,
        args=sft_config,
        train_dataset=train_dataset,
        eval_dataset=eval_dataset,
        peft_config=lora,
        tokenizer=tokenizer,
    )

    trainer.train()
    trainer.save_model(args.output)
    tokenizer.save_pretrained(args.output)
    print(f"\nLoRA adapter saved to: {args.output}")

    if args.push_to_hub and args.hub_model_id:
        trainer.push_to_hub()
        print(f"Pushed adapter to HuggingFace Hub: {args.hub_model_id}")

    print("Serve it with vLLM (see README) and point OPENAI_LORA_BASE_URL / OPENAI_LORA_MODEL at it.")


if __name__ == "__main__":
    main()