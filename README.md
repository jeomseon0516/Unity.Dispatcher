# Jeomseon Unity Dispatcher — 폐기됨

Unity 6000.6부터 공식 `Awaitable.MainThreadAsync`가 Play Mode·Player뿐 아니라 Edit Mode와
batchmode에서도 백그라운드 스레드를 Unity Editor 메인 스레드로 전환합니다. 이 패키지의 자체
Dispatcher는 공식 기능과 중복되어 0.4.0에서 제거했습니다(설계 근거는 하네스 ADR-0005).

새 프로젝트에는 이 패키지를 설치하지 마세요. 기존 사용자는
[0.3.1 → 0.4.0 마이그레이션](Documentation~/Migration-0.3.1-to-0.4.0.md)을 확인하세요.
