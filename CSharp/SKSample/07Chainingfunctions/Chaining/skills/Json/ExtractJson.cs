using System.ComponentModel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Newtonsoft.Json.Linq;

namespace skills.Json;

public class ExtractJson
{

    [SKFunction, Description("Extract information from the JSON")]
    public SKContext? ExtractInformation(SKContext? context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        JObject jsonJObject = JObject.Parse(context["input"]);

        if (jsonJObject.TryGetValue("cityname", out var city))
        {
            context["input"] = city.ToString();
        }

        if (jsonJObject.TryGetValue("history", out var histpory))
        {
            context["history"] = histpory.ToString();
        }

        return context;
    }

}