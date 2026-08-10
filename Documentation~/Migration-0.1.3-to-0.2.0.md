# Migration: 0.1.3 → 0.2.0

0.2.0부터 `UnitySyncContextDispatcher`는 **Edit Mode(비-Play) 전용**입니다. Play Mode 진입 시
초기화되던 Runtime 경로(`RuntimeInitializeOnLoadMethod`, `Application.quitting` 구독)를 제거했습니다.

## Play Mode·Player 런타임에서 `Enqueue`를 사용하던 경우

Unity 6의 `Awaitable.MainThreadAsync`/`Awaitable.BackgroundThreadAsync`로 교체하세요. 별도 큐나
정적 상태 없이 `async`/`await`만으로 동일한 결과를 얻으며, `CancellationToken` 기반 취소도
기본 제공됩니다.

```csharp
// 0.1.3
private async void Run()
{
    int result = await Task.Run(() => Compute());
    UnitySyncContextDispatcher.Enqueue(() => Debug.Log(result));
}

// 0.2.0 (Play Mode / Player)
private async void Run()
{
    await Awaitable.BackgroundThreadAsync();
    int result = Compute();
    await Awaitable.MainThreadAsync();
    Debug.Log(result);
}
```

`await`로 이어지는 코드는 Unity가 메인 스레드에 설치한 `SynchronizationContext` 덕분에
`Enqueue` 없이도 이미 메인 스레드에서 재개됩니다. 0.1.3의 `DispatcherSample`이 보여주던
`await Task.Run(...)` 뒤의 `Enqueue` 호출은 애초에 불필요한 방어 코드였습니다.

## Edit Mode(비-Play)에서 `Enqueue`를 사용하던 경우

변경 없이 계속 사용할 수 있습니다. `Enqueue`의 시그니처와 동작(대기 큐, 예외 격리)은 그대로이며,
초기화·정리 시점만 `[InitializeOnLoadMethod]`/`AssemblyReloadEvents.beforeAssemblyReload`
기반으로 바뀌었습니다.
