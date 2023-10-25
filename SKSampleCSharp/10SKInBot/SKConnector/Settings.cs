using SKConnector.config;

namespace SKConnector;

public static class Settings
{
    public static string ServiceType => ServiceTypes.AzureOpenAI;
    public static string EndpointType => EndpointTypes.TextCompletion;
    public static string ServiceId => string.Empty;
    public static string DeploymentOrModelId => "gpt4model";
    public static string Endpoint => "";
    public static string ApiKey => "";

    public static string WeatherApiKey => "";
}