use std::{collections::VecDeque, future::Future, pin::Pin};

use tracing::debug;

struct Task {
    inner: Pin<Box<dyn Future<Output = ()> + Send>>,
    times_polled: usize,
    name: &'static str,
}

pub struct TaskRuntime {
    tasks: VecDeque<Task>,
}

impl TaskRuntime {
    pub(super) fn new() -> Self {
        TaskRuntime {
            tasks: VecDeque::new(),
        }
    }

    pub fn spawn<F>(&mut self, name: &'static str, future: F)
    where
        F: Future<Output = ()> + Send + 'static,
    {
        let task = Task {
            inner: Box::pin(future),
            times_polled: 0,
            name,
        };

        self.tasks.push_back(task);
    }

    pub fn run(&mut self) {
        // Rust futures requires a "Context"
        let waker = futures::task::noop_waker();
        let mut context = std::task::Context::from_waker(&waker);

        // While there's still tasks in queue.
        while let Some(mut task) = self.tasks.pop_front() {
            task.times_polled += 1;
            let poll_result = task.inner.as_mut().poll(&mut context);

            match poll_result {
                std::task::Poll::Ready(_) => {
                    // Task is done, we just forget it here
                    debug!(
                        "Task: {}, was polled {} times",
                        task.name, task.times_polled
                    )
                }
                std::task::Poll::Pending => {
                    // Task is not done, poll it later
                    self.tasks.push_back(task);
                }
            }
        }
    }
}
