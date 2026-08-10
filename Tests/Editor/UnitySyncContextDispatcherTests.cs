using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Jeomseon.Dispatcher;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace Jeomseon.Tests
{
    public sealed class UnitySyncContextDispatcherTests
    {
        private static readonly Type DispatcherType = typeof(UnitySyncContextDispatcher);

        private static readonly FieldInfo ContextField = DispatcherType.GetField(
            "_unityContext", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo QueueField = DispatcherType.GetField(
            "ExecutionQueue", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly MethodInfo HandleBeforeAssemblyReloadMethod = DispatcherType.GetMethod(
            "HandleBeforeAssemblyReload", BindingFlags.Static | BindingFlags.NonPublic);

        private static Queue<Action> Queue => (Queue<Action>)QueueField.GetValue(null);

        [Test]
        public void Enqueue_Throws_WhenContextNotInitialized()
        {
            object previousContext = ContextField.GetValue(null);
            ContextField.SetValue(null, null);

            try
            {
                Assert.Throws<InvalidOperationException>(() => UnitySyncContextDispatcher.Enqueue(() => { }));
            }
            finally
            {
                ContextField.SetValue(null, previousContext);
            }
        }

        [UnityTest]
        public IEnumerator Enqueue_ExecutesQueuedAction_OnCapturedContext()
        {
            Assert.That(ContextField.GetValue(null), Is.Not.Null,
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
            object previousContext = ContextField.GetValue(null);
            var handler = (AssemblyReloadEvents.AssemblyReloadCallback)Delegate.CreateDelegate(
                typeof(AssemblyReloadEvents.AssemblyReloadCallback), HandleBeforeAssemblyReloadMethod);

            try
            {
                ContextField.SetValue(null, new SynchronizationContext());
                Queue.Enqueue(() => { });

                HandleBeforeAssemblyReloadMethod.Invoke(null, null);

                Assert.That(ContextField.GetValue(null), Is.Null);
                Assert.That(Queue.Count, Is.Zero);
            }
            finally
            {
                // .. HandleBeforeAssemblyReload가 해제한 실제 Editor 세션 구독을 원상 복구한다
                AssemblyReloadEvents.beforeAssemblyReload += handler;
                ContextField.SetValue(null, previousContext);
            }
        }
    }
}
