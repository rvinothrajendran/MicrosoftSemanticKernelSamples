using System.Diagnostics;

namespace KernelFilterDemo.Watcher;

public class TimingLogger : ITimingLogger
{
    private readonly Stopwatch stopwatch = new();
    private long startTime = 0;
    private string functionName = string.Empty;

    public void Start(string name)
    {
        functionName = name;
        stopwatch.Reset();
        startTime = GetCurrentTime();
        stopwatch.Start();
    }

    public void Stop()
    {
        stopwatch.Stop();
        long elapsedTime = stopwatch.ElapsedMilliseconds;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine($"Function: {functionName}, Start Time: {startTime} ms, Stop Time: {GetCurrentTime()} ms, Elapsed Time: {elapsedTime} ms\n");
    }

    private long GetCurrentTime()
    {
        return stopwatch.ElapsedMilliseconds;
    }
}