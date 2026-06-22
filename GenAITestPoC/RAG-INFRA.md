# RAG infrastructure: vector DB + local embeddings

The retrieval modes (RAG, Hybrid, LoRA+RAG) support a real **vector database (Qdrant)** and an
optional **local embedding model (SentenceTransformers-style via Ollama)**. Both are selected by
environment variables; if unset, the framework uses its in-memory cosine store + OpenAI embeddings.

## Vector database — Qdrant
Run it (Docker Desktop must be running):
```powershell
docker run -d --name qdrant -p 6333:6333 --restart unless-stopped qdrant/qdrant
```
Enable it (already set in launchSettings):
```
QDRANT_URL=http://localhost:6333
```
The engine creates a collection per run, upserts the corpus embeddings, and does top-K vector
search there. **If Qdrant is unreachable it transparently falls back to the in-memory cosine
store** (logged), so RAG never breaks — safe to leave enabled.

## Local embeddings — SentenceTransformers (via Ollama)
Pull a local embedding model (already done: `nomic-embed-text`, 768-dim):
```powershell
ollama pull nomic-embed-text     # or: all-minilm (384-dim, smaller)
```
Enable it (NOT set by default, to keep RAG/Hybrid comparable on OpenAI embeddings):
```
OPENAI_EMBED_BASE_URL=http://localhost:11434/v1
OPENAI_EMBED_MODEL=nomic-embed-text
```
With both this and the LoRA model set, **LoRA + RAG runs fully local** — local embeddings, local
vector DB (Qdrant), local generation (Ollama). No OpenAI calls.

> Note: `OPENAI_EMBED_*` applies to **all** retrieval modes. If you've recorded RAG/Hybrid numbers
> with OpenAI embeddings, only enable it for the LoRA+RAG runs (or accept that all modes switch to
> local embeddings). The embedding cache keys on the model id, so switching models won't collide.