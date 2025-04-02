using System.Collections;
using System.Diagnostics;

namespace Futures;

public partial class Future
{
    public static DelayFuture Delay(TimeSpan duration) => new(duration);
}

public class DelayFuture(TimeSpan delay) : IEnumerable
{
    private readonly DelayFutureImpl inner = new(delay);

    public IEnumerator GetEnumerator()
    {
        return inner;
    }
    
    private class DelayFutureImpl(TimeSpan delay) : IEnumerator
    {
    
        private readonly long targetTime = Stopwatch.GetTimestamp();

        bool IEnumerator.MoveNext()
        {
            var elapsed = Stopwatch.GetElapsedTime(targetTime);
            if (elapsed > delay)
            {
                Current = true;
                return false;
            }

            return true;
        }

        public bool Current { get; private set; } = false;
        public void Reset() { }
        object IEnumerator.Current => Current;
    }
}

