using System;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal partial class ResponseView
    {
        private VisualElement _root;
        public ResponseView(VisualElement root)
        {
            _root = root;
            AssignQueryResults(root);
            ToggleLoadingOverlay(false);
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
        }

        public void ToggleLoadingOverlay(bool value)
        {
            loadingOverlay.ToggleDisplayStyle(value);
        }

        public void Setup(SerializedProperty responseProperty)
        {
            
            resBodyField.Field.BindProperty(responseProperty.FindPropertyRelative(nameof(HDResponse.payload)));
            _root.TrackPropertyChange(responseProperty.FindPropertyRelative(nameof(HDResponse.statusCode)), prop =>
            {
                var v = prop.intValue;
                var isDefined = Enum.IsDefined(typeof(HttpStatusCode2), v);

                var message = $"{v}";
                if (isDefined)
                    message += $" {(HttpStatusCode2)v}";
                resStatusLabel.text = message;
                HttpStatusCodeUtil.ToggleStatusCodeClass(resStatusLabel, v);
            });

            headersView.Q<Label>().BindProperty(responseProperty.FindPropertyRelative(nameof(HDResponse.headers)));
        }
    }
}