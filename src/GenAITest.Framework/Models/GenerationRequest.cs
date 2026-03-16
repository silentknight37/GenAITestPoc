namespace GenAITest.Framework.Models;

public sealed record GenerationRequest(
    string TargetPath,
    string? DocsPath,
    string OutputPath,
    string TestFramework,
    string? TargetProjectFile
);