using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using SKWeatherPlugin.Config;
using SKWeatherPlugin.Plugins.Weather;

namespace SKWeatherPlugin
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Microsoft Semantic Kernel Plugin");

            //Create a kernel builder
            var builder = new KernelBuilder();

            builder.WithAzureTextCompletionService(Settings.DeploymentName, Settings.Endpoint, Settings.ApiKey);

            //Build the kernel
            var kernel = builder.Build();

            //Import the Plugin
            var weatherPlugin = kernel.ImportSkill(new WeatherPlugin(Settings.WeatherApiKey), nameof(WeatherPlugin));

            //var report = await weatherPlugin["GetWeatherAsync"].InvokeAsync("chennai");

            //Create a planner

            var planner = new SequentialPlanner(kernel);

            var plan = await planner.CreatePlanAsync("I am planning to travel to chennai");

            var planResult = await plan.InvokeAsync();

            Console.WriteLine(planResult.Result);

            Console.ReadKey();
        }
    }
}