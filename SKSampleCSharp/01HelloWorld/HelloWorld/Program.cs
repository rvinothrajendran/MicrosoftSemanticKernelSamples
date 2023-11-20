using ConfigSettings;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.TemplateEngine;

namespace HelloWorld
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            //1. Create Semantic Kernel Builder

            var builder = new KernelBuilder();
            builder.WithAzureTextCompletionService(ConfigParameters.DeploymentOrModelId, ConfigParameters.Endpoint, ConfigParameters.ApiKey);
            var kernel = builder.Build();

            //2.Create Prompt Template 

            var template = "Summarize the context of the following text {{$prompt}}";

            var aiRequestSettings = new AIRequestSettings();
            aiRequestSettings.ExtensionData.Add("prompt", "Summarize the context of the following text");
            aiRequestSettings.ExtensionData.Add("max_tokens", 1000);
            aiRequestSettings.ExtensionData.Add("temperature", 0.7);
            aiRequestSettings.ExtensionData.Add("top_p", 1);

            var promptConfig = new PromptTemplateConfig();
            promptConfig.ModelSettings.Add(aiRequestSettings);
            
            var promptTemplate = new PromptTemplate(template, promptConfig,kernel);

            //3. Create Semantic Function

            //var semanticConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var regSemanticFunc = kernel.RegisterSemanticFunction("SummarizeContext", promptConfig,
                new PromptTemplate(template, promptConfig, kernel.PromptTemplateEngine));

            //4. Invoke Semantic Function

            Console.WriteLine("Write the text you want understand the context");
            var input = Console.ReadLine();

            SKContext context = kernel.CreateNewContext();
            context.Variables.Set("input", input);

            var result = await regSemanticFunc.InvokeAsync(context);

            //5. Print Result
            Console.WriteLine(result.GetValue<string>());

        }
    }
}