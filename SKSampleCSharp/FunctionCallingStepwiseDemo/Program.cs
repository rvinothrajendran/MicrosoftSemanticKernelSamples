using FunctionCallingStepwiseDemo.Plugins.WeatherPlugin;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;

namespace FunctionCallingStepwiseDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - FunctionCalling-StepwisePlanner Demo");

            var kernel = CreateKernelBuilder();

            var prompt = "Provide me with the historical and weather details for Chennai.";

#pragma warning  disable SKEXP0061

            //Create a function calling stepwise planner
            FunctionCallingStepwisePlanner functionCallingStepwisePlanner = new();

            //Execute the planner
            var result = await functionCallingStepwisePlanner.ExecuteAsync(kernel!, prompt);

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine();

            //Display the final answer
            Console.WriteLine(result.FinalAnswer);

            //Display the chat history ( plan )
            if (result.ChatHistory != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                foreach (var chat in result.ChatHistory)
                {
                    Console.WriteLine($"{chat.Role} - {chat.Content}");
                }
            }
            Console.Read();

#pragma warning restore SKEXP0061

        }


        private static Kernel? CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            //Add the location plugin
            var plugInDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "location");
            builder.Plugins.AddFromPromptDirectory(plugInDirectory, "location");
            
            //Add the weather plugin
            builder.Plugins.AddFromObject(new ExternalWeatherPlugin(Config.WeatherApiKey));

            return builder.Build()!;
        }
    }
}
