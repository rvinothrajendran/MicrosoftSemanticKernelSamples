using AzureAI.Community.SK.Connector.GitHub.Model;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace GitHubSK
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Hello,Connect with GitHub AI Modules!!!\n");
            
            var kernel = CreateKernelBuilder();

            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            OpenAIPromptExecutionSettings settings = new()
            {
                MaxTokens = 100,
            };

            var chatHistory = new ChatHistory();

            chatHistory.AddUserMessage("Could you provide me with the history of India");

            var modelResult = await chatService.GetChatMessageContentAsync(chatHistory, settings, kernel);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(modelResult.Content);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nPress any key to exit...");

            Console.Read();
        }

        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddGitHubChatCompletion(Config.ModelName, Config.Endpoint, Config.GitHubToken);           

            return builder.Build()!;
        }
    }

    
}
