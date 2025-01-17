using System.Collections;

namespace Futures;

public class TestClass
{
    private readonly string name;

    public TestClass(string name)
    {
        this.name = name;
    }
    
    public IEnumerable DoStuff()
    {
        Utils.Log($"Hello world from {name}");
        
        yield return DoOtherStuff();

        Utils.Log($"Hello world from {name} part 2");

        yield return true;
    }


    public IEnumerable DoOtherStuff()
    {
        Utils.Log($"DoOtherStuff {name}");
        
        yield return Task.Delay(2000).ToFuture();

        Utils.Log($"DoOtherStuff {name} part 2");

        yield return true;
    }
}