using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MftTask;

public class TaskDelay
{
    // Manually written version that mimics the compiler-generated state machine
    public static Task Delay(uint delayMiliseconds)
    {
        // 1) Create the state machine instance
        // 2) Initialize its fields
        // 3) Start the state machine
        // 4) Return the Task from the builder
        var stateMachine = new SimpleDelayAsyncStateMachine
        {
            Builder = AsyncTaskMethodBuilder.Create(),
            State = -1,
            Time = delayMiliseconds,
        };

        // The builder calls back into this struct's MoveNext
        stateMachine.Builder.Start(ref stateMachine);

        // Return the Task that represents this async method
        return stateMachine.Builder.Task;
    }
    
    // The struct implementing IAsyncStateMachine for the "SimpleDelayAsync" method
    private struct SimpleDelayAsyncStateMachine : IAsyncStateMachine
    {
        public int State;
        public uint Time;
        public AsyncTaskMethodBuilder Builder;

        // We'll store our custom awaiter here while we yield
        private DelayAwaiter awaiter;

        void IAsyncStateMachine.MoveNext()
        {
            int state = State;
            try
            {
                switch (state)
                {
                    case 0:
                        // We’re resuming after the await
                        awaiter.GetResult();
                        // Reset the state to -1 ("running") before continuing
                        State = -1;
                        break;

                    default:
                        // Initial entry: print a message and start the delay
                        awaiter = new DelayAwaiter(Time);

                        if (!awaiter.IsCompleted)
                        {
                            State = 0;  // Next time MoveNext is called, jump to case 0
                            // Tell the builder we need to await
                            Builder.AwaitOnCompleted(ref awaiter, ref this);
                            return;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                // If an exception was thrown, let the builder know
                State = -2; // -2 = "completed with error"
                Builder.SetException(ex);
                return;
            }

            // If we got here without errors, we're done
            State = -2; // -2 = "completed successfully"
            Builder.SetResult();
        }

        // Part of IAsyncStateMachine. The builder uses this to set a new state machine, if needed.
        [DebuggerHidden]
        void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
        {
            Builder.SetStateMachine(stateMachine);
        }
    }
    
    // ---------------------------------------------
    // 4. The custom awaiter implementing INotifyCompletion
    // ---------------------------------------------
    private struct DelayAwaiter(uint milliseconds) : INotifyCompletion
    {
        private readonly int milliseconds = (int)milliseconds;
        private Action? continuation = null;

        // For this demo, we always say “not completed yet” so that
        // OnCompleted is used to schedule a continuation.
        public bool IsCompleted => false;

        // This is called once the awaiter is ready to schedule a continuation.
        // We'll spin up a new thread to sleep and then invoke the continuation.
        public void OnCompleted(Action? continuation)
        {
            this.continuation = continuation;
            Thread t = new Thread(Run);
            t.Start();
        }

        // This is where we actually do our “delay”
        private void Run()
        {
            Thread.Sleep(milliseconds);
            // Call the continuation to resume the async method
            continuation?.Invoke();
        }

        // Called when the await is done (no result, so it's empty)
        public void GetResult()
        {
            // For a “delay,” there's nothing to return. We just do nothing.
        }
    }
}