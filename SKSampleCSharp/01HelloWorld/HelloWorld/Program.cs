using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.TemplateEngine;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HelloWorld
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            //getting input from user
            Console.WriteLine("Write the text you want understand the context");
            string? input = Console.ReadLine();

#if  DISABLE
            

            //1. Create Semantic Kernel Builder

            var builder = new KernelBuilder();
            builder.WithAzureTextCompletionService(ConfigParameters.DeploymentOrModelId, ConfigParameters.Endpoint, ConfigParameters.ApiKey);
            var kernel = builder.Build();

            //2.Create Prompt Template 

            var template = "Summarize the context of the following text {{$prompt}}";

            var aiSettings = new AIRequestSettings();
            aiSettings.ExtensionData = new Dictionary<string, object>()
            {
                { "MaxTokens", 1000 },
                { "Temperature", 0.7 },
                { "TopP", 1 },
            };

            var promptConfig = new PromptTemplateConfig
            {
                ModelSettings = new List<AIRequestSettings> { aiSettings }
            };

            var promptTemplate = new PromptTemplate(template, promptConfig,kernel);

            //3. Create Semantic Function

            //var semanticConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            if (input is not null)
            {
                string prompt = input;
                var regSemanticFunc = kernel.RegisterSemanticFunction("Summarize the context of the following text ",input,promptConfig);

                //4. Invoke Semantic Function



                var result = await kernel.RunAsync(regSemanticFunc);

                //5. Print Result
                Console.WriteLine(result.GetValue<string>());
            }

#endif
        }
    }
}