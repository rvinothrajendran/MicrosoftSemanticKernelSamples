namespace LLMModelFactory;

#pragma warning disable
public class Config 
{
    public string DeploymentOrModelId { get; } = Environment.GetEnvironmentVariable("MODEL_40",
        EnvironmentVariableTarget.Machine) ?? string.Empty;
    public string Endpoint { get; } = Environment.GetEnvironmentVariable("ENDPOINT",
        EnvironmentVariableTarget.Machine) ?? string.Empty;
    public string ApiKey { get; } = Environment.GetEnvironmentVariable("APIKEY",
        EnvironmentVariableTarget.Machine) ?? string.Empty;
  
    
    public string LlamaModel { get; } = "llama3.2";
    public string OllamaUri { get; } = "http://localhost:11434";

    public string GitHubModel { get; } = "gpt-4o-mini"; // Default model for GitHub, can be overridden by environment variable
    public string GitHubUri { get; } = "https://models.github.ai/inference"; // Default URI for GitHub, can be overridden by environment variable
    public string GitHubToken { get; } = Environment.GetEnvironmentVariable("GITHUB_TOKEN", EnvironmentVariableTarget.User) ?? string.Empty;
}