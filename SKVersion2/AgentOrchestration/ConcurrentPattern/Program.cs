using LLMModelFactory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration.Concurrent;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;

namespace ConcurrentPattern;

#pragma warning disable

internal abstract class Program
{
    public static List<ChatMessageContent> AgentHistory { get; } = [];

    static async Task Main(string[] args)
    {
            Console.WriteLine("Hello, Concurrent Pattern !!!");

            var kernel = KernelFactory.CreateKernelBuilder(LLMModel.Azure);
            
            ChatCompletionAgent actionFan = new ChatCompletionAgent
            {
                Name = "ActionFan",
                Description = "Recommends high-energy action-packed movies with thrilling scenes and heroic characters.",
                Instructions = "You are a movie lover who only recommends top action movies in tamil.",
                Kernel = kernel,
            };

            ChatCompletionAgent romComFan = new ChatCompletionAgent
            {
                Name = "RomComFan",
                Description = "Suggests feel-good romantic comedies that blend love, humor, and lighthearted storytelling.",
                Instructions = "You love romantic comedies and suggest only feel-good rom-coms in tamil.",
                Kernel = kernel,
            };

            ChatCompletionAgent classicFan = new ChatCompletionAgent
            {
                Name = "ClassicFan",
                Description = "Provides timeless classic movie recommendations known for their cultural impact and storytelling.",
                Instructions = "You're a classic movie enthusiast who always recommends timeless classics in tamil.",
                Kernel = kernel,
            };

#pragma warning disable SKEXP0110

        ConcurrentOrchestration concurrentOrchestration = new(classicFan, actionFan, romComFan)
        {
            ResponseCallback = ResponseCallback
        };
            
            // Create runtime
            InProcessRuntime runtime = new();
            await runtime.StartAsync();

            var input = "Suggest a good movie to watch this weekend in tamil";
            
            var result = await concurrentOrchestration.InvokeAsync(input,runtime);

            var agentResult = await result.GetValueAsync();

            foreach (var agent in agentResult)
            {
                Console.WriteLine(agent);
            }
            
            // Wait for all agents to finish
            await runtime.RunUntilIdleAsync();
            
            
            foreach (var agent in AgentHistory)
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine(agent.AuthorName);
                Console.WriteLine(agent.Content);
                Console.WriteLine("-------------------------------------");
            }
            
            Console.WriteLine("Press any key to exit");
            
            Console.Read();
    }
    
    private static ValueTask ResponseCallback(ChatMessageContent response)
    {
        AgentHistory.Add(response);
        return ValueTask.CompletedTask;
    }
}