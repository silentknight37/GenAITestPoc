namespace GenAITest.Framework.Abstractions;

public interface ILLMClient
{
    Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default);
}