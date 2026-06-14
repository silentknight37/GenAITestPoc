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

var discoveredEndpoints = ApiEndpointScanner.Scan(target);
Console.WriteLine($"Discovered {discoveredEndpoints.Count} API endpoints from source code:");
foreach (var ep in discoveredEndpoints)
    Console.WriteLine($"  {ep.HttpMethod,-7} {ep.Route}");

var request = new GenerationRequest(
    TargetPath:          target,
    DocsPath:            docs,
    OutputPath:          outDir,
    TargetProjectFile:   targetProject,
    ModelType:           mode,
    TestStyle:           style,
    TestFramework:       framework,
    ModuleFilter:        module,
    DiscoveredEndpoints: discoveredEndpoints
);

Directory.CreateDirectory(request.OutputPath);

var normalizedMode = mode.ToLowerInvariant();
if (normalizedMode is not ("llm" or "rag" or "hybrid" or "lora" or "lora-rag"))
    throw new ArgumentException($"Unsupported mode: {mode}");

// Model selection: LoRA / LoRA+RAG use a fine-tuned model id, everything else the base model.
var baseModel = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4.1-mini";
var loraModel = Environment.GetEnvironmentVariable("OPENAI_LORA_MODEL");
var isLora      = normalizedMode is "lora" or "lora-rag";
var isRetrieval = normalizedMode is "rag" or "hybrid" or "lora-rag";

if (isLora && string.IsNullOrWhiteSpace(loraModel))
{
    Console.WriteLine("ERROR: LoRA modes require a fine-tuned model id. Set the OPENAI_LORA_MODEL environment variable and retry.");
    return;
}

var runModel = isLora ? loraModel! : baseModel;
var label    = normalizedMode.ToUpperInvariant();   // output folder + namespace suffix

Console.WriteLine($"Running {label} pipeline (model: {runModel})...");

IReadOnlyList<GeneratedTestCase> testCases;

if (isRetrieval)
{
    if (string.IsNullOrWhiteSpace(request.DocsPath))
        throw new ArgumentException($"--docs is required for {normalizedMode} mode. Provide the path to the BA document PDF.");

    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
        ?? throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");

    var embeddingService = new OpenAiEmbeddingService(apiKey);
    var pdfExtractor     = new PdfDocumentExtractor();

    // Hybrid is differentiated inside the engine via request.ModelType == "hybrid".
    var ragEngine = new RagTestGenerationEngine(
        new PlainTextDocumentIngestor(pdfExtractor),
        new SimpleChunker(),
        embeddingService,
        new CosineSimilarityRetriever(embeddingService),
        new RagPromptBuilder(),
        new OpenAiTestGenerator(runModel),
        new GeneratedTestJsonWriter(),
        pdfExtractor,
        new RequirementExtractor());

    var userQuery = string.IsNullOrWhiteSpace(query)
        ? "Generate integration tests covering all business requirements and validation rules"
        : query;

    var suite = await ragEngine.GenerateAsync(request, userQuery);
    testCases = suite.TestCases;
}
else // llm, lora — direct generation, no retrieval
{
    var engine = new TestGenerationEngine(
        new PdfDocumentExtractor(),
        new RequirementExtractor(),
        new SmartRequirementGrouper(),
        new ContextBuilderFactory(),
        new OpenAiTestGenerator(runModel),
        new GeneratedTestJsonWriter());

    var suite = await engine.GenerateAsync(request);
    testCases = suite.TestCases;
}

// Write C# test classes (CLI always writes all tests)
Console.WriteLine("Generating C# test classes...");
var outputDir   = Path.Combine(DiscoverTestOutputPath(target!, outDir!), label);
var nsNamespace = SanitizeNs(Path.GetFileName(target!.TrimEnd('\\','/')))
                  + ".Tests.Integration.Generated." + SanitizeNs(label);
var files = await new XunitTestClassGenerator()
    .GenerateAsync(testCases, outputDir, nsNamespace);
Console.WriteLine($"Generated {files.Count} test class files → {outputDir}");
Console.WriteLine($"{label} generation completed.");

string? GetArg(string key)
{
    var index = Array.FindIndex(args, a => a.Equals(key, StringComparison.OrdinalIgnoreCase));
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
}

// Finds the first *Tests* / *Test* folder inside targetPath, falls back to fallback/Generated
static string DiscoverTestOutputPath(string targetPath, string fallbackOutput)
{
    if (Directory.Exists(targetPath))
    {
        var candidates = Directory
            .GetDirectories(targetPath, "*Tests*", SearchOption.AllDirectories)
            .Concat(Directory.GetDirectories(targetPath, "*Test*", SearchOption.AllDirectories))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(d => !d.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar) &&
                        !d.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar));

        var best = candidates.FirstOrDefault(d =>
                       d.Contains("Integration", StringComparison.OrdinalIgnoreCase))
                   ?? candidates.FirstOrDefault();

        if (best != null)
            return Path.Combine(best, "Generated");
    }
    return Path.Combine(fallbackOutput, "Generated");
}

static string SanitizeNs(string value) =>
    string.Join(".",
        value.Split(new[] { '-', ' ', '\\', '/', '.', '_' }, StringSplitOptions.RemoveEmptyEntries)
             .Select(p => char.IsDigit(p[0]) ? "_" + p : p));

static void PrintUsage()
{
    Console.WriteLine();
    Console.WriteLine("  GenAI Test Framework — generate xUnit tests from BA documents");
    Console.WriteLine();
    Console.WriteLine("  Usage:");
    Console.WriteLine("    genaitest generate --mode <mode> --docs <pdf> --target <api-path> --out <output-path>");
    Console.WriteLine();
    Console.WriteLine("  Required:");
    Console.WriteLine("    --mode    llm | rag | hybrid | lora | lora-rag");
    Console.WriteLine("    --docs    Full path to your BA requirements PDF");
    Console.WriteLine("    --target  Root folder of the API project to generate tests for");
    Console.WriteLine("    --out     Folder where test files will be written");
    Console.WriteLine();
    Console.WriteLine("  Optional:");
    Console.WriteLine("    --query   Custom retrieval query (RAG mode only)");
    Console.WriteLine("    --framework  xunit (default) | nunit | mstest");
    Console.WriteLine("    --style   integration (default) | unit");
    Console.WriteLine();
    Console.WriteLine("  Environment:");
    Console.WriteLine("    OPENAI_API_KEY   Required — your OpenAI API key");
    Console.WriteLine();
    Console.WriteLine("  Example:");
    Console.WriteLine("    $env:OPENAI_API_KEY = \"sk-proj-...\"");
    Console.WriteLine("    genaitest generate --mode llm \\");
    Console.WriteLine("      --docs C:\\docs\\requirements.pdf \\");
    Console.WriteLine("      --target C:\\GIT\\LogisticsPro_API\\LogisticsPro_API \\");
    Console.WriteLine("      --out C:\\GIT\\LogisticsPro_API");
    Console.WriteLine();
}
