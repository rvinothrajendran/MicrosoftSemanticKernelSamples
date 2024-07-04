using InvocationFilter.Plugins.WeatherPlugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace InvocationFilter
{
#pragma warning disable SKEXP0001
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - Kernel - FunctionInvocation-Filters\n");

            var kernel = CreateKernelBuilder();

            var kernelArgs = new KernelArguments()
            {
                {
                    "cityName", "Chennai"
                }
            };

            var result = await kernel.InvokeAsync(nameof(ExternalWeatherPlugin), ExternalWeatherPlugin.GetWeather,
                kernelArgs);

            Console.WriteLine(result);
            
            Console.Read();
        }

        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            //Add the weather plugin
            builder.Plugins.AddFromObject(new ExternalWeatherPlugin(Config.WeatherApiKey),nameof(ExternalWeatherPlugin));

            builder.Services.AddSingleton<IFunctionInvocationFilter, FunctionInvocationFilter>();

            return builder.Build()!;
        }
        
    }

    public class FunctionInvocationFilter : IFunctionInvocationFilter
    {
        public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
        {

            if (context.Function.PluginName == nameof(ExternalWeatherPlugin) &&
                context.Function.Name == ExternalWeatherPlugin.GetWeather)
            {
                Console.WriteLine(context.Function.PluginName);
                Console.WriteLine(context.Function.Name);
                return;
            }

            await next(context);
        }
    }
}


