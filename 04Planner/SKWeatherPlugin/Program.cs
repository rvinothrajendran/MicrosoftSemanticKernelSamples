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

            try
            {
                var plugins = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
                
                var plugIn = kernel.ImportSemanticSkillFromDirectory(plugins, "History");
                kernel.ImportSkill(new WeatherPlugin(Settings.WeatherApiKey), nameof(WeatherPlugin));
            
                //Create a planner

                var planner = new SequentialPlanner(kernel);
                //var planner = new ActionPlanner(kernel);
                var input = "I am planning to travel to chennai";

                var plan = await planner.CreatePlanAsync(input);

                // var result = await plan.InvokeAsync(input);
               
                for (int step = 1; plan.HasNextStep && step < 10; step++)
                {
                    if (string.IsNullOrEmpty(input))
                    {
                        var context = kernel.CreateNewContext();
                        context["input"] = input;
                        plan = await plan.InvokeNextStepAsync(context);
                    }
                    else
                    {
                        plan = await kernel.StepAsync(input, plan);
                       // input = string.Empty;
                    }

                    if (!plan.HasNextStep)
                    {
                        Console.WriteLine($"Step {step} - COMPLETE!");
                        Console.WriteLine(plan.State.ToString());
                        break;
                    }

                    Console.WriteLine($"Step {step} - Results so far:");
                    Console.WriteLine(plan.State.ToString());
                }

                var planResult = await plan.InvokeAsync();

                Console.WriteLine(planResult.Result);
            }
            catch (Exception e)
            {
                
            }
            Console.ReadKey();
        }
    }
}