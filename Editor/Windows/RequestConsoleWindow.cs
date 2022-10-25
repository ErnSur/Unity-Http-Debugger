using UnityEditor;
using UnityEngine;

namespace QuickEye.WebTools.Editor
{
    public class RequestConsoleWindow : VisualElementWindow
    {
        public static RequestConsoleWindow Open() => Open<RequestConsoleWindow>("Request Console");

        private RequestConsoleDatabase _database;

        [SerializeField]
        private RequestConsole requestConsole = new RequestConsole();

        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent = EditorGUIUtility.IconContent("UnityEditor.ConsoleWindow");
            titleContent.text = "Request Console";
            _database = RequestConsoleDatabase.instance;
            rootVisualElement.AddToClassList(EditorGUIUtility.isProSkin ? "skin--dark" : "skin--light");
            requestConsole.Init(rootVisualElement);
            requestConsole.Setup(_database.requests);
            requestConsole.SelectionChanged += request => { ExchangeInspectorWindow.Select(request, true); };
        }
    }
}