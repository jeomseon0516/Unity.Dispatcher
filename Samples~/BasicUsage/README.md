# Dispatcher 기본 예제

`Jeomseon/Dispatcher/Basic Usage Sample` 메뉴로 `DispatcherSampleWindow`를 열고 "백그라운드
스레드에서 작업 실행" 버튼을 누릅니다. 백그라운드 스레드에서 계산한 결과가
`UnitySyncContextDispatcher.Enqueue`를 거쳐 Editor 메인 스레드에서 창에 표시되는지 확인합니다
(Play Mode 진입이 필요하지 않습니다).

이 패키지는 Edit Mode(비-Play) 전용 Editor 디스패처만 지원합니다(Unity 6 `Awaitable`이 Play
Mode·Player 런타임의 메인 스레드 동기화를 대체하므로 Runtime 경로는 의도적으로 제공하지 않습니다).
그래서 이 샘플도 Scene이 아니라 Editor 전용 도구(`EditorWindow`)로 제공합니다 — Editor 전용
어셈블리의 `MonoBehaviour`를 Scene에 붙이는 방식은 스크립트 로드 오류를 유발할 수 있어 사용하지
않습니다.
