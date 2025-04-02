namespace MftTask;

public class Utils
{
    public static void Log(string log)
    {
        var thread = Thread.CurrentThread.Name ?? Environment.CurrentManagedThreadId.ToString();
        Console.WriteLine($"{DateTime.UtcNow:O}: [{thread}] {log}");
        Console.ResetColor();
    }
}