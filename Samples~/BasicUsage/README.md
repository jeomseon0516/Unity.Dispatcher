# Dispatcher 기본 예제

`DispatcherBasicUsageSample.unity`를 열어 `Dispatcher Sample` GameObject를 선택하고, `Dispatcher Sample`
컴포넌트의 컨텍스트 메뉴("백그라운드 스레드에서 작업 실행")를 실행합니다. 백그라운드 스레드에서 계산한
결과가 `UnitySyncContextDispatcher.Enqueue`를 거쳐 Editor 메인 스레드에서 Console에 출력되는지
확인합니다(Play Mode 진입이 필요하지 않습니다).
