# Dispatcher 기본 예제

`Window > Jeomseon > Dispatcher Sample`로 `DispatcherSampleWindow`를 열고 "백그라운드 스레드에서 작업 실행" 버튼을 누릅니다.
백그라운드 스레드에서 계산한 결과가 `UnitySyncContextDispatcher.Enqueue`를 거쳐 Editor 메인 스레드에서 표시되는지 확인합니다(Play Mode 진입이 필요하지 않습니다).
