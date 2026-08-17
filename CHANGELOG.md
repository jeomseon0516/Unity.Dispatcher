# 변경 기록

## [Unreleased]

- **(P0-02, Unity 검증 대기)** `Basic Usage` 샘플의 `DispatcherSampleWindow`(`EditorWindow`, 잘못된
  메뉴 루트 `Window/Jeomseon/...`)를 제거하고, `DispatcherSample`(`MonoBehaviour` +
  `[ContextMenu]`)과 `DispatcherBasicUsageSample.unity`로 교체했습니다. 별도 메뉴를 찾아 창을 열
  필요 없이 샘플 Import 직후 Scene을 열어 바로 컨텍스트 메뉴로 확인할 수 있습니다. 아직 Unity에서
  실제로 열어 확인하지 못했습니다.

## [0.3.0] - 2026-08-13

- **(Breaking)** 네임스페이스를 `Jeomseon.Dispatcher` → `Jeomseon.Unity.Dispatcher`로 변경했습니다.
  워크스페이스 전체 네임스페이스 규칙(`AGENTS.md` 참고)을 적용한 것으로, 폴더 구조 변경은 없습니다.

## [0.2.1] - 2026-08-11

- 워크스페이스 명명 규칙에 맞춰 `UnitySyncContextDispatcher`의 `private static readonly` 필드
  (`ExecutionQueue` → `_executionQueue`)와 테스트의 reflection 필드 이름을 정리했습니다. 공개
  API 변경은 없습니다.

## [0.2.0] - 2026-08-10

- **Breaking**: Play Mode·Runtime 디스패치 경로(`RuntimeInitializeOnLoadMethod`, `Application.quitting`
  구독)를 제거하고 Editor 전용 패키지로 범위를 좁혔습니다. Play Mode·Player 런타임의 메인 스레드
  동기화는 Unity `Awaitable.MainThreadAsync`/`BackgroundThreadAsync`로 대체됩니다.
  [Migration 0.1.3 to 0.2.0](Documentation~/Migration-0.1.3-to-0.2.0.md)을 확인하세요.
- 초기화·정리 수명을 `[InitializeOnLoadMethod]`와 `AssemblyReloadEvents.beforeAssemblyReload`
  기반으로 재작성했습니다. `Application.quitting`은 Editor에서 사실상 발동하지 않아 대기 큐가
  Assembly Reload 전까지 정리되지 않던 결함을 함께 수정했습니다.
- `Runtime/` asmdef를 제거하고 `Editor/` 전용 asmdef로 이전했습니다.
- Play Mode `MonoBehaviour` 샘플을 Editor 전용 `DispatcherSampleWindow`로 교체했습니다.
- `Enqueue` 초기화 예외, Assembly Reload 정리, 큐 전달을 검증하는 EditMode 테스트를 추가했습니다.

## [0.1.2] - 2026-07-29

- asmdef의 `rootNamespace`와 소스 파일 위치를 namespace에 맞게 정리했습니다.

## [0.1.1] - 2026-07-29

- 백그라운드 결과의 메인 스레드 전달을 확인하는 `Basic Usage` 샘플을 추가했습니다.

## [0.1.0] - 2026-07-29

- JeomseonScriptPack의 관련 모듈을 독립 UPM 패키지로 분리했습니다.


## [0.1.3] - 2026-08-05

- Unity 6000.5.7f1을 최소 지원 버전으로 상향했습니다.
