using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;


namespace CustomLLMYoutubeStreamingDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - YouTube Custom - LLM");

            var kernel = CreateKernelBuilder();

            var promptFunction = kernel.CreateFunctionFromPrompt("find videos {{$input}}");

            KernelArguments arguments = new() { { "input", "Microsoft Semantic Kernel" } };

            var result = kernel.InvokeStreamingAsync(promptFunction, arguments);

            await foreach (var item in result)
            {
                Console.WriteLine(item);
                Thread.Sleep(100);
            }

            Console.Read();
        }


        private static Kernel CreateKernelBuilder()
        {
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey)
                .AddYouTubeChatCompletion(Config.YoutubeKey);

            return builder.Build()!;
        }
    }
}
