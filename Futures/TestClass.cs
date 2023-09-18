using System.Collections;

namespace Futures;

public class TestClass
{
    private readonly string name;
    private readonly ConsoleColor color;

    public TestClass(string name, ConsoleColor color)
    {
        this.name = name;
        this.color = color;
    }
    
    public IEnumerable DoStuff()
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"Hello world from {name}");
        Console.ResetColor();
        
        yield return DoOtherStuff();

        Console.ForegroundColor = color;
        Console.WriteLine($"Hello world from {name} part 2");
        Console.ResetColor();

        yield return true;
    }


    public IEnumerable DoOtherStuff()
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"DoOtherStuff {name}");
        Console.ResetColor();
        
        yield return false;

        Console.ForegroundColor = color;
        Console.WriteLine($"DoOtherStuff {name} part 2");
        Console.ResetColor();

        yield return true;
    }
}