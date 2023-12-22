using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SKConnector.Plugins.Json;
using SKConnector.Plugins.Weather;

namespace SKConnector
{
    public class SemanticHelper
    {
        private readonly IKernel kernel = new KernelBuilder()
            .WithAzureChatCompletionService(Settings.DeploymentOrModelId, Settings.Endpoint, Settings.ApiKey)
            .Build();

        public async Task<string> RequestInformation(string prompt)
        {
            
            var plugInsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            var skFunctions = kernel.ImportSemanticFunctionsFromDirectory(plugInsDir, "citySkill");
            
            kernel.ImportFunctions(new ExtractJson(), nameof(ExtractJson));
            kernel.ImportFunctions(new WeatherPlugIn(Settings.WeatherApiKey), nameof(WeatherPlugIn));

            
            var jsonSkillFunc = kernel.Functions.GetFunction(nameof(ExtractJson), "ExtractInformation");
            var weatherFunc = kernel.Functions.GetFunction(nameof(WeatherPlugIn), "GetWeatherAsync");

            var context = new ContextVariables();
            context.Set("input", prompt);

            var result = await kernel.RunAsync(context, skFunctions["history"], jsonSkillFunc, weatherFunc);

            var funResults = result.FunctionResults;

            string? historyResult = string.Empty;
            string? weatherResult = string.Empty;
            string? cityResult = string.Empty;

            var funcHistory = funResults.FirstOrDefault(x => x.FunctionName == "history");

            if (funcHistory != null)
            {
                var jsonValue = funcHistory.GetValue<string>();
                if (jsonValue != null)
                {
                    JObject jsonJObject = JObject.Parse(jsonValue);
                    if (jsonJObject.TryGetValue("history", out var jsonHistory))
                    {
                        historyResult = jsonHistory.ToString();
                    }

                    if (jsonJObject.TryGetValue("cityname", out var city))
                    {
                        cityResult = city.ToString();
                    }
                }
            }

            var funcWeather = funResults.FirstOrDefault(x => x.FunctionName == "GetWeatherAsync");
            if (funcWeather != null)
            {
                weatherResult = funcWeather.GetValue<string>();
            }

            var data = new
            {
                HistoryResult = historyResult,
                WeatherResult = weatherResult,
                CityResult = cityResult
            };

            return JsonConvert.SerializeObject(data);
        }
    }
}
