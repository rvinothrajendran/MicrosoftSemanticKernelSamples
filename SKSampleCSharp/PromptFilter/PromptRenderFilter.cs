using Microsoft.SemanticKernel;

namespace PromptFilter;

public class PromptRenderFilter : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        Console.WriteLine($"{context.Function.PluginName}");
        Console.WriteLine($"{context.Function.Name}");

        await next(context);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{context.RenderedPrompt}");
    }
}