using GenAITest.Framework.Abstractions;
using GenAITest.Framework.Models;

namespace GenAITest.Pipeline.Hybrid;

public sealed class HybridPipeline : ITestGenerationPipeline
{
    public string Name => "hybrid";

    public Task<GenerationResult> GenerateAsync(
        GenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = new GenerationResult(
            Mode: Name,
            FilesGenerated: 0,
            GeneratedFiles: Array.Empty<string>(),
            Warnings: new[] { "Hybrid pipeline not implemented yet." }
        );

        return Task.FromResult(result);
    }
}