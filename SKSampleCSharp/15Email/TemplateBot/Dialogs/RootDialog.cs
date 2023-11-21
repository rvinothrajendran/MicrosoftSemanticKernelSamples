using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using TemplateBot.Dialogs.MovieTemplate;
using TemplateBot.Dialogs.SKernel;
using Attachment = Microsoft.Bot.Schema.Attachment;

namespace TemplateBot.Dialogs;

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

    private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        var text = stepContext.Context.Activity.Text;

        var result = await SKHandler.ProcessMessage(text);

        //var movieTemplate = Movie.GenerateTemplate();

        //List<string> intentList = new List<string>
        //{
        //    "Movie Booking",
        //    "Rebook",
        //    "Cancel Booking",
        //    "Ticket Availability",
        //    "Find Movie Name",
        //};

        //SKHelper skHelper = new SKHelper();

        //var skTuple = await skHelper.ProcessAsync(text,movieTemplate, intentList, cancellationToken);


        //var attachments = new List<Attachment>();
        //foreach (var videoUrl in skTuple.VideoList)
        //{
        //    var videoAttachment = CreateVideoCardVideoAttachment(videoUrl);
        //    attachments.Add(videoAttachment);
        //}

        //var message = MessageFactory.Carousel(attachments);

        //await stepContext.Context.SendActivityAsync(message, cancellationToken);

        //await stepContext.Context.SendActivityAsync(text, cancellationToken: cancellationToken);

        return await stepContext.EndDialogAsync(null, cancellationToken);
    }


    private static Attachment CreateVideoCardVideoAttachment(string videoUrl)
    {
        var videoAttachment = new VideoCard
        {
            Media = new List<MediaUrl>
            {
                new MediaUrl()
                {
                    Url = videoUrl,
                    Profile = videoUrl
                }
            },
            Title = videoUrl,
        };

        return videoAttachment.ToAttachment();
    }

    private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        var text = stepContext.Context.Activity.Text;
        return await stepContext.EndDialogAsync(MessageFactory.Text(text), cancellationToken);
    }
}

