// See https://aka.ms/new-console-template for more information

using System.Collections;
using Futures;

var runner = new TaskRunner();

runner.Add(DoSomeStuffAndWait("NR 1"));
runner.Add(DoSomeStuffAndWait("NR 2"));

runner.Execute();

IEnumerable DoSomeStuffAndWait(string name)
{
    Utils.Log($"DoSomeStuffAndWait: {name} part 1");

    yield return false;

    Utils.Log($"DoSomeStuffAndWait: {name} part 2");

    yield return true;
}
