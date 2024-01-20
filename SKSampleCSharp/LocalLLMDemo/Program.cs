using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace LocalLLMDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            
                Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - Local LLM Connection - Demo");
                Console.ForegroundColor = ConsoleColor.Green;


                HttpClient client = new(new LocalServerClientHandler("http://localhost:1234/v1/chat/completions"));
                
                //Create a kernel builder and add the Azure OpenAI Chat Completion service
                var kernelBuilder = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion("local-model", "http://localhost:1234/v1/chat/completions","Is not Required",httpClient:client)
                    .Build();

                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    MaxTokens = 100,
                    Temperature = 0.7
                };

                var kernelFunction = KernelFunctionFactory.CreateFromPrompt("history information about chennai", openAIPromptExecutionSettings);

                var response = await kernelBuilder.InvokeAsync(kernelFunction);

                Console.WriteLine(response.GetValue<string>());


                var responseStream = kernelBuilder.InvokeStreamingAsync(kernelFunction);

                await foreach (var item in responseStream)
                {
                    Console.Write(item);
                }

                Console.Read();
            
        }
    }
}

