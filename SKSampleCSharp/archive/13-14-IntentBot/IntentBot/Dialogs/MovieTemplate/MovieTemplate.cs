using System.Collections.Generic;
using System.IO;
using System.Text;
using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Intent.Template;
using Newtonsoft.Json;

namespace TemplateBot.Dialogs.MovieTemplate;

public static class Movie
{
    public static TemplateExamples[] GenerateTemplate()
    {
        string jsonContent = File.ReadAllText(@"Dialogs\MovieTemplate\movie.json");

        TemplateExamples[] examples;
        try
        {
            var booking = JsonConvert.DeserializeObject<List<BookingTemplate>>(jsonContent);
            int index = 0;

            examples = new TemplateExamples[booking.Count];

            foreach (var order in booking)
            {
                string input = order.Input;
                string outputText = GetFormattedOutput(order.Output);

                var templateExamples = new TemplateExamples
                {
                    Input = input,
                    Output = outputText
                };

                examples[index++] = templateExamples;
            }
        }
        catch (System.Exception ex)
        {
            throw;
        }

        return examples;
    }

    static string GetFormattedOutput(BookingOutput order)
    {
        StringBuilder formattedOutput = new StringBuilder();

        formattedOutput.AppendLine("Output:");

        formattedOutput.AppendLine($"  Text: {order.Text}");
        formattedOutput.AppendLine($"  Intent Name: {order.Intent.Name}");
        formattedOutput.AppendLine($"  Intent Score: {order.Intent.Score}");

        formattedOutput.AppendLine("  Entities:");
        foreach (var entity in order.Entities)
        {
            formattedOutput.AppendLine($"    Entity: {entity.Entity}");
            formattedOutput.AppendLine($"    Type: {entity.Type}");
            formattedOutput.AppendLine($"    StartPos: {entity.StartPos}");
            formattedOutput.AppendLine($"    EndPos: {entity.EndPos}");
            formattedOutput.AppendLine($"    Value: {entity.Value}");
            formattedOutput.AppendLine($"    Size: {entity.Size}");
        }



        return formattedOutput.ToString();
    }

}