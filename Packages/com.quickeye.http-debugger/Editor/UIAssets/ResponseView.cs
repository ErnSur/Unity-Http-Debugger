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
        private SerializedProperty contentProperty;
        private bool showRawContent;

        public ResponseView(VisualElement root)
        {
            _root = root;
            AssignQueryResults(root);
            ToggleLoadingOverlay(false);
            _headersViewController = new HeadersView(headersView);
            bodyTab.TabContent = resBodyField;
            headersTab.TabContent = headersView;
            bodyTab.value = true;
            bodyTab.BeforeMenuShow += menu =>
            {
                menu.AddItem("Preview", !showRawContent, () =>
                {
                    showRawContent = false;
                    UpdateBodyContent();
                });
                menu.AddItem("Raw", showRawContent, () =>
                {
                    showRawContent = true;
                    UpdateBodyContent();
                });
            };
            UiUtils.InitializeTabStatePersistence(_root, "responseView-active-tab", bodyTab, headersTab);
        }

        private void UpdateBodyContent()
        {
            var target = contentProperty.serializedObject.targetObject as RequestData;
            resBodyField.value = showRawContent ? contentProperty.stringValue : target.GetFormattedResponseContent();
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
            _root.ToggleDisplayStyle(true);
            loadingOverlay.ToggleDisplayStyle(value);
        }

        public void Setup(SerializedProperty responseProperty)
        {
            contentProperty = responseProperty.FindPropertyRelative(nameof(ResponseData.content));
            resBodyField.TrackPropertyValue(contentProperty, _ => UpdateBodyContent());
            UpdateBodyContent();
            var statusCodeProp = responseProperty.FindPropertyRelative(nameof(ResponseData.statusCode));
            _root.TrackPropertyValue(statusCodeProp, UpdateStatusCodeLabel);
            UpdateStatusCodeLabel(statusCodeProp);
            _headersViewController.Setup(responseProperty.FindPropertyRelative(nameof(ResponseData.headers)));
        }

        private void UpdateStatusCodeLabel(SerializedProperty statusCodeProp)
        {
            var statusCode = statusCodeProp.intValue;
            resStatusLabel.text = HttpStatusCodeUtil.StatusCodeToNiceString(statusCode);
            HttpStatusCodeUtil.ToggleStatusCodeClass(resStatusLabel, statusCode);
            _root.ToggleDisplayStyle(statusCode != 0);
        }
    }
}