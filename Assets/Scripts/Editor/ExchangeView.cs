using System;
using System.Net;
using System.Security.Policy;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal class ExchangeView : VisualElement
    {
        public ResponseView ResView => resView;

        public RequestView ReqView => reqView;

        [Q("exchange-view")]
        private VisualElement exchangeView;

        [Q("req-view")]
        private QuickEye.RequestWatcher.RequestView reqView;

        [Q("res-view")]
        private QuickEye.RequestWatcher.ResponseView resView;

        [Q("no-select-view")]
        private Label noSelectView;

        private SerializedProperty requestProperty;

        public ExchangeView()
        {
            this.InitFromUxml();
        }

        public void SetupView(SerializedProperty requestProperty)
        {
            this.requestProperty = requestProperty;
            RefreshReqView();
        }

        private void RefreshReqView()
        {
            UpdateSelectedView();
            if (requestProperty == null)
                return;
            reqView.BindProperty(requestProperty);
            resView.BindProperty(requestProperty);
            if (TryGetStatusCode(out var statusCode))
                resView.UpdateStatusLabel(statusCode);
            else
                resView.UpdateStatusLabel("Response Status");
        }

        private bool TryGetStatusCode(out int code)
        {
            try
            {
                //var values = (HttpStatusCode[])Enum.GetValues(typeof(HttpStatusCode));
                code = requestProperty
                    .FindPropertyRelative($"{nameof(HDRequest.lastResponse)}.{nameof(HDResponse.statusCode)}")
                    .intValue;
                return true;
            }
            catch (Exception e)
            {
                code = 0;
                return false;
            }
        }

        private void UpdateSelectedView()
        {
            var hasSelection = requestProperty != null;
            noSelectView.ToggleDisplayStyle(!hasSelection);
            exchangeView.ToggleDisplayStyle(hasSelection);
        }

        private class UxmlFactory : UxmlFactory<ExchangeView>
        {
        }
    }
}