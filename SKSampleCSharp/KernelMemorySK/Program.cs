using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;

namespace KernelMemorySK
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - Kernel Memory IN SK - Demo");

            var kernelMemory = await CreateKernelMemory();

            var kernel = CreateKernelBuilder();

            kernel.ImportPluginFromObject(new MemoryPlugin(kernelMemory),nameof(MemoryPlugin));

            var result = await kernel.InvokeAsync(nameof(MemoryPlugin), "ask", new KernelArguments()
            {
                {
                    "question", "What is .NET"
                }
            });

            Console.WriteLine(result.GetValue<string>());

            //var result = await kernelMemory.AskAsync("What is .NET");

            //Console.WriteLine(result.Result);

            Console.Read();
        }

        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            return builder.Build()!;
        }

        private static async Task<IKernelMemory> CreateKernelMemory()
        {
            var embeddingAzureConfig = CreateAzureOpenAIConfig(Config.DeploymentEmbeddingModel, AzureOpenAIConfig.APITypes.EmbeddingGeneration);

            var textAzureConfig = CreateAzureOpenAIConfig(Config.DeploymentOrModelId, AzureOpenAIConfig.APITypes.ChatCompletion);

            var kernelMemory = new KernelMemoryBuilder()
                .WithAzureOpenAITextEmbeddingGeneration(embeddingAzureConfig)
                .WithAzureOpenAITextGeneration(textAzureConfig)
                .WithSimpleVectorDb()
                .Build();


            await kernelMemory.ImportDocumentAsync(".NET.pdf");
            return kernelMemory;
        }

        static AzureOpenAIConfig CreateAzureOpenAIConfig(string deploymentName, AzureOpenAIConfig.APITypes apiTypes)
        {
            return new AzureOpenAIConfig()
            {
                APIKey = Config.ApiKey,
                Deployment = deploymentName,
                Endpoint = Config.Endpoint,
                APIType = apiTypes,
                Auth = AzureOpenAIConfig.AuthTypes.APIKey
            };
        }
    }
}
