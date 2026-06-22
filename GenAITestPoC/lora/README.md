# Local LoRA for the GenAI Test Framework

This implements the **LoRA** and **LoRA + RAG** modes with a *real* Low-Rank Adapter trained on an
open model (Llama/Mistral) and served behind an OpenAI-compatible endpoint. The framework already
knows how to call it — it just needs the endpoint + model id.

```
 ┌─ teacher runs (LLM/RAG/Hybrid) ─┐      ┌─ train ─┐      ┌─ serve ─┐      ┌─ use ─┐
 │ OPENAI_TRAINING_DATA=...jsonl   │ ───▶ │ PEFT    │ ───▶ │ vLLM    │ ───▶ │ LoRA  │
 │ captures (prompt → JSON) pairs  │      │ LoRA    │      │ /v1     │      │ modes │
 └─────────────────────────────────┘      └─────────┘      └─────────┘      └───────┘
```

## 1. Capture a training dataset (from the teacher model)

Run the framework with capture enabled — every `(prompt → test-case JSON)` pair from the base
model is appended as a chat example (same format OpenAI fine-tuning uses).

PowerShell, before starting the API or CLI:
```powershell
$env:OPENAI_TRAINING_DATA = "C:\GIT\GenAITestPoc_GIT\LogisticsPro_API\artifacts\training.jsonl"
```
Then generate in **LLM**, **RAG**, and **Hybrid** modes over one or more BA documents. The richer
and more varied the runs, the better the adapter. (Capture is skipped for the LoRA modes — you
don't train the student on its own output.) Aim for at least a few hundred examples.

## 1b. (Optional) Convert to the instruction dataset format

The capture is chat JSONL (`messages[]`). To produce the Alpaca-style **instruction dataset**
(`{instruction, input, output}`) documented in the thesis:
```bash
python convert_dataset.py --in training.jsonl --out dataset.instruction.jsonl
```
`train_lora.py` accepts **either** format, so this step is only for matching the documented shape.

## 2. Train the adapter (GPU required)

**No local GPU?** Train on a free **Google Colab / Kaggle T4** — upload `training.jsonl` and this
folder, `pip install -r requirements.txt`, run the command below. ~30–60 min for a 3B QLoRA.

> **The base model must match what you serve.** A LoRA adapter is tied to its base. Since the
> framework's LoRA mode currently serves `qwen2.5-coder`, train on **Qwen2.5-Coder** so the adapter
> attaches to it. (Using a different base — e.g. Llama — produces an adapter that won't load on Qwen.)

Linux / WSL2 / Colab with a CUDA GPU:
```bash
cd GenAITestPoC/lora
python -m venv .venv && source .venv/bin/activate
pip install -r requirements.txt

python train_lora.py \
  --data training.jsonl \
  --base-model Qwen/Qwen2.5-Coder-3B-Instruct \
  --output ./genai-lora-adapter \
  --epochs 3 \
  --val-frac 0.1 \
  --report-to wandb \
  --push-to-hub --hub-model-id your-org/genai-lora
```
`--val-frac 0.1` holds out 10% for an **eval loss** you can cite in the methodology/validity section.
`train_lora.py` reads `training.jsonl` (chat format) directly — the convert step is optional.
- **Quantization:** defaults to 4-bit QLoRA; add `--no-4bit` (or use a smaller base) if bitsandbytes
  isn't available (plain Windows).
- **Experiment tracking:** `--report-to wandb` or `mlflow` logs loss/metrics (run `wandb login`
  first, or set `MLFLOW_TRACKING_URI`). Default `none`.
- **Model storage:** `--push-to-hub --hub-model-id your-org/genai-lora` saves the adapter to the
  HuggingFace Hub (run `huggingface-cli login` first); it's always also saved locally to `--output`.

## 3. Serve it (OpenAI-compatible, with LoRA)

vLLM exposes `/v1/chat/completions` and can attach the adapter as a named model (serve on a GPU box
so generation is fast — the whole point of moving off CPU):
```bash
vllm serve Qwen/Qwen2.5-Coder-3B-Instruct \
  --enable-lora \
  --lora-modules genai-lora=./genai-lora-adapter \
  --port 8000
```
Now `http://localhost:8000/v1` answers as model **`genai-lora`**.

**Managed cloud (no GPU box to run):** upload the adapter to **Fireworks** or **Together**, which
serve LoRA adapters behind their OpenAI-compatible API — then just point the framework at their URL,
the served adapter id, and your provider key (`OPENAI_LORA_API_KEY`).

*(Ollama alternative: convert the merged model to GGUF, `ollama create genai-lora -f Modelfile`,
then `ollama serve` exposes `http://localhost:11434/v1`.)*

## 4. Point the framework at it

Set these before starting the host API, then pick **LoRA** or **LoRA + RAG** in the portal:
```powershell
$env:OPENAI_LORA_BASE_URL = "http://localhost:8000/v1"   # your vLLM server (or cloud provider URL)
$env:OPENAI_LORA_MODEL    = "genai-lora"                 # the served adapter id
$env:OPENAI_LORA_API_KEY  = "..."                        # provider key for a cloud adapter (local vLLM ignores it)
$env:OPENAI_API_KEY       = "sk-..."                     # real OpenAI key — still used for RAG embeddings
```
`OPENAI_LORA_API_KEY` (separate from `OPENAI_API_KEY`) lets a cloud-served adapter use its own key
without disturbing the OpenAI key the other modes need.
or in `Program.cs`:
```csharp
builder.Services.AddGenAITestFramework(opt =>
{
    opt.LoraBaseUrl = "http://localhost:8000/v1";
    opt.LoraModelId = "genai-lora";
});
```

**Notes**
- **LoRA** = the local adapter generates the test cases (no retrieval).
- **LoRA + RAG** = retrieval (OpenAI embeddings) + the local adapter for generation — so a real
  `OPENAI_API_KEY` is still needed for the embedding calls; the local server ignores the key.
- The local server only needs to serve **chat completions**; embeddings stay on OpenAI.