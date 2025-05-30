using LLMModelFactory;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace MCPClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Microsoft Extensions AI!");


            var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
            {
                Name = "Local-MCP-Server",
                Command = "dotnet",
                Arguments =
                [
                    "run",
                    "--project",
                    "C:\\Vinoth\\SK\\SKVersion2\\MCP\\SKPluginInMCPServer\\MCPServerSKPlugins",
                    "--no-build"
                ],
            });

            var mcpClient = await McpClientFactory.CreateAsync(clientTransport);

            var tools = await mcpClient.ListToolsAsync().ConfigureAwait(false);

            Console.ForegroundColor = ConsoleColor.DarkCyan;

            foreach (var tool in tools)
            {
                Console.WriteLine($"{tool.Name}: {tool.Description}");
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;

            ChatOptions chatOptions = new()
            {
                Tools = [..tools]
            };


            IChatClient client = ExtensionsClientFactory.CreateClient(LLMModel.Ollama);

            var response = client.GetStreamingResponseAsync("what is exchange rate eur to inr",chatOptions);

            await foreach (var update in response)
            {
                Console.Write(update);
            }

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
