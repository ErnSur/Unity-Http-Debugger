using System;
using System.Linq;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace QuickEye.RequestWatcher
{
    public class PostmanWindow : EditorWindow
    {
        private static Type prevWindowType;

        [MenuItem("Window/UIElements/PostmanWindow #&p")]
        public static void Open()
        {
            EditorFullscreenUtility.ToggleEditorFullscreen<PostmanWindow>();
        }

        [MenuItem("Window/UI Toolkit/UI Builder Toggle Fullscreen #&b")]
        public static void ToggleUIBuilderFullScreen()
        {
            EditorFullscreenUtility.ToggleEditorFullscreen(Type.GetType("Unity.UI.Builder.Builder, UnityEditor.UIBuilderModule"));
        }

        [Q("main-panel")]
        private VisualElement mainPanel;

        [Q("req-panel")]
        private VisualElement reqPanel;

        [Q("req-type-menu")]
        private EnumField reqTypeMenu;

        [Q("req-url-field")]
        private TextField reqUrlField;

        [Q("req-send-button")]
        private ToolbarButton reqSendButton;

        [Q("req-body-field")]
        private TextField reqBodyField;

        [Q("res-panel")]
        private VisualElement resPanel;

        [Q("res-status-label")]
        private Label resStatusLabel;

        [Q("res-body-field")]
        private TextField resBodyField;

        [Q("req-headers-view")]
        private VisualElement reqHeadersView;

        [Q("sidebar")]
        private VisualElement sidebar;

        [SerializeField]
        private HttpRequestSettings selectedRequestSettings;

        private void OnEnable()
        {
            if (selectedRequestSettings == null)
                selectedRequestSettings = CreateInstance<HttpRequestSettings>();
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/PostmanWindow.uxml");
            VisualElement labelFromUXML = visualTree.CloneTree();
            root.Add(labelFromUXML);
            root.AssignQueryResults(this);
            InitReqTypeField();
            InitReqUrlField();
            InitReqBodyField();
            InitReqSendButton();
            InitSidebar();
            GetHeaders();
        }

        private void InitSidebar()
        {
        }

        private void InitReqBodyField()
        {
            reqBodyField.value = selectedRequestSettings.body;
            reqBodyField.RegisterValueChangedCallback(evt => { selectedRequestSettings.body = evt.newValue; });
        }

        private void InitReqUrlField()
        {
            reqUrlField.value = selectedRequestSettings.url;
            reqUrlField.RegisterValueChangedCallback(evt => { selectedRequestSettings.url = evt.newValue; });
        }

        private void InitReqSendButton()
        {
            reqSendButton.clicked += async () =>
            {
                resStatusLabel.text = "Loading...";
                var res = await selectedRequestSettings.SendAsync();
                resStatusLabel.text = res.StatusCode.ToString();
                var prettyJson = new JsonFormatter(await res.Content.ReadAsStringAsync()).Format();
                resBodyField.value = prettyJson;
            };
        }

        private void InitReqTypeField()
        {
            reqTypeMenu.Init(selectedRequestSettings.type);

            reqTypeMenu.RegisterCallback<ChangeEvent<Enum>>((evt) =>
            {
                selectedRequestSettings.type = (HttpReqType)evt.newValue;
            });
        }

        private (string header, string value)[] GetHeaders()
        {
            return reqHeadersView.Query(null, "header-element").ToList().Select(v =>
            {
                var headerName = v.Q<TextField>(null, "header-name").value;
                var headerValue = v.Q<TextField>(null, "header-value").value;
                return (headerName, headerValue);
                Debug.Log($"MES: {headerName} : {headerValue}");
            }).ToArray();
        }
    }
}