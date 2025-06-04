using LLMModelFactory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;

namespace MCPClient.Azure
{
    /// <summary>
    /// Entry point for the MCPClient.Azure application.
    /// Connects to the MCP server, retrieves available tools, and demonstrates Semantic Kernel usage.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main method for the MCPClient.Azure application.
        /// Connects to the MCP server, lists available tools, and invokes a prompt using Semantic Kernel.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, MCP Server");

            try
            {
                Console.WriteLine("Semantic Kernel Connect to Azure");

                var isServer = true;

                // Set the host URL based on the environment
                var hostUrl = isServer ? 
                                    "https://mcpserverazuredemo-gcaggahaaxcugbfy.canadacentral-01.azurewebsites.net/" :
                                    "http://localhost:5000";

                SseClientTransportOptions options = new SseClientTransportOptions()
                {
                    Endpoint = new Uri(hostUrl)
                };

                IClientTransport transport = new SseClientTransport(options);

                // Create the MCP client using the specified transport
                var mcpClient = await McpClientFactory.CreateAsync(transport);

                // Get the list of available tools from the server
                var tools = await mcpClient.ListToolsAsync().ConfigureAwait(false);

                foreach (var mcpClientTool in tools)
                {
                    Console.WriteLine($"Tool: {mcpClientTool.Name} - {mcpClientTool.Description}");
                }

                Console.WriteLine("Ready to use Kernel");

                // Create a Semantic Kernel instance with the specified LLM model
                var kernel = KernelFactory.CreateKernelBuilder(LLMModel.Ollama);

                OpenAIPromptExecutionSettings executionSettings = new()
                {
                    Temperature = 0,
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new()
                        { RetainArgumentTypes = true })
                };

                // Add the available tools as plugins to the kernel
                kernel.Plugins.AddFromFunctions("ASPServer", tools.Select(aiFunction => aiFunction.AsKernelFunction()));

                var prompt = "get the list of books";
                var result = await kernel.InvokePromptAsync(prompt, new(executionSettings)).ConfigureAwait(false);

                Console.WriteLine($"\n\n{prompt}\n{result}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
            }
            Console.Read();
        }
    }
}
