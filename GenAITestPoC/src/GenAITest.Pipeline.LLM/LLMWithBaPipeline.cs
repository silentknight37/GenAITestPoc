using System.Text;
using GenAITest.Framework.Abstractions;
using GenAITest.Framework.Models;

namespace GenAITest.Pipeline.LLM;

public sealed class LLMWithBaPipeline : ITestGenerationPipeline
{
    public string Name => "llm-ba";

    public async Task<GenerationResult> GenerateAsync(
        GenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.TargetPath) || !Directory.Exists(request.TargetPath))
        {
            return new GenerationResult(
                Mode: Name,
                FilesGenerated: 0,
                GeneratedFiles: Array.Empty<string>(),
                Warnings: new[] { $"TargetPath not found: {request.TargetPath}" });
        }

        if (string.IsNullOrWhiteSpace(request.DocsPath) || !Directory.Exists(request.DocsPath))
        {
            return new GenerationResult(
                Mode: Name,
                FilesGenerated: 0,
                GeneratedFiles: Array.Empty<string>(),
                Warnings: new[] { $"DocsPath not found (required for llm-ba): {request.DocsPath}" });
        }

        Directory.CreateDirectory(request.OutputPath);

        // 1) Load BA documents (full injection, no retrieval)
        var baText = await LoadAllDocsAsTextAsync(request.DocsPath, cancellationToken);

        if (string.IsNullOrWhiteSpace(baText))
        {
            return new GenerationResult(
                Mode: Name,
                FilesGenerated: 0,
                GeneratedFiles: Array.Empty<string>(),
                Warnings: new[] { "No .md/.txt BA docs found or docs were empty." });
        }

        // Keep BA bounded (avoid massive prompts)
        const int maxBaChars = 20000;
        if (baText.Length > maxBaChars)
            baText = baText.Substring(0, maxBaChars);

        // 2) Load code (same strategy as your working LLM pipeline)
        var codeFiles = Directory.GetFiles(request.TargetPath, "*.cs", SearchOption.AllDirectories);
        if (codeFiles.Length == 0)
        {
            return new GenerationResult(
                Mode: Name,
                FilesGenerated: 0,
                GeneratedFiles: Array.Empty<string>(),
                Warnings: new[] { "No C# files found under target path." });
        }

        var primaryPath =
            codeFiles.FirstOrDefault(f => f.EndsWith("UserService.cs", StringComparison.OrdinalIgnoreCase))
            ?? codeFiles.FirstOrDefault(f => f.EndsWith("UsersController.cs", StringComparison.OrdinalIgnoreCase))
            ?? codeFiles.First();

        var primaryCode = await File.ReadAllTextAsync(primaryPath, cancellationToken);

        // 3) Prompt: BA + Code
        var prompt = BuildPrompt(
            baText: baText,
            primaryFileName: Path.GetFileName(primaryPath),
            primaryCode: primaryCode,
            testFramework: request.TestFramework);

        // 4) Call LLM
        ILLMClient client = new OpenAiLLMClient();
        var raw = await client.GenerateAsync(prompt, cancellationToken);

        // 5) Clean output (remove markdown fences)
        var cleaned = SanitizeToCSharp(raw);

        var outputFile = Path.Combine(request.OutputPath, "GeneratedTests.cs");
        await File.WriteAllTextAsync(outputFile, cleaned, cancellationToken);

        return new GenerationResult(
            Mode: Name,
            FilesGenerated: 1,
            GeneratedFiles: new[] { outputFile },
            Warnings: Array.Empty<string>());
    }

    private static async Task<string> LoadAllDocsAsTextAsync(string docsPath, CancellationToken ct)
    {
        var sb = new StringBuilder();

        var files = Directory.GetFiles(docsPath, "*.*", SearchOption.AllDirectories)
            .Where(f =>
            {
                var ext = Path.GetExtension(f).ToLowerInvariant();
                return ext == ".md" || ext == ".txt";
            })
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var file in files)
        {
            var content = await File.ReadAllTextAsync(file, ct);
            if (string.IsNullOrWhiteSpace(content)) continue;

            sb.AppendLine($"# SOURCE FILE: {Path.GetFileName(file)}");
            sb.AppendLine(content.Trim());
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string BuildPrompt(string baText, string primaryFileName, string primaryCode, string testFramework)
    {
        // Bound code too
        const int maxPrimaryChars = 14000;
        if (primaryCode.Length > maxPrimaryChars)
            primaryCode = primaryCode.Substring(0, maxPrimaryChars);

        var sb = new StringBuilder();

        sb.AppendLine("You are a senior .NET test engineer for an enterprise backend.");
        sb.AppendLine($"Generate {testFramework} unit tests that satisfy BOTH the BA rules and the code behavior.");
        sb.AppendLine();

        sb.AppendLine("OUTPUT RULES:");
        sb.AppendLine("- Output ONLY raw C# code (no markdown, no ``` fences).");
        sb.AppendLine("- Do NOT invent classes/methods/DTOs/validators that do not exist in code.");
        sb.AppendLine("- Prefer compiling tests. Use xUnit Assert.* only (no FluentAssertions).");
        sb.AppendLine("- Use Moq only if needed. Avoid optional-argument setups (pass all args explicitly).");
        sb.AppendLine();

        sb.AppendLine("BUSINESS REQUIREMENTS (FULL BA DOCUMENT):");
        sb.AppendLine("---- BA START ----");
        sb.AppendLine(baText);
        sb.AppendLine("---- BA END ----");
        sb.AppendLine();

        sb.AppendLine($"PRIMARY CODE FILE: {primaryFileName}");
        sb.AppendLine("---- CODE START ----");
        sb.AppendLine(primaryCode);
        sb.AppendLine("---- CODE END ----");
        sb.AppendLine();

        sb.AppendLine("Generate tests that cover:");
        sb.AppendLine("- Happy path");
        sb.AppendLine("- BA-driven edge cases and validations");
        sb.AppendLine("- State transitions / error codes if present in code");
        sb.AppendLine("- Idempotency / retry rules IF implemented in code");
        sb.AppendLine();
        sb.AppendLine("Return a single compilable C# test file.");

        return sb.ToString();
    }

    private static string SanitizeToCSharp(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return content ?? string.Empty;

        content = content.Trim();

        if (content.StartsWith("```", StringComparison.Ordinal))
        {
            var firstNl = content.IndexOf('\n');
            if (firstNl >= 0)
                content = content[(firstNl + 1)..];
        }

        content = content.TrimEnd();
        if (content.EndsWith("```", StringComparison.Ordinal))
            content = content[..^3];

        content = content.Replace("```", string.Empty);

        return content.Trim() + Environment.NewLine;
    }
}