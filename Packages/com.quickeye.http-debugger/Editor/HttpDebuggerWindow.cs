using System;
using System.Net;
using System.Net.Http;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace QuickEye.RequestWatcher
{
    public class HttpDebuggerWindow : EditorWindow
    {
        [MenuItem("Window/Http Debugger #&p")]
        public static void Open()
        {
            var wnd = EditorFullscreenUtility.ToggleEditorFullscreen<HttpDebuggerWindow>();
            wnd.titleContent = new GUIContent("HTTP Debugger");
        }

        [MenuItem("Window/UI Builder Toggle Fullscreen #&b")]
        public static void ToggleUIBuilderFullScreen()
        {
            EditorFullscreenUtility.ToggleEditorFullscreen(
                Type.GetType("Unity.UI.Builder.Builder, UnityEditor.UIBuilderModule"));
        }

        public static void PrintEnum()
        {
            var values = Enum.GetValues(typeof(HttpStatusCode));
            foreach (var value in values)
            {
                Debug.Log($"{value.ToString()}, ({(int)value})");
            }
        }

        private const string StatePrefsKey = "httpdebugger.state";

        private VisualElement[] tabViews;

        [Q("stash-tab")]
        private QuickEye.RequestWatcher.Tab stashTab;

        [Q("playmode-tab")]
        private QuickEye.RequestWatcher.Tab playmodeTab;

        [Q("mock-tab")]
        private QuickEye.RequestWatcher.Tab mockTab;

        [Q("stash-view")]
        private QuickEye.RequestWatcher.StashView stashView;

        [Q("playmode-view")]
        private QuickEye.RequestWatcher.PlaymodeView playmodeView;

        [SerializeField]
        private PostmanData stashData;

        [SerializeField]
        private PostmanData playmodeData;

        [SerializeField]
        private int tabOpen;

        private SerializedProperty stashRequestsProp;
        private SerializedProperty playmodeRequestsProp;

        private SerializedObject serializedObject;

        private void LoadData()
        {
            var stateJson = EditorPrefs.GetString(StatePrefsKey, JsonUtility.ToJson(this));
            JsonUtility.FromJsonOverwrite(stateJson, this);
        }

        private void SaveData()
        {
            EditorPrefs.SetString(StatePrefsKey, JsonUtility.ToJson(this));
        }

        private void OnEnable()
        {
            HttpClientLoggerEditorWrapper.ExchangeLogged += RefreshPlaymodeView;
            LoadData();
        }

        private void OnDisable()
        {
            HttpClientLoggerEditorWrapper.ExchangeLogged -= RefreshPlaymodeView;
            SaveData();
        }

        private void RefreshPlaymodeView(HDRequest data)
        {
            playmodeData.requests.Add(data);
            if (serializedObject == null)
                return;
            // TODO: when window was docked but not active we wont get serObj, when making it active again we reed to do this refresh
            serializedObject.Update();
            rootVisualElement.Bind(serializedObject);
            playmodeView.Refresh();
        }

        public void CreateGUI()
        {
            try
            {
                HttpDebuggerResources.TryLoadTree<HttpDebuggerWindow>(out var tree);
                tree.CloneTree(rootVisualElement);
                rootVisualElement.AssignQueryResults(this);
                InitSendButton(stashView, stashData);
                InitSendButton(playmodeView, playmodeData);
                InitSaveToStashAction();
                RefreshSerializedObj();
                InitTabs();

                stashView.Setup(stashRequestsProp);
                playmodeView.Setup(playmodeRequestsProp);

                rootVisualElement.Bind(serializedObject);
                mockTab.clicked += () =>
                {
                    rootVisualElement.Bind(serializedObject);
                    Debug.Log($"MES: Bind");
                };
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void InitSaveToStashAction()
        {
            try
            {
                var listView = playmodeView.Q<ListView>();
                listView.bindItem += (element, i) =>
                {
                    element.AddManipulator(new ContextualMenuManipulator(evt =>
                    {
                        evt.menu.AppendAction("Save To Stash", _ =>
                        {
                            var index = (int)element.userData;
                            stashData.requests.Add(playmodeData.requests[index].Clone());
                        });
                    }));
                };
            }
            catch (Exception e)
            {
                Debug.Log($"Failed at InitSaveToStashAction");
                Debug.LogException(e);
                throw;
            }
        }

        private void InitSendButton<T>(T root, PostmanData dataStore) where T : VisualElement, IRequestListView
        {
            try
            {
                var sendButton = root.Q<Button>("req-send-button");
                var resView = root.Q<ResponseView>();
                sendButton.clicked += async () =>
                {
                    resView.ToggleLoadingOverlay(true);
                    try
                    {
                        var index = root.GetSelectedIndex();
                        var hdReq = dataStore.requests[index];
                        using (var res = await hdReq.SendAsync())
                        {
                            var newReq = await HDRequest.FromHttpResponseMessage(hdReq.name, res);
                            dataStore.requests[index] = newReq;
                        }

                        serializedObject.UpdateIfRequiredOrScript();
                        rootVisualElement.Bind(serializedObject);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                    finally
                    {
                        resView.ToggleLoadingOverlay(false);
                    }

                    serializedObject.Update();
                    rootVisualElement.Bind(serializedObject);
                };
            }
            catch (Exception e)
            {
                Debug.Log($"Failed at Init Send Button");
                Debug.LogException(e);
                throw;
            }
        }

        private void RefreshSerializedObj()
        {
            serializedObject = new SerializedObject(this);
            stashRequestsProp =
                serializedObject.FindProperty($"{nameof(stashData)}.{nameof(PostmanData.requests)}");
            playmodeRequestsProp =
                serializedObject.FindProperty($"{nameof(playmodeData)}.{nameof(PostmanData.requests)}");
        }

        private void InitTabs()
        {
            var tabs = new[]
            {
                stashTab,
                playmodeTab
            };

            stashTab.TabContent = stashView;
            playmodeTab.TabContent = playmodeView;
            for (var i = 0; i < tabs.Length; i++)
            {
                var tab = tabs[i];
                var index = i;
                tab.clicked += () => tabOpen = index;
            }

            tabs[tabOpen].value = true;
        }
    }

    class Test
    {
        [MenuItem("Test/SendSampleReq")]
        public static void SendSampleRequest()
        {
            SendRequestAsync();
        }

        private static async void SendRequestAsync()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://dummy.restapiexample.com/api/v1/employee/1"),
            };
            request.Headers.Add("X-CustomHeader", "Dupa");
            using (var response = await client.SendAsync($"Employee", request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Debug.Log(body);
            }
        }
    }
}