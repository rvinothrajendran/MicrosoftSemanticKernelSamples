using System.ComponentModel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Newtonsoft.Json.Linq;

namespace FunctionHooks.skills.Json;

public class ExtractJson
{

    [SKFunction, Description("Extract information from the JSON")]
    public SKContext? ExtractInformation(SKContext? context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        context.Variables.TryGetValue("INPUT", out var inputValue);

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