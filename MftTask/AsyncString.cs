using System.Runtime.CompilerServices;

namespace MftTask;

public static class AsyncStringExt
{
    public static StringAwaiter GetAwaiter(this string str)
    {
        return new StringAwaiter(str);
    }
    
    public struct StringAwaiter(string str) : INotifyCompletion
    {
        // For this demo, we always say “not completed yet” so that
        // OnCompleted is used to schedule a continuation.
        public bool IsCompleted => true;

        // This is called once the awaiter is ready to schedule a continuation.
        // We'll spin up a new thread to sleep and then invoke the continuation.
        public void OnCompleted(Action? continuation)
        {   
            continuation?.Invoke();
        }

        // Called when the await is done (no result, so it's empty)
        public string GetResult()
        {
            Utils.Log($"awaiting str done: {str}");
            return str;
        }
    }
}

