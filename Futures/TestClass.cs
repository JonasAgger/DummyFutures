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
        Utils.Log($"DoStuff: {name} part 1");
        
        yield return DoOtherStuff();

        Utils.Log($"DoStuff: {name} part 2");

        yield return true;
    }


    public IEnumerable DoOtherStuff()
    {
        Utils.Log($"DoOtherStuff: {name} part 1");

        yield return Future.Delay(TimeSpan.FromMilliseconds(500));

        Utils.Log($"DoOtherStuff: {name} part 2");

        yield return true;
    }
    
    public IEnumerable SayHello() {
        yield return Future.Delay(TimeSpan.FromSeconds(1));
        Utils.Log("Hello from async task");
        yield return Future.Delay(TimeSpan.FromSeconds(1));
        Utils.Log("async task is done");
    }
}