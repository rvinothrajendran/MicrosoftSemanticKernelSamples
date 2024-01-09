using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Web.BingMap.SuggestionAddress;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using TinyCopilot.Plugins.WeatherPlugin;

namespace TinyCopilotGPT4VisionDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - Tiny-Copilot with GPT4 - Vision- Demo");

            Console.WriteLine("\nGreetings! I'm Tiny-Copilot, here to assist with historical, weather, and city-specific suggestions such as coffee shops and hotels etc.");

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("");

            var kernel = CreateKernelBuilder();

            VisionService visionService = new();

            IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            ChatHistory chatHistory = new(
                " You are a friendly assistant you can handle only historical , weather and suggestion (like coffee,hotels,etc..) address details based on the city and request approval before taking any consequential actions. If the user doesn't provide enough information for you to complete a task, you will keep asking questions until you have enough information to complete the task");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;

                //Get the user query 
                Console.Write("User > ");
                var userMessages = Console.ReadLine()!;

                if (IsUrlValid(userMessages))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nTinyCopilot > Analyzing the image. Please wait a moment.");
                    
                    userMessages = await visionService.FindCityNameAsync(userMessages);

                    Console.WriteLine($"TinyCopilot > {userMessages}\n");
                }

                //Add the user query to the chat history
                chatHistory.AddUserMessage(userMessages!);


                //Create the execution settings 
                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                    MaxTokens = 100
                };

                //Get the response from the chat completion service
                var response = chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory,
                        openAIPromptExecutionSettings, kernel);


                Console.ForegroundColor = ConsoleColor.Green;

                Console.Write("\nTinyCopilot > ");

                string combinedResponse = string.Empty;
                await foreach (var message in response)
                {
                    //Write the response to the console
                    Console.Write(message);

                    combinedResponse += message;
                }

                Console.WriteLine();
                Console.WriteLine();

                //Add the response to the chat history
                chatHistory.AddAssistantMessage(combinedResponse);

            }

            Console.Read();
        }

        static bool IsUrlValid(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out Uri? result) && result.Scheme == Uri.UriSchemeHttp || result?.Scheme == Uri.UriSchemeHttps;
        }

        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            //Add the location plugin
            var plugInDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "location");
            builder.Plugins.AddFromPromptDirectory(plugInDirectory, "location");

            //Add the weather plugin
            builder.Plugins.AddFromObject(new ExternalWeatherPlugin(Config.WeatherApiKey));

            //Add the Bing Map plugin
            SuggestionAddressPlugin suggestionAddressPlugin = new(Config.BingMapKey);

            builder.Plugins.AddFromObject(suggestionAddressPlugin);


            return builder.Build()!;
        }
    }
}
