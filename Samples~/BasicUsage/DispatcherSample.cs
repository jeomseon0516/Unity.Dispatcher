using System.Threading.Tasks;
using Jeomseon.Dispatcher;
using UnityEngine;

namespace Jeomseon.Samples.Dispatcher
{
    public sealed class DispatcherSample : MonoBehaviour
    {
        [ContextMenu("백그라운드 작업 실행")]
        private async void Run()
        {
            int result = await Task.Run(() => 21 * 2);
            UnitySyncContextDispatcher.Enqueue(
                () => Debug.Log($"메인 스레드 결과: {result}"));
        }
    }
}
