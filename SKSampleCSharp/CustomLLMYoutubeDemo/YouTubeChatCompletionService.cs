using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Web.YouTube;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace CustomLLMYoutubeDemo;

public class YouTubeChatCompletionService(string youtubeKey,string channelYoutubeKey="") : IChatCompletionService
{
    
    readonly YouTubeConnector? youTubeConnector = new(youtubeKey,channelId: channelYoutubeKey);

    public IReadOnlyDictionary<string, object?> Attributes => throw new NotImplementedException();

    public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
    {
        var youtubeResult = await YoutubeSearch(chatHistory, cancellationToken);

        if (youtubeResult == null)
            return Array.Empty<ChatMessageContent>();

        var result = "";

        foreach (var youtube in youtubeResult)
        {
            result += youtube + ",";
        }

        result = result.TrimEnd(',');

        List<ChatMessageContent> chatMessageContents =
        [
            new ChatMessageContent(AuthorRole.Assistant, result)
        ];

        return chatMessageContents;
    }

    public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private async Task<IEnumerable<string>?> YoutubeSearch(ChatHistory chatHistory, CancellationToken cancellationToken)
    {

        if (chatHistory.Count <= 0)
            return null;

        var chatMessage = chatHistory.Last();

        string prompt = chatMessage.Content!;
        return await youTubeConnector!.SearchAsync(prompt, cancellationToken: cancellationToken);
    }
}
