# Dispatcher 로드맵

우선순위: `P0` 결함·안전성 → `P1` 핵심 구조 → `P2` API·성능 → `P3` 장기 확장

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
2. **P0-02 (완료, 2026-08-17) — Basic Usage 샘플을 Import 즉시 사용 가능한 Scene으로 교체**
   - 기존 `DispatcherSampleWindow`(`EditorWindow`)는 `[MenuItem("Window/Jeomseon/Dispatcher
     Sample")]`을 썼는데, `AGENTS.md`가 명시한 `[MenuItem]` 루트 규칙(`Jeomseon/`으로 시작)을
     지키지 않고 있었습니다. 또한 샘플을 쓰려면 메뉴를 찾아 창을 여는 별도 단계가 필요해, 사용자가
     "Import하면 바로 쓸 수 있어야 한다"고 지적한 문제(다른 패키지의 Setup-메뉴 샘플과 동일한 종류의
     불편)이기도 했습니다.
   - `DispatcherSampleWindow`를 제거하고, 같은 동작(백그라운드 스레드 → `Enqueue` → Editor 메인
     스레드 결과 확인)을 `DispatcherSample`(`MonoBehaviour` + `[ContextMenu]`, `ShaderLookupSample`/
     `SafeAreaSample`과 같은 패턴)로 재구현했습니다. `DispatcherBasicUsageSample.unity`에 이미
     컴포넌트가 부착된 GameObject가 포함되어 있어, `[MenuItem]` 자체가 필요 없어졌습니다.
3. **P1-01 — 진단 API**
   - 대기 작업 수와 마지막 예외를 Editor 툴링에서 관찰할 수 있는 선택적 API를 검토합니다.
4. **P2-01 — Samples~/BasicUsage 확장**
   - 현재 `DispatcherSample`은 단일 콜백 예시입니다. 여러 백그라운드 작업이 겹칠 때의 큐 동작을
     보여주는 예시 추가를 검토합니다.
