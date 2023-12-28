using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace SKConnector.Plugins.Json;

public class ExtractJson
{

    [KernelFunction, Description("Extract information from the JSON")]
    public KernelArguments ExtractInformation(KernelArguments context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var arguments = new KernelArguments();

        if (context.Names.Contains(KeyConst.HistoryPromptResult))
        {
            string? inputValue = context[KeyConst.HistoryPromptResult]?.ToString();

            if (inputValue is not null)
            {
                JObject jsonJObject = JObject.Parse(inputValue);

                if (jsonJObject.TryGetValue(KeyConst.CityName, out var city))
                {
                    arguments.Add(KeyConst.CityName, city);
                }

                if (jsonJObject.TryGetValue(KeyConst.History, out var history))
                {
                    arguments.Add(KeyConst.History, history);
                }
            }
        }

        return arguments;

    }
}