using GenAITest.Framework.Models;

namespace GenAITest.Framework.Abstractions;

public interface ITestGenerationPipeline
{
    string Name { get; }

    Task<GenerationResult> GenerateAsync(
        GenerationRequest request,
        CancellationToken cancellationToken = default);
}