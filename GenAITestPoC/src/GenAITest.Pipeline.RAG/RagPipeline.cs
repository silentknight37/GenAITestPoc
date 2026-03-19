using GenAITest.Engine.Abstractions;
using GenAITest.Engine.Models;

namespace GenAITest.Pipeline.RAG;

public sealed class RagPipeline : ITestGenerationPipeline
{
    public string Name => "rag";

    public Task<GenerationResult> GenerateAsync(
        GenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = new GenerationResult(
            Mode: Name,
            FilesGenerated: 0,
            GeneratedFiles: Array.Empty<string>(),
            Warnings: new[] { "RAG pipeline not implemented yet." }
        );

        return Task.FromResult(result);
    }
}