using System.Linq;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    public class RequestConsoleWindow : VisualElementWindow
    {
        public static void Open() => Open<RequestConsoleWindow>("Request Console");

        private Database _database;

        [SerializeField]
        private RequestConsole requestConsole = new RequestConsole();

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _database = Database.Instance;
            requestConsole.Init(rootVisualElement);
            requestConsole.Setup(_database.playmodeRequests);
            requestConsole.ItemsChosen += requests =>
            {
                ExchangeInspectorWindow.Select(requests.FirstOrDefault(),true);
            };
        }
    }
}