using ConfigSettings;
using Microsoft.SemanticKernel;
using SKWeatherPlugin.Plugins.Weather;

namespace SKWeatherPlugin
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Microsoft Semantic Kernel Plugin");

            var kernel = new KernelBuilder().Build();

            var weatherPlugin = kernel.ImportSkill(new WeatherPlugin(ConfigParameters.WeatherApiKey), nameof(WeatherPlugin));

            var report = await weatherPlugin["GetWeatherAsync"].InvokeAsync("chennai");

            Console.WriteLine(report);

            Console.ReadKey();
        }
    }
}