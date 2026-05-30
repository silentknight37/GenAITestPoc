using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GenAITest.Engine.Abstractions;
using GenAITest.Engine.Models;

namespace GenAITest.Pipeline.LLM;

public sealed class OpenAiLLMClient : ILLMClient
{
    private readonly LlmOptions _options;
    private readonly HttpClient _httpClient;

    // Parameterless constructor reads entirely from environment variables.
    public OpenAiLLMClient() : this(new LlmOptions()) { }

    public OpenAiLLMClient(LlmOptions options, HttpClient? httpClient = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            _options.ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(_options.Model))
            _options.Model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o-mini";
        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
            _options.BaseUrl = "https://api.openai.com/v1/chat/completions";

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new InvalidOperationException(
                "OpenAI API key is not configured. Set the OPENAI_API_KEY environment variable.");

        _httpClient = httpClient ?? new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ApiKey);
    }

    public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var requestBody = new
        {
            model = _options.Model,
            messages = new[]
            {
                new { role = "system", content = "You are a senior .NET test engineer. Output only valid C# code with no markdown fences." },
                new { role = "user",   content = prompt }
            },
            max_tokens = 4096
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(
            _options.BaseUrl,
            content,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"OpenAI API error {(int)response.StatusCode}: {Truncate(error)}");
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;
    }

    private static string Truncate(string value, int max = 500) =>
        value.Length > max ? value[..max] : value;
}
