using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace CustomLLMYoutubeStreamingDemo
{
    public static class YouTubeChatServiceExtension
    {
        public static IKernelBuilder AddYouTubeChatCompletion(this IKernelBuilder builder, string youTubeKey,string channelKey = "")
        {
            if(string.IsNullOrWhiteSpace(youTubeKey))
                throw new ArgumentException("YouTube Key is required");

            builder.Services.AddKeyedSingleton<IChatCompletionService>(nameof(YouTubeChatCompletionService),
                new YouTubeChatCompletionService(youTubeKey));

            return builder;
        }
    }
}
