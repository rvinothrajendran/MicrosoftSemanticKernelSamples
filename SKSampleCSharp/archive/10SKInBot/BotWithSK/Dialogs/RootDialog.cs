using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SKConnector;

namespace BotWithSK.Dialogs;

public class RootDialog : ComponentDialog
{
    private readonly UserState _userState;

    public RootDialog(UserState userState)
        : base(nameof(RootDialog))
    {
        _userState = userState;

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
        {
            InitialStepAsync,
            FinalStepAsync,
        }));

        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var text = stepContext.Context.Activity.Text;

        SemanticHelper helper = new SemanticHelper();
        var result = await helper.RequestInformation(text);

        var card = CreateAdaptiveCardAttachment(result);

        var adaptiveCardAttachment = new Attachment()
        {
            ContentType = "application/vnd.microsoft.card.adaptive",
            Content = JsonConvert.DeserializeObject(card),
        };

        await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);

        return await stepContext.EndDialogAsync(null,cancellationToken);
    }

    private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
       
        return await stepContext.EndDialogAsync(null, cancellationToken);
    }

    private string CreateAdaptiveCardAttachment(string result)
    {

        JObject json = JObject.Parse(result);

        // Create an Adaptive Card
        AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 2));

        card.Body.Add(new AdaptiveTextBlock()
        {
            Text = "Weather and City Information",
            Size = AdaptiveTextSize.ExtraLarge,
            Weight = AdaptiveTextWeight.Bolder
        });

        card.Body.Add(new AdaptiveTextBlock()
        {
            Text = json["CityResult"]?.ToString(),
            Size = AdaptiveTextSize.Medium,
            Weight = AdaptiveTextWeight.Bolder
        });

        // Assuming "WeatherResult" contains JSON data, parse and extract the relevant information
        JObject weatherData = JObject.Parse(json["WeatherResult"]?.ToString());

        card.Body.Add(new AdaptiveTextBlock()
        {
            Text = $"Current Temperature: {weatherData["current"]?["temp_c"]}°C",
            Size = AdaptiveTextSize.Medium
        });

        card.Body.Add(new AdaptiveTextBlock()
        {
            Text = json["HistoryResult"]?.ToString(),
            Size = AdaptiveTextSize.Medium,
            Wrap = true
        });


        // Serialize the Adaptive Card to JSON
        string adaptiveCardJson = card.ToJson();

        return adaptiveCardJson;
    }

}