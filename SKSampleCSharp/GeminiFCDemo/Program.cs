using GeminiFCDemo.Plugins;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;

namespace GeminiFCDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Google Gemini - Function Calling Demo");

#pragma warning disable SKEXP0070

            Kernel kernel = Kernel.CreateBuilder()
                .AddGoogleAIGeminiChatCompletion(Config.GeminiModelId, Config.GeminiApiKey)
                .Build();

            kernel.Plugins.AddFromType<CurrencyConverterPlugin>();
           
            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("If you can’t find the answer, try using function calls to locate the information.");
            chatHistory.AddUserMessage("what is google gemini & let me know about 1 euro exchange rate of its currency to INR?");

            GeminiPromptExecutionSettings settings = new()
            {
                ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions
            };

            var response = await chatService.GetChatMessageContentsAsync(chatHistory,settings,kernel);

            if (response?.Count > 0)
            {
                Console.WriteLine(response[0].Content);
            }
            
            Console.WriteLine("Press any key to exit");

            Console.Read();

        }
    }
}
