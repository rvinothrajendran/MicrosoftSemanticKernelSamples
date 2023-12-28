using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextToImage;

namespace SKConnector;

public partial class SemanticKernelProcessor
{
    /// <summary>
    /// Read the city name , history information from the history plugin
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    private async Task<string?> ReadHistoryInfo(string prompt)
    {
        if (historyPlugin == null || !historyPlugin.Contains("history"))
            return null;


        var kernelArguments = new KernelArguments()
        {
            ["input"] = prompt,
        };

        var historyFunctionResult = await kernelBuilder.InvokeAsync(function: historyPlugin["history"], kernelArguments);

        var historyResults = historyFunctionResult.GetValue<string>();

        return historyResults;

    }

    /// <summary>
    /// Extract the historyPlugin result from the JSON plugin
    /// </summary>
    /// <param name="jsonInformation"></param>
    /// <returns></returns>
    private async Task<KernelArguments?> ExtractJsonInformation(string jsonInformation)
    {
        if (extractKernelPlugin is null || !extractKernelPlugin.Contains("ExtractInformation"))
            return null;

        var kernelArguments = new KernelArguments()
        {
            [KeyConst.HistoryPromptResult] = jsonInformation,
        };

        var extractFunctionResult =
            await kernelBuilder.InvokeAsync(extractKernelPlugin["ExtractInformation"], kernelArguments);

        var jsonResult = extractFunctionResult.GetValue<KernelArguments>();

        return jsonResult;

    }

    /// <summary>
    /// Get the weather information from the weather plugin
    /// </summary>
    /// <param name="cityName"></param>
    /// <returns></returns>
    private async Task<string?> GetWeatherInformation(string? cityName)
    {
        if (weatherKernelPlugin is null || !weatherKernelPlugin.Contains("GetWeatherAsync"))
            return null;

        var kernelArguments = new KernelArguments()
        {
            [KeyConst.CityName] = cityName,
        };

        var weatherFunctionResult =
            await kernelBuilder.InvokeAsync(weatherKernelPlugin["GetWeatherAsync"], kernelArguments);

        var weatherResults = weatherFunctionResult.GetValue<string>();

        return weatherResults;
    }

    private async Task<(string landMarkInfo,string imageUrl)?> GenerateImage(string cityName)
    {
        try
        {
            //Create landmark prompt
            var prompt = "landmark of the city {{$input}}";

            OpenAIPromptExecutionSettings aiPromptExecutionSettings = new()
            {
                MaxTokens = 20,
                Temperature = 1,
            };

            var promptTemplate = kernelBuilder.CreateFunctionFromPrompt(prompt, aiPromptExecutionSettings);

            //Connect to Azure Openai to find landmark information
            var landmarkResult = await kernelBuilder.InvokeAsync(promptTemplate, new KernelArguments()
            {
                ["input"] = cityName,
            });

            var landmarkInformation = landmarkResult.GetValue<string>();

            //Create image prompt

#pragma warning disable SKEXP0002
            var imageService = kernelBuilder.GetRequiredService<ITextToImageService>();

            var imageUrl = await imageService.GenerateImageAsync(landmarkInformation!, 1024, 1024, kernelBuilder);

#pragma warning restore SKEXP0002

            return (landmarkInformation!, imageUrl!);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);

        }

        return null;
    }
    
}