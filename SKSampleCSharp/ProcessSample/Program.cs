using AzureAI.Community.SK.Connector.GitHub.Models.AzureOpenAI;
using Microsoft.SemanticKernel;
using ProcessLibSample.Step1;
using ProcessLibSample.Step2;
using ProcessLibSample.Step3;
using ProcessLibSample.StepEvent;

namespace ProcessLibSample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var kernel = CreateKernelBuilder();

            ProcessBuilder processBuilder = new("ProcessLibBot");

            // Add the first step to the process
            var step1 = processBuilder.AddStepFromType<IntroStep>();
            var step2 = processBuilder.AddStepFromType<QuizzesStep>();
            var step3 = processBuilder.AddStepFromType<LLMResponse>();

            // Add the second step to the process

            processBuilder.OnExternalEvent(StepEvents.StartProcess)
                .SendEventTo(new ProcessFunctionTargetBuilder(step1));

            step1.OnFunctionResult(nameof(IntroStep.WelcomeMessage))
                .SendEventTo(new ProcessFunctionTargetBuilder(step2));

            step2.OnFunctionResult(nameof(QuizzesStep.GetQuizzes))
                .StopProcess();

            step2.OnEvent(StepEvents.GetQuizzes)
                .SendEventTo(new ProcessFunctionTargetBuilder(step3));

            step3.OnFunctionResult(nameof(LLMResponse.GetLLMResponse));

            step3.OnEvent(StepEvents.GetLLMResponse)
                .SendEventTo(new ProcessFunctionTargetBuilder(step2));

            KernelProcess kernelProcess = processBuilder.Build();

            await kernelProcess.StartAsync(kernel, new KernelProcessEvent()
            {
                Id = StepEvents.StartProcess
            });

            Console.Read();

        }

        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddGitHubAzureOpenAIChatCompletion(GitConfig.ModelId, GitConfig.Endpoint, GitConfig.GitHubToken);

            return builder.Build()!;
        }
    }
    
}
