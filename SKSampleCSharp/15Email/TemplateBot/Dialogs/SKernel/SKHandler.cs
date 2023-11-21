using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using System.IO;
using System.Threading.Tasks;
using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Google;
using Google.Apis.Gmail.v1;
using Microsoft.SemanticKernel.Plugins.MsGraph;
using TemplateBot.Dialogs.MailService;

namespace TemplateBot.Dialogs.SKernel;

public static class SKHandler
{
    private static readonly GmailService GMailService;
    static SKHandler()
    {
        GMailService = GmailServiceHelper.GetGmailService();
    }
    public static async Task<bool> ProcessMessage(string prompt)
    {

        //Create Kernel
        IKernel kernel = new KernelBuilder().WithAzureOpenAIChatCompletionService(Settings.DeploymentName,
            Settings.EndPoint,
            Settings.Key).Build();

        kernel.FunctionInvoked += HandleFunctionInvoked;

        var plugsDirectory= Path.Combine(Directory.GetCurrentDirectory(), @"Dialogs\ServiceTemplate");
        var importFunctions = kernel.ImportSemanticFunctionsFromDirectory(plugsDirectory, "Request");

        //Gmail Service Plugin

        IEmailConnector gMailConnector = new GMailConnector(GMailService);

        EmailPlugin emailPlugin = new EmailPlugin(gMailConnector);
        var gmailFunctions = kernel.ImportFunctions(emailPlugin, nameof(EmailPlugin));

        var context = new ContextVariables();
        context.Set("input", "I'm having trouble logging into my movie streaming account.");

        var result = await kernel.RunAsync(context, importFunctions["employee"], gmailFunctions["SendEmail"]);

        return true;
    }

    private static void HandleFunctionInvoked(object sender, Microsoft.SemanticKernel.Events.FunctionInvokedEventArgs e)
    {
        if (e.FunctionView.Name == "employee")
        {
            e.SKContext.Variables.TryGetValue("input", out var inputValue);

            if (inputValue != null)
            {
                var customerQuery = System.Text.Json.JsonSerializer.Deserialize<CustomerQuery>(inputValue);

                var mailInformation = customerQuery.ToString();
                e.SKContext.Variables.Set("input", mailInformation);

                if (customerQuery.Main == "Buffering issues")
                {
                    e.SKContext.Variables.Set(EmailPlugin.Parameters.Subject, "Buffering issues");
                    e.SKContext.Variables.Set(EmailPlugin.Parameters.Recipients, "r.vinoth@live.com");
                }
            }
        }
        
    }
}

public class CustomerQuery
{
    public string Main { get; set; }
    public string SubCategory { get; set; }
    public string CustomerQueries { get; set; }

    public override string ToString()
    {
        return  $"Main Issue: {Main}\nSub Category: {SubCategory}\nCustomer Queries: {CustomerQueries}";
    }
}

