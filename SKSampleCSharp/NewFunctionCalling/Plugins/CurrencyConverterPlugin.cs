using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace AutoFunctionCalling.Plugins;

[Description("CurrencyConverter")]
public class CurrencyConverterPlugin
{
    private const double EuroToInrRate = 88.0;
    private const double EuroToDollarRate = 1.1;
    private const double DollarToInrRate = 80.0;
    private const double InrToEuroRate = 1 / EuroToInrRate;
    private const double InrToDollarRate = 1 / DollarToInrRate;
    private const double DollarToEuroRate = 1 / EuroToDollarRate;

    [KernelFunction, Description("Convert Euro to INR")]
    public double EuroToInr(double amount)
    {
        return amount * EuroToInrRate;
    }

    [KernelFunction, Description("Convert Euro to Dollar")]
    public double EuroToDollar(double amount)
    {
        return amount * EuroToDollarRate;
    }

    [KernelFunction, Description("Convert Dollar to INR")]
    public double DollarToInr(double amount)
    {
        return amount * DollarToInrRate;
    }

    [KernelFunction, Description("Convert INR to Euro")]
    public double InrToEuro(double amount)
    {
        return amount * InrToEuroRate;
    }

    [KernelFunction, Description("Convert INR to Dollar")]
    public double InrToDollar(double amount)
    {
        return amount * InrToDollarRate;
    }

    [KernelFunction, Description("Convert Dollar to Euro")]
    public double DollarToEuro(double amount)
    {
        return amount * DollarToEuroRate;
    }

}