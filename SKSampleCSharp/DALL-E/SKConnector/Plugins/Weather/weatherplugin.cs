using System.ComponentModel;

namespace SKConnector.Plugins.Weather;
public class WeatherPlugIn
{
    private readonly string apiKey;
    private readonly HttpClient httpClient;

    public WeatherPlugIn(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
            throw new ArgumentNullException(nameof(apiKey));

        this.apiKey = apiKey;
        httpClient = new HttpClient();
    }

    [KernelFunction, Description("get weather information based on the location")]
    public string GetWeatherAsync(KernelArguments context)
    {
        var weatherResult = string.Empty;

        if (context.Names.Contains("cityname"))
        {
            var cityName = context["cityname"]?.ToString();

            var apiUrl = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={cityName}&aqi=no";

            try
            {
                HttpResponseMessage response = httpClient.GetAsync(apiUrl).Result;
                response.EnsureSuccessStatusCode();

                weatherResult = response.Content.ReadAsStringAsync().Result;

                
            }
            catch (HttpRequestException ex)
            {
                
            }

        }

        return weatherResult;
    }

  
}