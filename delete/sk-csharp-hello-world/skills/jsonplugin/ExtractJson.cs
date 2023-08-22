using System.ComponentModel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Newtonsoft.Json.Linq;

namespace skills.jsonplugin;

public class ExtractJson
{
    [SKFunction,Description("Extract information from json")]
    public SKContext?  ExtractInformation(SKContext? context)
    {
        if (context != null)
        {
            JObject obj = JObject.Parse(context["input"]);
        
            if (obj.ContainsKey("cityname"))
            {
                context["input"] = (string)obj["cityname"];
            }
            
            if (obj.ContainsKey("history"))
            {
                context["history"] = (string)obj["history"];
            }
        }

        return context;
    }
}