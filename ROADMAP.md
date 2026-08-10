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
2. **P1-01 — 진단 API**
   - 대기 작업 수와 마지막 예외를 Editor 툴링에서 관찰할 수 있는 선택적 API를 검토합니다.
3. **P2-01 — Samples~/BasicUsage 확장**
   - 현재 `DispatcherSampleWindow`는 단일 콜백 예시입니다. 여러 백그라운드 작업이 겹칠 때의 큐
     동작을 보여주는 예시 추가를 검토합니다.
