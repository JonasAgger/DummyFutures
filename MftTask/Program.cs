// See https://aka.ms/new-console-template for more information

using System.Collections;
using MftTask;

await Example1();

// await Example2();

await CallTheApi();

var x = YieldTheApi();


async Task CallTheApi()
{
    using var client = new HttpClient();
    
    var result = await client.GetStringAsync("https://httpbin.org/delay/1");

    Console.WriteLine(result);
}

IEnumerable YieldTheApi()
{
    int value = 1;
    
    yield return value;

    value += 5;
    
    yield return value;
}

async Task Example1()
{
    Utils.Log("Running example");

    await TaskDelay.Delay(1000);

    var s = "Hello async str";

    await s;

    Utils.Log("Done!");
}

async Task Example2()
{
    Utils.Log("Running example2");
    var scheduler = new SingleThreadTaskScheduler();
    var taskFactory = new TaskFactory(scheduler);
    
    // Start an async task on our TaskScheduler
    var t = await taskFactory.StartNew(Example1);

    await t;
    
    Utils.Log("Done running example2");
}