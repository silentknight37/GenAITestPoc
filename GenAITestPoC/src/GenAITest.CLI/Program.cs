using System.Text.Json;
using GenAITest.Engine;
using GenAITest.Engine.Abstractions;
using GenAITest.Engine.Models;
using GenAITest.Engine.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddGenAITestEngine();
var provider = services.BuildServiceProvider();

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

string? mode      = GetArg("--mode");
string? target    = GetArg("--target");
string? outDir    = GetArg("--out");
string? docs      = GetArg("--docs");
string? framework = GetArg("--framework");
string? style     = GetArg("--style");
string? module    = GetArg("--module");
string? query     = GetArg("--query");

if (string.IsNullOrWhiteSpace(mode) ||
    string.IsNullOrWhiteSpace(target) ||
    string.IsNullOrWhiteSpace(outDir))
{
    PrintUsage();
    return;
}

framework ??= TestFrameworkType.XUnit;
style     ??= TestStyleType.Integration;

string? targetProject = null;
if (Directory.Exists(target))
    targetProject = Directory.GetFiles(target, "*.csproj", SearchOption.AllDirectories).FirstOrDefault();

var request = new GenerationRequest(
    TargetPath:       target,
    DocsPath:         docs,
    OutputPath:       outDir,
    TargetProjectFile: targetProject,
    ModelType:        mode,
    TestStyle:        style,
    TestFramework:    framework,
    ModuleFilter:     module
);

Directory.CreateDirectory(request.OutputPath);

switch (mode.ToLowerInvariant())
{
    case "llm":
    {
        Console.WriteLine("Running LLM pipeline...");

        var engine = new TestGenerationEngine(
            new PdfDocumentExtractor(),
            new RequirementExtractor(),
            new SmartRequirementGrouper(),
            new ContextBuilderFactory(),
            new OpenAiTestGenerator(),
            new XunitTestClassGenerator(),
            new GeneratedTestJsonWriter());

        await engine.GenerateAsync(request);
        Console.WriteLine("LLM generation completed.");
        break;
    }

    case "rag":
    {
        Console.WriteLine("Running RAG pipeline...");

        if (string.IsNullOrWhiteSpace(request.DocsPath))
            throw new ArgumentException(
                "--docs is required for rag mode. Provide the path to the BA document PDF.");

        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            ?? throw new InvalidOperationException(
                "OPENAI_API_KEY environment variable is not set.");

        var embeddingService = new OpenAiEmbeddingService(apiKey);
        var retriever        = new CosineSimilarityRetriever(embeddingService);

        var pdfExtractor = new PdfDocumentExtractor();

        var ragEngine = new RagTestGenerationEngine(
            new PlainTextDocumentIngestor(pdfExtractor),
            new SimpleChunker(),
            embeddingService,
            retriever,
            new RagPromptBuilder(),
            new OpenAiTestGenerator(),
            new GeneratedTestJsonWriter(),
            new XunitTestClassGenerator(),
            pdfExtractor,               // IDocumentExtractor — for requirement extraction
            new RequirementExtractor()); // IRequirementExtractor — per-requirement loop

        var userQuery = string.IsNullOrWhiteSpace(query)
            ? "Generate integration tests covering all business requirements and validation rules"
            : query;

        await ragEngine.GenerateAsync(request, userQuery);
        Console.WriteLine("RAG generation completed.");
        break;
    }

    case "hybrid":
    case "lora":
    case "lora-rag":
        Console.WriteLine($"Mode '{mode}' is not implemented yet.");
        break;

    default:
        throw new ArgumentException($"Unsupported mode: {mode}");
}

string? GetArg(string key)
{
    var index = Array.FindIndex(args, a => a.Equals(key, StringComparison.OrdinalIgnoreCase));
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
}

static void PrintUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  generate --mode llm|rag --target <path> --out <path>");
    Console.WriteLine("           --docs <ba-pdf>");
    Console.WriteLine("           [--framework xunit|nunit|mstest]");
    Console.WriteLine("           [--style integration|api|unit]");
    Console.WriteLine("           [--module <module-name>]");
    Console.WriteLine("           [--query \"<rag retrieval query>\"]   (rag mode only)");
}
