using System;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal class ResponseView : VisualElement
    {
        [Q("res-status-label")]
        private Label resStatusLabel;

        [Q("res-body-field")]
        private CodeField resBodyField;

        [Q("loading-overlay")]
        private Label loadingOverlay;

        [Q("body-tab")]
        private Tab bodyTab;

        [Q("headers-tab")]
        private Tab headersTab;

        [Q("cookie-tab")]
        private Tab cookieTab;

        [Q("headers-view")]
        private VisualElement headersView;

        public ResponseView()
        {
            this.InitResources();
            ToggleLoadingOverlay(false);
            bodyTab.TabContent = resBodyField;
            headersTab.TabContent = headersView;
            bodyTab.value = true;
        }

        public void ToggleLoadingOverlay(bool value)
        {
            loadingOverlay.ToggleDisplayStyle(value);
        }

        public void BindProperty(SerializedProperty property)
        {
            resBodyField.Field.bindingPath = $"{property.propertyPath}.lastResponse.payload";
            this.TrackPropertyChange<int>($"{property.propertyPath}.lastResponse.statusCode", v =>
            {
                var isDefined = Enum.IsDefined(typeof(HttpStatusCode2), v);

                var message = $"{v}";
                if (isDefined)
                    message += $" {(HttpStatusCode2)v}";
                resStatusLabel.text = message;
                HttpStatusCodeUtil.ToggleStatusCodeClass(resStatusLabel, v);
            });

            headersView.Q<Label>().bindingPath = $"{property.propertyPath}.lastResponse.headers";
            this.Bind(property.serializedObject);
        }

        public new class UxmlFactory : UxmlFactory<ResponseView>
        {
        }
    }
}