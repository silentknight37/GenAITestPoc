using GenAITest.Engine.Abstractions;
using GenAITest.Engine.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GenAITest.Pipeline.LLM;

public sealed class OpenAiLLMClient : ILLMClient
{
    private readonly LlmOptions _options;
    private readonly HttpClient _httpClient;

    public OpenAiLLMClient(LlmOptions options, HttpClient httpClient = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        // Fallback to environment variables if not set in options
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            _options.ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrWhiteSpace(_options.Model))
            _options.Model = Environment.GetEnvironmentVariable("OPENAI_MODEL");
        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
            _options.BaseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL") ?? "https://api.openai.com/v1/";

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new InvalidOperationException("OpenAI API key is not configured. Set OPENAI_API_KEY or appsettings.");
        if (string.IsNullOrWhiteSpace(_options.Model))
            throw new InvalidOperationException("OpenAI model is not configured. Set OPENAI_MODEL or appsettings.");

        _httpClient = httpClient ?? new HttpClient();
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
    }

    public async Task<string> CompleteAsync(string prompt)
    {
        try
        {
            var requestBody = new
            {
                model = _options.Model,
                prompt = prompt,
                max_tokens = 512
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("completions", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"OpenAI API error: {response.StatusCode} - {SanitizeForLog(errorContent)}";
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseContent);
            var completion = doc.RootElement.GetProperty("choices")[0].GetProperty("text").GetString();
            return completion;
        }
        catch (Exception ex)
        {
            return $"OpenAI client error: {SanitizeForLog(ex.Message)}";
        }
    }

    private string SanitizeForLog(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        var sanitized = input.Replace("\r", " ").Replace("\n", " ");
        return sanitized.Length > 500 ? sanitized.Substring(0, 500) : sanitized;
    }
}