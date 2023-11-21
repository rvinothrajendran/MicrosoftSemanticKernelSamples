using System.Collections.Generic;
using TemplateBot.Dialogs.Template;

namespace TemplateBot.Dialogs.MovieTemplate;

public class BookingResult
{
    public string Input { get; set; }
    public BookingOutput Output { get; set; }
    public List<string> YouTubeLinks { get; set; }
}

public class BookingOutput
{
    public string Text { get; set; }
    public Intent Intent { get; set; }
    public List<EntityList> Entities { get; set; }
}

public class Entity
{
    public string Type { get; set; }
    public string Value { get; set; }
}

public class MovieData
{
    public string Intent { get; set; }
    public List<Entity> Entities { get; set; }
    public List<string> YouTubeLinks { get; set; }
}