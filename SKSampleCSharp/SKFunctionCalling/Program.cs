using System.Text.Json;
using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SKFunctionCalling.Plugins;

namespace SKFunctionCalling
{
#pragma warning disable SKEXP0001
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var kernel = CreateKernelBuilder();

            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            OpenAIPromptExecutionSettings settings = new()
            {
                ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions
            };

            var chatHistory = new ChatHistory();

            chatHistory.AddUserMessage("Could you provide me with the history of Austria and the current exchange rate of its currency to INR?");

            var modelResult = await chatService.GetChatMessageContentAsync(chatHistory, settings, kernel);

            Console.WriteLine(modelResult.Content);

            chatHistory.Add(modelResult);
            
            foreach(var kernelContent in modelResult.Items)
            {
                if (kernelContent.InnerContent is ChatCompletionsFunctionToolCall chatCompletionsFunctionTool)
                {
                    string result;
                    if (kernel.Plugins.TryGetFunctionAndArguments(chatCompletionsFunctionTool, out var kernelFunction,
                            out var kernelArguments))
                    {
                        var functionResult = await kernelFunction.InvokeAsync(kernel, kernelArguments);
                        result = functionResult.ToString();
                    }
                    else
                    {
                        result = "Error: Could not find the function.";
                    }

                    result = JsonSerializer.Serialize(result);

                    var metaData = new Dictionary<string, object>
                    {
                        { OpenAIChatMessageContent.ToolIdProperty, chatCompletionsFunctionTool.Id }
                    };

                    chatHistory.AddMessage(AuthorRole.Tool, result,metadata:metaData!);

                    var modelToolResult =await chatService.GetChatMessageContentAsync(chatHistory, settings, kernel);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(modelToolResult.Content);
                }
            }

            Console.WriteLine("\n\nPress any key to exit...");
            Console.Read();
        }


        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            builder.Plugins.AddFromType<CurrencyConverterPlugin>();
            
            return builder.Build()!;
        }
    }
}
