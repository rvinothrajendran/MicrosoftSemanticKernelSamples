using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LocalMCPServer.Tool;

[McpServerToolType]
public class ExternalWeatherTool
{
    private readonly HttpClient _httpClient = new();

    [McpServerTool, Description("get weather information based on the location")]
    public string? GetWeatherAsync(string cityName)
    {

        var apiUrl = $"http://api.weatherapi.com/v1/current.json?key={Config.WeatherApiKey}&q={cityName}&aqi=no";

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