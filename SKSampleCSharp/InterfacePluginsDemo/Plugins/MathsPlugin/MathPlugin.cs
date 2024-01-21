using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace InterfacePluginsDemo.Plugins.MathsPlugin;

public class MathPlugin : IAddPlugin
{
    public double Add([Description("The first number to add")] double number1,
        [Description("The second number to add")] double number2
    )
    {
        return number1 + number2;
    }

    public double Subtract([Description("The first number to subtract from")] double number1,
        [Description("The second number to subtract away")] double number2)
    
    {
        return number1 - number2;
    }

    [KernelFunction, Description("Multiply two numbers. When increasing by a percentage, don't forget to add 1 to the percentage.")]
    public double Multiply(
        [Description("The first number to multiply")] double number1,
        [Description("The second number to multiply")] double number2)
    {
        return number1 * number2;
    }

    [KernelFunction, Description("Divide two numbers")]
    public double Divide(
        [Description("The first number to divide from")] double number1,
        [Description("The second number to divide by")] double number2)
    {
        return number1 / number2;
    }
    
}