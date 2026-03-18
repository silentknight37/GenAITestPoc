using GenAITest.Framework.Models;

namespace GenAITest.Framework.Abstractions;

public interface ITestEvaluator
{
    Task<EvaluationResult> EvaluateAsync(
        string generatedTestFilePath,
        string outputDir,
        string? targetProjectFile,
        CancellationToken cancellationToken = default);
}