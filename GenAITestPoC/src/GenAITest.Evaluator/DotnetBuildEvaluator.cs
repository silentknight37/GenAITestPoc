using System.Diagnostics;
using GenAITest.Engine.Abstractions;
using GenAITest.Engine.Models;

namespace GenAITest.Evaluator;

public sealed class DotnetBuildEvaluator : ITestEvaluator
{
    public async Task<EvaluationResult> EvaluateAsync(
        string generatedTestFilePath,
        string outputDir,
        string? targetProjectFile,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(outputDir);

        var tempProjDir = Path.Combine(outputDir, "_compilecheck");
        if (Directory.Exists(tempProjDir))
            Directory.Delete(tempProjDir, true);

        Directory.CreateDirectory(tempProjDir);

        // 1️⃣ Create xUnit project
        await Run("dotnet", "new xunit -n CompileCheck", tempProjDir, cancellationToken);

        var projDir = Path.Combine(tempProjDir, "CompileCheck");

        // 2️⃣ Add reference to target project
        if (!string.IsNullOrWhiteSpace(targetProjectFile) && File.Exists(targetProjectFile))
        {
            targetProjectFile = Path.GetFullPath(targetProjectFile);
            await Run("dotnet", $"add reference \"{targetProjectFile}\"", projDir, cancellationToken);
        }

        // 3️⃣ Add required packages (pin EF Core to net8 compatible version)
        await Run("dotnet", "add package Microsoft.EntityFrameworkCore.InMemory --version 8.0.0", projDir, cancellationToken);
        await Run("dotnet", "add package Moq --version 4.20.72", projDir, cancellationToken);
        await Run("dotnet", "add package FluentAssertions --version 6.12.0", projDir, cancellationToken);
        // 4️⃣ Copy generated test file
        var generatedDir = Path.Combine(projDir, "Generated");
        Directory.CreateDirectory(generatedDir);

        var destFile = Path.Combine(generatedDir, Path.GetFileName(generatedTestFilePath));
        File.Copy(generatedTestFilePath, destFile, overwrite: true);

        // 5️⃣ BUILD
        var buildLogPath = Path.Combine(outputDir, "build.log");
        var (buildExit, buildOut, buildErr) = await RunWithOutput("dotnet", "build", projDir, cancellationToken);
        await File.WriteAllTextAsync(buildLogPath, buildOut + Environment.NewLine + buildErr, cancellationToken);

        if (buildExit != 0)
        {
            return new EvaluationResult(
                CompileSuccess: false,
                BuildExitCode: buildExit,
                BuildLogPath: buildLogPath,
                TestSuccess: false,
                TestExitCode: -1,
                TestLogPath: string.Empty
            );
        }

        // 6️⃣ TEST EXECUTION
        var testLogPath = Path.Combine(outputDir, "test.log");
        var (testExit, testOut, testErr) = await RunWithOutput("dotnet", "test --no-build", projDir, cancellationToken);
        await File.WriteAllTextAsync(testLogPath, testOut + Environment.NewLine + testErr, cancellationToken);

        return new EvaluationResult(
            CompileSuccess: true,
            BuildExitCode: buildExit,
            BuildLogPath: buildLogPath,
            TestSuccess: testExit == 0,
            TestExitCode: testExit,
            TestLogPath: testLogPath
        );
    }

    private static async Task Run(string file, string args, string workingDir, CancellationToken ct)
    {
        var (exitCode, stdout, stderr) = await RunWithOutput(file, args, workingDir, ct);

        if (exitCode != 0)
        {
            throw new Exception(
                $"Command failed: {file} {args}\n" +
                $"WorkingDir: {workingDir}\n" +
                $"STDOUT:\n{stdout}\n" +
                $"STDERR:\n{stderr}\n");
        }
    }

    private static async Task<(int exitCode, string stdout, string stderr)> RunWithOutput(
        string file,
        string args,
        string workingDir,
        CancellationToken ct)
    {
        var psi = new ProcessStartInfo
        {
            FileName = file,
            Arguments = args,
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };
        process.Start();

        var stdout = await process.StandardOutput.ReadToEndAsync();
        var stderr = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync(ct);

        return (process.ExitCode, stdout, stderr);
    }
}