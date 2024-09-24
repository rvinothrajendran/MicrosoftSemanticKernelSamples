using AzureAI.Community.SK.Connector.GitHub.Models.AzureOpenAI;
using Microsoft.SemanticKernel;
using NoneFunctionCalling.Plugins;

namespace NoneFunctionCalling
{
#pragma warning disable SKEXP0001
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Hello, World!");

            var kernel = CreateKernelBuilder();

            kernel.Plugins.AddFromType<ExternalWeatherPlugin>(nameof(ExternalWeatherPlugin));

            KernelFunction weatherKernelFunction =
                kernel.Plugins.GetFunction(nameof(ExternalWeatherPlugin), "GetWeatherAsync");

            // NewWay of Function Calling
            PromptExecutionSettings settings = new PromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.None()
            };

            var result =
                await kernel.InvokePromptAsync("Provide history and weather information based on chennai",
                    new KernelArguments(settings));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(result);

            Console.WriteLine("\n\nPress any key to exit...");

            Console.Read();
        }


        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddGitHubAzureOpenAIChatCompletion(GitConfig.ModelId, GitConfig.Endpoint, GitConfig.GitHubToken);

            return builder.Build()!;
        }
    }
}