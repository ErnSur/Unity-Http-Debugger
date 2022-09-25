using System;
using UnityEditor;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    public class HttpExchangeInspectorWindow : VisualElementWindow
    {
        [MenuItem("Window/Http Debugger/Http Exchange Inspector")]
        public static void Open() => Open<HttpExchangeInspectorWindow>("Http Exchange Inspector");

        internal static void OpenNew(HDRequest request)
        {
            var newWindow = CreateInstance<HttpExchangeInspectorWindow>();
            newWindow.titleContent = new GUIContent(request.name);
            newWindow.position = new Rect(newWindow.position) { size = new Vector2(600, 500) };
            newWindow.Controller.Setup(request);
            newWindow.ShowModalUtility();
        }
        
        internal static void Open(HDRequest request)
        {
            var newWindow = GetWindow<HttpExchangeInspectorWindow>();
            var prop = new SerializedObject(HttpDebuggerDatabase.Instance);
            newWindow.Controller.Setup(prop.FindProperty(nameof(HttpDebuggerDatabase.playmodeRequests)).GetArrayElementAtIndex(0));
        }
        [SerializeField]
        private HttpDebuggerDatabase _database;

        private HttpExchangeInspector inspector;
        internal HttpExchangeInspector Controller => inspector;

        protected override void OnEnable()
        {
            base.OnEnable();
            _database = HttpDebuggerDatabase.Instance;
            inspector = new HttpExchangeInspector(rootVisualElement);
            //inspector.Setup(_database.playmodeRequests);
        }
    }
}