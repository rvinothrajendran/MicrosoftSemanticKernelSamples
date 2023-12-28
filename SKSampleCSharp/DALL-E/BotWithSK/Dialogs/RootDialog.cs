using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using SKConnector;

namespace BotWithSK.Dialogs;

public class RootDialog : ComponentDialog
{
    private readonly SemanticKernelProcessor semanticKernelProcessor;
    private readonly AdaptiveHelper adaptiveHelper;

    public RootDialog(SemanticKernelProcessor semanticKernelProcessor)
        : base(nameof(RootDialog))
    {
        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
        {
            InitialStepAsync,
            FinalStepAsync,
        }));

        InitialDialogId = nameof(WaterfallDialog);
        this.semanticKernelProcessor = semanticKernelProcessor;
        adaptiveHelper = new AdaptiveHelper();
    }

    private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var text = stepContext.Context.Activity.Text;

        var result = await semanticKernelProcessor.ProcessInformation(text);

        var card = result is null ? adaptiveHelper.CreateErrorAdaptiveCardAttachment("No city information has been found !!!") 
            : adaptiveHelper.CreateAdaptiveCardAttachment(result);

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
}