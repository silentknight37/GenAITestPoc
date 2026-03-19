using GenAITest.Engine.Models;
using GenAITest.Engine.Services;

if (args.Length == 0)
{
    PrintUsage();
    return;
}

var command = args[0].ToLowerInvariant();
if (command != "generate")
{
    PrintUsage();
    return;
}

string? mode = GetArg("--mode");
string? target = GetArg("--target");
string? outDir = GetArg("--out");
string? docs = GetArg("--docs");
string? framework = GetArg("--framework");
string? style = GetArg("--style");
string? module = GetArg("--module");

if (string.IsNullOrWhiteSpace(mode) || string.IsNullOrWhiteSpace(target) || string.IsNullOrWhiteSpace(outDir))
{
    PrintUsage();
    return;
}

framework ??= TestFrameworkType.XUnit;
style ??= TestStyleType.Integration;

string? targetProject = null;
if (Directory.Exists(target))
{
    targetProject = Directory.GetFiles(target, "*.csproj", SearchOption.AllDirectories).FirstOrDefault();
}

var request = new GenerationRequest(
    TargetPath: target,
    DocsPath: docs,
    OutputPath: outDir,
    TargetProjectFile: targetProject,
    ModelType: mode,
    TestStyle: style,
    TestFramework: framework,
    ModuleFilter: module
);

var engine = new TestGenerationEngine(
    new PdfDocumentExtractor(),
    new RequirementExtractor(),
    new SmartRequirementGrouper(),
    new ContextBuilderFactory(),
    new StubTestGenerator()
);

await engine.GenerateAsync(request);

Console.WriteLine("Generation completed.");

string? GetArg(string key)
{
    var index = Array.FindIndex(args, a => a.Equals(key, StringComparison.OrdinalIgnoreCase));
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
}

static void PrintUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  generate --mode llm|rag|hybrid|lora|lora-rag --target <path> --out <path> [--docs <ba-pdf>] [--framework xunit|nunit|mstest] [--style integration|api|unit] [--module <module-name>]");
}