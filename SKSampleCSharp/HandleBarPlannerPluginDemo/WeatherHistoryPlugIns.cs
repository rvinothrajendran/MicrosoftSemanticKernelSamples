using System.ComponentModel;
using HandleBarPlannerPluginDemo.Plugins.WeatherPlugin;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning.Handlebars;

namespace HandleBarPlannerPluginDemo;

public class WeatherHistoryPlugIns
{
    private Kernel kernelCopy = default;

    [KernelFunction]
    [Description("to find history and weather information based on the city")]
    public async Task<string> CollectWeatherHistoryInfo(Kernel kernel,
        [Description("descibe the user goal it in 1 or 2 lines to ensure full context is provided")]string goal)
    {

        if (kernelCopy == default)
        {
            kernelCopy = kernel.Clone();
            kernelCopy.Plugins.Remove(kernelCopy.Plugins[nameof(WeatherHistoryPlugIns)]);
        }

        //Add the location plugin
        var plugInDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "location");

        if(!kernelCopy.Plugins.Contains(nameof(plugInDirectory)))
            kernelCopy.ImportPluginFromPromptDirectory(plugInDirectory, nameof(plugInDirectory));
        

        //Add the weather plugin
        if(!kernelCopy.Plugins.Contains(nameof(ExternalWeatherPlugin)))
            kernelCopy.Plugins.AddFromObject(new ExternalWeatherPlugin(Config.WeatherApiKey),
            nameof(ExternalWeatherPlugin));

        //Create a planner
        HandlebarsPlanner handlebarsPlanner = new();

        //Create a plan
        HandlebarsPlan handlebarsPlan = await handlebarsPlanner.CreatePlanAsync(kernelCopy!, goal);

        Console.WriteLine(handlebarsPlan.ToString());
        
        //Execute the plan
        var result = await handlebarsPlan.InvokeAsync(kernelCopy!);

        return result;
        
    }
}