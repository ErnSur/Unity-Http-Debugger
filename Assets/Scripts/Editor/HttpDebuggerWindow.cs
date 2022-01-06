using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace QuickEye.RequestWatcher
{
    public class HttpDebuggerWindow : EditorWindow
    {
        [MenuItem("Test/PostmanWindow #&p")]
        public static void Open()
        {
            var wnd = EditorFullscreenUtility.ToggleEditorFullscreen<HttpDebuggerWindow>();
            wnd.titleContent = new GUIContent("HTTP Debugger");
        }

        [MenuItem("Test/UI Builder Toggle Fullscreen #&b")]
        public static void ToggleUIBuilderFullScreen()
        {
            EditorFullscreenUtility.ToggleEditorFullscreen(
                Type.GetType("Unity.UI.Builder.Builder, UnityEditor.UIBuilderModule"));
        }

        [MenuItem("Test/Print enum")]
        public static void PrintEnum()
        {
            var values = Enum.GetValues(typeof(HttpStatusCode));
            foreach (var value in values)
            {
                Debug.Log($"{value.ToString()}, ({(int)value})");
            }
        }

        private const string PrefsKey = "postmanData";

        private VisualElement[] tabViews;

        [Q("stash-tab")]
        private Button stashTab;

        [Q("playmode-tab")]
        private Button playmodeTab;

        [Q("mock-tab")]
        private Button mockTab;

        [Q("stash-view")]
        private QuickEye.RequestWatcher.StashView stashView;

        [Q("playmode-view")]
        private QuickEye.RequestWatcher.PlaymodeView playmodeView;


        [SerializeField]
        private PostmanData stashData;

        [SerializeField]
        private PostmanData playmodeData;

        [SerializeField]
        private int tabOpened;

        private SerializedProperty stashRequestsProp;
        private SerializedProperty playmodeRequestsProp;

        private SerializedObject serializedObject;

        private void LoadData()
        {
            var json = EditorPrefs.GetString(PrefsKey, JsonUtility.ToJson(new PostmanData()));
            stashData = JsonUtility.FromJson<PostmanData>(json);
        }

        private void SaveData()
        {
            EditorPrefs.SetString(PrefsKey, JsonUtility.ToJson(stashData));
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
            serializedObject.Update();
            rootVisualElement.Bind(serializedObject);
            playmodeView.Refresh();
        }

        public void CreateGUI()
        {
            try
            {
                var tree = HttpDebuggerResources.LoadTree<HttpDebuggerWindow>();
                tree.CloneTree(rootVisualElement);
                rootVisualElement.AssignQueryResults(this);
                InitSendButton(stashView, stashData.requests);
                //InitSendButton(playmodeView, playmodeData.requests);
                RefreshSerializedObj();
                InitTabs();

                stashView.Init(stashRequestsProp, stashData.requests);
                playmodeView.Init(playmodeRequestsProp, playmodeData.requests);

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

        private void InitSendButton<T>(T root, List<HDRequest> requestList) where T : VisualElement, IRequestListView
        {
            var stashButton = root.Q<Button>("req-send-button");
            var resView = root.Q<ResponseView>();
            stashButton.clicked += async () =>
            {
                resView.UpdateStatusLabel("Status: Loading...");
                try
                {
                    var res = await requestList[root.GetSelectedIndex()].SendAsync();
                    var statusCode = res.StatusCode;
                    resView.UpdateStatusLabel((int)statusCode);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    resView.UpdateStatusLabel("Failed");
                }

                serializedObject.Update();
                rootVisualElement.Bind(serializedObject);
            };
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
            var tabs = new (VisualElement tab, VisualElement view)[]
            {
                (stashTab, stashView),
                (playmodeTab, playmodeView)
            };

            for (var i = 0; i < tabs.Length; i++)
            {
                var index = i;
                var (tab, view) = tabs[index];
                tab.Clicked(() => OnTabClicked(index));
            }

            RefreshTabState();

            void OnTabClicked(int index)
            {
                tabOpened = index;
                RefreshTabState();
            }

            void RefreshTabState()
            {
                for (var y = 0; y < tabs.Length; y++)
                {
                    var (tab, view) = tabs[y];
                    tab.EnableInClassList("tab--active", y == tabOpened);
                    view.ToggleDisplayStyle(y == tabOpened);
                }
            }
        }
    }
}