using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace TinyCopilotGPT4VisionDemo;

public class VisionService
{
    private static IChatCompletionService? chatCompletionService;

    public VisionService()
    {
        var kernel = CreateKernelBuilder();
        chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
    }
    public async Task<string> FindCityNameAsync(string imageUrl)
    {
        ChatHistory chatHistory = new("Detect the city in the image and return the city name.");

        chatHistory.AddUserMessage(
            [
                new TextContent("Can you identify the city shown in the attached image"),
                new ImageContent(new Uri(imageUrl))
            ]
            );

        //Connect to the GPT4 Vision
        var chatMessageContent = await chatCompletionService!.GetChatMessageContentsAsync(chatHistory);

        return chatMessageContent[0].Content!;

    }
    private Kernel CreateKernelBuilder()
    {
        //Create a kernel builder and add the Azure OpenAI Chat Completion service
        var builder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(Config.ImageDeploymentOrModelId, Config.ImageEndpoint, Config.ImageApiKey);

        return builder.Build()!;
    }
}