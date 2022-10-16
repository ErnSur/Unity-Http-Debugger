using System.Linq;
using UnityEngine;

namespace QuickEye.RequestWatcher
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

            _database = RequestConsoleDatabase.instance;
            requestConsole.Init(rootVisualElement);
            requestConsole.Setup(_database.requests);
            requestConsole.SelectionChanged += request => { ExchangeInspectorWindow.Select(request, true); };
        }
    }
}