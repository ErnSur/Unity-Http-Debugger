using UnityEditor;

namespace QuickEye.RequestWatcher
{
    public class RequestStashWindow : VisualElementWindow
    {
        [MenuItem("Window/Http Debugger/Request Stash")]
        public static void Open() => Open<RequestStashWindow>("Request Stash");

        private Database _database;

        private RequestStash _requestStash;

        protected override void OnEnable()
        {
            base.OnEnable();
            _database = Database.Instance;
            _requestStash = new RequestStash(rootVisualElement);
            _requestStash.Setup(new SerializedObject(_database).FindProperty(nameof(Database.stash)));
        }
    }
}