using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace CustomLLMYoutubeDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - YouTube Custom - LLM");

            var kernel = CreateKernelBuilder();

            var promptFunction = kernel.CreateFunctionFromPrompt("find videos {{$input}}");

            KernelArguments arguments = new() { { "input", "Microsoft Semantic Kernel" } };

            var result = await kernel.InvokeAsync(promptFunction, arguments);

            Console.WriteLine(result.GetValue<string>());

            Console.Read();
        }


        private static Kernel CreateKernelBuilder()
        {
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            builder.Services.AddKeyedSingleton<IChatCompletionService>(nameof(YouTubeChatCompletionService),
                new YouTubeChatCompletionService(Config.YoutubeKey));

            return builder.Build()!;
        }
    }
}
