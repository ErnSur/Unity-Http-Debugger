using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    public class RequestConsoleWindow : VisualElementWindow
    {
        [MenuItem("Window/Http Debugger/Request Console")]
        public static void Open() => Open<RequestConsoleWindow>("Request Console");

        private Database _database;

        [SerializeField]
        private RequestConsole _requestConsole = new RequestConsole();

        protected override void OnEnable()
        {
            base.OnEnable();
            _database = Database.Instance;
            _requestConsole.Init(rootVisualElement);
            _requestConsole.Setup(_database.playmodeRequests);
            _requestConsole.ItemsChosen += requests =>
            {
                ScriptableRequest.Select(requests.FirstOrDefault());
            };
        }
    }
}