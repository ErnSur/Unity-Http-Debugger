using System;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
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

        [Q("body-tab")]
        private QuickEye.RequestWatcher.Tab bodyTab;

        [Q("auth-tab")]
        private QuickEye.RequestWatcher.TabDropdown authTab;

        [Q("headers-tab")]
        private QuickEye.RequestWatcher.Tab headersTab;

        [Q("headers-view")]
        private VisualElement headersView;


        public RequestView()
        {
            this.InitResources();
            reqTypeMenu.bindingPath = nameof(HDRequest.type);
            reqUrlField.bindingPath = nameof(HDRequest.url);
            reqBodyField.Field.bindingPath = nameof(HDRequest.body);
            reqSendButton.clicked += () => SendButtonClicked?.Invoke();
            InitTabs();
            InitHeadersView();
        }

        private void InitHeadersView()
        {
        }

        public void Bind(SerializedProperty requestProp)
        {
            reqTypeMenu.bindingPath = $"{requestProp.propertyPath}.{nameof(HDRequest.type)}";
            reqUrlField.bindingPath = $"{requestProp.propertyPath}.{nameof(HDRequest.url)}";
            reqBodyField.Field.bindingPath = $"{requestProp.propertyPath}.{nameof(HDRequest.body)}";
            headersView.Q<Label>().bindingPath = $"{requestProp.propertyPath}.{nameof(HDRequest.headers)}";

            this.Bind(requestProp.serializedObject);
        }

        private void InitTabs()
        {
            var tabs = new[]
            {
                bodyTab,
                authTab,
                headersTab
            };

            bodyTab.TabContent = reqBodyField;
            headersTab.TabContent = headersView;
            authTab.BeforeMenuShow += menu =>
            {
                menu.AddItem(new GUIContent("Basic Auth"), false, null);
                menu.AddItem(new GUIContent("Digest Auth"), false, null);
                menu.AddItem(new GUIContent("OAuth 1.0"), false, null);
                menu.AddItem(new GUIContent("OAuth 2.0"), false, null);
                menu.AddItem(new GUIContent("Bearer Token"), false, null);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("No Authentication"), true, null);
            };

            bodyTab.value = true;
        }


        public new class UxmlFactory : UxmlFactory<RequestView>
        {
        }
    }
}