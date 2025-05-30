using Microsoft.Extensions.AI;

namespace LLMModelFactory;

public static class ExtensionsClientFactory
{
    private static readonly Config Config = new();
    public static IChatClient CreateClient(LLMModel model)
    {
        return model switch
        {
            LLMModel.Ollama => CreateOllamaClient(),
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
        };
    }


    private static IChatClient CreateOllamaClient()
    {
        IChatClient client = new OllamaChatClient(new Uri(Config.OllamaUri),Config.LlamaModel)
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();

        return client;
    }

}