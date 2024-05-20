using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace HandleBarPlannerPluginDemo;

static class Program
{
    static async Task Main()
    {
        Console.WriteLine("HandleBars Planner in Plugins Demo");

        var kernel = CreateKernelBuilder();

        var chatService = kernel.GetRequiredService<IChatCompletionService>();

        var goal = "Provide me with the historical and weather details for Chennai.";

        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage(goal);

        OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
        {
            MaxTokens = 100,
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        var response = await chatService.GetChatMessageContentsAsync(chatHistory, settings,kernel);

        if (response?.Count > 0)
        {
            Console.WriteLine(response[0].Content);
        }

        Console.Read();
    }
    
    private static Kernel CreateKernelBuilder()
    {
        //Create a kernel builder and add the Azure OpenAI Chat Completion service
        var builder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

        builder.Plugins.AddFromType<WeatherHistoryPlugIns>(nameof(WeatherHistoryPlugIns));

        return builder.Build();
    }
}