# Dispatcher 로드맵

우선순위: `P0` 결함·안전성 → `P1` 핵심 구조 → `P2` API·성능 → `P3` 장기 확장

## 테스트 경계 정리 (2026-08-18, Unity 실행 검증 대기)

- 테스트가 private 정적 Context·Queue·reload 메서드를 reflection으로 변경하던 방식을 제거했습니다.
- 초기화·reload 정리와 최소 진단 상태를 테스트 어셈블리에만 노출되는 internal 계약으로 검증합니다.
- 반복 초기화 시 assembly reload 구독이 중복되지 않도록 등록을 멱등화했습니다.

## 범위 결정 (완료, 2026-08-10)

Unity 6 `Awaitable.MainThreadAsync`/`BackgroundThreadAsync`가 Play Mode·Player 런타임의 메인 스레드
동기화를 대체하므로, 이 패키지는 **Edit Mode(비-Play) 전용** 백그라운드 스레드 → Editor 메인 스레드
디스패처로 범위를 좁혔습니다. Runtime 경로와 관련된 이전 `P0-01`(Play Mode 재진입 수명),
`P0-02`(다중 스레드 경쟁 조건), `P1-01`(실행 예산·취소 정책), `P2-01`(Awaitable 경로 병행 제공)은
Runtime 경로 자체가 제거되어 더 이상 적용되지 않습니다.

## 작업 순서

1. **P0-01 (완료) — Editor 초기화·정리 수명 보장**
   - `[InitializeOnLoadMethod]`와 `AssemblyReloadEvents.beforeAssemblyReload`로 재작성했습니다.
   - `InitializeOnLoadMethod`는 실제 도메인 로드당 1회만 호출되므로 이전의 "Play Mode 재진입 시
     구독 중복" 문제 자체가 구조적으로 발생하지 않습니다.
2. **P0-02 (완료, 2026-08-17, 2026-08-18 정정) — Basic Usage 샘플 메뉴 경로 수정**
   - 기존 `DispatcherSampleWindow`(`EditorWindow`)는 `[MenuItem("Window/Jeomseon/Dispatcher
     Sample")]`을 썼는데, `AGENTS.md`가 명시한 `[MenuItem]` 루트 규칙(`Jeomseon/`으로 시작)을
     지키지 않고 있었습니다.
   - **2026-08-17 시도 — 되돌림**: 이 문제를 고치면서 `DispatcherSampleWindow`를 완전히 없애고
     Scene에 부착된 `DispatcherSample`(`MonoBehaviour` + `[ContextMenu]`, `ShaderLookupSample`/
     `SafeAreaSample`과 같은 패턴)로 재구현했으나, **사용자가 Unity에서 열어보니 해당 컴포넌트가
     "The associated script can not be loaded: <unknown>"로 뜨는 버그가 있었습니다.** 원인은
     이 패키지 자체가 `범위 결정(2026-08-10)`에 따라 **Editor 전용 어셈블리**
     (`includePlatforms: ["Editor"]`)로 좁혀져 있는데, `MonoBehaviour`를 **Scene의 GameObject에
     직접 부착**하는 패턴은 Runtime(Player)에서 해당 타입이 아예 존재할 수 없는 조합이라 Unity가
     스크립트 로드를 안정적으로 보장하지 않는 것으로 판단했습니다(`dotnet build` 기준 컴파일 오류는
     없었음 — 컴파일이 아니라 Editor-only 어셈블리 스크립트를 Scene 컴포넌트로 쓰는 조합 자체의
     문제). 애초에 이 패키지가 Runtime 경로를 의도적으로 제거한 Editor 전용 도구이므로, 결과물도
     Scene Sample이 아니라 `EditorWindow` 같은 **Editor 전용 도구**로 제공하는 게 구조적으로
     맞습니다(`AGENTS.md`의 "Editor 전용 기능은 Scene Sample 대신 Editor 전용 디버깅 도구 제공"
     예외 조항, `Jeomseon.Unity.EditorToolkit`과 같은 패턴).
   - **최종 수정**: `DispatcherSample.cs`(MonoBehaviour)와 `DispatcherBasicUsageSample.unity`를
     제거하고, 원래의 `DispatcherSampleWindow`(`EditorWindow`)를 복원하되 메뉴 경로만
     `Jeomseon/Dispatcher/Basic Usage Sample`로 고쳤습니다. 동작은 동일(백그라운드 스레드 →
     `Enqueue` → Editor 메인 스레드 결과를 창에 표시).
3. **P1-01 — 진단 API**
   - 대기 작업 수와 마지막 예외를 Editor 툴링에서 관찰할 수 있는 선택적 API를 검토합니다.
4. **P2-01 — Samples~/BasicUsage 확장**
   - 현재 `DispatcherSampleWindow`는 단일 콜백 예시입니다. 여러 백그라운드 작업이 겹칠 때의 큐 동작을
     보여주는 예시 추가를 검토합니다.
