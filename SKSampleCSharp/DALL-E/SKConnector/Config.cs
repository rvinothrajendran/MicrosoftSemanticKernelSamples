namespace SKConnector;

public static class Config
{

    /// <summary>
    /// Azure OpenAI deployment name
    /// </summary>
    public static string DeploymentOrModelId => "gpt4model";

    /// <summary>
    /// Azure OpenAI endpoint
    /// </summary>
    public static string Endpoint => "";

    /// <summary>
    /// Azure OpenAI Key 
    /// </summary>
    public static string ApiKey => "";

    /// <summary>
    /// External API key for weather service
    /// </summary>
    public static string WeatherApiKey => "";

    //DALL-E Image Generation

    /// <summary>
    /// Image endpoint
    /// </summary>
    public static string ImageEndpoint => "";

    /// <summary>
    /// Image ApiKey 
    /// </summary>
    public static string ImageApiKey => "";

    /// <summary>
    /// Image ModelId
    /// </summary>
    public static string ImageModelId => "";
}