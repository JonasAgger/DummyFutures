using System.Collections;

namespace Futures;

public class Future : IEnumerator
{
    public static int CURRENT_ID;
    private static int COUNTER;
    private int id;
    private IEnumerator current;
    private IEnumerator? inner;

    public Future(IEnumerable current)
    {
        this.id = Interlocked.Increment(ref COUNTER);
        this.current = current.GetEnumerator();
    }
    
    public Future(TaskFuture current)
    {
        this.id = Interlocked.Increment(ref COUNTER);
        this.current = current;
    }

    public bool MoveNext()
    {
        CURRENT_ID = id;
        if (inner != null && inner.MoveNext())
        {
            return true;
        }

        inner = null;
        if (current.MoveNext())
        {
            if (current.Current is IEnumerable next)
            {
                inner = new Future(next);
            }
            else if (current.Current is Task task)
            {
                inner = new TaskFuture(task);
            }
            else if (current.Current is TaskFuture future)
            {
                inner = future;
            }
            
            return true;
        }
        
        return false;
    }


    public void Reset() { }
    object IEnumerator.Current => current.Current!;
    public void Dispose() {}
}

public class TaskFuture : IEnumerator
{
    protected readonly Task inner;

    public TaskFuture(Task inner)
    {
        this.inner = inner;
    }

    public bool MoveNext()
    {
        if (inner.IsCompleted)
        {
            Current = true;
            return false;
        }

        return true;
    }
    
    public bool Current { get; private set; } = false;
    
    
    public void Reset() { }
    object IEnumerator.Current => Current;
    public void Dispose() {}
}

public class TypedTaskFuture<TOut> : TaskFuture
{
    private readonly Task<TOut> typedInner;

    public TypedTaskFuture(Task<TOut> inner) : base(inner)
    {
        this.typedInner = inner;
    }

    public TOut Value => typedInner.Result;

    public static implicit operator TypedTaskFuture<TOut>(Task<TOut> inner) => new(inner);
}

public static class FutureExtensions
{
    public static TypedTaskFuture<TOut> ToFuture<TOut>(this Task<TOut> inner) => inner;
    public static TaskFuture ToFuture(this Task inner) => new TaskFuture(inner);
}