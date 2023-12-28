using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace NativePlugin.Plugins.WeatherPlugin;

public class ExternalWeatherPlugin
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public static string GetWeather => "GetWeatherAsync";

    public ExternalWeatherPlugin()
    {
        _apiKey = Config.WeatherApiKey;
        _httpClient = new HttpClient();
    }

    [KernelFunction, Description("get weather information based on the location")]
    public string? GetWeatherAsync(KernelArguments kernelArguments)
    {

        kernelArguments.TryGetValue("input", out var cityName);


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