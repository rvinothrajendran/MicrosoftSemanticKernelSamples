using System.ComponentModel.DataAnnotations;
using AzureAI.Community.Microsoft.Semantic.Kernel.Speech;
using ConfigSettings;
using Microsoft.SemanticKernel;
using AzureAI.Community.Microsoft.Semantic.Kernel.Speech.Plugin;
using Microsoft.CognitiveServices.Speech;

namespace SpeechInPlugin
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Speech PlugIn Demo");

var kernel = CreateKernelBuilder();

ISpeechToText speechToText = new SpeechToText(ConfigParameters.SpeechKey, ConfigParameters.SpeechRegion);
speechToText.SpeechConfig.SetProperty(PropertyId.Speech_SegmentationSilenceTimeoutMs,"3000");
speechToText.Build();

    //Import Speech plugin
    var speech = kernel.ImportSkill(new SpeechPlugin(speechToText), nameof(SpeechPlugin));

        //Create Semantic function
        var skFunction = kernel.CreateSemanticFunction("Process the request of the input {{$input}}");

    var result = await kernel.RunAsync(speech["ListenSpeechVoice"], skFunction);

    Console.WriteLine(result);
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
