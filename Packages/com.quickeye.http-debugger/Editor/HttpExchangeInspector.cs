using QuickEye.UIToolkit;
using UnityEditor;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal partial class HttpExchangeInspector
    {
        private (HDRequest obj, SerializedProperty prop) target;
        private RequestView _requestViewController;
        private ResponseView _responseViewController;

        public HttpExchangeInspector(VisualElement root)
        {
            AssignQueryResults(root);
            _requestViewController = new RequestView(requestView);
            _responseViewController = new ResponseView(responseView);
            exchangePane.fixedPaneIndex = 1;
            exchangePane.fixedPaneInitialDimension = 400;
        }

        public void Setup(HDRequest request)
        {
            target = (request, null);
            RefreshReqView();
        }

        public void Setup(SerializedProperty requestProperty, bool readOnly = false)
        {
            target = (null, requestProperty);
            RefreshReqView();
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