using Microsoft.SemanticKernel;

namespace ProcessLibSample.Step1;

public class IntroStep : KernelProcessStep
{
    [KernelFunction]
    public void WelcomeMessage()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Welcome to the ProcessLibBot!");
    }
}