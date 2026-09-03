# Migration 0.3.1 to 0.4.0

`UnitySyncContextDispatcher` was removed because Unity 6000.6 resumes background-thread work on the
Editor main thread through the official `Awaitable.MainThreadAsync` / `Awaitable.BackgroundThreadAsync`
API, in Edit Mode and batchmode as well as Play Mode and the Player. The dispatcher only ever existed
to fill the Edit Mode gap, so it is now redundant.

## Replace the dispatch pattern

The old pattern was: run work on a background thread with `Task.Run`, then hop back to the Editor main
thread to touch Unity objects. Keep the `Task.Run` — only the hop back changes.

```csharp
// Before (0.3.1)
Task.Run(() =>
{
    int result = Compute();
    UnitySyncContextDispatcher.Enqueue(() =>
    {
        UpdateEditorObject(result);
        Repaint();
    });
});

// After (0.4.0) — from any async context (an async EditorWindow handler, [MenuItem], etc.)
int result = await Task.Run(Compute);
await Awaitable.MainThreadAsync();
UpdateEditorObject(result);
Repaint();
```

If the caller is not already `async` (a pure native/SDK callback), wrap it in a fire-and-forget entry
point and catch exceptions inside it:

```csharp
void OnNativeCallback() => _ = HandleAsync();

async Awaitable HandleAsync()
{
    try
    {
        int result = await Task.Run(Compute);
        await Awaitable.MainThreadAsync();
        UpdateEditorObject(result);
    }
    catch (Exception exception)
    {
        Debug.LogException(exception);
    }
}
```

## Assembly reload

If work must be abandoned for an assembly reload, keep a caller-owned `CancellationTokenSource`, cancel
it from `AssemblyReloadEvents.beforeAssemblyReload`, and pass its token through the surrounding
operation. (The removed dispatcher cleared its own queue on `beforeAssemblyReload`; with `Awaitable`
you own that cancellation.)

## Verify on your Unity version

Run this once (an EditMode test, or a `[MenuItem]`) to confirm `Awaitable.MainThreadAsync` resumes on
the Editor main thread on your install. It passed on Unity 6000.6.0f1 batchmode on 2026-09-02.

```csharp
[Test]
public async Task Awaitable_ResumesOnEditorMainThread()
{
    int mainThreadId = Thread.CurrentThread.ManagedThreadId;

    await Task.Run(() => Thread.Sleep(10));      // now on a background thread
    await Awaitable.MainThreadAsync();           // hop back

    Assert.AreEqual(mainThreadId, Thread.CurrentThread.ManagedThreadId);
}
```

If this fails on your Unity version, stay on 0.3.1 and do not upgrade to 0.4.0.

## Remove the dependency

After migrating every call site, remove `com.jeomseon.unity.dispatcher` from `Packages/manifest.json`.
