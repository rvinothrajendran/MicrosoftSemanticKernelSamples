using System.Collections.Generic;

namespace TemplateBot.Dialogs.Template;

public class Output
{
    public string Text { get; set; }
    public Intent Intent { get; set; }
    public List<Entities> Entities { get; set; }
    
}