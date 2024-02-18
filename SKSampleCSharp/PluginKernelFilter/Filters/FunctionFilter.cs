using InterruptPlanner.Watcher;
using Microsoft.SemanticKernel;

namespace InterruptPlanner.Filters;

public class FunctionFilter(ITimingLogger functionTimingLogger, IScreenDisplay display) : IFunctionFilter
{
    public void OnFunctionInvoked(FunctionInvokedContext context)
    {
        var functionHooks = $"Function Invoked(after) {context.Function.Name}";
        display.WriteLine(functionHooks);

        functionTimingLogger.Stop();
    }

    public void OnFunctionInvoking(FunctionInvokingContext context)
    {
        var functionHooks = $"Function Hooks - {context.Function.Name}";
        functionTimingLogger.Start(functionHooks);

        functionHooks = $"Function Invoking(Before) {context.Function.Name}";
        display.WriteLine(functionHooks);
    }
}