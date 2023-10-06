
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using System.ComponentModel;

namespace skills.Weather;
public class WeatherPlugIn
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public WeatherPlugIn(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
            throw new ArgumentNullException(nameof(apiKey));

        _apiKey = apiKey;
        _httpClient = new HttpClient();
    }

    [SKFunction, Description("get weather information based on the location")]
    [SKParameter("input", "location or cityName")]
    public SKContext? GetWeatherAsync(SKContext context)
    {
        
        context.Variables.TryGetValue("input", out var cityName);
        

        var apiUrl = $"http://api.weatherapi.com/v1/current.json?key={_apiKey}&q={cityName}&aqi=no";

        try
        {
            HttpResponseMessage response = _httpClient.GetAsync(apiUrl).Result;
            response.EnsureSuccessStatusCode();

            var responseBody = response.Content.ReadAsStringAsync().Result;

            context.Variables.Set("weather",responseBody);

            return context;
        }
        catch (HttpRequestException ex)
        {
            // Handle any exceptions here, such as network errors.
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }
}