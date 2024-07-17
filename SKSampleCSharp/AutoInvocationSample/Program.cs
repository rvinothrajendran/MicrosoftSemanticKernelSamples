using AutoInvocationFilterSample.Plugins.WeatherPlugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AutoInvocationFilterSample
{
    internal class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - Kernel - AutoFunctionInvocation-Filters\n");

            var kernel = CreateKernelBuilder();

            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            var chatHistory = new ChatHistory("You are a friendly assistant you can handle only historical , weather");
            chatHistory.AddUserMessage("what is weather in chennai");

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


        static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            //Add the weather plugin
            builder.Plugins.AddFromObject(new ExternalWeatherPlugin(Config.WeatherApiKey), nameof(ExternalWeatherPlugin));

            builder.Services.AddSingleton<IAutoFunctionInvocationFilter, AutoFunctionInvocationFilter>();
            builder.Services.AddSingleton<IFunctionInvocationFilter, FunctionInvocationFilter>();

            return builder.Build();
        }
    }

    public class AutoFunctionInvocationFilter : IAutoFunctionInvocationFilter
    {
        public async Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, Func<AutoFunctionInvocationContext, Task> next)
        {
            Console.WriteLine("\nBefore Prompt Executed");

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

            if (context.Function.PluginName == nameof(ExternalWeatherPlugin) &&
                context.Function.Name == ExternalWeatherPlugin.GetWeather)
            {
                Console.WriteLine(context.Function.PluginName);
                Console.WriteLine(context.Function.Name);
            }

            await next(context);

            Console.WriteLine("\n After FunctionInvocationFilter Executed");

        }
    }
}