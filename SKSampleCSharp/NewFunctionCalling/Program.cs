using AutoFunctionCalling.Plugins;
using Microsoft.SemanticKernel;

namespace AutoFunctionCalling
{
#pragma warning disable SKEXP0001
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var kernel = CreateKernelBuilder();

            kernel.Plugins.AddFromType<CurrencyConverterPlugin>(nameof(CurrencyConverterPlugin));

            KernelFunction euroToInrKernelFunction = kernel.Plugins.GetFunction(nameof(CurrencyConverterPlugin), "EuroToInr");
            KernelFunction euroToDollarKernelFunction = kernel.Plugins.GetFunction(nameof(CurrencyConverterPlugin), "EuroToDollar");

            // NewWay of Function Calling
            PromptExecutionSettings settings = new PromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(new List<KernelFunction>()
                {
                    euroToInrKernelFunction,
                    euroToDollarKernelFunction
                })
            };

            var result = await kernel.InvokePromptAsync("Convert 1 Euro to Dollar",new KernelArguments(settings));

            Console.WriteLine(result);

            Console.WriteLine("\n\nPress any key to exit...");

            Console.Read();
        }


        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            return builder.Build()!;
        }
    }
}
