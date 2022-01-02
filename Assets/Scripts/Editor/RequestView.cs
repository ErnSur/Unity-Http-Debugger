using System;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal class RequestView : VisualElement
    {
        public Action SendButtonClicked;

        [Q("req-type-menu")]
        private EnumField reqTypeMenu;

        [Q("req-url-field")]
        private TextField reqUrlField;

        [Q("req-send-button")]
        private ToolbarButton reqSendButton;

        [Q("req-body-field")]
        private QuickEye.RequestWatcher.CodeField reqBodyField;

        public RequestView()
        {
            this.InitFromUxml();
            reqTypeMenu.bindingPath = nameof(HDRequest.type);
            reqUrlField.bindingPath = nameof(HDRequest.url);
            reqBodyField.bindingPath = nameof(HDRequest.body);
            reqSendButton.clicked += () => SendButtonClicked?.Invoke();
        }

        public void BindProperty(SerializedProperty property)
        {
            reqTypeMenu.bindingPath  = $"{property.propertyPath}.{nameof(HDRequest.type)}";
            reqUrlField.bindingPath  = $"{property.propertyPath}.{nameof(HDRequest.url)}";
            reqBodyField.bindingPath = $"{property.propertyPath}.{nameof(HDRequest.body)}";
            
            reqTypeMenu.Bind(property.serializedObject);
            reqUrlField.Bind(property.serializedObject);
            reqBodyField.Bind(property.serializedObject);
        }


        public new class UxmlFactory : UxmlFactory<RequestView>
        {
        }
    }
}