using System.IO;
using System.Text;
using AzureAI.Community.Microsoft.Semantic.Kernel.PlugIn.Intent.Template;
using Newtonsoft.Json;

namespace TemplateBot.Dialogs.PizzaTemplate;

public static class PizzaTemplate
{
    public static TemplateExamples[] GenerateTemplate()
    {
        string jsonContent = File.ReadAllText(@"Dialogs\PizzaTemplate\pizzaexample.json");

        Pizza data = JsonConvert.DeserializeObject<Pizza>(jsonContent);

        TemplateExamples[] examples = new TemplateExamples[data.TotalCount];

        int index = 0;

        foreach (var order in data.Orders)
        {
            string input = order.Input;
            string outputText = GetFormattedOutput(order);

            var templateExamples = new TemplateExamples
            {
                Input = input,
                Output = outputText
            };

            examples[index++] = templateExamples;
        }

        foreach (var order in data.ReOrder)
        {
            string input = order.Input;
            string outputText = GetFormattedOutput(order);

            var templateExamples = new TemplateExamples
            {
                Input = input,
                Output = outputText
            };

            examples[index++] = templateExamples;
        }

        foreach (var order in data.CancelOrder)
        {
            string input = order.Input;
            string outputText = GetFormattedOutput(order);

            var templateExamples = new TemplateExamples
            {
                Input = input,
                Output = outputText
            };

            examples[index++] = templateExamples;
        }

        foreach (var order in data.Unknown)
        {
            string input = order.Input;
            string outputText = GetFormattedOutput(order);

            var templateExamples = new TemplateExamples
            {
                Input = input,
                Output = outputText
            };

            examples[index++] = templateExamples;
        }

        return examples;
    }

    static string GetFormattedOutput(Order order)
    {
        StringBuilder formattedOutput = new StringBuilder();

        formattedOutput.AppendLine("Output:");

        formattedOutput.AppendLine($"  Text: {order.Output.Text}");
        formattedOutput.AppendLine($"  Intent Name: {order.Output.Intent.Name}");
        formattedOutput.AppendLine($"  Intent Score: {order.Output.Intent.Score}");

        formattedOutput.AppendLine("  Entities:");
        foreach (var entity in order.Output.Entities)
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