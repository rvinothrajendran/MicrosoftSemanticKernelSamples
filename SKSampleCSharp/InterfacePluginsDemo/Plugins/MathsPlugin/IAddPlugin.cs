using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace InterfacePluginsDemo.Plugins.MathsPlugin;


[Description("This interface contains simple maths functions")]
public interface IAddPlugin
{
    [KernelFunction, Description("Add two numbers")]
    double Add(
        [Description("The first number to add")] double number1,
        [Description("The second number to add")] double number2
    );

    [KernelFunction, Description("Subtract two numbers")]
    double Subtract(
        [Description("The first number to subtract from")] double number1,
        [Description("The second number to subtract away")] double number2
    );
   
}