using System.Collections;

namespace Futures;

public class HttpFuture
{
    private readonly HttpClient client = new();

    private IEnumerable MakeHttpCall()
    {
        yield return client.GetStringAsync("https://httpbin.org/delay/1");
    }

    public IEnumerable MakeHttpCallAndWriteResults()
    {
        var future = client.GetStringAsync("https://httpbin.org/delay/1").ToFuture();

        yield return future;
        
        Console.WriteLine("RESPONSE: --------");
        Console.WriteLine(future.Value);

        yield return true;
    }
}