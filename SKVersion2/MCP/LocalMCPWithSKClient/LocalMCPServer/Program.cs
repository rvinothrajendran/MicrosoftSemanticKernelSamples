using ExtensionsLibrary.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LocalMCPServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello , MCP Server");

            var builder = Host.CreateEmptyApplicationBuilder(settings: null);

            // Register the MCP server and its core services in the DI container.
            builder.Services.AddMcpServer()
                // Configure the server to use standard input/output (Stdio) for communication.
                .WithStdioServerTransport()
                // Automatically discover and register tools from the current assembly.
                .WithToolsFromAssembly()
                .WithTools<ExternalWeatherTool>();


            var app = builder.Build();


            await app.RunAsync();
        }
    }
}
