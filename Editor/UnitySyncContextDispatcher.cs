#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Threading;
using Jeomseon.Collections;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Dispatcher
{
    /// <summary>
    /// Edit Mode 백그라운드 스레드 작업 결과를 Editor 메인 스레드로 전달하는 디스패처입니다.
    /// Play Mode와 Player 런타임에서는 Unity Awaitable.MainThreadAsync/BackgroundThreadAsync를 사용합니다.
    /// </summary>
    public static class UnitySyncContextDispatcher
    {
        private static readonly Queue<Action> ExecutionQueue = new();
        private static SynchronizationContext _unityContext;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            // .. InitializeOnLoadMethod를 호출하는 Context는 유니티의 메인 스레드이므로 현재 선택된 스레드는 메인 스레드이다 현재 스레드를 캐쉬해둔다
            _unityContext = SynchronizationContext.Current;
            AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeAssemblyReload;
        }

        // .. Assembly Reload 직전에 다음 도메인으로 넘어가지 못할 작업과 구독을 정리한다
        private static void HandleBeforeAssemblyReload()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= HandleBeforeAssemblyReload;
            _unityContext = null;

            lock (ExecutionQueue)
            {
                ExecutionQueue.Clear();
            }
        }

        private static void ExecuteActions()
        {
#if DEBUG
            Debug.Log("Execute Actions");
#endif

            lock (ExecutionQueue)
            {
                foreach (Action action in ExecutionQueue.Drain())
                {
                    try // .. 예외 발생시 큐에 담긴 나머지 콜백들이 처리되도록 보장
                    {
                        action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Exception in UnitySyncContextDispatcher.ExecuteActions: {ex}");
                    }
                }
            }
        }

        public static void Enqueue(Action action)
        {
            if (_unityContext == null)
            {
                throw new InvalidOperationException("UnitySyncContextDispatcher 가 초기화 되어있지 않습니다");
            }

            lock (ExecutionQueue)
            {
                if (action is not null)
                {
                    ExecutionQueue.Enqueue(action);
                    _unityContext.Post(_ => ExecuteActions(), null); // .. 유니티 스레드의 메세지큐에 동기화 시켜서 호출할 메서드를 보낸다
                }
            }
        }
    }
}
#endif
