namespace PromptFilter
{
    /// <summary>
    /// Configurations for Azure OpenAI Chat Completion service
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Azure OpenAI deployment name
        /// </summary>
        public static string DeploymentOrModelId => "youtube4o";

        /// <summary>
        /// Azure OpenAI endpoint
        /// </summary>
        public static string Endpoint => "https://openai-youtubedemo.openai.azure.com/";

        /// <summary>
        /// Azure OpenAI Key 
        /// </summary>
        public static string ApiKey => "fd6ae1af67b44ba6a72164ea10fbc5eb";

        /// <summary>
        /// External API key for weather service
        /// </summary>
        public static string WeatherApiKey => "";
    }
}
