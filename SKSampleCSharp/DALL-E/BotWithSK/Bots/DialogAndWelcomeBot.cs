using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace BotWithSK.Bots;

public class DialogAndWelcomeBot<T> : DialogBot<T> where T : Dialog
{
    public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
        : base(conversationState, userState, dialog, logger)
    {
    }

    protected override async Task OnMembersAddedAsync(
        IList<ChannelAccount> membersAdded,
        ITurnContext<IConversationUpdateActivity> turnContext,
        CancellationToken cancellationToken)
    {
        foreach (var member in membersAdded)
        {
            // Greet anyone that was not the target (recipient) of this message.
            // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                var adaptiveCardAttachment = CreateAdaptiveCard("Welcome to the weather chatbot! \r\nExplore city history, landmark(image), thanks to Semantic Kernel.", member.Name);
                await turnContext.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);

                adaptiveCardAttachment = CreateAdaptiveCard("Specify the city of your interest.");
                await turnContext.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);


                //var reply = MessageFactory.Text($"Welcome to Bot Framework demo : {member.Name}. ");
                //await turnContext.SendActivityAsync(reply, cancellationToken);
            }
        }
    }

    private Attachment CreateAdaptiveCard(string information, string memberName=null)
    {
        var welcomeCard = CreateAdaptiveCardAttachment(memberName,information);

        var adaptiveCardAttachment = new Attachment()
        {
            ContentType = "application/vnd.microsoft.card.adaptive",
            Content = JsonConvert.DeserializeObject(welcomeCard),
        };
        return adaptiveCardAttachment;
    }


    private string CreateAdaptiveCardAttachment(string userName,string result)
    {

        // Create an Adaptive Card
        AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 2));

        if (!string.IsNullOrEmpty(userName))
        {
            card.Body.Add(new AdaptiveTextBlock()
            {
                Text = "Hello " + userName,
                Size = AdaptiveTextSize.Medium,
                Weight = AdaptiveTextWeight.Bolder
            });
        }

        card.Body.Add(new AdaptiveTextBlock()
        {
            Text = result,
            Size = AdaptiveTextSize.Large,
            Weight = AdaptiveTextWeight.Bolder,
            Wrap = true,
            Color = AdaptiveTextColor.Good
        });

        string adaptiveCardJson = card.ToJson();

        return adaptiveCardJson;
    }
}