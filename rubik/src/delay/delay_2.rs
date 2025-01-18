use std::{future::Future, time::Instant};

use tracing::trace;

pub struct Delay {
    pub(super) timeout: Instant,
}

impl Future for Delay {
    type Output = ();

    fn poll(
        self: std::pin::Pin<&mut Self>,
        cx: &mut std::task::Context<'_>,
    ) -> std::task::Poll<Self::Output> {
        trace!("polling");

        // If we delayed enough time, then our future is "ready" or done
        if Instant::now() >= self.timeout {
            return std::task::Poll::Ready(());
        }

        // Lets take a reference to our waker
        let waker = cx.waker().clone();
        let timeout = self.timeout;

        // spawn a thread to sleep the provided duration, such that we can get woken up after it's done.
        std::thread::spawn(move || {
            // See if we still need to sleep, after paying the thread spawn overhead.
            if let Some(sleep_duration) = timeout.checked_duration_since(Instant::now()) {
                std::thread::sleep(sleep_duration);
            }

            waker.wake();
        });

        // When we return Pending here, we MUST ensure to call waker.wake() at some point, otherwise we're just stuck in limbo, and would never be "completed" from an executor PoV.
        std::task::Poll::Pending
    }
}
