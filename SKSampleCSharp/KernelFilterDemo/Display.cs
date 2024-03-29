﻿namespace KernelFilterDemo;

public interface IScreenDisplay
{
    void WriteLine(params string[] displays);
}

public class ScreenDisplay : IScreenDisplay
{
    readonly ConsoleColor[] allowedColors = {
            ConsoleColor.Yellow,
            ConsoleColor.Magenta,
            ConsoleColor.Cyan,
            ConsoleColor.White,
            ConsoleColor.Gray
        };

    readonly Random Random = new();
    private readonly List<ConsoleColor> remainingColors = new();

    public void WriteLine(params string[] displays)
    {
        foreach (string display in displays)
        {
            ConsoleColor randomColor = GetRandomColor();
            Console.ForegroundColor = randomColor;

            Console.WriteLine($"{display}");

            Console.ResetColor();
        }
    }

    private ConsoleColor GetRandomColor()
    {
        lock (Random)
        {
            if (remainingColors.Count == 0)
            {
                remainingColors.AddRange(allowedColors);
            }

            int index = Random.Next(0, remainingColors.Count);
            ConsoleColor randomColor = remainingColors[index];
            remainingColors.RemoveAt(index);

            return randomColor;
        }
    }
}
