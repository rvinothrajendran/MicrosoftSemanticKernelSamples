using System.Collections.Generic;

namespace TemplateBot.Dialogs.PizzaTemplate;

public class Pizza
{
    public List<Order> Orders { get; set; }
    public List<Order> ReOrder { get; set; }
    public List<Order> CancelOrder { get; set; }

    public List<Order> Unknown { get; set; }

    private int count = 0;
    public int TotalCount
    {
        get
        {
            count = Orders?.Count ?? 0;
            count += ReOrder?.Count ?? 0;
            count += CancelOrder?.Count ?? 0;
            count += Unknown?.Count ?? 0;
            return count;
        }
    }
}