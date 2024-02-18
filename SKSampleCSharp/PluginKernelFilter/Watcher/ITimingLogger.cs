namespace InterruptPlanner.Watcher;

public interface ITimingLogger
{
    void Start(string name);
    void Stop();
}