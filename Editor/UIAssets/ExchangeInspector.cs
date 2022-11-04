using QuickEye.UIToolkit;
using UnityEditor;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    internal partial class ExchangeInspector
    {
        private SerializedObject _target;
        private readonly RequestView _requestViewController;
        private readonly ResponseView _responseViewController;
        private readonly RequestHeaderView _requestHeaderViewController;
        private VisualElement _root;

        public ExchangeInspector(VisualElement root)
        {
            _root = root;
            AssignQueryResults(root);
            _requestViewController = new RequestView(requestViewRoot);
            _responseViewController = new ResponseView(responseViewRoot);
            _requestHeaderViewController = new RequestHeaderView(requestHeaderView);

            _requestHeaderViewController.RequestAwaitStarted +=
                () => _responseViewController.ToggleLoadingOverlay(true);
            _requestHeaderViewController.RequestAwaitEnded += _ => _responseViewController.ToggleLoadingOverlay(false);
            exchangePane.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                exchangePane.orientation = evt.newRect.width < 500
                    ? TwoPaneSplitViewOrientation.Vertical
                    : TwoPaneSplitViewOrientation.Horizontal;
            });
            RefreshReqView();
        }

        public void Setup(SerializedObject serializedObject)
        {
            _target = serializedObject;
            RefreshReqView();
            ToggleReadOnlyMode(serializedObject.FindProperty(nameof(RequestData.isReadOnly)).boolValue);
        }

        private void ToggleReadOnlyMode(bool value)
        {
            _requestViewController.ToggleReadOnlyMode(value);
            _responseViewController.ToggleReadOnlyMode(value);
            _requestHeaderViewController.ToggleReadOnlyMode(value);
        }

        private void RefreshReqView()
        {
            UpdateSelectedView();
            if (_target == null)
                return;
            _requestHeaderViewController.Setup(_target);
            _requestViewController.Setup(_target);
            _responseViewController.Setup(_target.FindProperty(nameof(RequestData.lastResponse)));
        }

        private void UpdateSelectedView()
        {
            var hasSelection = _target != null;
            noSelectView.ToggleDisplayStyle(!hasSelection);
            exchangePane.ToggleDisplayStyle(hasSelection);
        }
    }
}