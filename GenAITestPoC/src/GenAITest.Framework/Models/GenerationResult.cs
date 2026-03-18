namespace GenAITest.Framework.Models;

public sealed record GenerationResult(
    string Mode,
    int FilesGenerated,
    IReadOnlyList<string> GeneratedFiles,
    IReadOnlyList<string> Warnings
);