using LLMModelFactory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;

namespace GroupPattern
{
#pragma warning disable
    internal class Program
    {
        static List<ChatMessageContent> chatHistory = new List<ChatMessageContent>();
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, GroupChat Orchestration!!!");

            Kernel kernel = KernelFactory.CreateKernelBuilder(LLMModel.Azure);

            ChatCompletionAgent softwareAgent = CreateSoftwareAgent(kernel);
            ChatCompletionAgent qaAgent = CreateQAAgent(kernel);
            ChatCompletionAgent codeReviewAgent = CreateCodeReviewAgent(kernel);


            GroupChatOrchestration groupChatOrchestration = new(new SmartRoundRobinGroupChatManager()
            {
                MaximumInvocationCount = 5,
                InteractiveCallback = InteractiveCallback
            }, softwareAgent, qaAgent, codeReviewAgent)
            {
                ResponseCallback = ResponseCallback
            };

            InProcessRuntime runtime = new();
            await runtime.StartAsync();

            var prompt = "Write a C# Method to calculate the factorial of a number.";

            OrchestrationResult<string> orchestrationResult = await groupChatOrchestration.InvokeAsync(prompt, runtime);

            string output = await orchestrationResult.GetValueAsync();

            await runtime.RunUntilIdleAsync();

            Console.WriteLine("Orchestration Result: " + output);

            Console.WriteLine("Chat History: ");

            Console.ForegroundColor = ConsoleColor.Green;

            foreach (var message in chatHistory)
            {
                Console.WriteLine("\n--------------------------------------------------");
                Console.WriteLine($"{message.AuthorName}");
                Console.WriteLine($"{message.Content}");
                Console.WriteLine("--------------------------------------------------\n");
            }

            Console.ForegroundColor = ConsoleColor.White;

            Console.Read();
        }

        private static ValueTask<ChatMessageContent> InteractiveCallback()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("Interactive callback triggered. Please provide your input: ");
            string userInput = Console.ReadLine() ?? string.Empty;
            ChatMessageContent userMessage = new ChatMessageContent(AuthorRole.User, userInput);

            return ValueTask.FromResult(userMessage);
        }

        private static ValueTask ResponseCallback(ChatMessageContent response)
        {
            chatHistory.Add(response);
            return ValueTask.CompletedTask;
        }

        private static ChatCompletionAgent CreateSoftwareAgent(Kernel kernel)
        {
            ChatCompletionAgent softwareEngineerAgent = new ChatCompletionAgent
            {
                Name = "SoftwareEngineerAgent",
                Description = "Implements features based on product requirements.",
                Instructions = """
                               You are a senior software engineer. Given a feature request, write a clear and simple code snippet in C# 
                                  to implement the functionality. Focus on correctness and readability. 
                                  Return only one implementation per response. No extra comments or chit-chat.
                               """,
                Kernel = kernel.Clone()
            };

            return softwareEngineerAgent;
        }

        private static ChatCompletionAgent CreateQAAgent(Kernel kernel)
        {
            ChatCompletionAgent qaTesterAgent = new ChatCompletionAgent
            {
                Name = "QATesterAgent",
                Description = "Tests and validates software feature implementations.",
                Instructions = """
                               You are a meticulous QA tester. Analyze the provided code to determine if it meets the feature requirements. 
                                  If it works as intended, state that it is approved for release.
                                  If not, identify functional issues or edge cases that the implementation misses.
                                  Do not provide code — only feedback.
                                  Send your approval or not in a response.
                               """,
                Kernel = kernel.Clone()
            };

            return qaTesterAgent;
        }

        private static ChatCompletionAgent CreateCodeReviewAgent(Kernel kernel)
        {
            ChatCompletionAgent codeReviewAgent = new ChatCompletionAgent()
            {
                Name = "CodeReviewAgent",
                Description = "Reviews code quality and adherence to best practices.",
                Instructions = """
                               You are an experienced software engineer specializing in code reviews.
                               Review the given code snippet for style, maintainability, readability, and adherence to best practices.
                               Provide constructive feedback and suggestions for improvement, but do not rewrite the code.
                               If the code looks good, simply approve it.
                               """,
                Kernel = kernel.Clone()
            };
            return codeReviewAgent;
        }

        
    }

    sealed class SmartRoundRobinGroupChatManager : RoundRobinGroupChatManager
    {
        public override ValueTask<GroupChatManagerResult<bool>> ShouldRequestUserInput(ChatHistory history, 
            CancellationToken cancellationToken = new CancellationToken())
        {
            var isApprove = history.Last().Content.Contains("approve");

            if (isApprove)
            {
                return ValueTask.FromResult(new GroupChatManagerResult<bool>(true)
                    { Reason = "User approve is required" });
            }

            return ValueTask.FromResult(new GroupChatManagerResult<bool>(false)
                { Reason = "No user input required" });
        }
    }
}
