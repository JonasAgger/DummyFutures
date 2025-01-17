// See https://aka.ms/new-console-template for more information

using Futures;

var tc1 = new TestClass("tc1");
var tc2 = new TestClass("tc2");

var testHttp = new HttpFuture();

var runner = new TaskRunner();

// runner.Add(tc1.DoStuff());

runner.Add(tc2.DoStuff());
runner.Add(testHttp.MakeHttpCallAndWriteResults());
runner.Add(SayHello().ToFuture());

runner.Execute();


async Task SayHello() {
    await Task.Delay(1000);
    Utils.Log("Hello from async task");
    await Task.Delay(1000);
    Utils.Log("async task is done");
}