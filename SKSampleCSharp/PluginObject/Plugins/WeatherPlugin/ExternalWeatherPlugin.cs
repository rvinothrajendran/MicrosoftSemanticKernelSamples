using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace PluginObject.Plugins.WeatherPlugin;

public class ExternalWeatherPlugin(string apiKey)
{
    private readonly HttpClient _httpClient = new HttpClient();

    public static string GetWeather => "GetWeatherAsync";

    [KernelFunction, Description("get weather information based on the location")]
    public string? GetWeatherAsync(KernelArguments kernelArguments)
    {

        kernelArguments.TryGetValue("input", out var cityName);


        var apiUrl = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={cityName.ToString()}&aqi=no";

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