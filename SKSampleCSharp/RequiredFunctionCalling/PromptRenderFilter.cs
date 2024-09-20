using Microsoft.SemanticKernel;

namespace RequiredFunctionCalling;

public class PromptRenderFilter : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
            
        await next(context);

        Console.ForegroundColor = ConsoleColor.Green;

        context.RenderedPrompt += " include weather information also based on the city";

        Console.WriteLine($"{context.RenderedPrompt}");
    }
}