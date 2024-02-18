using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace InterruptPlanner.Plugins.WeatherPlugin;

public class ExternalWeatherPlugin
{
    private readonly HttpClient httpClient = new();

    public static string GetWeather => "GetWeatherAsync";

    private string? ApiKey;

    public ExternalWeatherPlugin(Kernel kernel, string apikey)
    {
        kernel.FunctionFilters.Add(new WeatherFilter());
        ApiKey = apikey;
    }

    [KernelFunction, Description("get weather information based on the location")]
    public string? GetWeatherAsync(string cityName)
    {

        return "Weather information for " + cityName + " is not available";

        var apiUrl = $"http://api.weatherapi.com/v1/current.json?key={ApiKey}&q={cityName.ToString()}&aqi=no";

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

public class WeatherFilter : IFunctionFilter
{
    public void OnFunctionInvoked(FunctionInvokedContext context)
    {
        
    }

    public void OnFunctionInvoking(FunctionInvokingContext context)
    {
        if (context.Function.Name == ExternalWeatherPlugin.GetWeather)
        {
            
        }
    }
}