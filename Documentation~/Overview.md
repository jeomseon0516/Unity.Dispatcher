# Jeomseon Unity Dispatcher (Deprecated)

[English](Overview.en.md) · [설치](../README.md)

## 목적

Unity 6000.6 공식 Awaitable로 대체된 폐기 패키지입니다. 신규 설치 대상이 아닙니다.

## 사용 흐름

기존 Dispatcher 호출을 Awaitable.MainThreadAsync/BackgroundThreadAsync로 이전하고 폐기된 패키지 의존성을 제거합니다. 상세 교체 예는 Migration 문서를 따릅니다.

## 계약과 제한

현재 버전에는 이전 Dispatcher runtime 구현이 없습니다. 과거 README나 예제의 클래스를 새 프로젝트에서 사용할 수 있다고 가정하지 마세요.

## 패키지 요구 사항

- Package ID: `com.jeomseon.unity.dispatcher`
- Version: `0.4.0`
- Unity: `6000.6` / `0f1`

## 샘플과 검증

package.json에 Package Manager 샘플이 등록되어 있지 않습니다.

## 코드 지도

README의 Shader/자산 구성 또는 Migration 절을 참고하세요.
