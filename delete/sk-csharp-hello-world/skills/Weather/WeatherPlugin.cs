using System.ComponentModel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace SKWeatherPlugin.Plugins.Weather;

public class WeatherPlugin
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public WeatherPlugin(string apiKey)
    {
        _apiKey = "a72328bf37064cc6b9a81119232707";
        //_apiKey = apiKey;
        _httpClient = new HttpClient();
    }

    [SKFunction, Description("get weather information based on the location")]
    [SKParameter("input", "location or cityName")]
    public string? GetWeatherAsync(SKContext context)
    {

        var cityName = context["input"];

        var apiUrl = $"http://api.weatherapi.com/v1/current.json?key={_apiKey}&q={cityName}&aqi=no";

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