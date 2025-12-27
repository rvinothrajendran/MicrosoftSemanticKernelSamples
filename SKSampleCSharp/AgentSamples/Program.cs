using System.ClientModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AgentSamples
{
#pragma warning disable SKEXP0110
    internal class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Hello, Agent Samples in Semantic Kernel");

            var kernel = CreateKernelBuild();

            var openAIClientProvider = OpenAIClientProvider.ForAzureOpenAI(new ApiKeyCredential(Config.ApiKey),
                new Uri(Config.Endpoint));

            OpenAIAssistantDefinition assistantDefinition = new(Config.ModelName)
            {
                Name = "Agent Sample Assistant",
                Instructions = "You are a helpful AI assistant. Answer questions based on user input."
            };

            var agent = await OpenAIAssistantAgent.CreateAsync(kernel, openAIClientProvider, assistantDefinition);

            var threadId = await agent.CreateThreadAsync();

            await agent.AddChatMessageAsync(threadId, new ChatMessageContent(AuthorRole.User, "What is Semantic Kernel?"));

            Console.ForegroundColor = ConsoleColor.Green;

            await foreach (var content in agent.InvokeAsync(threadId))
            {
                Console.WriteLine(content);
            }

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Press any key to exit");

            Console.Read();

        }

        public static Kernel CreateKernelBuild()
        {
            var kernelBuilder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.ModelName, Config.Endpoint, Config.ApiKey).Build();

            return kernelBuilder;
        }
    }
}
