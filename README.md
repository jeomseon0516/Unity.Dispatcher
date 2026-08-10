# Jeomseon Unity Dispatcher

Edit Mode 백그라운드 스레드 작업 결과를 Unity Editor 메인 스레드로 전달하는 Editor 전용 디스패처입니다.

## 설치

OpenUPM 등록 전에는 Package Manager의 **Add package from git URL**에서 다음 주소를 사용합니다.

```text
https://github.com/jeomseon0516/Unity.Dispatcher.git#v0.2.0
```

## 범위: Editor 전용

`Awaitable.MainThreadAsync`/`Awaitable.BackgroundThreadAsync`는 Play Mode·Player 런타임에서만
동작하며 Edit Mode에서는 사용할 수 없습니다. 이 패키지는 그 빈틈, 즉 **Edit Mode(비-Play)에서
백그라운드 스레드 콜백을 Editor 메인 스레드로 되돌리는 용도**로 범위를 좁혔습니다.

Play Mode·Player 런타임에서 메인 스레드 동기화가 필요하면 Unity의 `Awaitable`을 직접 사용하세요.

```csharp
private async void OnBackgroundCallback()
{
    await Awaitable.BackgroundThreadAsync();
    int result = Compute();
    await Awaitable.MainThreadAsync();
    Debug.Log(result);
}
```

0.1.x의 `UnitySyncContextDispatcher`를 Runtime 코드에서 사용하던 경우
[Migration 0.1.3 to 0.2.0](Documentation~/Migration-0.1.3-to-0.2.0.md)을 확인하세요.

## 사용

```csharp
Task.Run(() =>
{
    int result = Compute();
    UnitySyncContextDispatcher.Enqueue(() => Debug.Log($"Editor 메인 스레드 결과: {result}"));
});
```

`await`로 이어지는 코드는 Unity Editor가 설치한 `SynchronizationContext` 덕분에 별도 처리 없이도
메인 스레드에서 재개됩니다. `Enqueue`는 `async`/`await` 문맥이 없는 순수 콜백(네이티브 플러그인,
서드파티 SDK 이벤트 등)에서 Editor 오브젝트에 안전하게 접근해야 할 때 사용하세요.

## 수명

- 초기화는 Editor 로드·스크립트 재컴파일마다 `[InitializeOnLoadMethod]`로 수행합니다.
- Assembly Reload 직전(`AssemblyReloadEvents.beforeAssemblyReload`)에 대기 중인 작업 큐와
  `SynchronizationContext`를 정리해, 다음 도메인으로 넘어가지 못할 콜백이 실행되지 않도록 합니다.
