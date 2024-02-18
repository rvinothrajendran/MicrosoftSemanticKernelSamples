using KernelFilterDemo.Watcher;
using Microsoft.SemanticKernel;

namespace KernelFilterDemo;

#pragma warning disable SKEXP0004
public class PromptFilter(ITimingLogger promptTimingLogger, IScreenDisplay display) : IPromptFilter
{
    public void OnPromptRendered(PromptRenderedContext context)
    {
        var promptHooks = $"Prompt Rendered(after) {context.Function.Name}";
        display.WriteLine(promptHooks);
        promptTimingLogger.Stop();
    }

    public void OnPromptRendering(PromptRenderingContext context)
    {
        var promptHooks = $"Prompt Hooks - {context.Function.Name}";
        promptTimingLogger.Start(promptHooks);

        promptHooks = $"Prompt Rendering(Before) {context.Function.Name}";
        display.WriteLine(promptHooks);
    }
}