# Dispatcher 로드맵

우선순위: `P0` 결함·안전성 → `P1` 핵심 구조 → `P2` API·성능 → `P3` 장기 확장

## 작업 순서

1. **P0-01 — 정적 큐와 SynchronizationContext 수명 보장**
   - Domain Reload 비활성화, Play Mode 재진입 및 종료 구독 중복을 테스트합니다.
2. **P0-02 — 큐 스케줄링 경쟁 조건 검증**
   - 다중 스레드 Enqueue, 중첩 Enqueue, 예외 이후 나머지 작업 실행을 검증합니다.
3. **P1-01 — 실행 예산과 취소 정책**
   - 프레임당 처리량 제한, 취소, 종료 시 대기 작업 처리 정책을 정의합니다.
4. **P2-01 — Unity 6 Awaitable 경로 제공**
   - `MainThreadAsync`와 `BackgroundThreadAsync`로 대체 가능한 API를 버전별로 분리합니다.
5. **P2-02 — 진단 API**
   - 대기 작업 수와 마지막 예외를 선택적으로 관찰할 수 있도록 검토합니다.
