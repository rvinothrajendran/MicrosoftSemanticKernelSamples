using ConfigSettings;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using SKWeatherPlugin.Plugins.Weather;

namespace SKWeatherPlugin
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Microsoft Semantic Kernel Plugin");

            var kernel = new KernelBuilder().Build();

            var weatherPlugin = kernel.ImportFunctions(new WeatherPlugin(ConfigParameters.WeatherApiKey), nameof(WeatherPlugin));

            SKContext context = kernel.CreateNewContext();
            context.Variables.Set("input", "chennai");

            var report = await weatherPlugin["GetWeatherAsync"].InvokeAsync(context);

            Console.WriteLine(report.GetValue<string>());

            Console.ReadKey();
        }
    }
}