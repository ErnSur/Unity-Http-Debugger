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
        public event Action SendButtonClicked;
        public event Action RequestAwaitStarted;
        public event Action<HDResponse> RequestAwaitEnded;
        private readonly VisualElement _root;
        private HDRequest target;

        public RequestView(VisualElement root)
        {
            _root = root;
            AssignQueryResults(root);
            reqSendButton.clicked += OnSendButtonClick;
            reqSendButton.clicked += () => SendButtonClicked?.Invoke();
            InitTabs();
        }

        public void ToggleReadOnlyMode(bool value)
        {
            reqSendButton.ToggleDisplayStyle(!value);
            reqTypeMenu.SetEnabled(!value);
            foreach (var textField in _root.Query<TextField>().Build())
            {
                textField.isReadOnly = value;
            }
        }

        // public void Setup(HDRequest request)
        // {
        //     if (request is null)
        //         return;
        //     target = request;
        //     reqTypeMenu.Init(HttpMethodType.Get);
        //     reqTypeMenu.value = request.type;
        //     reqUrlField.value = request.url;
        //     reqBodyField.Field.value = request.body;
        //     headersView.Q<Label>().text = request.headers;
        // }

        public void Setup(SerializedObject serializedObject)
        {
            if (serializedObject is null)
                return;
            target = (HDRequest)serializedObject.targetObject;
            reqTypeMenu.BindProperty(serializedObject.FindProperty(nameof(HDRequest.type)));
            reqUrlField.BindProperty(serializedObject.FindProperty(nameof(HDRequest.url)));
            reqBodyField.Field.BindProperty(serializedObject.FindProperty(nameof(HDRequest.body)));
            headersView.Q<Label>().BindProperty(serializedObject.FindProperty(nameof(HDRequest.headers)));
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

        private async void OnSendButtonClick()
        {
            RequestAwaitStarted?.Invoke();
            try
            {
                using var res = await target.SendAsync();
                using var tempReq = await HDRequest.FromHttpResponseMessage(target.id, res);
                target.lastResponse = tempReq.lastResponse;
                Debug.Log($"{target.lastResponse.statusCode}");
                RequestAwaitEnded?.Invoke(target.lastResponse);
            }
            catch (Exception e)
            {
                Debug.Log($"URL: {target.url}");
                Debug.LogException(e);
                RequestAwaitEnded?.Invoke(null);
            }
        }
    }
}