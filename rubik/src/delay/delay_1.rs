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

        // Simply telling the runtime to poll us again, ASAP.
        cx.waker().wake_by_ref();
        std::task::Poll::Pending
    }
}
