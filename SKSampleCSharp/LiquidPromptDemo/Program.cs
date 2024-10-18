using AzureAI.Community.SK.Connector.GitHub.Models.AzureOpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Liquid;

namespace LiquidPromptDemo
{
#pragma warning disable SKEXP0040
    internal class Program
    {
        static async Task Main(string[] args)
        {

            var kernel = CreateKernelBuilder();

            var template = "Retrieve the historical information based on the city {{CityName}} " +
                           "and make sure to include the user's name, {{name}}, in your response.";


            Console.ForegroundColor = ConsoleColor.White;
            var PromptTemplateConfig = new PromptTemplateConfig()
            {
                TemplateFormat = "liquid",
                Template = template,
                Name = "LiquidPromptTemplate"
            };

            var factory = new LiquidPromptTemplateFactory();

            if (factory.TryCreate(PromptTemplateConfig, out var promptTemplate))
            {
                var renderPrompt = await promptTemplate.RenderAsync(kernel, new KernelArguments()
                {
                    {
                        "CityName" , "Thanjavur"
                    },
                    {
                        "name" , "vinoth"
                    }
                });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(renderPrompt);

                KernelFunction kernelFunction = kernel.CreateFunctionFromPrompt(renderPrompt);

                var result = await kernel.InvokeAsync(kernelFunction);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(result.GetValue<string>());
            }
            else
            {
                Console.WriteLine("Failed to create the prompt Template");
            }
            
            Console.ReadLine();

        }

        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddGitHubAzureOpenAIChatCompletion(GitConfig.ModelId, GitConfig.Endpoint, GitConfig.GitHubToken);

            return builder.Build()!;
        }

    }
}
