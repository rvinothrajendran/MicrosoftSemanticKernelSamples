using KernelFunctionFactoryDemo.Plugins.VehiclePlugin;
using Microsoft.SemanticKernel;


namespace KernelFunctionFactoryDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - Dynamic-Plugin");

            Console.ForegroundColor = ConsoleColor.Green;

            
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var kernelBuilder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey)
                .Build();

            KernelFunction? bikeFunction = null;

            //Load the plugin
            VehiclePlugin vehiclePlugin = new();
            bikeFunction = KernelFunctionFactory.CreateFromMethod((string input) => vehiclePlugin.BikeInformation(input),
                nameof(VehiclePlugin));
            
            kernelBuilder.Plugins.AddFromFunctions("FunctionPlugin","find bike model information",new[] { bikeFunction });


            //KernelArguments 
            var kernelArguments = new KernelArguments
            {
                { "input", "bike 123" }
            };

            //Invoke the kernel function
            var result = await kernelBuilder.InvokeAsync(bikeFunction, kernelArguments);

            //Print the result
            Console.WriteLine(result.GetValue<string>());
            Console.Read();
        }
        
    }
}
