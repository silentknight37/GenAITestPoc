namespace GenAITest.Framework.Models;

public sealed record EvaluationResult(
    bool CompileSuccess,
    int BuildExitCode,
    string BuildLogPath,

    bool TestSuccess,
    int TestExitCode,
    string TestLogPath
);