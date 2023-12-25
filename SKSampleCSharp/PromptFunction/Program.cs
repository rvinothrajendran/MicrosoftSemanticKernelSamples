using Microsoft.SemanticKernel;

namespace PromptFunctionDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - Prompt Functions");

            Console.WriteLine("Enter the text with city information");

            var userInput = Console.ReadLine();

            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey)
                .Build();

            //Import Prompt Functions

            var plugInDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins","location");
            kernel.ImportPluginFromPromptDirectory(plugInDirectory, "location");

            //Get the function from location plugin
            var cityKernelFunction = kernel.Plugins.GetFunction("location", "city");

            //Kernel arguments

            var kernelArguments = new KernelArguments()
            {
                {
                    "input", userInput
                }
            };


            var functionResults = await kernel.InvokeAsync(cityKernelFunction, kernelArguments);

            Console.WriteLine(functionResults.GetValue<string>());

            Console.Read();


        }
    }
}
