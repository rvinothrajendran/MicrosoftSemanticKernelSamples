using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace HelloWorld
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0");

            Console.WriteLine("Enter the text you want to summarize");

            var userInput = Console.ReadLine();

            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey)
                .Build();

            // Create a prompt to summarize the text
            var prompt = "Summarize the context of the following text {{$input}}}";

            var skFunctions = kernel.CreateFunctionFromPrompt(prompt, new OpenAIPromptExecutionSettings()
            {
                Temperature = 0.7,
                MaxTokens = 100,
            });

            // Create a kernel arguments object and add the user input
            var kernelArguments = new KernelArguments
            {
                { "input", userInput }
            };

            // Invoke the kernel and get the result
            var functionResult = await kernel.InvokeAsync(skFunctions, kernelArguments);

            // Get the result as a string
            var result = functionResult.GetValue<string>();

            Console.WriteLine(result);

            Console.Read();

        }
    }



}
