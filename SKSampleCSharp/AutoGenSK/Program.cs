using AutoGen.Core;
using AutoGen.SemanticKernel;
using AzureAI.Community.SK.Connector.GitHub.Model;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AutoGenSK
{
    internal class Program
    {
        static async Task Main(string[] args)
        { 
            Console.WriteLine("Hello, AutoGen - SK");

            var kernel = CreateKernelBuilder();

            var chatMessageContent = new ChatMessageContent(AuthorRole.User, 
                "Create a Python program to add two numbers.");

            var skAgent = new SemanticKernelAgent(kernel, "SKAgent");

            var messageContent = MessageEnvelope.Create(chatMessageContent);

            var response = await skAgent.SendAsync(messageContent);

            ChatMessageContent? responseContent = null;

            if (response is MessageEnvelope<ChatMessageContent> envelope)
            {
                responseContent = envelope.Content;
            }

            Console.WriteLine(responseContent?.Content);

            Console.ReadLine();
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
