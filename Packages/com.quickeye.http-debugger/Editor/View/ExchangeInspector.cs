using QuickEye.UIToolkit;
using UnityEditor;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal partial class ExchangeInspector
    {
        private (HDRequest obj, SerializedProperty prop) target;
        private readonly RequestView _requestViewController;
        private readonly ResponseView _responseViewController;
        private VisualElement _root;

        public ExchangeInspector(VisualElement root)
        {
            _root = root;
            AssignQueryResults(root);
            _requestViewController = new RequestView(requestViewRoot);
            _responseViewController = new ResponseView(responseViewRoot);
            exchangePane.fixedPaneIndex = 1;
            exchangePane.fixedPaneInitialDimension = 400;
            exchangePane.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                exchangePane.orientation = evt.newRect.width < 500
                    ? TwoPaneSplitViewOrientation.Vertical
                    : TwoPaneSplitViewOrientation.Horizontal;
            });
            RefreshReqView();
        }

        public void Setup(HDRequest request)
        {
            target = (request, null);
            RefreshReqView();
        }

        public void Setup(SerializedProperty requestProperty, bool readOnly = false)
        {
            ToggleReadOnlyMode(readOnly);
            target = (null, requestProperty);
            RefreshReqView();
        }

        private void ToggleReadOnlyMode(bool value)
        {
            _requestViewController.ToggleReadOnlyMode(value);
            _responseViewController.ToggleReadOnlyMode(value);
        }

        private void RefreshReqView()
        {
            UpdateSelectedView();
            if (target == (null, null))
                return;
            _requestViewController.Setup(target.obj);
            _requestViewController.Setup(target.prop);
            _responseViewController.Setup(target.prop?.FindPropertyRelative(nameof(HDRequest.lastResponse)));
        }

        private void UpdateSelectedView()
        {
            var hasSelection = target != (null, null);
            noSelectView.ToggleDisplayStyle(!hasSelection);
            exchangePane.ToggleDisplayStyle(hasSelection);
        }
    }
}