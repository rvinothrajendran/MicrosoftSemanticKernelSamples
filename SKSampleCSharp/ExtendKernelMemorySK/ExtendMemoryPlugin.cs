using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.SemanticKernelPlugin.Internals;
using Microsoft.SemanticKernel;

namespace ExtendKernelMemorySK
{
    internal sealed class ExtendMemoryPlugin
    {
        private readonly IKernelMemory memory;

        private readonly Kernel? kernel = null;

        private readonly string? defaultIndex = null;
        private readonly TagCollection? defaultRetrievalTags = null;

        private readonly MemoryPlugin memoryPlugin;

        private readonly string memoryKey = "MemoryReference";

        public ExtendMemoryPlugin(Kernel kernel, Uri endpoint, IKernelMemory memory, string apiKey = "", string apiKeyHeader = "Authorization", string defaultIndex = "", TagCollection? defaultIngestionTags = null, TagCollection? defaultRetrievalTags = null, List<string>? defaultIngestionSteps = null, bool waitForIngestionToComplete = false)
        {
            memoryPlugin = new MemoryPlugin(endpoint, apiKey, apiKeyHeader, defaultIndex, defaultIngestionTags,
                defaultRetrievalTags, defaultIngestionSteps, waitForIngestionToComplete);
            this.memory = memory;
            this.kernel = kernel;
        }

        public ExtendMemoryPlugin(Kernel kernel, string serviceUrl, IKernelMemory memory, string apiKey = "", bool waitForIngestionToComplete = false)
        {
            memoryPlugin = new MemoryPlugin(serviceUrl, apiKey, waitForIngestionToComplete);
            this.memory = memory;
            this.kernel = kernel;
        }

        public ExtendMemoryPlugin(Kernel kernel, IKernelMemory memoryClient, string defaultIndex = "", TagCollection? defaultIngestionTags = null, TagCollection? defaultRetrievalTags = null, List<string>? defaultIngestionSteps = null, bool waitForIngestionToComplete = false)
        {
            memory = memoryClient;
            this.kernel = kernel;
            this.defaultIndex = defaultIndex;
            this.defaultRetrievalTags = defaultRetrievalTags;
            memoryPlugin = new MemoryPlugin(memoryClient, defaultIndex, defaultIngestionTags, defaultRetrievalTags,
                defaultIngestionSteps, waitForIngestionToComplete);
        }

        [KernelFunction, Description("Store in memory the given text")]
        public async Task<string> SaveAsync(
            [Description("The text to save in memory")]
            string input,
            [ /*SKName(DocumentIdParam),*/ Description("The document ID associated with the information to save"),
                                           DefaultValue(null)]
            string? documentId = null,
            [ /*SKName(IndexParam),*/ Description("Memories index associated with the information to save"),
                                      DefaultValue(null)]
            string? index = null,
            [ /*SKName(TagsParam),*/ Description("Memories index associated with the information to save"),
                                     DefaultValue(null)]
            TagCollectionWrapper? tags = null,
            [ /*SKName(StepsParam),*/ Description("Steps to parse the information and store in memory"),
                                      DefaultValue(null)]
            ListOfStringsWrapper? steps = null,
            ILoggerFactory? loggerFactory = null,
            CancellationToken cancellationToken = default)
        {
            return await memoryPlugin.SaveAsync(input, documentId, index, tags, steps, loggerFactory, cancellationToken);
        }

        [KernelFunction, Description("Store in memory the information extracted from a file")]
        public async Task<string> SaveFileAsync(
            [ /*SKName(FilePathParam),*/ Description("Path of the file to save in memory")]
            string filePath,
            [ /*SKName(DocumentIdParam),*/ Description("The document ID associated with the information to save"),
                                           DefaultValue(null)]
            string? documentId = null,
            [ /*SKName(IndexParam),*/ Description("Memories index associated with the information to save"),
                                      DefaultValue(null)]
            string? index = null,
            [ /*SKName(TagsParam),*/ Description("Memories index associated with the information to save"),
                                     DefaultValue(null)]
            TagCollectionWrapper? tags = null,
            [ /*SKName(StepsParam),*/ Description("Steps to parse the information and store in memory"),
                                      DefaultValue(null)]
            ListOfStringsWrapper? steps = null,
            ILoggerFactory? loggerFactory = null,
            CancellationToken cancellationToken = default)
        {
            return await memoryPlugin.SaveFileAsync(filePath, documentId, index, tags, steps, loggerFactory,
                cancellationToken);
        }

        [KernelFunction, Description("Store in memory the information extracted from a web page")]
        public async Task<string> SaveWebPageAsync(
            [ /*SKName(UrlParam),*/ Description("Complete URL of the web page to save")]
            string url,
            [ /*SKName(DocumentIdParam),*/ Description("The document ID associated with the information to save"),
                                           DefaultValue(null)]
            string? documentId = null,
            [ /*SKName(IndexParam),*/ Description("Memories index associated with the information to save"),
                                      DefaultValue(null)]
            string? index = null,
            [ /*SKName(TagsParam),*/ Description("Memories index associated with the information to save"),
                                     DefaultValue(null)]
            TagCollectionWrapper? tags = null,
            [ /*SKName(StepsParam),*/ Description("Steps to parse the information and store in memory"),
                                      DefaultValue(null)]
            ListOfStringsWrapper? steps = null,
            ILoggerFactory? loggerFactory = null,
            CancellationToken cancellationToken = default)
        {
            return await memoryPlugin.SaveWebPageAsync(url, documentId, index, tags, steps, loggerFactory,
                cancellationToken);
        }

        [KernelFunction, Description("Return up to N memories related to the input text")]
        public async Task<string> SearchAsync(
            [ /*SKName(QueryParam),*/ Description("The text to search in memory")]
            string query,
            [ /*SKName(IndexParam),*/ Description("Memories index to search for information"), DefaultValue("")]
            string? index = null,
            [ /*SKName(MinRelevanceParam),*/ Description("Minimum relevance of{ the memories to return"),
                                             DefaultValue(0d)]
            double minRelevance = 0,
            [ /*SKName(LimitParam),*/ Description("Maximum number of memories to return"), DefaultValue(1)]
            int limit = 1,
            [ /*SKName(TagsParam),*/ Description("Memories tags to search for information"), DefaultValue(null)]
            TagCollectionWrapper? tags = null,
            CancellationToken cancellationToken = default)
        {
            return await memoryPlugin.SearchAsync(query, index, minRelevance, limit, tags, cancellationToken);
        }

        [KernelFunction, Description("Use long term memory to answer a question")]
        public async Task<string> AskAsync(
            [ /*SKName(QuestionParam),*/ Description("The question to answer")]
            string question,
            [ /*SKName(IndexParam),*/ Description("Memories index to search for answers"), DefaultValue("")]
            string? index = null,
            [ /*SKName(MinRelevanceParam),*/ Description("Minimum relevance of the sources to consider"), DefaultValue(0d)]
            double minRelevance = 0,
            [ /*SKName(TagsParam),*/ Description("Memories tags to search for information"), DefaultValue(null)]
            TagCollectionWrapper? tags = null,
            ILoggerFactory? loggerFactory = null,
            CancellationToken cancellationToken = default)
        {
            MemoryAnswer answer = await this.memory.AskAsync(
                question: question,
                index: index ?? this.defaultIndex,
                filter: TagsToMemoryFilter(tags ?? this.defaultRetrievalTags),
                minRelevance: minRelevance,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (kernel != null && kernel.Data.ContainsKey(memoryKey))
            {
                kernel.Data.Remove(memoryKey);
            }

            kernel?.Data.Add(memoryKey, answer);

            return answer.Result;
        }

        [KernelFunction, Description("Remove from memory all the information extracted from the given document ID")]
        public Task DeleteAsync(
            [ /*SKName(DocumentIdParam),*/ Description("The document to delete")]
            string documentId,
            [ /*SKName(IndexParam),*/ Description("Memories index where the document is stored"), DefaultValue("")]
            string? index = null,
            CancellationToken cancellationToken = default)
        {
            return memoryPlugin.DeleteAsync(documentId, index, cancellationToken);
        }

        private static MemoryFilter? TagsToMemoryFilter(TagCollection? tags)
        {
            if (tags == null)
            {
                return null;
            }

            var filters = new MemoryFilter();

            foreach (var tag in tags)
            {
                filters.Add(tag.Key, tag.Value);
            }

            return filters;
        }
    }
}
