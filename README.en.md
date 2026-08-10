# Jeomseon Unity Dispatcher

An Editor-only dispatcher that forwards Edit Mode background-thread work back to the Unity Editor
main thread.

## Scope: Editor only

`Awaitable.MainThreadAsync`/`Awaitable.BackgroundThreadAsync` only work in Play Mode and Player
runtimes; they are unavailable in Edit Mode. This package narrows its scope to fill that gap:
**forwarding background-thread callbacks to the Editor main thread outside Play Mode.**

For main-thread synchronization in Play Mode or Player runtime, use Unity's `Awaitable` directly.

```csharp
private async void OnBackgroundCallback()
{
    await Awaitable.BackgroundThreadAsync();
    int result = Compute();
    await Awaitable.MainThreadAsync();
    Debug.Log(result);
}
```

If you used `UnitySyncContextDispatcher` from Runtime code in 0.1.x, see
[Migration 0.1.3 to 0.2.0](Documentation~/Migration-0.1.3-to-0.2.0.md).

## Usage

```csharp
Task.Run(() =>
{
    int result = Compute();
    UnitySyncContextDispatcher.Enqueue(() => Debug.Log($"Editor main thread result: {result}"));
});
```

Code that continues after an `await` already resumes on the main thread, since Unity Editor installs
a `SynchronizationContext`. Use `Enqueue` for plain callbacks without an `async`/`await` context
(native plugins, third-party SDK events) that need to touch Editor objects safely.

## Lifecycle

- Initialization runs on every Editor load and script recompile via `[InitializeOnLoadMethod]`.
- Pending work and the captured `SynchronizationContext` are cleared right before an assembly reload
  (`AssemblyReloadEvents.beforeAssemblyReload`), so no callback survives into the next domain.
