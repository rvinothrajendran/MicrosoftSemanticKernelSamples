using Microsoft.SemanticKernel;

namespace Transform.AgentOrchestration;

/// <summary>
/// Provides functionality to transform user prompts into clear, grammatically correct, and AI-ready input using a semantic kernel.
/// </summary>
public class PromptInputTransform
{
    private readonly Kernel _kernel;

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptInputTransform"/> class with the specified semantic kernel.
    /// </summary>
    /// <param name="kernel">The semantic kernel used for prompt transformation.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="kernel"/> is null.</exception>
    public PromptInputTransform(Kernel kernel)
    {
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
    }

    /// <summary>
    /// Transforms the specified input prompt to be clear, grammatically correct, and suitable for AI processing.
    /// </summary>
    /// <param name="input">The user input prompt to transform.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the transformed input string.
    /// If the transformation fails or returns null/empty, the original input is returned.
    /// </returns>
    public async Task<string> TransformAsync(string input, CancellationToken cancellationToken)
    {
        // Simulate some transformation logic
        string chatPrompt = $"""
                            <message role="system">
                            You are a prompt engineer. 
                            Rewrite user prompts to be clear, grammatically correct, and ready for an AI model to understand.
                            </message>
                            <message role="user">{input}</message>
                            """;

        var function = KernelFunctionFactory.CreateFromPrompt(chatPrompt);

        var result = await _kernel.InvokeAsync(function, cancellationToken: cancellationToken);

        var transformedInput = result.GetValue<string>();

        if (string.IsNullOrEmpty(transformedInput))
            transformedInput = input;

        // Return the modified input
        return await Task.FromResult(transformedInput);
    }
}