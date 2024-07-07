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

            builder.Services.AddSingleton<IPromptRenderFilter, PromptRenderFilters>();
            builder.Services.AddSingleton<IFunctionInvocationFilter, FunctionInvocationFilter>();
            builder.Services.AddSingleton<IAutoFunctionInvocationFilter, AutoFunctionInvocationFilter>();

            return builder.Build()!;
        }
    }

    public class PromptRenderFilters : IPromptRenderFilter
    {
        public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
        {
            Console.WriteLine("\nBefore OnPromptRenderAsync");
            await next(context);
            Console.WriteLine("\nAfter OnPromptRenderAsync");
        }
    }

    public class AutoFunctionInvocationFilter : IAutoFunctionInvocationFilter
    {
        public async Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, Func<AutoFunctionInvocationContext, Task> next)
        {
            Console.WriteLine("\nAutoFunctionInvocationFilter");

            Console.WriteLine(context.Function.PluginName);
            Console.WriteLine(context.Function.Name);

            await next(context);

            Console.WriteLine("\nAfter AutoFunctionInvocationFilter Executed");
        }
    }
    public class FunctionInvocationFilter : IFunctionInvocationFilter
    {
        public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
        {
            Console.WriteLine("\nFunctionInvocationFilter");

            Console.WriteLine(context.Function.PluginName);
            Console.WriteLine(context.Function.Name);
            
            
            await next(context);

            Console.WriteLine("\n After FunctionInvocationFilter Executed");

        }
    }
}
