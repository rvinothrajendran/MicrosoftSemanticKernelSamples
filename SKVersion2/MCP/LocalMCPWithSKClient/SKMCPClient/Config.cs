namespace SKMCPClient;

#pragma warning disable
public class Config 
{
    public string DeploymentOrModelId { get; } = Environment.GetEnvironmentVariable("MODEL_40",
        EnvironmentVariableTarget.Machine) ?? string.Empty;
    public string Endpoint { get; } = Environment.GetEnvironmentVariable("ENDPOINT",
        EnvironmentVariableTarget.Machine) ?? string.Empty;
    public string ApiKey { get; } = Environment.GetEnvironmentVariable("APIKEY",
        EnvironmentVariableTarget.Machine) ?? string.Empty;
    public string BingMapKey { get; } = Environment.GetEnvironmentVariable("BINGMAP",
        EnvironmentVariableTarget.Machine) ?? string.Empty;
      
}