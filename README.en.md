# Jeomseon Unity Dispatcher — Retired

From Unity 6000.6, the official `Awaitable.MainThreadAsync` switches background work to the Unity Editor
main thread in Edit Mode and batch mode as well as in Play Mode and Players. The custom dispatcher became
redundant and was removed in 0.4.0.

Do not install this package in new projects. Existing users should follow the
[0.3.1 to 0.4.0 migration guide](Documentation~/Migration-0.3.1-to-0.4.0.md).
