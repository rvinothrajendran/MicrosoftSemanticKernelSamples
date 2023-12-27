using HandlebarsDotNet;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace HandleBarsPromptTemplate
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Hello, Microsoft Semantic Kernel : 1.0 - Handlebars Prompt Template Factory");

            Console.ForegroundColor = ConsoleColor.Green;

            var userInput = "please let me know information of Bangalore";

            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var kernelBuilder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey)
                .Build();


            //Create a prompt template
            var handleBarsTemplate = CreateHandleBarsTemplate();

            var promptTemplate = await handleBarsTemplate.RenderAsync(kernelBuilder, new KernelArguments()
            {
                {
                    "input", userInput
                }
            });

            //Kernel execution
            var functionResult = await kernelBuilder.InvokePromptAsync(promptTemplate);

            Console.WriteLine(functionResult.GetValue<string>());

            Console.Read();

        }

        private static IPromptTemplate CreateHandleBarsTemplate()
        {
            PromptTemplateConfig promptTemplateConfig = new()
            {
                TemplateFormat = HandlebarsPromptTemplateFactory.HandlebarsTemplateFormat,
                Name = "PromptHistory",
                Description = "Retrieve history for recognized location. Summarize in two lines.",
                InputVariables =
                [
                    new InputVariable()
                    {
                        Name = "input",
                        Description = "The user's request which contains city or location name.",
                        IsRequired = true
                    }
                ],
                ExecutionSettings = new Dictionary<string, PromptExecutionSettings>()
                {
                    {
                        "default", new OpenAIPromptExecutionSettings()
                        {
                            MaxTokens = 20,
                            Temperature = 0.7,
                        }
                    }
                },
                Template = "{{history input}}"
            };

            HandlebarsPromptTemplateOptions options = new()
            {
                RegisterCustomHelpers = (registerHelper, options, variables) =>
                {
                    registerHelper("history", (Context context, Arguments arguments) =>
                    {

                        var template1 = "First, detect the city or location name in the given input";
                        var template2 =
                            "Once the city or location name is recognized, proceed to fetch historical information, requiring only a brief summary comprising two lines.";

                        var result = string.Empty;

                        foreach (var argument in arguments)
                        {
                            result = $"{template1} \"{argument}\" {template2}";
                        }

                        return result;

                    });
                }
            };

            HandlebarsPromptTemplateFactory handlebarsPromptTemplate = new(options);

            return handlebarsPromptTemplate.Create(promptTemplateConfig);

        }
    }
}
