using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Web.BingMap.SuggestionAddress;
using Microsoft.SemanticKernel;

namespace BingMapPluginDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Bing Map Plugin in Semantic Kernel - Demo");

            Console.ForegroundColor = ConsoleColor.Green;

            var kernel = CreateKernelBuilder();

            //Create Kernel function
            var searchKernelFunction = kernel.Plugins.GetFunction(nameof(SuggestionAddressPlugin), "SearchSuggestion");

            //Call the function
            var result = await kernel.InvokeAsync(searchKernelFunction, new KernelArguments()
            {
                { "Suggestion", "Coffee" },
                { "city", "Thanjavur" }
            });

            Console.WriteLine(result.GetValue<string>());
            Console.Read();
        }

        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            //Add the Bing Map plugin
            SuggestionAddressPlugin suggestionAddressPlugin = new(Config.BingMapKey);

            builder.Plugins.AddFromObject(suggestionAddressPlugin);

            return builder.Build()!;
        }
    }
}
