using HandleBarsPlannerSaveDemo.Plugins.WeatherPlugin;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning.Handlebars;

namespace HandleBarsPlannerSaveDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - HandlebarsPlanner - Demo");

            ScreenDisplay display = new(); 

            var kernel = CreateKernelBuilder();

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

            //Display the plan
            display.WriteLine(handlebarsPlan.ToString());

            //Execute the plan

            var result = await handlebarsPlan.InvokeAsync(kernel!);
            display.WriteLine(result.ToString());

#pragma warning restore SKEXP0060

            Console.Read();
        }


        private static Kernel? CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            //Add the location plugin
            var plugInDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "location");
            builder.Plugins.AddFromPromptDirectory(plugInDirectory, "location");

            //Add the weather plugin
            builder.Plugins.AddFromObject(new ExternalWeatherPlugin(Config.WeatherApiKey));

            return builder.Build()!;
        }
    }
}
