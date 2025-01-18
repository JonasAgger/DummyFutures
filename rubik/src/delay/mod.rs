#[cfg(feature = "v1")]
mod delay_1;
#[cfg(feature = "v2")]
mod delay_2;

use std::time::{Duration, Instant};

#[cfg(feature = "v1")]
pub use delay_1::Delay;
#[cfg(feature = "v2")]
pub use delay_2::Delay;

pub fn delay(how_long: Duration) -> Delay {
    let timeout = Instant::now() + how_long;
    Delay { timeout }
}
