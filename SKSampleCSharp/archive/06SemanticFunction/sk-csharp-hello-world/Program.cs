using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Orchestration;

var kernelSettings = KernelSettings.LoadSettings();

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(kernelSettings.LogLevel ?? LogLevel.Warning)
        .AddConsole()
        .AddDebug();
});

IKernel kernel = new KernelBuilder()
    .WithCompletionService(kernelSettings)
    .Build();

if (kernelSettings.EndpointType == EndpointTypes.TextCompletion)
{
    // note: using skills from the repo
    var skillsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "skills");
    var skill = kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "citySkill");

    var context = new ContextVariables();
    context.Set("input", "I plan to visit Paris");

    var result = await kernel.RunAsync(context, skill["history"]);
    Console.WriteLine(result);
}



