﻿using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace InvocationFilter.Plugins.WeatherPlugin;

public class ExternalWeatherPlugin(string apikey)
{
    private readonly HttpClient httpClient = new();

    public static string GetWeather => "GetWeatherAsync";

    [KernelFunction, Description("get weather information based on the location")]
    public string? GetWeatherAsync(string cityName)
    {
        
        return "Weather information for " + cityName + " is not available";

        var apiUrl = $"http://api.weatherapi.com/v1/current.json?key={apikey}&q={cityName.ToString()}&aqi=no";

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