using ConfigSettings;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planners;
using SKWeatherPlugin.Plugins.Weather;
using System;

namespace WeatherSKPlugin
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Microsoft Semantic Kernel Plugin");

            //Create a kernel builder
            var builder = new KernelBuilder();

            builder.WithAzureTextCompletionService(ConfigParameters.DeploymentOrModelId, ConfigParameters.Endpoint, ConfigParameters.ApiKey);
            
            //Build the kernel
            var kernel = builder.Build();

            //Import the Plugin

            try
            {
                var plugins = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
                
                var plugIn = kernel.ImportSemanticFunctionsFromDirectory(plugins, "History");
                kernel.ImportFunctions(new WeatherPlugin(ConfigParameters.WeatherApiKey), nameof(WeatherPlugin));
            
                //Create a planner

                var planner = new SequentialPlanner(kernel);
                //var planner = new ActionPlanner(kernel);
                var input = "I am planning to travel to chennai";

                var plan = await planner.CreatePlanAsync(input);

                
                var result = await kernel.RunAsync(plan);

                Console.WriteLine(result.GetValue<string>());

                // var result = await plan.InvokeAsync(input);

                for (int step = 1; plan.HasNextStep && step < 10; step++)
                {
                    if (string.IsNullOrEmpty(input))
                    {
                        var context = kernel.CreateNewContext();

                        context.Variables.Set("input", input);
                        plan = await plan.InvokeNextStepAsync(context);
                    }
                    else
                    {
                        plan = await kernel.StepAsync(input, plan);
                        //input = string.Empty;
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

                var contextResult = kernel.CreateNewContext();
                contextResult.Variables.Set("input", input);

                var planResult = await plan.InvokeAsync(contextResult);

                //Console.WriteLine(planResult.Result);
            }
            catch (Exception e)
            {
                
            }
            Console.ReadKey();
        }
    }
}