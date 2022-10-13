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
            _requestStash.Setup(new SerializedObject(_stashDatabase).FindProperty(nameof(StashDatabase.requests)));
            // (HDRequest)property.boxedValue returns a copy of HDRequest
            // becaues its scriptable request is not serialized it get duplicated on each selection change
            _requestStash.SelectionChanged += property => ExchangeInspectorWindow.Select((HDRequest)property.boxedValue);
        }
    }
}