using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Interface;
using InterfacePluginsDemo.Plugins.MathsPlugin;
using Microsoft.SemanticKernel;

namespace InterfacePluginsDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - Interface Plugins Demo");

            Console.ForegroundColor = ConsoleColor.Green;


            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var kernelBuilder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey)
                .Build();


            //Load the plugin

            kernelBuilder.ImportPluginFromInterface<IAddPlugin>(new MathPlugin(), nameof(IAddPlugin));


            double answer = await kernelBuilder.InvokeAsync<double>(
                nameof(IAddPlugin), "Add",
                new() {
                    { "number1", 12 },
                    { "number2", 24 }
                }
            );

            Console.WriteLine($"The Add is {answer}.");


            //Print the result

            Console.Read();
        }

    }
}
