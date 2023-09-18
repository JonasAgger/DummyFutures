using System.Collections;

namespace Futures;

public class TaskRunner
{
    private Queue<Future> tasks = new();
    private Queue<Future> runningTasks = new();

    // private Dictionary<>

    public void Add(IEnumerable task) => tasks.Enqueue(new Future(task.GetEnumerator()));

    public void Execute()
    {
        while (TryGetNext(out var next))
        {
            Console.WriteLine("Running step!");
            var moved = next.MoveNext();
            if (moved)
            {
                Thread.Sleep(1);
                runningTasks.Enqueue(next);
            }
        }
    }

    private bool TryGetNext(out Future? next)
    {
        if (tasks.TryDequeue(out var task))
        {
            next = task;
            return true;
        }

        return runningTasks.TryDequeue(out next);
    }
}