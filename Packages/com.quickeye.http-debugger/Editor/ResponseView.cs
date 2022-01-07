using System;
using System.Net;
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
        private QuickEye.RequestWatcher.CodeField resBodyField;

        [Q("loading-overlay")]
        private Label loadingOverlay;


        public ResponseView()
        {
            this.InitResources();
            ToggleLoadingOverlay(false);
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
                HttpStatusCodeUtil.ToggleStatusCodeClass(resStatusLabel,v);
            });
            //resStatusLabel.bindingPath = $"{property.propertyPath}.lastResponse.statusCode";
            this.Bind(property.serializedObject);
        }

        public new class UxmlFactory : UxmlFactory<ResponseView>
        {
        }
    }
}