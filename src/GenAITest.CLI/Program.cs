using GenAITest.Framework.Models;
using GenAITest.Pipeline.LLM;
using GenAITest.Pipeline.RAG;
using GenAITest.Pipeline.Hybrid;
using System.Text.Json;
using GenAITest.Evaluator;

if (args.Length == 0)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  --mode llm|rag|hybrid --target <path> --out <path> [--docs <path>]");
    return;
}

string? mode = GetArg("--mode");
string? target = GetArg("--target");
string? outDir = GetArg("--out");
string? docs = GetArg("--docs");

if (mode is null || target is null || outDir is null)
{
    Console.WriteLine("Missing required arguments.");
    return;
}

string? targetProject = Directory.GetFiles(target, "*.csproj", SearchOption.AllDirectories)
                                 .FirstOrDefault();

if (targetProject is null)
    Console.WriteLine($"Target project: {targetProject ?? "(not found)"}");

var request = new GenerationRequest(
    TargetPath: target,
    DocsPath: docs,
    OutputPath: outDir,
    TestFramework: "xunit",
    TargetProjectFile: targetProject
);

GenAITest.Framework.Abstractions.ITestGenerationPipeline pipeline =
mode.ToLowerInvariant() switch
{
    "llm" => new LLMOnlyPipeline(),
    "llm-ba" => new LLMWithBaPipeline(),
    "rag" => new RagPipeline(),
    "hybrid" => new HybridPipeline(),
    _ => throw new ArgumentException("Invalid --mode")
};

var result = await pipeline.GenerateAsync(request);

Console.WriteLine($"Mode: {result.Mode}");
Console.WriteLine($"Files Generated: {result.FilesGenerated}");

foreach (var warning in result.Warnings)
{
    Console.WriteLine($"WARNING: {warning}");
}

string? GetArg(string key)
{
    var index = Array.FindIndex(args, a =>
        a.Equals(key, StringComparison.OrdinalIgnoreCase));

    return index >= 0 && index + 1 < args.Length
        ? args[index + 1]
        : null;
}

var evaluator = new DotnetBuildEvaluator();
var generatedFilePath = result.GeneratedFiles.FirstOrDefault();

if (!string.IsNullOrWhiteSpace(generatedFilePath) && File.Exists(generatedFilePath))
{
    var targetProjectPath = Directory.GetFiles(target, "*.csproj", SearchOption.AllDirectories)
                             .FirstOrDefault();

    if (!string.IsNullOrWhiteSpace(targetProjectPath))
    {
        targetProjectPath = Path.GetFullPath(targetProjectPath);
    }

    Console.WriteLine($"Target project: {targetProjectPath ?? "(not found)"}");

    var eval = await evaluator.EvaluateAsync(generatedFilePath, outDir, targetProjectPath);

    var metricsPath = Path.Combine(outDir, "metrics.json");
    var json = JsonSerializer.Serialize(eval, new JsonSerializerOptions { WriteIndented = true });
    await File.WriteAllTextAsync(metricsPath, json);

    Console.WriteLine($"Compile Success: {eval.CompileSuccess}");
    Console.WriteLine($"Build Log: {eval.BuildLogPath}");
    Console.WriteLine($"Metrics: {metricsPath}");
}
else
{
    Console.WriteLine("No generated test file found to evaluate.");
}