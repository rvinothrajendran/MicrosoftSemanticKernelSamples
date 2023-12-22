using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace SKConnector.config;

internal class KernelSettings
{
    public const string DefaultConfigFile = "config/appsettings.json";

    [JsonPropertyName("endpointType")]
    public string EndpointType { get; set; } = EndpointTypes.TextCompletion;

    [JsonPropertyName("serviceType")]
    public string ServiceType { get; set; } = string.Empty;

    [JsonPropertyName("serviceId")]
    public string ServiceId { get; set; } = string.Empty;

    [JsonPropertyName("deploymentOrModelId")]
    public string DeploymentOrModelId { get; set; } = string.Empty;

    [JsonPropertyName("endpoint")]
    public string Endpoint { get; set; } = string.Empty;

    [JsonPropertyName("apiKey")]
    public string ApiKey { get; set; } = string.Empty;

    [JsonPropertyName("orgId")]
    public string OrgId { get; set; } = string.Empty;

    [JsonPropertyName("logLevel")]
    public LogLevel? LogLevel { get; set; }

    /// <summary>
    /// Load the kernel settings from settings.json if the file exists and if not attempt to use user secrets.
    /// </summary>
    internal static KernelSettings LoadSettings()
    {
        try
        {
            KernelSettings settings = new KernelSettings
            {
                ApiKey = Settings.ApiKey,
                Endpoint = Settings.Endpoint,
                DeploymentOrModelId = Settings.DeploymentOrModelId,
                EndpointType = Settings.EndpointType,
                ServiceType = Settings.ServiceType
            };

            return settings;
           
        }
        catch (InvalidDataException ide)
        {
            Console.Error.WriteLine(
                "Unable to load semantic kernel settings, please provide configuration settings using instructions in the README.\n" +
                "Please refer to: https://github.com/microsoft/semantic-kernel-starters/blob/main/sk-csharp-hello-world/README.md#configuring-the-starter"
            );
            throw new InvalidOperationException(ide.Message);
        }
    }
    
}