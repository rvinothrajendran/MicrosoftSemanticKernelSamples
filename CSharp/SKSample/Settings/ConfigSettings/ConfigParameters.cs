namespace ConfigSettings;

public static class ConfigParameters
{
    public static string ApiKey { get; } = "";
    public static string Endpoint { get; } = "";
    public static string DeploymentOrModelId { get; } = "text-davinci-003";

    public static string EndpointType { get; } = "text-completion";
    public static string ServiceType { get; } = "AZUREOPENAI";

    public static string WeatherApiKey { get; } = "";


    public static string SpeechKey = "";
    public static string SpeechRegion = "";

}
