mod task;

use std::{
    future::Future,
    sync::{
        mpsc::{Receiver, Sender},
        Arc, Mutex,
    },
    task::{Context, Poll},
};

use task::{Task, TaskFuture};
use tracing::debug;

pub struct TaskRuntime {
    task_queue: Receiver<Arc<Task>>,
    scheduler: Sender<Arc<Task>>,
}

impl TaskRuntime {
    pub(super) fn new() -> Self {
        let (tx, rx) = std::sync::mpsc::channel();
        TaskRuntime {
            task_queue: rx,
            scheduler: tx,
        }
    }

    // Now spawn does not have to be mut.
    pub fn spawn<F>(&self, name: &'static str, future: F)
    where
        F: Future<Output = ()> + Send + 'static,
    {
        // Make the complete task
        let task = Arc::new(Task {
            future: Mutex::new(TaskFuture {
                inner: Box::pin(future),
                poll_result: Poll::Pending,
                times_polled: 0,
                name,
            }),
            executor_ref: self.scheduler.clone(),
        });

        // Schedule the task to be ran once at least.
        _ = self.scheduler.send(task);
    }

    // Run does also not have to be mut here.
    pub fn run(&self) {
        // Now we run through our queue, and wait for tasks if none are currently in there.
        while let Ok(task) = self.task_queue.recv() {
            // we now have to hold a waker and context for every task, but the task themselves hold the actual ref.

            let waker = futures::task::waker(task.clone());
            let mut context = Context::from_waker(&waker);

            let mut task_future = task.future.lock().unwrap();

            // Polling a task which has already returned a result is not allowed, so check for pending state.
            if task_future.poll_result.is_pending() {
                // Update poll result.
                task_future.times_polled += 1;
                task_future.poll_result = task_future.inner.as_mut().poll(&mut context);

                // If it's done, write status
                if task_future.poll_result.is_ready() {
                    debug!(
                        "Task: {}, was polled {} times",
                        task_future.name, task_future.times_polled
                    )
                }
            } else {
                debug!(
                    "Task: {}, was polled {} times",
                    task_future.name, task_future.times_polled
                )
            }
        }
    }
}
