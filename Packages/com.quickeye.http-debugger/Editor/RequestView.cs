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
            this.InitResources();
            reqTypeMenu.bindingPath = nameof(HDRequest.type);
            reqUrlField.bindingPath = nameof(HDRequest.url);
            reqBodyField.Field.bindingPath = nameof(HDRequest.body);
            reqSendButton.clicked += () => SendButtonClicked?.Invoke();
        }

        public void Bind(SerializedProperty requestProp)
        {
            reqTypeMenu.bindingPath  = $"{requestProp.propertyPath}.{nameof(HDRequest.type)}";
            reqUrlField.bindingPath  = $"{requestProp.propertyPath}.{nameof(HDRequest.url)}";
            reqBodyField.Field.bindingPath = $"{requestProp.propertyPath}.{nameof(HDRequest.body)}";
            
            this.Bind(requestProp.serializedObject);
        }


        public new class UxmlFactory : UxmlFactory<RequestView>
        {
        }
    }
}