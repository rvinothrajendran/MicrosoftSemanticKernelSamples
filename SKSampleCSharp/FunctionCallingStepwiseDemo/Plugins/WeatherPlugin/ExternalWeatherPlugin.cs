using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace FunctionCallingStepwiseDemo.Plugins.WeatherPlugin;

public class ExternalWeatherPlugin(string apikey)
{
    private readonly HttpClient httpClient = new();

    public static string GetWeather => "GetWeatherAsync";

    [KernelFunction, Description("get weather information based on the location")]
    public string? GetWeatherAsync(string cityName)
    {
        
        var apiUrl = $"http://api.weatherapi.com/v1/current.json?key={apikey}&q={cityName}&aqi=no";

        try
        {
            HttpResponseMessage response = httpClient.GetAsync(apiUrl).Result;
            response.EnsureSuccessStatusCode();

            var responseBody = response.Content.ReadAsStringAsync().Result;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");
            Console.WriteLine("Weather Information");
            Console.WriteLine(responseBody);
            Console.WriteLine("");

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