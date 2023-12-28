using System;
using AdaptiveCards;
using Newtonsoft.Json.Linq;

namespace BotWithSK.Dialogs;

public class AdaptiveHelper
{
    public AdaptiveHelper()
    {
    }

    public string CreateErrorAdaptiveCardAttachment(string information)
    {
        var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 2));

        card.Body.Add(new AdaptiveTextBlock()
        {
            Text = information,
            Size = AdaptiveTextSize.ExtraLarge,
            Weight = AdaptiveTextWeight.Bolder
        });

        return card.ToJson();
    }

    public string CreateAdaptiveCardAttachment(string result)
    {

        JObject json = JObject.Parse(result);

        // Create an Adaptive Card
        AdaptiveCard card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 2));

        // Create a container to hold the content with vertical alignment
        AdaptiveContainer contentContainer = new AdaptiveContainer();
        contentContainer.Style = AdaptiveContainerStyle.Emphasis;

        // Text data
        contentContainer.Items.Add(new AdaptiveTextBlock()
        {
            Text = "Weather Information",
            Size = AdaptiveTextSize.ExtraLarge,
            Weight = AdaptiveTextWeight.Bolder
        });

        //contentContainer.Items.Add(new AdaptiveTextBlock()
        //{
        //    Text = json["CityResult"]?.ToString(),
        //    Size = AdaptiveTextSize.Medium,
        //    Weight = AdaptiveTextWeight.Bolder,
        //    Color = AdaptiveTextColor.Accent
        //});

        JObject weatherData = JObject.Parse(json["WeatherResult"]?.ToString());

        //var weatherInfo = $"{json["CityResult"]?.ToString()} Current Temperature: - {weatherData["current"]?["temp_c"]}°C}";

        var weatherInfo = $"{json["CityResult"]?.ToString()} \n\nCurrent Temperature: {weatherData["current"]?["temp_c"]}°C";


        contentContainer.Items.Add(new AdaptiveTextBlock()
        {
            Text = weatherInfo,
            Size = AdaptiveTextSize.Medium,
            Color = AdaptiveTextColor.Good
        });

        contentContainer.Items.Add(new AdaptiveTextBlock()
        {
            Text = "",
            Size = AdaptiveTextSize.ExtraLarge,
            Weight = AdaptiveTextWeight.Bolder
        });

        contentContainer.Items.Add(new AdaptiveTextBlock()
        {
            Text = "City-History Information",
            Size = AdaptiveTextSize.ExtraLarge,
            Weight = AdaptiveTextWeight.Bolder
        });


        contentContainer.Items.Add(new AdaptiveTextBlock()
        {
            Text = json["HistoryResult"]?.ToString(),
            Size = AdaptiveTextSize.Medium,
            Wrap = true,
            Color = AdaptiveTextColor.Attention
        });

        contentContainer.Items.Add(new AdaptiveTextBlock()
        {
            Text = "",
            Size = AdaptiveTextSize.ExtraLarge,
            Weight = AdaptiveTextWeight.Bolder
        });

        //landmark image
        contentContainer.Items.Add(new AdaptiveTextBlock()
        {
            Text = "Landmark - DALL-E Image",
            Size = AdaptiveTextSize.ExtraLarge,
            Weight = AdaptiveTextWeight.Bolder
        });

        // Image and Image Name
        AdaptiveColumnSet imageColumnSet = new AdaptiveColumnSet();

        // Left side: Image
        AdaptiveColumn imageColumn = new AdaptiveColumn();
        imageColumn.Items.Add(new AdaptiveImage()
        {
            Url = new Uri(json["ImageUrl"]?.ToString()),
            Size = AdaptiveImageSize.Auto,
            Style = AdaptiveImageStyle.Default,
            AltText = "DALL-E Image"
        });
        imageColumnSet.Columns.Add(imageColumn);

        // Right side: Image Name
        AdaptiveColumn nameColumn = new AdaptiveColumn();
        nameColumn.Items.Add(new AdaptiveTextBlock()
        {
            Text = json["ImageName"]?.ToString(),
            Size = AdaptiveTextSize.Medium,
            Weight = AdaptiveTextWeight.Bolder,
            Wrap = true
        });
        imageColumnSet.Columns.Add(nameColumn);

        contentContainer.Items.Add(imageColumnSet);

        card.Body.Add(contentContainer);



        // Serialize the Adaptive Card to JSON
        string adaptiveCardJson = card.ToJson();

        return adaptiveCardJson;
    }
}