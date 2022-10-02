using System;
using UnityEditor;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    public class ExchangeInspectorWindow : VisualElementWindow
    {
        [MenuItem("Window/Http Debugger/Http Exchange Inspector")]
        public static ExchangeInspectorWindow Open() => Open<ExchangeInspectorWindow>("Exchange Inspector");

        internal static void OpenNew(HDRequest request)
        {
            var newWindow = CreateInstance<ExchangeInspectorWindow>();
            newWindow.titleContent = new GUIContent(request.name);
            newWindow.position = new Rect(newWindow.position) { size = new Vector2(600, 500) };
            newWindow.Controller.Setup(request);
            newWindow.ShowModalUtility();
        }

        internal static void Open(HDRequest request) { }

        public static void Open(int playmodeRequestIndex)
        {
            var prop = new SerializedObject(Database.Instance);
            var requestProp = prop.FindProperty(nameof(Database.playmodeRequests))
                .GetArrayElementAtIndex(playmodeRequestIndex);
            Open(requestProp);
        }

        [SerializeField]
        private Database _database;

        private ExchangeInspector inspector;
        internal ExchangeInspector Controller => inspector;

        protected override void OnEnable()
        {
            base.OnEnable();
            _database = Database.Instance;
            inspector = new ExchangeInspector(rootVisualElement);
            //inspector.Setup(_database.playmodeRequests);
        }


        public static void Open(SerializedProperty property)
        {
            var window = Open();

            window.Controller.Setup(property);
        }
    }
}