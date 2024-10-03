using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace ProcessLibSample.Step3;

public class LLMResponse : KernelProcessStep
{

    [KernelFunction]
    public async Task GetLLMResponse(KernelProcessStepContext context,string message,Kernel kernel)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        //Console.WriteLine($"The LLM response is : {count} ");

        var prompt = $"check the answer is correct or not based on the question {message}";

        IChatCompletionService chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();

        var result = await chatCompletionService.GetChatMessageContentsAsync(prompt);

        Console.WriteLine(result[0].Content);

        await context.EmitEventAsync(new KernelProcessEvent()
        {
            Id = StepEvent.StepEvents.GetLLMResponse

        });
    }

}