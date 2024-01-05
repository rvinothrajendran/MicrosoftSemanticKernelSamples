namespace BingMapPluginDemo
{
    /// <summary>
    /// Configurations for Azure OpenAI Chat Completion service
    /// </summary>
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
      /// Bing Map Key ( Create a Key in Azure Portal )
      /// </summary>
        public static string BingMapKey => "";
    }
}
