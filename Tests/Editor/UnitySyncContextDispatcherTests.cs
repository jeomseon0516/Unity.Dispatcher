using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Jeomseon.Unity.Dispatcher;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace Jeomseon.Tests
{
    public sealed class UnitySyncContextDispatcherTests
    {
        private static readonly Type _dispatcherType = typeof(UnitySyncContextDispatcher);

        private static readonly FieldInfo _contextField = _dispatcherType.GetField(
            "_unityContext", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo _queueField = _dispatcherType.GetField(
            "_executionQueue", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly MethodInfo _handleBeforeAssemblyReloadMethod = _dispatcherType.GetMethod(
            "HandleBeforeAssemblyReload", BindingFlags.Static | BindingFlags.NonPublic);

        private static Queue<Action> Queue => (Queue<Action>)_queueField.GetValue(null);

        [Test]
        public void Enqueue_Throws_WhenContextNotInitialized()
        {
            object previousContext = _contextField.GetValue(null);
            _contextField.SetValue(null, null);

            try
            {
                Assert.Throws<InvalidOperationException>(() => UnitySyncContextDispatcher.Enqueue(() => { }));
            }
            finally
            {
                _contextField.SetValue(null, previousContext);
            }
        }

        [UnityTest]
        public IEnumerator Enqueue_ExecutesQueuedAction_OnCapturedContext()
        {
            Assert.That(_contextField.GetValue(null), Is.Not.Null,
                "Editor 세션이 UnitySyncContextDispatcher.Initialize()로 초기화되어 있어야 합니다.");

            bool executed = false;
            UnitySyncContextDispatcher.Enqueue(() => executed = true);

            // .. SynchronizationContext.Post의 실제 pump 주기가 한 프레임보다 느릴 수 있어 시간 제한을 두고 대기한다
            double timeoutAt = EditorApplication.timeSinceStartup + 2d;
            while (!executed && EditorApplication.timeSinceStartup < timeoutAt)
            {
                yield return null;
            }

            Assert.That(executed, Is.True);
        }

        [Test]
        public void HandleBeforeAssemblyReload_ClearsQueueAndContext()
        {
            object previousContext = _contextField.GetValue(null);
            var handler = (AssemblyReloadEvents.AssemblyReloadCallback)Delegate.CreateDelegate(
                typeof(AssemblyReloadEvents.AssemblyReloadCallback), _handleBeforeAssemblyReloadMethod);

            try
            {
                _contextField.SetValue(null, new SynchronizationContext());
                Queue.Enqueue(() => { });

                _handleBeforeAssemblyReloadMethod.Invoke(null, null);

                Assert.That(_contextField.GetValue(null), Is.Null);
                Assert.That(Queue.Count, Is.Zero);
            }
            finally
            {
                // .. HandleBeforeAssemblyReload가 해제한 실제 Editor 세션 구독을 원상 복구한다
                AssemblyReloadEvents.beforeAssemblyReload += handler;
                _contextField.SetValue(null, previousContext);
            }
        }
    }
}
