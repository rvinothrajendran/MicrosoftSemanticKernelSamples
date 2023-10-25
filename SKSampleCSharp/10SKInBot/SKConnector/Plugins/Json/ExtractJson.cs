using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Newtonsoft.Json.Linq;

namespace SKConnector.Plugins.Json;

public class ExtractJson
{

    [SKFunction, Description("Extract information from the JSON")]
    public SKContext? ExtractInformation(SKContext? context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        context.Variables.TryGetValue("input", out var inputValue);

        JObject jsonJObject = JObject.Parse(inputValue);

        if (jsonJObject.TryGetValue("cityname", out var city))
        {
            context.Variables.Set("input", city.ToString());
        }

        if (jsonJObject.TryGetValue("history", out var histpory))
        {
            context.Variables.Set("history", city.ToString());
        }

        return context;
    }

}