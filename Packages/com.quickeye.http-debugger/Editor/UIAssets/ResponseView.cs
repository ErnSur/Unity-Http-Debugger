using System;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    internal partial class ResponseView
    {
        private VisualElement _root;
        private HeadersView _headersViewController;
        public ResponseView(VisualElement root)
        {
            _root = root;
            AssignQueryResults(root);
            ToggleLoadingOverlay(false);
            _headersViewController = new HeadersView(headersView);
            bodyTab.TabContent = resBodyField;
            headersTab.TabContent = headersView;
            bodyTab.value = true;
        }
        
        public void ToggleReadOnlyMode(bool value)
        {
            foreach (var textField in _root.Query<TextField>().Build())
            {
                textField.isReadOnly = value;
            }
            _headersViewController.ToggleReadOnlyMode(value);
        }

        public void ToggleLoadingOverlay(bool value)
        {
            loadingOverlay.ToggleDisplayStyle(value);
        }

        public void Setup(SerializedProperty responseProperty)
        {
            resBodyField.BindProperty(responseProperty.FindPropertyRelative(nameof(ResponseData.content)));
            _root.TrackPropertyValueAndInit(responseProperty.FindPropertyRelative(nameof(ResponseData.statusCode)), prop =>
            {
                var v = prop.intValue;
                var isDefined = Enum.IsDefined(typeof(HttpStatusCode2), v);

                var message = $"{v}";
                if (isDefined)
                    message += $" {(HttpStatusCode2)v}";
                resStatusLabel.text = message;
                HttpStatusCodeUtil.ToggleStatusCodeClass(resStatusLabel, v);
            });
            _headersViewController.Setup(responseProperty.FindPropertyRelative(nameof(ResponseData.headers)));
        }
    }
}