using System;
using System.Collections;
using System.Threading;
using Jeomseon.Unity.Dispatcher;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace Jeomseon.Tests
{
    public sealed class UnitySyncContextDispatcherTests
    {
        private sealed class NonExecutingSynchronizationContext : SynchronizationContext
        {
            public override void Post(SendOrPostCallback callback, object state)
            {
            }
        }

        [Test]
        public void Enqueue_Throws_WhenContextNotInitialized()
        {
            SynchronizationContext previousContext = SynchronizationContext.Current;
            UnitySyncContextDispatcher.HandleBeforeAssemblyReload();

            try
            {
                Assert.Throws<InvalidOperationException>(() => UnitySyncContextDispatcher.Enqueue(() => { }));
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previousContext);
                UnitySyncContextDispatcher.Initialize();
            }
        }

        [UnityTest]
        public IEnumerator Enqueue_ExecutesQueuedAction_OnCapturedContext()
        {
            Assert.That(UnitySyncContextDispatcher.IsInitialized, Is.True,
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
            SynchronizationContext previousContext = SynchronizationContext.Current;

            try
            {
                SynchronizationContext.SetSynchronizationContext(
                    new NonExecutingSynchronizationContext());
                UnitySyncContextDispatcher.Initialize();
                UnitySyncContextDispatcher.Enqueue(() => { });

                Assert.That(UnitySyncContextDispatcher.PendingActionCount, Is.EqualTo(1));
                UnitySyncContextDispatcher.HandleBeforeAssemblyReload();

                Assert.That(UnitySyncContextDispatcher.IsInitialized, Is.False);
                Assert.That(UnitySyncContextDispatcher.PendingActionCount, Is.Zero);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previousContext);
                UnitySyncContextDispatcher.Initialize();
            }
        }
    }
}
