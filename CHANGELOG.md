# 변경 기록

## [0.4.0] - 2026-09-02

- **(Breaking/Retired)** ADR-0005의 재검토 트리거("Unity가 Awaitable을 Edit Mode까지 확장하면
  패키지 존속 재검토")에 따라, Unity 6000.6이 `Awaitable.MainThreadAsync`/`BackgroundThreadAsync`를
  Edit Mode와 batchmode 백그라운드 스레드까지 확장한 것을 근거로 이 패키지를 폐기합니다.
  `UnitySyncContextDispatcher`의 queue·`SynchronizationContext.Post` 구현은 공식 기능과 중복되므로
  구현·전용 asmdef·테스트·Sample을 제거했습니다.
- 이 패키지는 마이그레이션 안내만 남긴 retired 호환 패키지입니다. 새 의존성을 추가하지 말고
  `Awaitable.MainThreadAsync`/`BackgroundThreadAsync`를 직접 사용하세요.
- **실측 확인 (2026-09-02, Unity 6000.6.0f1 batchmode)**: 백그라운드 `Task.Run` 이후
  `await Awaitable.MainThreadAsync()`가 Editor 메인 스레드로 재개되는 것을 EditMode Test
  Framework로 확인했습니다(스레드 ID 일치, 0.05초, 무한 대기 없음). 폐기 근거가 확정됐습니다.

## [0.3.1] - 2026-08-18

- **(버그 수정)** `Basic Usage` 샘플의 `DispatcherSampleWindow`(`EditorWindow`, 잘못된 메뉴 루트
  `Window/Jeomseon/...`)를 Scene 기반 `DispatcherSample`(`MonoBehaviour` + `[ContextMenu]`)로
  교체했다가(8/17), 이 패키지가 Editor 전용 어셈블리라 Scene에 부착된 컴포넌트의 스크립트를 Unity가
  로드하지 못하는 버그("The associated script can not be loaded")가 발견됐습니다. Scene 기반
  샘플을 폐기하고 `DispatcherSampleWindow`(`EditorWindow`)를 복원했습니다 — 메뉴 경로만
  `Jeomseon/Dispatcher/Basic Usage Sample`로 고쳐 원래 목적(`AGENTS.md` `[MenuItem]` 루트 규칙
  준수)을 유지합니다. 사용자가 Unity에서 정상 동작 확인.

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
