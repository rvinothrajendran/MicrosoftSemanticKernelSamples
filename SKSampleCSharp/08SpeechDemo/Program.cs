using AzureAI.Community.Microsoft.Semantic.Kernel.Speech;
using ConfigSettings;
using Microsoft.SemanticKernel;
using AzureAI.Community.Microsoft.Semantic.Kernel.Speech.Plugin;
using Microsoft.CognitiveServices.Speech;

namespace SpeechDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("SK Speech plug In Demo");

            var kernel = CreateKernelBuilder();

            //Create Speech to text object

            ISpeechToText speechToText = new SpeechToText(ConfigParameters.SpeechKey, ConfigParameters.SpeechRegion);
            speechToText.SpeechConfig.SetProperty(PropertyId.Speech_SegmentationSilenceTimeoutMs,"2000");
            
            speechToText.Build();

            //Import Speech plugin 
            var speechDictionary = kernel.ImportSkill(new SpeechPlugin(speechToText), nameof(SpeechPlugin));

            //Create a SK Function

            var skFunction = kernel.CreateSemanticFunction("Process the input {{$input}}");

            
            var result = await kernel.RunAsync(speechDictionary["ListenSpeechVoice"],skFunction);

            Console.WriteLine(result);
            Console.WriteLine("Done");

            Console.Read();
        }


        private static IKernel CreateKernelBuilder()
        {
            //Create a kernel builder
            var builder = new KernelBuilder();

            builder.WithAzureTextCompletionService(ConfigParameters.DeploymentOrModelId, ConfigParameters.Endpoint,
                ConfigParameters.ApiKey);

            return builder.Build();
        }
    }
}
