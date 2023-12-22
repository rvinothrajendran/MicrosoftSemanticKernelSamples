using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Intent;
using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Intent.Template;
using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Web.YouTube;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Events;
using Microsoft.SemanticKernel.Planners;
using Microsoft.SemanticKernel.Plugins.Web;
using TemplateBot.Dialogs.MovieTemplate;

namespace TemplateBot.Dialogs.SKernel;

public class SKHelper
{
    public async Task<BookingResult> ProcessAsync(string prompt,TemplateExamples[] movieTemplate,List<string> intentList, CancellationToken cancellationToken)
    {
        IKernel kernel = new KernelBuilder().WithAzureOpenAIChatCompletionService(Settings.DeploymentName,
            Settings.EndPoint,
            Settings.Key).Build();

        //Youtube Connector
        var youTubeConnector = new YouTubeConnector(Settings.YouTubeKey);
        kernel.ImportFunctions(new WebSearchEnginePlugin(youTubeConnector), nameof(YouTubeConnector));

        //Movie Template
        kernel.ImportFunctions(new IntentPlugIn(kernel, "movie booking", intentList, movieTemplate));

        // Create a Stepwise plan to execute the prompt
        var planner = new StepwisePlanner(kernel);


        // Create a prompt planner
        var promptPlanner = PromptPlanner(prompt);
        var sequencePlan = planner.CreatePlan(promptPlanner);
        
        // Execute the plan
        var sequenceKernelResult = await kernel.RunAsync(sequencePlan,cancellationToken:cancellationToken);

        var result = sequenceKernelResult.GetValue<string>();

        var bookingResult = JsonSerializer.Deserialize<BookingResult>(result.ToString());

        return bookingResult;


    }

    private static string PromptPlanner(string prompt)
    {

        string outputFormat = @"
        {
            ""Input"": ""I need to cancel my ticket for the 3:30 PM show of 'The Avengers' at your theater."",
            ""Output"": {
                ""Text"": ""I need to cancel my ticket for the 3:30 PM show of 'The Avengers' at your theater."",
                ""Intent"": {
                    ""name"": ""Cancel Ticket"",
                    ""score"": 0.95
                },
                ""Entities"": [
                    {
                        ""Entity"": ""The Avengers"",
                        ""Type"": ""Movie"",
                        ""StartPos"": 51,
                        ""EndPos"": 63,
                        ""Value"": ""The Avengers"",
                        ""Size"": 0
                    },
                    {
                        ""Entity"": ""3:30 PM"",
                        ""Type"": ""Showtime"",
                        ""StartPos"": 41,
                        ""EndPos"": 47,
                        ""Value"": ""3:30 PM"",
                        ""Size"": 0                        
                    }
                ]
            },
            ""YouTubeLinks"": [
                ""https://www.youtube.com/watch?v=1F3hm6MfR1k"",
                ""https://www.youtube.com/watch?v=gB2zKZxESTg""               
            ]
        }";


        var promptPlanner =
            $"Determine the intention behind the provided text: { prompt}. If the recognized entity pertains to a movie, extract the value(movie name), " +
            "and then proceed to search for relevant information about the movie on YouTube." +
            $"Finally, produce an output in JSON format({ outputFormat}) that includes all the YouTube links";

        return promptPlanner;
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