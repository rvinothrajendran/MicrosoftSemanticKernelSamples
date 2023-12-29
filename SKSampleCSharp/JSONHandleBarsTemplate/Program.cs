using HandlebarsDotNet;
using JSONHandleBarsTemplate.Plugins.WeatherPlugin;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace JSONHandleBarsTemplate
{
    internal class Program
    {
        private static Kernel? kernel = default;
        static async Task Main(string[] args)
        {

            Console.ForegroundColor = ConsoleColor.White;

            while (true)
            {

                Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - JSON Prompt");

                Console.WriteLine("Enter the text with city information");

                var userInput = Console.ReadLine();

                //var userInput = "please let me know information of Chennai";

                kernel = CreateKernelBuilder();

                var weatherInformation = await GetWeatherInformation(userInput!);

                var prompt = await CreatePromptTemplate(kernel!, userInput!, weatherInformation!);

                var result = await kernel!.InvokePromptAsync(prompt);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(result.GetValue<string>());
                Console.Read();
            }

        }

        private static Kernel? CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            //Add the weather plugin
            builder.Plugins.AddFromObject(new ExternalWeatherPlugin(Config.WeatherApiKey));

            return builder.Build()!;
        }

        private static async Task<string?> GetWeatherInformation(string userInput)
        {

            var weatherKernelFunction = kernel!.Plugins.GetFunction(nameof(ExternalWeatherPlugin), ExternalWeatherPlugin.GetWeather);

            //KernelArguments 
            var kernelArguments = new KernelArguments
            {
                { "input", userInput }
            };

            //Invoke the kernel function
            var weatherDetails = await kernel!.InvokeAsync(weatherKernelFunction, kernelArguments);

            var weatherInformation = weatherDetails.GetValue<string>();

            return weatherInformation;
        }

        private static async Task<string> CreatePromptTemplate(Kernel kernelBuilder,string userInput,string weatherInfo)
        {
            var handleBarsTemplate = CreateHandleBarsTemplate();

            var prompt = await handleBarsTemplate.RenderAsync(kernelBuilder, new KernelArguments()
            {
                { "input", userInput },
                { "weather", weatherInfo }
            });
            return prompt;
        }

        private static IPromptTemplate CreateHandleBarsTemplate()
        {
            PromptTemplateConfig promptTemplateConfig = new()
            {
                TemplateFormat = HandlebarsPromptTemplateFactory.HandlebarsTemplateFormat,
                Name = "JSONPrompt",
                Description = "Locate the weather data within the provided JSON.",
                InputVariables = [
                    new () { Name = "input",Description = "User's location input",IsRequired = true},
                    new () { Name = "weather",Description = "Weather data",IsRequired = true}
                    ],
                Template = "{{JsonPrompt input weather}}"
            };

            HandlebarsPromptTemplateOptions options = new()
            {
                RegisterCustomHelpers = (registerHelper, options, variables) =>
                {
                    registerHelper("JsonPrompt", (Context context, Arguments arguments) =>
                    {

                        var userInput = arguments[0].ToString();
                        var weatherInfo = arguments[1].ToString();

                        var prompt = $"First, locate the city name in the provided text: {userInput}. Afterward, " +
                                      $"retrieve only the temperature data for the identified city from the weather information, " +
                                      $"ensuring the exclusion of other details, even if the city name is found in the text also should not included " +
                                      $"For example, the current temperature in the city is: 25.0";

                        prompt += "\nweather information is given below";

                        prompt += "\n" + weatherInfo;

                        return prompt;
                        
                    });
                }
            };

            HandlebarsPromptTemplateFactory handlebarsPromptTemplate = new(options);

            return handlebarsPromptTemplate.Create(promptTemplateConfig);

        }

    }
}
