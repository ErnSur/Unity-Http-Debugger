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
            this.InitFromUxml();
            ToggleLoadingOverlay(false);
        }

        public void ToggleLoadingOverlay(bool value)
        {
            loadingOverlay.ToggleDisplayStyle(value);
        }

        public void BindProperty(SerializedProperty property)
        {
            resBodyField.Field.bindingPath = $"{property.propertyPath}.lastResponse.payload";
            resStatusLabel.bindingPath = $"{property.propertyPath}.lastResponse.statusCode";
            this.Bind(property.serializedObject);
        }

        public new class UxmlFactory : UxmlFactory<ResponseView>
        {
        }
    }
}