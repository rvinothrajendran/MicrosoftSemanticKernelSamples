using FunctionHooks.skills.Json;
using FunctionHooks.skills.Weather;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Events;

namespace FunctionHooks
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Microsoft Semantic Kernel - Pre / Post Hooks");

            IKernel kernel = new KernelBuilder()
                .WithAzureChatCompletionService(Settings.DeploymentOrModelId, Settings.Endpoint, Settings.ApiKey)
                .Build();

            kernel.FunctionInvoking += Kernel_FunctionInvoking;
            kernel.FunctionInvoked += Kernel_FunctionInvoked;

            var skillsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "skills");
            var skill = kernel.ImportSemanticFunctionsFromDirectory(skillsDirectory, "citySkill");

            kernel.ImportFunctions(new ExtractJson(), nameof(ExtractJson));
            kernel.ImportFunctions(new WeatherPlugIn(Settings.WeatherApiKey), nameof(WeatherPlugIn));

            var jsonSkillFunc = kernel.Functions.GetFunction(nameof(ExtractJson), "ExtractInformation");
            var weatherFunc = kernel.Functions.GetFunction(nameof(WeatherPlugIn), "GetWeatherAsync");

            //var jsonSkillFunc = kernel.Skills.GetFunction(nameof(ExtractJson), "ExtractInformation");
            //var weatherFunc = kernel.Skills.GetFunction(nameof(WeatherPlugIn), "GetWeatherAsync");

            var context = new ContextVariables();
            context.Set("input", "I plan to visit Paris");

            var result = await kernel.RunAsync(context, skill["history"], jsonSkillFunc, weatherFunc);


            Console.WriteLine(result);
        }

        //Post-Hook
        private static void Kernel_FunctionInvoked(object? sender, FunctionInvokedEventArgs e)
        {
            Console.WriteLine($"Post-Hook Function Name {e.FunctionView.Name}");

            if (e.SKContext.Result.Contains("Chennai"))
            {
                //e.Cancel();
                e.SKContext.Variables.Set("input", "I plan to visit Bangalore");
                e.Repeat();
            }
        }

        //Pre-Hook
        private static void Kernel_FunctionInvoking(object? sender, FunctionInvokingEventArgs e)
        {
            Console.WriteLine($"Pre-Hook Function Name {e.FunctionView.Name}");

            e.SKContext.Variables.TryGetValue("input", out var inputValue);

            if (!string.IsNullOrEmpty(inputValue) && inputValue.Contains("Paris"))
            {
                e.SKContext.Variables.Set("input", "I plan to visit Chennai");
            }
        }
    }
}
