use std::{
    future::Future,
    pin::Pin,
    sync::{mpsc::Sender, Arc, Mutex},
};

use futures::task::ArcWake;

pub struct TaskFuture {
    pub inner: Pin<Box<dyn Future<Output = ()> + Send>>,
    pub poll_result: std::task::Poll<()>,

    // Runtime fields
    pub times_polled: usize,
    pub name: &'static str,
}

// There's a lot of wiring going on here, which is just to juggle when we're scheduled on the executor.
pub struct Task {
    // Mutex here just to impl Sync to trick Rust. Only 1 thread has access to a task at any gievn time.
    pub future: Mutex<TaskFuture>,
    pub executor_ref: Sender<Arc<Task>>,
}

impl Task {
    pub fn shedule(self: &Arc<Self>) {
        _ = self.executor_ref.send(self.clone());
    }
}

impl ArcWake for Task {
    fn wake_by_ref(arc_self: &Arc<Self>) {
        arc_self.shedule();
    }
}
