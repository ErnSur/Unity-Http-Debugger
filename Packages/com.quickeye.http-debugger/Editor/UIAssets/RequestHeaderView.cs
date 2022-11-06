using System;
using System.Collections;
using System.Net.Http;
using QuickEye.UIToolkit;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    public partial class RequestHeaderView
    {
        public event Action RequestAwaitStarted;
        public event Action<ResponseData> RequestAwaitEnded;
        private RequestData _target;

        public RequestHeaderView(VisualElement root)
        {
            AssignQueryResults(root);
            reqSendButton.clickable.activators.Add(new ManipulatorActivationFilter()
            {
                modifiers = EventModifiers.None | EventModifiers.Shift
            });
            reqSendButton.clickable.clickedWithEventInfo += e =>
            {
                if (e.imguiEvent.shift)
                    SendRequestUsingHttpClient();
                else
                    SendRequestUsingUnityWebReq();
            };
        }

        public void Setup(SerializedObject serializedObject)
        {
            if (serializedObject is null)
                return;
            _target = (RequestData)serializedObject.targetObject;
            var isConsoleRequest = _target is ConsoleRequestData;
            UpdateBreakpointToggle(isConsoleRequest);
            UpdateIcon(isConsoleRequest);
            nameField.SetEnabled(!AssetDatabase.IsMainAsset(_target));
            reqTypeMenu.BindProperty(serializedObject.FindProperty(nameof(RequestData.type)));
            reqUrlField.BindProperty(serializedObject.FindProperty(nameof(RequestData.url)));
            nameField.BindProperty(serializedObject.FindProperty(RequestData.NamePropertyName));
        }

        private void UpdateBreakpointToggle(bool isConsoleRequest)
        {
            breakpointToggle.ToggleDisplayStyle(isConsoleRequest);
            if (isConsoleRequest)
            {
                breakpointToggle.BreakpointName = _target.name;
            }
        }

        public void ToggleReadOnlyMode(bool enabled)
        {
            reqSendButton.ToggleDisplayStyle(!enabled);
            reqTypeMenu.SetEnabled(!enabled);
            nameField.isReadOnly = enabled;
            reqUrlField.isReadOnly = enabled;
        }

        private void UpdateIcon(bool isConsoleRequest)
        {
            icon.EnableInClassList("icon-blue", !isConsoleRequest);
            icon.EnableInClassList("icon-gray", isConsoleRequest);
        }

        private void SendRequestUsingUnityWebReq()
        {
            var unityWebRequest = _target.ToUnityWebRequest();
            EditorCoroutineUtility.StartCoroutineOwnerless(HandleWebRequest(unityWebRequest));

            IEnumerator HandleWebRequest(UnityWebRequest request)
            {
                RequestAwaitStarted?.Invoke();
                yield return request.SendWebRequest();
                _target.lastResponse = RequestDataUtility.ResponseFromUnityWebRequest(request);
                RequestAwaitEnded?.Invoke(_target.lastResponse);
                request.Dispose();
            }
        }

        private async void SendRequestUsingHttpClient()
        {
            var message = _target.ToHttpRequestMessage();
            using var client = new HttpClient();
            RequestAwaitStarted?.Invoke();
            using (var res = await client.SendAsync(message))
            {
                _target.lastResponse = await RequestDataUtility.ResponseFromHttpResponseMessageAsync(res);
            }
            RequestAwaitEnded?.Invoke(_target.lastResponse);
        }
    }
}