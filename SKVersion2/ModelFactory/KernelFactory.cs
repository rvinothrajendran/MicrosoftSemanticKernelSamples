using System.ClientModel;
using Microsoft.SemanticKernel;
using OpenAI;

namespace LLMModelFactory
{
#pragma warning disable SKEXP0070

    public static class KernelFactory
    {
        private static readonly Config Config = new();
        /// <summary>
        /// Creates a Semantic Kernel instance based on the selected llmModel.
        /// </summary>
        public static Kernel CreateKernelBuilder(LLMModel llmModel)
        {
            return llmModel switch
            {
                LLMModel.Azure => AzureOpenAIKernel(),
                LLMModel.Ollama => LocalOllamaClientKernel(),
                LLMModel.GitHub => GitHubModel(),
                _ => throw new ArgumentOutOfRangeException(nameof(llmModel), $"Unknown llmModel: {llmModel}")
            };
        }

        /// <summary>
        /// Creates a kernel configured for Azure OpenAI.
        /// </summary>
        private static Kernel AzureOpenAIKernel()
        {
            if (string.IsNullOrWhiteSpace(Config.DeploymentOrModelId) || string.IsNullOrWhiteSpace(Config.Endpoint) || string.IsNullOrWhiteSpace(Config.ApiKey))
            {
                throw new InvalidOperationException("Azure OpenAI configuration is missing required values.");
            }
            var builder = Kernel.CreateBuilder();
            builder.Services.AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);
            return builder.Build();
        }

        /// <summary>
        /// Creates a kernel configured for a local Ollama instance.
        /// </summary>
        private static Kernel LocalOllamaClientKernel()
        {
            var builder = Kernel.CreateBuilder();
            builder.AddOllamaChatCompletion(Config.LlamaModel, new Uri(Config.OllamaUri));
            return builder.Build();
        }

        /// <summary>
        /// Creates a kernel configured for GitHub's OpenAI endpoint.
        /// </summary>
        private static Kernel GitHubModel()
        {
            var client = new OpenAIClient(new ApiKeyCredential(Config.GitHubToken),
                new OpenAIClientOptions { Endpoint = new Uri(Config.GitHubUri)});

            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(Config.GitHubModel, client);
            return builder.Build();
        }
    }
}
