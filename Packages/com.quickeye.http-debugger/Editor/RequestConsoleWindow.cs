using System;
using UnityEditor;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    public class RequestConsoleWindow : VisualElementWindow
    {
        [MenuItem("Window/Http Debugger/Request Console")]
        public static void Open() => Open<RequestConsoleWindow>("Request Console");
        
        [SerializeField]
        private HttpDebuggerDatabase _database;

        private RequestConsole _requestConsole;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _database = HttpDebuggerDatabase.Instance;
            _requestConsole = new RequestConsole(rootVisualElement);
            _requestConsole.Setup(_database.playmodeRequests);
        }
    }
}