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
        public event Action<ResponseData> RequestAwaitEnded;
        private readonly VisualElement _root;
        private RequestData target;
        private readonly HeadersView _headersViewController;
        
        public RequestView(VisualElement root)
        {
            _root = root;
            AssignQueryResults(root);
            _headersViewController = new HeadersView(headersView);
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
            _headersViewController.ToggleReadOnlyMode(value);
        }

        public void Setup(SerializedObject serializedObject)
        {
            if (serializedObject is null)
                return;
            target = (RequestData)serializedObject.targetObject;
            reqTypeMenu.BindProperty(serializedObject.FindProperty(nameof(RequestData.type)));
            reqUrlField.BindProperty(serializedObject.FindProperty(nameof(RequestData.url)));
            reqBodyField.Field.BindProperty(serializedObject.FindProperty(nameof(RequestData.body)));
            _headersViewController.Setup(serializedObject.FindProperty(nameof(RequestData.headers)));
        }

        private void InitTabs()
        {
            bodyTab.TabContent = reqBodyField;
            headersTab.TabContent = headersView;
            authTab.BeforeMenuShow += menu =>
            {
                menu.AddItem("Basic Auth", false, null);
                menu.AddItem("Digest Auth", false, null);
                menu.AddItem("OAuth 1.0", false, null);
                menu.AddItem("OAuth 2.0", false, null);
                menu.AddItem("Bearer Token", false, null);
                menu.AddSeparator("");
                menu.AddItem("No Authentication", true, null);
            };

            bodyTab.value = true;
        }

        private async void OnSendButtonClick()
        {
            RequestAwaitStarted?.Invoke();
            try
            {
                using var res = await target.SendAsync();
                using var tempReq = await RequestDataUtility.FromHttpResponseMessage<RequestData>(res);
                target.lastResponse = tempReq.lastResponse;
                RequestAwaitEnded?.Invoke(target.lastResponse);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                RequestAwaitEnded?.Invoke(null);
            }
        }
    }
}