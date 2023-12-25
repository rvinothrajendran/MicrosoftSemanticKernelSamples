using Microsoft.SemanticKernel;

namespace PromptYAMLDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - YAML Config");

            Console.WriteLine("Enter the text with city name");

            var userInput = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Green;

            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var kernelBuilder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey)
                .Build();


            //Read the YAML config file
            var yamlDirectory = Path.Combine(Directory.GetCurrentDirectory(),"Plugins","Yaml","City.yaml");

            var yamlPromptConfig = await File.ReadAllTextAsync(yamlDirectory);

            //Create a SK function from the YAML config
            var yamlfunction = kernelBuilder.CreateFunctionFromPromptYaml(yamlPromptConfig);

            //Create Kernel arguments
            var kernelArguments = new KernelArguments()
            {
                {
                    "input", userInput
                }
            };

            //Run the SK function or Invoke the SK function

            var functionResult = await kernelBuilder.InvokeAsync(yamlfunction, kernelArguments);

            Console.WriteLine(functionResult.GetValue<string>());

            Console.Read();

        }
    }
}
