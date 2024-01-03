using KernelHooksDemo.Plugins.WeatherPlugin;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning.Handlebars;

namespace KernelHooksDemo
{
    internal class Program
    {
        private static readonly TimingLogger FunctionTimingLogger = new();
        private static readonly TimingLogger PromptTimingLogger = new();

        static readonly ScreenDisplay Display = new();

        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - Kernel - Hooks\n");

            var kernel = CreateKernelBuilder();

#pragma warning disable SKEXP0004

            //Function Hooks

            kernel.FunctionInvoking += Kernel_FunctionInvoking;
            kernel.FunctionInvoked += Kernel_FunctionInvoked;

            //Prompt Hooks  
            kernel.PromptRendering += Kernel_PromptRendering;
            kernel.PromptRendered += Kernel_PromptRendered;
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

        private static void Kernel_FunctionInvoking(object? sender, FunctionInvokingEventArgs e)
        {
            var functionHooks = $"Function Hooks - {e.Function.Name}";
            FunctionTimingLogger.Start(functionHooks);

            functionHooks = $"Function Invoking(Before) {e.Function.Name}";
            Display.WriteLine(functionHooks);
        }

        private static void Kernel_FunctionInvoked(object? sender, FunctionInvokedEventArgs e)
        {
            var functionHooks = $"Function Invoked(after) {e.Function.Name}";
            Display.WriteLine(functionHooks);

            FunctionTimingLogger.Stop();
        }

        private static void Kernel_PromptRendering(object? sender, PromptRenderingEventArgs e)
        {
            var promptHooks = $"Prompt Hooks - {e.Function.Name}";
            PromptTimingLogger.Start(promptHooks);

            promptHooks = $"Prompt Rendering(Before) {e.Function.Name}";
            Display.WriteLine(promptHooks);
        }

        private static void Kernel_PromptRendered(object? sender, PromptRenderedEventArgs e)
        {
            var promptHooks = $"Prompt Rendered(after) {e.Function.Name}";
            Display.WriteLine(promptHooks);
            PromptTimingLogger.Stop();
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
            builder.Plugins.AddFromObject(new ExternalWeatherPlugin(Config.WeatherApiKey));

            return builder.Build()!;
        }
    }
}
