﻿using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FunctionCallingDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 / Function Calling Demo");

            //var userInput = Console.ReadLine();

            const string userInput = "I need history information of Bangalore";
            
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var kernelBuilder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey)
                .Build();

            //Import Prompt Functions
            var plugInDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "location");
            kernelBuilder.ImportPluginFromPromptDirectory(plugInDirectory, "location");

            Console.ForegroundColor = ConsoleColor.Green;

            //OpenAIPrompt Settings

            var openAIPromptSettings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 50,
                ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions
            };

            //Chat Completion Service
            var chatCompletionService = kernelBuilder.GetRequiredService<IChatCompletionService>();


            //Chat History
            var chatHistory = new ChatHistory();
            chatHistory.AddUserMessage(userInput);

            var result =
                await chatCompletionService.GetChatMessageContentAsync(chatHistory, openAIPromptSettings,
                    kernelBuilder);

            //FunctionToolCalls

            var openAIChatMessageContent = (OpenAIChatMessageContent)result;

            var functionToolCalls = openAIChatMessageContent.GetOpenAIFunctionToolCalls();

            foreach (var functionTool in functionToolCalls)
            {
                kernelBuilder.Plugins.TryGetFunctionAndArguments(functionTool, out var kernelFunction,
                    out var kernelArguments);

                var functionResult = await kernelBuilder.InvokeAsync(kernelFunction!, kernelArguments!);

                var functionResponse = functionResult.GetValue<string>();
                Console.WriteLine(functionResponse);
                
                chatHistory.AddMessage(AuthorRole.Tool, functionResponse!);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("Final Result");
            Console.Read();

        }
    }
}
