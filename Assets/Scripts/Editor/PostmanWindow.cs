using System;
using System.Linq;
using ArteHacker.UITKEditorAid;
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

        [MenuItem("Test/PostmanWindow #&p")]
        public static void Open()
        {
            EditorFullscreenUtility.ToggleEditorFullscreen<PostmanWindow>();
        }

        [MenuItem("Test/UI Builder Toggle Fullscreen #&b")]
        public static void ToggleUIBuilderFullScreen()
        {
            EditorFullscreenUtility.ToggleEditorFullscreen(
                Type.GetType("Unity.UI.Builder.Builder, UnityEditor.UIBuilderModule"));
        }

        [Q("main-panel")]
        private VisualElement mainPanel;
        [Q("sidebar")]
        private VisualElement sidebar;
        [Q("req-list-create-button")]
        private ToolbarButton reqListCreateButton;
        [Q("req-list-search")]
        private ToolbarSearchField reqListSearch;
        [Q("req-list")]
        private ListView reqList;
        [Q("req-panel")]
        private VisualElement reqPanel;
        [Q("req-type-menu")]
        private EnumField reqTypeMenu;
        [Q("req-url-field")]
        private TextField reqUrlField;
        [Q("req-send-button")]
        private ToolbarButton reqSendButton;
        [Q("req-json-tab")]
        private ToolbarToggle reqJsonTab;
        [Q("req-headers-tab")]
        private ToolbarToggle reqHeadersTab;
        [Q("req-body-field")]
        private CodeField reqBodyField;
        [Q("req-headers-view")]
        private VisualElement reqHeadersView;
        [Q("res-panel")]
        private VisualElement resPanel;
        [Q("res-status-label")]
        private Label resStatusLabel;
        [Q("res-body-field")]
        private CodeField resBodyField;


        private HttpReq SelectedReq => data.requests[reqList.selectedIndex];

        private const string PrefsKey = "postmanData";

        [SerializeField]
        private PostmanData data;

        private SerializedObject serializedObject;

        private void LoadData()
        {
            var json = EditorPrefs.GetString(PrefsKey, JsonUtility.ToJson(new PostmanData()));
            data = JsonUtility.FromJson<PostmanData>(json);
        }

        private void SaveData()
        {
            EditorPrefs.SetString(PrefsKey, JsonUtility.ToJson(data));
        }

        private void OnEnable()
        {
            LoadData();
            Debug.Log($"OnEnable {serializedObject}");
        }

        private void OnDisable()
        {
            SaveData();
        }

        public void CreateGUI()
        {
            Debug.Log($"OnCreateGUI: {serializedObject}");
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;
            
            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/PostmanWindow.uxml");
            visualTree.CloneTree(root);
            root.AssignQueryResults(this);
            InitSidebar();
            InitReqSendButton();
            RefreshReqView();
            serializedObject = new SerializedObject(this);
            rootVisualElement.Bind(serializedObject);
        }

        private void RefreshReqView()
        {
            Debug.Log($"Refresh Req View");
            var i = reqList.selectedIndex;
            reqTypeMenu.bindingPath = $"data.requests.Array.data[{i}].type";
            reqUrlField.bindingPath = $"data.requests.Array.data[{i}].url";
            reqBodyField.bindingPath = $"data.requests.Array.data[{i}].body";

            var code = SelectedReq?.lastResponse.statusCode ?? 0;
            resStatusLabel.text = $"({(int) code}) {code}";
            resBodyField.bindingPath = $"data.requests.Array.data[{i}].lastResponse.payload";
            
            rootVisualElement.Unbind();
            rootVisualElement.Bind(serializedObject);
        }

        private void InitSidebar()
        {
            reqListCreateButton.Clicked(() =>
            {
                data.requests.Add(new HttpReq());
                reqList.Refresh();
            });
            InitRequestList();
        }

        private void InitRequestList()
        {
            var elementPrefab = reqList.Q(null, "sidebar-req-el");
            elementPrefab.ToggleDisplayStyle(false);
            reqList.itemHeight = 36;
            reqList.makeItem = () =>
            {
                var ve = new VisualElement();

                ve.Class("sidebar-req-el");
                ve.Add(new Label().Class("sidebar-req-el-type").BindingPath("type"));
                var renameField = new EditableLabel().Class("sidebar-req-el-name");
                ve.Add(renameField);
                return ve;
            };
            reqList.bindItem = (ve, index) =>
            {
                var typeProp = serializedObject.FindProperty($"data.requests.Array.data[{index}].type");
                var nameProp = serializedObject.FindProperty($"data.requests.Array.data[{index}].name");
                var typeLabel = ve.Q<Label>(null, "sidebar-req-el-type");
                typeLabel.BindProperty(typeProp);
                var renameLabel = ve.Q<EditableLabel>(null, "sidebar-req-el-name");
                renameLabel.BindProperty(nameProp);
            };
            reqList.onSelectionChanged += list => RefreshReqView();
            reqList.itemsSource = data.requests;
            reqList.selectedIndex = 0;
        }

        private void InitReqSendButton()
        {
            reqSendButton.clicked += async () =>
            {
                resStatusLabel.text = "Status: Loading...";
                await SelectedReq.SendAsync();
                RefreshReqView();
            };
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