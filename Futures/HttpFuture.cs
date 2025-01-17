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
        Utils.Log("MAKING CALL");
        var future = client.GetStringAsync("https://httpbin.org/delay/1").ToFuture();

        yield return future;
        
        Utils.Log("RESPONSE: --------");
        Utils.Log(future.Value);

        yield return true;
    }
}