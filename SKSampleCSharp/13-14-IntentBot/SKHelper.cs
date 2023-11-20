using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Intent;
using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Intent.Template;
using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Web.YouTube;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Events;
using Microsoft.SemanticKernel.Plugins.Web;
using TemplateBot.Dialogs.MovieTemplate;

namespace TemplateBot.Dialogs.SKernel;

public class SKHelper
{
    public async Task ProcessAsync(TemplateExamples[] movieTemplate,List<string> intentList, CancellationToken cancellationToken)
    {
        IKernel kernel = new KernelBuilder().WithAzureOpenAIChatCompletionService(Settings.DeploymentName,
            Settings.EndPoint,
            Settings.Key).Build();

        kernel.FunctionInvoked += KernelFunctionInvoked;
        kernel.FunctionInvoking += KernelFunctionInvoking;

        //Youtube Connector
        var youTubeConnector = new YouTubeConnector(Settings.YouTubeKey);
        var youtubeSkill = kernel.ImportFunctions(new WebSearchEnginePlugin(youTubeConnector), nameof(YouTubeConnector));

        //Movie Template
        IDictionary<string, ISKFunction> movieImportFunctions =
            kernel.ImportFunctions(new IntentPlugIn(kernel, "movie booking", intentList, movieTemplate));

        //Kernel Context
        var context = await kernel.RunAsync("I would like book my ticket for the 7 PM show of 'Spider-Man:",
            cancellationToken, movieImportFunctions["ExtractIntentAndEntity"], youtubeSkill["search"]);
    }

    private void KernelFunctionInvoking(object sender, FunctionInvokingEventArgs e)
    {
        if (e.FunctionView.Name == "ExtractIntentAndEntity")
        {
            
        }   
    }

    private void KernelFunctionInvoked(object sender, FunctionInvokedEventArgs e)
    {
        if (e.FunctionView.Name == "ExtractIntentAndEntity")
        {
            
            e.SKContext.Variables.TryGetValue("input", out var inputValue);

            if (inputValue != null)
            {
                e.Metadata.TryAdd("intent", inputValue);

                var intent = JsonSerializer.Deserialize<BookingOutput>(inputValue.ToString());

                if (intent?.Entities.Count > 0)
                {
                    foreach (var entity in intent.Entities)
                    {
                        if (entity.Entity== "Movie")
                        {
                            e.SKContext.Variables.Set("input", entity.Value);
                        }
                    }
                }
            }
        }
    }
}