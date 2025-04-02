using System.Collections.Concurrent;

namespace MftTask;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public sealed class SingleThreadTaskScheduler : TaskScheduler, IDisposable
{
    private readonly BlockingCollection<Task> tasks = new BlockingCollection<Task>();
    private readonly Thread thread;

    public SingleThreadTaskScheduler()
    {
        thread = new Thread(() =>
        {
            foreach (var task in tasks.GetConsumingEnumerable())
            {
                TryExecuteTask(task);
            }
        })
        {
            // It’s often helpful to give the thread a name for debugging.
            Name = "SingleThreadTaskSchedulerThread"
        };

        thread.Start();
    }

    protected override void QueueTask(Task task)
    {
        tasks.Add(task);
    }

    /// <summary>
    /// We never inline tasks (returning false means the task must always be queued).
    /// </summary>
    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        return false;
    }

    protected override IEnumerable<Task> GetScheduledTasks()
    {
        return tasks.ToArray();
    }

    public override int MaximumConcurrencyLevel => 1;

    public void Dispose()
    {
        // Signal that no more tasks will be queued
        tasks.CompleteAdding();

        // Wait for the thread to finish processing tasks
        if (thread.IsAlive)
        {
            thread.Join();
        }

        // Clean up
        tasks.Dispose();
    }
}
