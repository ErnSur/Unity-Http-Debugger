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
            loadingOverlay.ToggleDisplayStyle(value);
        }

        public void Setup(SerializedProperty responseProperty)
        {
            contentProperty = responseProperty.FindPropertyRelative(nameof(ResponseData.content));
            resBodyField.TrackPropertyValue(contentProperty, _ => UpdateBodyContent());
            UpdateBodyContent();
            _root.TrackPropertyValueAndInit(responseProperty.FindPropertyRelative(nameof(ResponseData.statusCode)),
                prop =>
                {
                    var v = prop.intValue;
                    var isDefined = Enum.IsDefined(typeof(HttpStatusCode2), v);

                    var message = $"{v}";
                    if (isDefined)
                        message += $" {ObjectNames.NicifyVariableName(((HttpStatusCode2)v).ToString())}";
                    resStatusLabel.text = message;
                    HttpStatusCodeUtil.ToggleStatusCodeClass(resStatusLabel, v);
                });
            _headersViewController.Setup(responseProperty.FindPropertyRelative(nameof(ResponseData.headers)));
        }
    }
}