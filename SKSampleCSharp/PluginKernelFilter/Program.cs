using InterruptPlanner.Filters;
using InterruptPlanner.Plugins.WeatherPlugin;
using InterruptPlanner.Watcher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning.Handlebars;

namespace InterruptPlanner
{
    internal class Program
    {
        private static readonly ITimingLogger FunctionTimingLogger = new TimingLogger();
        private static readonly ITimingLogger PromptTimingLogger = new TimingLogger();

        static readonly IScreenDisplay Display = new ScreenDisplay();

        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - Kernel - InterruptPlanner\n");

            var kernel = CreateKernelBuilder();

            kernel.Plugins.AddFromObject(new ExternalWeatherPlugin(kernel,Config.WeatherApiKey));

#pragma warning disable SKEXP0004

            kernel.PromptFilters.Add(new PromptFilter(PromptTimingLogger, Display));

          
            #region HandleBarsPlanner

            var goal = "Provide me with the historical and weather details for Chennai.";

#pragma warning disable SKEXP0060

            //Create a planner
            HandlebarsPlanner handlebarsPlanner = new();

            HandlebarsPlan handlebarsPlan;

            if (File.Exists("history.hbp"))
            {
                var plan = await File.ReadAllTextAsync("history.hbp");
                handlebarsPlan = new HandlebarsPlan(plan);
            }
            else
            {
                //Create a plan
                handlebarsPlan = await handlebarsPlanner.CreatePlanAsync(kernel!, goal);
                await File.WriteAllTextAsync("history.hbp", handlebarsPlan.ToString());
            }


            //Execute the plan
            var result = await handlebarsPlan.InvokeAsync(kernel!);
            Display.WriteLine(result.ToString());

#pragma warning restore SKEXP0060

            #endregion

            Console.Read();
        }

        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            //Add the location plugin
            var plugInDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "location");
            builder.Plugins.AddFromPromptDirectory(plugInDirectory, "location");

            //Add the weather plugin
            //builder.Plugins.AddFromObject(new ExternalWeatherPlugin(Config.WeatherApiKey));

            //Add the function filter
            builder.Services.AddSingleton<IFunctionFilter>(new FunctionFilter(FunctionTimingLogger,Display));

            return builder.Build()!;
        }
    }
}
