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

        public ResponseView()
        {
            this.InitFromUxml();
        }

        public void UpdateStatusLabel(string text)
        {
            resStatusLabel.text = text;
        }
        
        public void UpdateStatusLabel( HttpStatusCode statusCode)
        {
            resStatusLabel.text = $"({(int)statusCode}) {statusCode.ToString()}";
        }
        
        public void BindProperty(SerializedProperty property)
        {
            resBodyField.bindingPath = $"{property.propertyPath}.lastResponse.payload";
            resBodyField.Bind(property.serializedObject);
        }

        public void Unbind()
        {
            resBodyField.Unbind();
        }
        public new class UxmlFactory : UxmlFactory<ResponseView>
        {
        }
    }
}