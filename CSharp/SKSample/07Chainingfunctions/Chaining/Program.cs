using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using skills.Json;
using skills.Weather;

var kernelSettings = KernelSettings.LoadSettings();

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(kernelSettings.LogLevel ?? LogLevel.Warning)
        .AddConsole()
        .AddDebug();
});

IKernel kernel = new KernelBuilder()
    .WithLogger(loggerFactory.CreateLogger<IKernel>())
    .WithCompletionService(kernelSettings)
    .Build();

if (kernelSettings.EndpointType == EndpointTypes.TextCompletion)
{
    // note: using skills from the repo
    var skillsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "skills");
    var skill = kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "citySkill");

    kernel.ImportSkill(new ExtractJson(), nameof(ExtractJson));
    kernel.ImportSkill(new WeatherPlugIn(ConfigSettings.ConfigParameters.WeatherApiKey), nameof(WeatherPlugIn));


    var jsonSkillFunc = kernel.Skills.GetFunction(nameof(ExtractJson), "ExtractInformation");
    var weatherFunc = kernel.Skills.GetFunction(nameof(WeatherPlugIn), "GetWeatherAsync");

    var context = new ContextVariables();
    context.Set("input", "I plan to visit Paris");

    var result = await kernel.RunAsync(context, skill["history"],jsonSkillFunc,weatherFunc);

    Console.WriteLine(result);
}



