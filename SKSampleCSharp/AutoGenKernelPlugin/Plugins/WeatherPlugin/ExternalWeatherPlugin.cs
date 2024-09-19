using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace AutoGenKernelPlugin.Plugins.WeatherPlugin;

public class ExternalWeatherPlugin
{
    private readonly string _apiKey = Config.WeatherApiKey;
    private readonly HttpClient _httpClient = new();

    public static string GetWeather => "GetWeatherAsync";

    [KernelFunction, Description("get weather information based on the location")]
    public string? GetWeatherAsync(string cityName)
    {

        var apiUrl = $"http://api.weatherapi.com/v1/current.json?key={_apiKey}&q={cityName.ToString()}&aqi=no";

        try
        {
            HttpResponseMessage response = _httpClient.GetAsync(apiUrl).Result;
            response.EnsureSuccessStatusCode();

            var responseBody = response.Content.ReadAsStringAsync().Result;

            return responseBody;
        }
        catch (HttpRequestException ex)
        {
            // Handle any exceptions here, such as network errors.
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }
}
