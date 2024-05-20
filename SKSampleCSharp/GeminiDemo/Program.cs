using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace GeminiDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Google Gemini Demo");

#pragma warning disable SKEXP0070

            Kernel kernel = Kernel.CreateBuilder()
                .AddGoogleAIGeminiChatCompletion(Config.GeminiModelId,Config.GeminiApiKey)
                .Build();

            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            var chatHistory = new ChatHistory();
            chatHistory.AddUserMessage("What is Google Gemini");

            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 100,
            };

            var response = await chatService.GetChatMessageContentsAsync(chatHistory, settings);

            if (response?.Count > 0)
            {
                Console.WriteLine(response[0].Content);
            }

            Console.Read();

        }
    }
}
