using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Intent;
using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Intent.Template;
using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Web.YouTube;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Events;
using Microsoft.SemanticKernel.Plugins.Web;
using Newtonsoft.Json;
using TemplateBot.Dialogs.MovieTemplate;

namespace TemplateBot.Dialogs.SKernel;

public class SKHelper
{
    public async Task<(BookingOutput Intent, List<string> VideoList)> ProcessAsync(string prompt,TemplateExamples[] movieTemplate,List<string> intentList, CancellationToken cancellationToken)
    {
        //Create Kernel
        IKernel kernel = new KernelBuilder().WithAzureOpenAIChatCompletionService(Settings.DeploymentName,
            Settings.EndPoint,
            Settings.Key).Build();

        kernel.FunctionInvoked += KernelFunctionInvoked;
        kernel.FunctionInvoking += KernelFunctionInvoking;

        //Movie Template
        IDictionary<string, ISKFunction> movieImportFunctions =
            kernel.ImportFunctions(new IntentPlugIn(kernel, "movie booking", intentList, movieTemplate));

        //Youtube Connector
        var youTubeConnector = new YouTubeConnector(Settings.YouTubeKey);
        var youTubeFunctions = kernel.ImportFunctions(new WebSearchEnginePlugin(youTubeConnector), nameof(YouTubeConnector));

        //Kernel Context
        var context = await kernel.RunAsync(prompt,
            cancellationToken, movieImportFunctions["ExtractIntentAndEntity"], youTubeFunctions["search"]);

     
        var inputValue = context.GetValue<string>();

        List<string> videoUrls = JsonConvert.DeserializeObject<List<string>>(inputValue);

        BookingOutput output = null;
        if (context.FunctionResults.Count > 0)
            output = context.FunctionResults.ElementAt(0).Metadata["Booking"] as BookingOutput;


        return (output, videoUrls);

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
                var intent = System.Text.Json.JsonSerializer.Deserialize<BookingOutput>(inputValue.ToString());

                e.Metadata.Add("Booking",intent);

                if (intent?.Entities.Count > 0)
                {
                    foreach (var entity in intent.Entities)
                    {
                        if (entity.Entity == "Movie")
                        {
                            e.SKContext.Variables.Set("input", entity.Value);
                        }
                    }
                }
            }
        }
    }

   
    
}