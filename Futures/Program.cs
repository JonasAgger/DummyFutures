// See https://aka.ms/new-console-template for more information

using Futures;

var tc1 = new TestClass("tc1", ConsoleColor.Blue);
var tc2 = new TestClass("tc2", ConsoleColor.Red);

var testHttp = new HttpFuture();

var runner = new TaskRunner();

// runner.Add(tc1.DoStuff());
// runner.Add(tc2.DoStuff());

runner.Add(testHttp.MakeHttpCallAndWriteResults());

runner.Execute();