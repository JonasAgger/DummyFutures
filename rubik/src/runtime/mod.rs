#[cfg(feature = "v1")]
mod runtime_1;
#[cfg(feature = "v2")]
mod runtime_2;

#[cfg(feature = "v1")]
pub use runtime_1::TaskRuntime;
#[cfg(feature = "v2")]
pub use runtime_2::TaskRuntime;

pub fn new_runtime() -> TaskRuntime {
    TaskRuntime::new()
}
