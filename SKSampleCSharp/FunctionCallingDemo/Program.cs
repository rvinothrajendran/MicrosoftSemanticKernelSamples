using Microsoft.SemanticKernel;

namespace FunctionCallingDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 / Function Calling Demo");

            Console.WriteLine("Enter the text with city information");

            //var userInput = Console.ReadLine();

            const string userInput = "I need history information of Bangalore";
            
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var kernelBuilder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey)
                .Build();


            //Import Prompt Functions
            var plugInDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "location");
            kernelBuilder.ImportPluginFromPromptDirectory(plugInDirectory, "location");



        }
    }
}
