using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SKAgent
{
#pragma warning disable SKEXP0110
#pragma warning disable SKEXP0001
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello,Agent in Semantic Kernel");

            var kernelBuilder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey).Build();

            // Define the agents
            var mathsAgent = CreateMathsAgent(kernelBuilder);

            var englishAgent = CreateEnglishAgent(kernelBuilder);

            var principalAgent = CreatePrincipalAgent(kernelBuilder);


            AgentGroupChat chat =
                new(mathsAgent, englishAgent, principalAgent)
                {
                    ExecutionSettings =
                        new()
                        {
                            TerminationStrategy =
                                new ApprovalTerminationStrategy()
                                {
                                    Agents = [principalAgent],
                                    MaximumIterations = 5,
                                }
                        }
                };

            //// Invoke chat and display messages.

            chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, "Semantic kernal is a powerfull tool for natural languge processing, enabling more acurate understanding of context and meaning."));
            chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, "Please provide the sum of 12 and 13"));

            await foreach (var content in chat.InvokeAsync())
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n");

                Console.ForegroundColor = content.AuthorName switch
                {
                    "Principal" => ConsoleColor.Red,
                    "EnglishTeacher" => ConsoleColor.Blue,
                    "MathsTeacher" => ConsoleColor.Green,
                    _ => Console.ForegroundColor
                };

                Console.WriteLine($"# {content.Role} - {content.AuthorName ?? "*"}: '{content.Content}'");
            }

            Console.WriteLine($"# IS COMPLETE: {chat.IsComplete}");

            Console.Read();
        }

        private static ChatCompletionAgent CreatePrincipalAgent(Kernel kernelBuilder)
        {

            string principalAgentInstructions =
                """
                You are PrincipalAgent. you should receive complete user information and 
                Your final task is to approve the results evaluated by the English and Math teachers.
                """;


            ChatCompletionAgent principalAgent =
                new()
                {
                    Instructions = principalAgentInstructions,
                    Name = "Principal",
                    Kernel = kernelBuilder,
                };

            return principalAgent;
        }

        private static ChatCompletionAgent CreateEnglishAgent(Kernel kernelBuilder)
        {

            string englishTeacherInstructions =
                """
            You are an English teacher with fifteen years of experience, known for your patience and clarity.
            Your goal is to refine and ensure the provided text is grammatically correct and well-structured. 
            Provide one clear and concise suggestion per response. Focus solely on improving the writing quality. 
            Avoid unnecessary comments or corrections. Do not handle any other subject requests.
            """;


            ChatCompletionAgent englishAgent =
                    new()
                    {
                        Instructions = englishTeacherInstructions,
                        Name = "EnglishTeacher",
                        Kernel = kernelBuilder,
                    };

            return englishAgent;
        }

        private static ChatCompletionAgent CreateMathsAgent(Kernel kernelBuilder)
        {
            string mathTeacherInstructions =
                """
            You are a math teacher passionate about making complex concepts understandable.
            Your goal is to determine if the given mathematical explanation or solution is clear and correct.
            Do not handle any other subject requests
            """;
            ChatCompletionAgent mathsAgent =
                    new()
                    {
                        Instructions = mathTeacherInstructions,
                        Name = "MathsTeacher",
                        Kernel = kernelBuilder,
                    };
            return mathsAgent;
        }

        private sealed class ApprovalTerminationStrategy : TerminationStrategy
        {
            // Terminate when the final message contains the term "approve"
            protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
                => Task.FromResult(history[history.Count - 1].Content?.Contains("approve", StringComparison.OrdinalIgnoreCase) ?? false);
        }
    }
}

