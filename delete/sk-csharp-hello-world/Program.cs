using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.SkillDefinition;
using skills.jsonplugin;
using SKWeatherPlugin.Plugins.Weather;

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
    var skill = kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "CitySkill");

    var extractJson = kernel.ImportSkill(new ExtractJson(), "ExtractJson");
    var weatherplugin = kernel.ImportSkill(new WeatherPlugin(""), "WeatherPlugin");
    
    var context = new ContextVariables();
    context.Set("input", "I am planning to visit chennai end of this month.");

    var jsonFunction = kernel.Skills.GetFunction("ExtractJson", "ExtractInformation");
    var weatherfunction = kernel.Skills.GetFunction("WeatherPlugin", "GetWeatherAsync");

    ISKFunction[] functions = new []{skill["CityLocation"], jsonFunction,weatherfunction};
    
    var output = await kernel.RunAsync(context, skill["CityLocation"],jsonFunction,weatherfunction);
    

    
    
    var planner = new SequentialPlanner(kernel);

    var plan = await planner.CreatePlanAsync("I am planning to visit chennai end of this month.");

    var result = await plan.InvokeAsync();
    Console.WriteLine(result);
}
else if (kernelSettings.EndpointType == EndpointTypes.ChatCompletion)
{
    var chatCompletionService = kernel.GetService<IChatCompletion>();

    var chat = chatCompletionService.CreateNewChat("You are an AI assistant that helps people find information.");
    chat.AddMessage(AuthorRole.User, "Hi, what information can you provide for me?");

    string response = await chatCompletionService.GenerateMessageAsync(chat, new ChatRequestSettings());
    Console.WriteLine(response);
}


