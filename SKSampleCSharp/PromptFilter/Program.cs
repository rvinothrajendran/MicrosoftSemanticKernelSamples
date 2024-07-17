using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;

namespace PromptFilter
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - Kernel - PromptRenderFilter\n");

            var kernel = CreateKernelBuilder();

            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            var chatHistory = new ChatHistory();
            chatHistory.AddUserMessage("let me know history of TamilNadu");

            OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 100,
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            var response = await chatService.GetChatMessageContentsAsync(chatHistory, settings, kernel);

            Console.ForegroundColor = ConsoleColor.White;

            if (response.Count > 0)
            {
                Console.WriteLine(response[0].Content);
            }

            Console.Read();
        }

        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            //Add the location plugin
            var plugInDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "location");
            builder.Plugins.AddFromPromptDirectory(plugInDirectory, "location");

            builder.Services.AddSingleton<IPromptRenderFilter, PromptRenderFilter>();

            return builder.Build()!;
        }
    }
}
