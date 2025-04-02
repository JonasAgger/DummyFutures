// See https://aka.ms/new-console-template for more information

using System.Collections;
using Futures;

var runner = new TaskRunner();

runner.Add(HttpCall("Dummy"));

runner.Execute();

IEnumerable HttpCall(string name)
{
    Utils.Log($"HttpCall: Before Result {name}");
    
    // Calling a Http Client!
    yield return Call();

    Utils.Log($"HttpCall: After Result {name}");

    yield return true;
}

IEnumerable Call()
{
    using var client = new HttpClient();
    
    // Wrapping the task into an IEnumerable
    var futureEnumerable = client.GetStringAsync("https://httpbin.org/delay/1").ToFuture();
    // Yielding the IEnumerable
    yield return futureEnumerable;
    
    // When we return here, the HttpClient got a response
    Utils.Log($"Call: Http Call Returned: {futureEnumerable.Value}");
}