"""
Convert the framework's captured chat JSONL (messages[]) into the Alpaca-style instruction
dataset documented in the thesis:

    {"instruction": "<system prompt>", "input": "<user prompt>", "output": "<assistant JSON>"}

Usage:
    python convert_dataset.py --in training.jsonl --out dataset.instruction.jsonl
"""
import argparse
import json


def main():
    p = argparse.ArgumentParser(description="Chat JSONL → instruction JSONL.")
    p.add_argument("--in", dest="inp", required=True, help="Captured chat JSONL (messages[]).")
    p.add_argument("--out", dest="out", required=True, help="Output instruction JSONL.")
    args = p.parse_args()

    written, skipped = 0, 0
    with open(args.inp, encoding="utf-8") as fin, open(args.out, "w", encoding="utf-8") as fout:
        for line in fin:
            line = line.strip()
            if not line:
                continue
            try:
                obj = json.loads(line)
                msgs = {m["role"]: m["content"] for m in obj.get("messages", [])}
            except Exception:
                skipped += 1
                continue

            instruction = msgs.get("system", "Generate xUnit integration tests from this business requirement.")
            user_input  = msgs.get("user", "")
            output      = msgs.get("assistant", "")
            if not user_input or not output:
                skipped += 1
                continue

            fout.write(json.dumps({
                "instruction": instruction,
                "input": user_input,
                "output": output,
            }, ensure_ascii=False) + "\n")
            written += 1

    print(f"Wrote {written} instruction examples to {args.out} ({skipped} skipped).")


if __name__ == "__main__":
    main()