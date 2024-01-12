using Microsoft.KernelMemory;

namespace MemoryServerlessDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - Kernel Memory - Demo");

            var embeddingAzureConfig = CreateAzureOpenAIConfig(Config.DeploymentEmbeddingModel,AzureOpenAIConfig.APITypes.EmbeddingGeneration);

            var textAzureConfig = CreateAzureOpenAIConfig(Config.DeploymentOrModelId, AzureOpenAIConfig.APITypes.ChatCompletion);

            var kernelMemory = new KernelMemoryBuilder()
                .WithAzureOpenAITextEmbeddingGeneration(embeddingAzureConfig)
                .WithAzureOpenAITextGeneration(textAzureConfig)
                .WithSimpleVectorDb()
                .Build();


            await kernelMemory.ImportDocumentAsync(".NET.pdf");

            var result = await kernelMemory.AskAsync("What is .NET");

            Console.WriteLine(result.Result);

            Console.Read();
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
