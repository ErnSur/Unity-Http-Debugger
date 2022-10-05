using UnityEditor;

namespace QuickEye.RequestWatcher
{
    public class RequestStashWindow : VisualElementWindow
    {
        public static void Open() => Open<RequestStashWindow>("Request Stash");

        private Database _database;

        private RequestStash _requestStash;

        protected override void OnEnable()
        {
            base.OnEnable();
            _database = Database.instance;
            _requestStash = new RequestStash(rootVisualElement);
            _requestStash.Setup(new SerializedObject(_database).FindProperty(nameof(Database.stash)));
            _requestStash.SelectionChanged += property => ExchangeInspectorWindow.Select((HDRequest)property.boxedValue);
        }
    }
}