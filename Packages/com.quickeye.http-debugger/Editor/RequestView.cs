using System;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal partial class RequestView
    {
        public Action SendButtonClicked;

        public RequestView(VisualElement root)
        {
            AssignQueryResults(root);
            reqSendButton.clicked += () => SendButtonClicked?.Invoke();
            InitTabs();
        }

        public void Setup(HDRequest request)
        {
            if (request is null)
                return;
            reqTypeMenu.Init(HttpMethodType.Get);
            reqTypeMenu.value = request.type;
            reqUrlField.value = request.url;
            reqBodyField.Field.value = request.body;
            headersView.Q<Label>().text = request.headers;
        }

        public void Setup(SerializedProperty requestProperty)
        {
            if (requestProperty is null)
                return;
            reqTypeMenu.BindProperty(requestProperty.FindPropertyRelative(nameof(HDRequest.type)));
            reqUrlField.BindProperty(requestProperty.FindPropertyRelative(nameof(HDRequest.url)));
            reqBodyField.Field.BindProperty(requestProperty.FindPropertyRelative(nameof(HDRequest.body)));
            headersView.Q<Label>().BindProperty(requestProperty.FindPropertyRelative(nameof(HDRequest.headers)));
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
    }
}