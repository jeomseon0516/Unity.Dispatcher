# Migration 0.3.1 to 0.4.0

`UnitySyncContextDispatcher` was removed because Unity 6000.6 supports Editor main-thread switching through
the official `Awaitable.MainThreadAsync` API.

Replace callback dispatch:

```csharp
// Before
UnitySyncContextDispatcher.Enqueue(() => UpdateEditorObject(result));

// After
_ = UpdateEditorObjectAsync(result);

static async Awaitable UpdateEditorObjectAsync(Result result)
{
    await Awaitable.MainThreadAsync();
    UpdateEditorObject(result);
}
```

Catch exceptions inside fire-and-forget entry points. If work must be abandoned for an assembly reload, keep a
caller-owned `CancellationTokenSource`, cancel it from `AssemblyReloadEvents.beforeAssemblyReload`, and pass its
token through the surrounding operation.

After migrating every call site, remove `com.jeomseon.unity.dispatcher` from `Packages/manifest.json`.
