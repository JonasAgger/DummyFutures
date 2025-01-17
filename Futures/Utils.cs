namespace Futures;

public class Utils
{
    
    
    public static void Log(string log)
    {
        var color = (ConsoleColor)((Future.CURRENT_ID + 1) % 15);
        Console.ForegroundColor = color;
        Console.WriteLine($"{DateTime.UtcNow:O} [{Future.CURRENT_ID}]: {log}");
        Console.ResetColor();
    }
}