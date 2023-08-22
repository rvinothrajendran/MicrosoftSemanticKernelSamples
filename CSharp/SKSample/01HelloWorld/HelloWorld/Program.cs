using ConfigSettings;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel;

namespace HelloWorld
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            //1. Create Semantic Kernel Builder

            var builder = new KernelBuilder();
            builder.WithAzureTextCompletionService(Settings.DeploymentName, Settings.Endpoint, Settings.ApiKey);
            var kernel = builder.Build();

            //2.Create Prompt Template 

            var template = "Summarize the context of the following text {{$prompt}}";

            var promptConfig = new PromptTemplateConfig()
            {
                Completion =
                {
                    MaxTokens = 1000,
                    Temperature = 0.7,
                    TopP = 1
                }
            };

            var promptTemplate = new PromptTemplate(template, promptConfig,kernel);

            //3. Create Semantic Function

            var semanticConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var regSemanticFunc = kernel.RegisterSemanticFunction("SummarizeContext", semanticConfig);

            //4. Invoke Semantic Function

            Console.WriteLine("Write the text you want understand the context");
            var input = Console.ReadLine();

            var result = await regSemanticFunc.InvokeAsync(input);

            //5. Print Result

        }
    }
}