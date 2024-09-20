using System.ComponentModel;
using AutoFunctionCalling;
using Microsoft.SemanticKernel;

namespace RequiredFunctionCalling.Plugins;

public class ExternalWeatherPlugin
{
    private readonly string apiKey = GitConfig.WeatherApiKey;
    private readonly HttpClient httpClient = new();

    public static string GetWeather => "GetWeatherAsync";

    [KernelFunction, Description("get weather information based on the location")]
    public string? GetWeatherAsync(string cityName)
    {

        var apiUrl = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={cityName.ToString()}&aqi=no";

        try
        {
            HttpResponseMessage response = httpClient.GetAsync(apiUrl).Result;
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