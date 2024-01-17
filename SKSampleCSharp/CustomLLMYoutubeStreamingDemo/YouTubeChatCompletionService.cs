using System.Runtime.CompilerServices;
using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Web.YouTube;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace CustomLLMYoutubeStreamingDemo;

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

    public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var youtubeResult = await YoutubeSearch(chatHistory, cancellationToken);

        if (youtubeResult == null)
            yield break;

        var enumerable = youtubeResult as string[] ?? youtubeResult.ToArray();
        var youtubeUrls = enumerable.ToList();

        foreach (var result in youtubeUrls)
        {
            var sendResult = result == enumerable.Last() ? result : result + ",";
            StreamingChatMessageContent streamingChatMessageContent = new(AuthorRole.Assistant, sendResult);
            yield return streamingChatMessageContent;
        }
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
