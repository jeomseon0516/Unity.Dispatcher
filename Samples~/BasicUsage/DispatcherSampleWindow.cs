using System.Threading.Tasks;
using Jeomseon.Unity.Dispatcher;
using UnityEditor;
using UnityEngine;

namespace Jeomseon.Samples.Dispatcher
{
    public sealed class DispatcherSampleWindow : EditorWindow
    {
        private string _status = "대기 중";

        [MenuItem("Jeomseon/Dispatcher/Basic Usage Sample")]
        private static void Open()
        {
            GetWindow<DispatcherSampleWindow>("Dispatcher Sample");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("상태", _status);

            if (GUILayout.Button("백그라운드 스레드에서 작업 실행"))
            {
                Run();
            }
        }

        private void Run()
        {
            _status = "백그라운드 작업 실행 중...";

            // .. async/await 없이 백그라운드 스레드에서 직접 콜백을 받는 경우를 재현한다
            // .. Editor 오브젝트(this, GUI)는 메인 스레드에서만 접근할 수 있으므로 Enqueue로 되돌린다
            Task.Run(() =>
            {
                int result = 21 * 2;
                UnitySyncContextDispatcher.Enqueue(() =>
                {
                    _status = $"Editor 메인 스레드 결과: {result}";
                    Repaint();
                });
            });
        }
    }
}
