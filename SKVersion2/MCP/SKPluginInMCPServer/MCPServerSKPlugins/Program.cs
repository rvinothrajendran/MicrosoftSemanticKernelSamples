using ExtensionsLibrary.Plugins;
using LLMModelFactory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using ModelContextProtocol.Server;

namespace MCPServerSKPlugins
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello,MCP Server");

            Kernel kernel= KernelFactory.CreateKernelBuilder(LLMModel.Ollama);
            kernel.Plugins.AddFromType<CurrencyConverterPlugin>();

            var builder = Host.CreateEmptyApplicationBuilder(settings: null);

            // Register the MCP server and its core services in the DI container.
            builder.Services.AddMcpServer()
                // Configure the server to use standard input/output (Stdio) for communication.
                .WithStdioServerTransport()
                // Automatically discover and register tools from the current assembly.
                .WithToolsFromAssembly()
                .AddSKPlugins(kernel.Plugins);

            
            var app = builder.Build();


            await app.RunAsync();

        }
    }

    public static class MCPExtensions
    {
        public static IMcpServerBuilder AddSKPlugins(this IMcpServerBuilder builder,
            KernelPluginCollection kernelPluginCollection)
        {
            foreach (var pluginFunction in kernelPluginCollection.SelectMany(plugin => plugin))
            {
                builder.Services.AddSingleton(McpServerTool.Create(pluginFunction.AsAIFunction()));
            }

            return builder;
        }
    }
}
