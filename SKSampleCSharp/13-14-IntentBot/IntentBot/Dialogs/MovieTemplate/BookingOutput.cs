using System.Collections.Generic;

namespace TemplateBot.Dialogs.MovieTemplate;

public class BookingOutput
{
    public string Text { get; set; }
    public Intent Intent { get; set; }
    public List<EntityList> Entities { get; set; }
}