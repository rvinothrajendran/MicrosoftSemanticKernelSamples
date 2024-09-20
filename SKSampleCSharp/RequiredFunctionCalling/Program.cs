using AutoFunctionCalling;
using AzureAI.Community.SK.Connector.GitHub.Models.AzureOpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using RequiredFunctionCalling.Plugins;

namespace RequiredFunctionCalling
{
#pragma warning disable SKEXP0001
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var kernel = CreateKernelBuilder();

            kernel.Plugins.AddFromType<ExternalWeatherPlugin>(nameof(ExternalWeatherPlugin));

            KernelFunction weatherKernelFunction =
                kernel.Plugins.GetFunction(nameof(ExternalWeatherPlugin), "GetWeatherAsync");


            // NewWay of Function Calling
            PromptExecutionSettings settings = new PromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Required(new List<KernelFunction>()
                {
                    weatherKernelFunction
                })
            };

            var result = await kernel.InvokePromptAsync("Provide the history of Chennai",new KernelArguments(settings));

            Console.WriteLine(result);

            Console.WriteLine("\n\nPress any key to exit...");

            Console.Read();
        }


        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddGitHubAzureOpenAIChatCompletion(GitConfig.ModelId, GitConfig.Endpoint, GitConfig.GitHubToken);

            builder.Services.AddSingleton<IPromptRenderFilter, PromptRenderFilter>();

            return builder.Build()!;
        }
    }
}
