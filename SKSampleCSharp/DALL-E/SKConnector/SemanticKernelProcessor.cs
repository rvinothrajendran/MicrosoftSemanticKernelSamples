using Azure.AI.OpenAI;

namespace SKConnector
{
    public partial class SemanticKernelProcessor
    {
        private readonly Kernel kernelBuilder;

        private KernelPlugin? historyPlugin;
        private KernelPlugin? extractKernelPlugin;
        private KernelPlugin? weatherKernelPlugin;

        public SemanticKernelProcessor()
        {

#pragma warning disable SKEXP0012
            kernelBuilder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey)
                .AddAzureOpenAITextToImage(Config.ImageModelId,Config.ImageEndpoint,Config.ImageApiKey)
                .Build();
#pragma warning restore SKEXP0012
            InitPlugin();
        }

        /// <summary>
        /// Import the plugins
        /// </summary>
        private void InitPlugin()
        {
            //import history plugin
            var plugInsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

            if (!kernelBuilder.Plugins.Contains("historyplugin"))
            {
                historyPlugin =
                    kernelBuilder.ImportPluginFromPromptDirectory(Path.Combine(plugInsDir, "CitySkill"),
                        "historyplugin");
            }

            //Import JSON plugin
            if (!kernelBuilder.Plugins.Contains(nameof(ExtractJson)))
            {
                extractKernelPlugin = kernelBuilder.ImportPluginFromType<ExtractJson>(nameof(ExtractJson));
            }

            //Import Weather plugin
            if (!kernelBuilder.Plugins.Contains(nameof(WeatherPlugIn)))
            {
                var weatherPlugin = new WeatherPlugIn(Config.WeatherApiKey);
                weatherKernelPlugin = kernelBuilder.ImportPluginFromObject(weatherPlugin, nameof(WeatherPlugIn));
            }
        }

        /// <summary>
        /// Process the information from the user
        /// </summary>
        /// <param name="prompt"> </param>
        /// <returns> </returns>
        public async Task<string?> ProcessInformation(string prompt)
        {
            try
            {
                //Read the history information from the history plugin
                var historyInfo = await ReadHistoryInfo(prompt);

                if (historyInfo is null)
                    return null;

                //Extract the history information from the JSON plugin
                var jsonResult = await ExtractJsonInformation(historyInfo);

                if (jsonResult is null)
                {
                    return null;
                }

                //Get the city name from the JSON plugin
                jsonResult.TryGetValue(KeyConst.CityName, out var cityObject);

                var cityName = cityObject?.ToString();

                if (string.IsNullOrEmpty(cityName))
                {
                    return null;
                }

                //Get the weather information from the weather plugin
                var weatherResults = await GetWeatherInformation(cityName) ?? "The weather data is unavailable.";

                var imageInformation = await GenerateImage(cityName);

                //Return the result
                var data = new
                {
                    HistoryResult = jsonResult[KeyConst.History],
                    WeatherResult = weatherResults,
                    CityResult = jsonResult[KeyConst.CityName],
                    ImageUrl = imageInformation?.imageUrl,
                    ImageName = imageInformation?.landMarkInfo
                };

                return JsonConvert.SerializeObject(data);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
    }
}
