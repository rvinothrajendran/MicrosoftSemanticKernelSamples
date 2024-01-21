using Microsoft.SemanticKernel;
using System.ComponentModel;
namespace KernelFunctionFactoryDemo.Plugins.VehiclePlugin;

public class VehiclePlugin
{
    [KernelFunction, Description("find bike model information")]
    public string BikeInformation([Description("model of the bike")] string model)
    {
        return $"Bike Model: {model}, Color: Red, Year: 2022";
    }

    [KernelFunction, Description("find car model information")]
    public string CarInformation([Description("model of the car")] string model)
    {
        return $"Car Model: {model}, Color: Blue, Year: 2023";
    }
}