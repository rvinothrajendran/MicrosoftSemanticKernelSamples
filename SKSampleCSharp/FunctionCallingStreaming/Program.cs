using FunctionCallingStreaming.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FunctionCallingStreaming
{
#pragma warning disable SKEXP0001
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Hello,Microsoft Semantic Kernel - FunctionCalling");

            var kernel = CreateKernelBuilder();

            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            OpenAIPromptExecutionSettings settings = new()
            {
                ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions
            };

            var chatHistory = new ChatHistory();

            chatHistory.AddUserMessage("Could you provide me with the history of Austria and the current exchange rate of its currency to INR?" +
                                       "and let me know weather information also");


            FunctionCallContentBuilder functionCallContentBuilder = new();

            await foreach (var streamContent in chatService.GetStreamingChatMessageContentsAsync(chatHistory, settings,
                               kernel))
            {
                functionCallContentBuilder.Append(streamContent);
            }

            var functionCalls = functionCallContentBuilder.Build();

            if (functionCalls.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                ChatMessageContent chatMessageContent = new ChatMessageContent(AuthorRole.Assistant, content: null);

                chatHistory.Add(chatMessageContent);

                foreach (var functionCallContent in functionCalls)
                {
                    chatMessageContent.Items.Add(functionCallContent);

                    var functionResultContent = await functionCallContent.InvokeAsync(kernel);

                    chatHistory.Add(functionResultContent.ToChatMessage());

                    await foreach (var streamContent in chatService.GetStreamingChatMessageContentsAsync(chatHistory,
                                       settings,
                                       kernel))
                    {
                        if (streamContent.Content is not null)
                            Console.Write(streamContent.Content);
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("\n\nPress any key to exit...");
            Console.Read();
        }


        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            builder.Plugins.AddFromType<CurrencyConverterPlugin>();
            builder.Plugins.AddFromObject(new ExternalWeatherPlugin(""));

            return builder.Build()!;
        }
    }
}
