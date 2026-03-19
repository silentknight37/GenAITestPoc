using GenAITest.Engine.Abstractions;
using OpenAI;
using OpenAI.Chat;

namespace GenAITest.Pipeline.LLM;

public sealed class OpenAiLLMClient : ILLMClient
{
    private readonly ChatClient _chatClient;

    public OpenAiLLMClient()
    {
        var apiKey ="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";

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OPENAI_API_KEY not set.");

        var client = new OpenAIClient(apiKey);

        _chatClient = client.GetChatClient("gpt-4o-mini");
    }

    public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage("You are a senior .NET test engineer."),
            ChatMessage.CreateUserMessage(prompt)
        };

        try
        {
            var response = await _chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
            return response.Value.Content[0].Text;
        }
        catch (Exception ex) when (ex.Message.Contains("insufficient_quota", StringComparison.OrdinalIgnoreCase))
        {
            return "// ERROR: OpenAI quota exceeded. Please enable billing or use local model (Ollama).\n";
        }
    }
}