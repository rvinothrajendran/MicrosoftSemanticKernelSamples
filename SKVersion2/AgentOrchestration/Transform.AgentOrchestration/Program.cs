using LLMModelFactory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration.Concurrent;
using Microsoft.SemanticKernel.Agents.Orchestration.Transforms;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Transform.AgentOrchestration;

#pragma warning disable SKEXP0110
/// <summary>
/// Entry point and orchestration logic for the Agent Orchestration Transformer application.
/// Demonstrates concurrent agent orchestration with input and result transforms for movie recommendations.
/// </summary>
internal abstract class Program
{
    /// <summary>
    /// Main entry point for the application.
    /// Sets up agents, orchestration, and executes the movie recommendation workflow.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, Agent Orchestration Transformer !!!");

        var kernel = KernelFactory.CreateKernelBuilder(LLMModel.Azure);

        ChatCompletionAgent actionFan = CreateActionFanAgent(kernel);
        ChatCompletionAgent romComFan = CreateRomComFanAgent(kernel);
        ChatCompletionAgent classicFan = CreateClassicFanAgent(kernel);

        ConcurrentOrchestration<string, List<MovieRecommendation>> concurrentOrchestration = new(classicFan, actionFan, romComFan)
        {
            InputTransform = InputTransform,
            ResultTransform = ResultTransform
        };

        // Create runtime
        InProcessRuntime runtime = new();
        await runtime.StartAsync();

        var input = "Suggest a good movie to watch this weekend in tamil";

        var result = await concurrentOrchestration.InvokeAsync(input, runtime);

        var agentResult = await result.GetValueAsync();

        foreach (var agent in agentResult)
        {
            Console.WriteLine(agent.ToString());
        }

        // Wait for all agents to finish
        await runtime.RunUntilIdleAsync();
    }

    /// <summary>
    /// Transforms the result from the agents into a list of <see cref="MovieRecommendation"/> objects.
    /// </summary>
    /// <param name="result">The list of chat message contents from the agents.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of <see cref="MovieRecommendation"/> objects.</returns>
    private static async ValueTask<List<MovieRecommendation>> ResultTransform(
        IList<ChatMessageContent> result,
        CancellationToken cancellationToken)
    {
        List<MovieRecommendation> recommendations = new();

        Kernel kernel = KernelFactory.CreateKernelBuilder(LLMModel.Azure);

        StructuredOutputTransform<MovieRecommendation> structuredOutputTransform =
            new(kernel.GetRequiredService<IChatCompletionService>(),
                new OpenAIPromptExecutionSettings()
                {
                    ResponseFormat = typeof(MovieRecommendation)
                });

        foreach (var chatMessage in result)
        {
            var movie = await structuredOutputTransform.TransformAsync(new List<ChatMessageContent>()
            {
                chatMessage
            }, cancellationToken);

            recommendations.Add(movie);
        }

        return recommendations;
    }

    /// <summary>
    /// Transforms the user input string into a collection of <see cref="ChatMessageContent"/> for agent processing.
    /// </summary>
    /// <param name="input">The user input string.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An enumerable of <see cref="ChatMessageContent"/> representing the transformed input.</returns>
    private static async ValueTask<IEnumerable<ChatMessageContent>> InputTransform(
        string input,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"User Input: {input}");
        Kernel kernel = KernelFactory.CreateKernelBuilder(LLMModel.Ollama);
        // Simulate some transformation logic
        PromptInputTransform promptInputTransform = new(kernel);

        var rewriteInput = await promptInputTransform.TransformAsync(input, cancellationToken);

        Console.WriteLine($"Transformed Input: {rewriteInput}");

        return new List<ChatMessageContent>()
        {
            new(AuthorRole.User, rewriteInput)
        };
    }

    /// <summary>
    /// Creates an agent that recommends high-energy action-packed movies.
    /// </summary>
    /// <param name="kernel">The kernel to use for the agent.</param>
    /// <returns>A configured <see cref="ChatCompletionAgent"/> for action movies.</returns>
    private static ChatCompletionAgent CreateActionFanAgent(Kernel kernel) =>
        new()
        {
            Description = "Recommends high-energy action-packed movies with thrilling scenes and heroic characters.",
            Instructions = "You are a movie lover who only recommends top action movies in tamil.",
            Kernel = kernel,
        };

    /// <summary>
    /// Creates an agent that suggests feel-good romantic comedies.
    /// </summary>
    /// <param name="kernel">The kernel to use for the agent.</param>
    /// <returns>A configured <see cref="ChatCompletionAgent"/> for romantic comedies.</returns>
    private static ChatCompletionAgent CreateRomComFanAgent(Kernel kernel) =>
        new()
        {
            Name = "RomComFan",
            Description = "Suggests feel-good romantic comedies that blend love, humor, and lighthearted storytelling.",
            Instructions = "You love romantic comedies and suggest only feel-good rom-coms in tamil.",
            Kernel = kernel,
        };

    /// <summary>
    ///  Creates an agent that recommends timeless classic movies. 
    /// </summary>
    /// <param name="kernel"></param>
    /// <returns></returns>
    private static ChatCompletionAgent CreateClassicFanAgent(Kernel kernel) =>
        new()
        {
            Name = "ClassicFan",
            Description = "Provides timeless classic movie recommendations known for their cultural impact and storytelling.",
            Instructions = "You're a classic movie enthusiast who always recommends timeless classics in tamil.",
            Kernel = kernel,
        };
}