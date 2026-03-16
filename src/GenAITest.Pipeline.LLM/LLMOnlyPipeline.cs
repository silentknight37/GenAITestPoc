using System.Text;
using GenAITest.Framework.Abstractions;
using GenAITest.Framework.Models;

namespace GenAITest.Pipeline.LLM;

public sealed class LLMOnlyPipeline : ITestGenerationPipeline
{
    public string Name => "llm";

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

        Directory.CreateDirectory(request.OutputPath);

        var codeFiles = Directory.GetFiles(request.TargetPath, "*.cs", SearchOption.AllDirectories);
        if (codeFiles.Length == 0)
        {
            return new GenerationResult(
                Mode: Name,
                FilesGenerated: 0,
                GeneratedFiles: Array.Empty<string>(),
                Warnings: new[] { "No C# files found under target path." });
        }

        // 1) Select a "focus" file (you can improve this later with CLI --focus)
        var primaryPath =
            codeFiles.FirstOrDefault(f => f.EndsWith("UserService.cs", StringComparison.OrdinalIgnoreCase))
            ?? codeFiles.FirstOrDefault(f => f.EndsWith("UsersController.cs", StringComparison.OrdinalIgnoreCase))
            ?? codeFiles.FirstOrDefault(f => f.EndsWith("UserController.cs", StringComparison.OrdinalIgnoreCase))
            ?? codeFiles.First();

        var primaryCode = await File.ReadAllTextAsync(primaryPath, cancellationToken);

        // 2) Bring in related files by searching for declared types (NOT by filename)
        //    We include types that are commonly used in user registration flows and types that previously caused errors.
        var relatedFiles = FindFilesContainingTypeDeclarations(codeFiles,
                "RegisterRequest",
                "RegisterResponse",
                "User",
                "AppUser",
                "UserEntity",
                "UserDto")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(10)
            .ToList();

        var relatedCodeBuilder = new StringBuilder();
        foreach (var f in relatedFiles)
        {
            // Avoid duplicating the primary file
            if (string.Equals(f, primaryPath, StringComparison.OrdinalIgnoreCase))
                continue;

            var txt = await File.ReadAllTextAsync(f, cancellationToken);
            relatedCodeBuilder.AppendLine($"// FILE: {Path.GetFileName(f)}");
            relatedCodeBuilder.AppendLine(txt);
            relatedCodeBuilder.AppendLine();
        }

        // 3) Build "allowed types" whitelist from the code we provide to the LLM
        var combinedForTypes = primaryCode + "\n" + relatedCodeBuilder.ToString();
        var allowedTypes = ExtractDeclaredTypes(combinedForTypes);

        // 4) Prompt
        var prompt = BuildPrompt(
            primaryFileName: Path.GetFileName(primaryPath),
            primaryCode: primaryCode,
            relatedCode: relatedCodeBuilder.ToString(),
            allowedTypes: allowedTypes,
            testFramework: request.TestFramework);

        // 5) Call LLM
        ILLMClient client = new OpenAiLLMClient();
        var raw = await client.GenerateAsync(prompt, cancellationToken);

        // 6) Clean output
        var cleaned = SanitizeToCSharp(raw);
        if (!cleaned.EndsWith(Environment.NewLine))
            cleaned += Environment.NewLine;

        if (cleaned.Contains("AnyAsync(", StringComparison.Ordinal) ||
    cleaned.Contains("FirstOrDefaultAsync(", StringComparison.Ordinal) ||
    cleaned.Contains("SingleOrDefaultAsync(", StringComparison.Ordinal) ||
    cleaned.Contains("ToListAsync(", StringComparison.Ordinal))
        {
            cleaned = EnsureUsing(cleaned, "Microsoft.EntityFrameworkCore");
        }
        // 7) Save
        var outputFile = Path.Combine(request.OutputPath, "GeneratedTests.cs");
        await File.WriteAllTextAsync(outputFile, cleaned, cancellationToken);

        return new GenerationResult(
            Mode: Name,
            FilesGenerated: 1,
            GeneratedFiles: new[] { outputFile },
            Warnings: Array.Empty<string>());
    }

    private static string BuildPrompt(
        string primaryFileName,
        string primaryCode,
        string relatedCode,
        IReadOnlyList<string> allowedTypes,
        string testFramework)
    {
        // Keep prompts bounded (cost + latency)
        const int maxPrimaryChars = 14000;
        const int maxRelatedChars = 14000;

        if (primaryCode.Length > maxPrimaryChars)
            primaryCode = primaryCode.Substring(0, maxPrimaryChars);

        if (!string.IsNullOrWhiteSpace(relatedCode) && relatedCode.Length > maxRelatedChars)
            relatedCode = relatedCode.Substring(0, maxRelatedChars);

        var sb = new StringBuilder();

        sb.AppendLine("You are a senior .NET test engineer.");
        sb.AppendLine($"Generate {testFramework} unit tests for the PRIMARY FILE.");
        sb.AppendLine();

        sb.AppendLine("OUTPUT FORMAT:");
        sb.AppendLine("- Output ONLY valid raw C# code.");
        sb.AppendLine("- Do NOT use markdown. Do NOT include ``` fences.");
        sb.AppendLine();

        sb.AppendLine("CRITICAL RULES (MUST FOLLOW):");
        sb.AppendLine("1) Do NOT invent any classes, methods, properties, DTOs, validators, or services.");
        sb.AppendLine("2) Use ONLY types/members that exist in the provided code blocks.");
        sb.AppendLine("3) Use xUnit Assert.* only (no FluentAssertions).");
        sb.AppendLine("4) Use Moq only if needed for mocking.");
        sb.AppendLine("5) Prefer tests that compile over speculative tests.");
        sb.AppendLine();

        // Hard-ban known hallucinations from your build logs
        sb.AppendLine("DO NOT USE THESE SYMBOLS (they do not exist):");
        sb.AppendLine("- RegisterRequestValidator");
        sb.AppendLine();

        // Allowed type whitelist
        sb.AppendLine("ALLOWED TYPES (You may only reference these type names):");
        sb.AppendLine(string.Join(", ", allowedTypes));
        sb.AppendLine();
        sb.AppendLine("If you need a 'user' entity type, choose one from ALLOWED TYPES (e.g., AppUser) and use that.");
        sb.AppendLine("Do NOT reference a type named 'User' unless it appears in ALLOWED TYPES.");
        sb.AppendLine("7) Avoid Moq setups that call methods with optional parameters. If mocking such methods, pass ALL parameters explicitly (no optional args).");
        sb.AppendLine("8) If EF Core is used, prefer using an InMemory DbContext instead of mocking DbSet/DbContext.");
        sb.AppendLine("Avoid mocking calls with optional parameters; if needed, pass ALL parameters explicitly.");
        sb.AppendLine();

        sb.AppendLine($"PRIMARY FILE: {primaryFileName}");
        sb.AppendLine("---- PRIMARY CODE START ----");
        sb.AppendLine(primaryCode);
        sb.AppendLine("---- PRIMARY CODE END ----");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(relatedCode))
        {
            sb.AppendLine("RELATED FILES (use to avoid wrong constructors/properties):");
            sb.AppendLine("---- RELATED CODE START ----");
            sb.AppendLine(relatedCode);
            sb.AppendLine("---- RELATED CODE END ----");
            sb.AppendLine();
        }

        sb.AppendLine("Return a single C# file containing:");
        sb.AppendLine("- using directives");
        sb.AppendLine("- namespace MyApi.Tests.Generated (or similar)");
        sb.AppendLine("- one test class and multiple [Fact] tests");
        sb.AppendLine("- meaningful assertions using Assert.*");
        sb.AppendLine("- minimal mocking (Moq) only when required");

        return sb.ToString();
    }

    private static string SanitizeToCSharp(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return content ?? string.Empty;

        content = content.Trim();

        // Strip a leading fenced block header if present
        if (content.StartsWith("```", StringComparison.Ordinal))
        {
            var firstNl = content.IndexOf('\n');
            if (firstNl >= 0)
                content = content[(firstNl + 1)..];
        }

        // Strip trailing fence if present
        content = content.TrimEnd();
        if (content.EndsWith("```", StringComparison.Ordinal))
            content = content[..^3];

        // Remove any remaining fence remnants
        content = content.Replace("```", string.Empty);

        return content.Trim();
    }

    /// <summary>
    /// Finds files that declare one of the given type names (class/record/interface/struct/enum).
    /// This is much more reliable than guessing by filename.
    /// </summary>
    private static IEnumerable<string> FindFilesContainingTypeDeclarations(string[] codeFiles, params string[] typeNames)
    {
        foreach (var f in codeFiles)
        {
            string txt;
            try { txt = File.ReadAllText(f); }
            catch { continue; }

            foreach (var t in typeNames)
            {
                if (ContainsTypeDeclaration(txt, t))
                {
                    yield return f;
                    break;
                }
            }
        }
    }

    private static bool ContainsTypeDeclaration(string txt, string typeName)
    {
        // very simple checks; enough for our purpose
        return txt.Contains($"class {typeName}", StringComparison.Ordinal)
            || txt.Contains($"record {typeName}", StringComparison.Ordinal)
            || txt.Contains($"interface {typeName}", StringComparison.Ordinal)
            || txt.Contains($"struct {typeName}", StringComparison.Ordinal)
            || txt.Contains($"enum {typeName}", StringComparison.Ordinal);
    }

    /// <summary>
    /// Extract declared type names from provided code blocks.
    /// This becomes a whitelist ("Allowed Types") to prevent hallucinations.
    /// </summary>
    private static IReadOnlyList<string> ExtractDeclaredTypes(string code)
    {
        var types = new HashSet<string>(StringComparer.Ordinal);

        var tokens = new[] { "class ", "record ", "interface ", "struct ", "enum " };
        foreach (var token in tokens)
        {
            int idx = 0;
            while (idx < code.Length)
            {
                idx = code.IndexOf(token, idx, StringComparison.Ordinal);
                if (idx < 0) break;

                idx += token.Length;

                // skip whitespace
                while (idx < code.Length && char.IsWhiteSpace(code[idx]))
                    idx++;

                // read identifier
                var start = idx;
                while (idx < code.Length && (char.IsLetterOrDigit(code[idx]) || code[idx] == '_'))
                    idx++;

                var name = code[start..idx].Trim();
                if (!string.IsNullOrWhiteSpace(name))
                    types.Add(name);
            }
        }

        return types.OrderBy(x => x, StringComparer.Ordinal).ToList();
    }

    private static string EnsureUsing(string code, string usingNamespace)
    {
        var usingLine = $"using {usingNamespace};";

        if (code.Contains(usingLine, StringComparison.Ordinal))
            return code;

        // If the code has usings, insert after the last using.
        var lines = code.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
        var lastUsingIndex = lines.FindLastIndex(l => l.TrimStart().StartsWith("using ", StringComparison.Ordinal));

        if (lastUsingIndex >= 0)
        {
            lines.Insert(lastUsingIndex + 1, usingLine);
            return string.Join(Environment.NewLine, lines);
        }

        // No using section found — prepend.
        return usingLine + Environment.NewLine + code;
    }
}