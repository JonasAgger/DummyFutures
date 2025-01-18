use std::time::Duration;

use runtime::new_runtime;
use tracing::info;

mod delay;
mod runtime;

fn main() {
    tracing_subscriber::fmt()
        .with_max_level(tracing::Level::DEBUG)
        .init();
    info!("Starting!");

    // let tokio_runtime = tokio::runtime::Builder::new_current_thread()
    //     .build()
    //     .unwrap();

    // tokio_runtime.block_on(async {
    //     info!("before");
    //     delay(Duration::from_secs(1)).await;
    //     info!("after");
    // })

    let mut task_runtime = new_runtime();

    task_runtime.spawn("delay", async {
        info!("before");
        delay::delay(Duration::from_secs(1)).await;
        info!("after");
    });

    task_runtime.run();
}
