using UnityEditor;

namespace QuickEye.RequestWatcher
{
    public class RequestStashWindow : VisualElementWindow
    {
        public static void Open() => Open<RequestStashWindow>("Request Stash");

        private StashDatabase _stashDatabase;

        private RequestStash _requestStash;

        protected override void OnEnable()
        {
            base.OnEnable();
            _stashDatabase = StashDatabase.instance;
            _requestStash = new RequestStash(rootVisualElement);
            _requestStash.Setup(_stashDatabase.requests);
            _requestStash.SelectionChanged += request => ExchangeInspectorWindow.Select(request);
        }
    }
}