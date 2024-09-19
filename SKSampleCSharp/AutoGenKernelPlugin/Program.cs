using AutoGen.Core;
using AutoGen.SemanticKernel.Extension;
using AutoGenKernelPlugin.Plugins.WeatherPlugin;
using AzureAI.Community.SK.Connector.GitHub.Model;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AutoGenKernelPlugin
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("AutoGen Agent using SK Plugin");

            Console.ForegroundColor = ConsoleColor.Green;

            var kernel = CreateKernelBuilder();

            kernel.Plugins.AddFromObject(new ExternalWeatherPlugin());

            var chatMessageContent = new ChatMessageContent(AuthorRole.User,
                "history of chennai and weather information");

            var systemMessage = "You are retrieving historical data and find weather information based on that city.";

            //Create Settings

            var settings = new OpenAIPromptExecutionSettings()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            // Create Agent

            var autoGenAgent = kernel.ToSemanticKernelAgent("AutogenPluginAgent", systemMessage, settings);

            var message = MessageEnvelope.Create(chatMessageContent);

            var response = await autoGenAgent.SendAsync(message);

            ChatMessageContent? responseContent = null;

            if (response is MessageEnvelope<ChatMessageContent> envelope)
            {
                responseContent = envelope.Content;
            }

            Console.WriteLine(responseContent?.Content);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n\nPress any key to exit");
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
