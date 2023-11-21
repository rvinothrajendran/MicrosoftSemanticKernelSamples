using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;

namespace TemplateBot.Dialogs.MailService;

public class GmailServiceHelper
{
    public static GmailService GetGmailService()
    {
        UserCredential credential;
        using (var stream = new FileStream(@"Dialogs\MailService\gmail.json", FileMode.Open, FileAccess.Read))
        {
            string[] scopes = { GmailService.Scope.GmailSend, GmailService.Scope.GmailCompose,GmailService.Scope.GmailReadonly };
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                scopes,
                "me",
                System.Threading.CancellationToken.None).Result;
        }

        // Create Gmail API service.
        var service = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Desktop client 1",
        });

        return service;
    }
}