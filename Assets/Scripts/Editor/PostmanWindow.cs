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
    
    //TODO: 
    // like in insomia add dropdown button to each list element that will have delete and duplicate options
    // you dont need to use icons for status respones, use background colors, you can also display error codes in tooltips
    public class PostmanWindow : EditorWindow
    {
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

        #region Fields

        [Q("main-panel")]
        private VisualElement mainPanel;

        [Q("sidebar")]
        private VisualElement sidebar;

        [Q("editor-tab-toggle")]
        private ToolbarToggle editorTabToggle;

        [Q("playmode-tab-toggle")]
        private ToolbarToggle playmodeTabToggle;

        [Q("req-list-create-button")]
        private ToolbarButton reqListCreateButton;

        [Q("req-list-search")]
        private ToolbarSearchField reqListSearch;

        [Q("req-list")]
        private ListView reqList;

        [Q("exchange-view")]
        private VisualElement exchangeView;

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
        private QuickEye.RequestWatcher.CodeField reqBodyField;

        [Q("req-headers-view")]
        private VisualElement reqHeadersView;

        [Q("res-panel")]
        private VisualElement resPanel;

        [Q("res-status-label")]
        private Label resStatusLabel;

        [Q("res-body-field")]
        private QuickEye.RequestWatcher.CodeField resBodyField;

        [Q("no-select-view")]
        private VisualElement noSelectView;

        #endregion

        private HttpExchange SelectedExchange =>
            reqList.selectedIndex < 0 ? null : data.requests[reqList.selectedIndex];

        private const string PrefsKey = "postmanData";

        [SerializeField]
        private PostmanData data;

        [SerializeField]
        private PostmanData playmodeData;

        [SerializeField]
        private bool editorTabOpened;

        private PostmanData Datasource => editorTabOpened ? data : playmodeData;
        private string DatasourcePropName => editorTabOpened ? nameof(data) : nameof(playmodeData);

        private SerializedProperty DatasourceRequestsProp =>
            editorTabOpened ? editorRequestsProp : playmodeRequestsProp;

        private SerializedProperty editorRequestsProp;
        private SerializedProperty playmodeRequestsProp;

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
            HttpClientLoggerEditorWrapper.ExchangeLogged += RefreshPlaymodeView;
            LoadData();
        }

        private void OnDisable()
        {
            HttpClientLoggerEditorWrapper.ExchangeLogged -= RefreshPlaymodeView;
            SaveData();
        }


        private void RefreshPlaymodeView(HttpExchange data)
        {
        }


        public void CreateGUI()
        {
            try
            {
                // Each editor window contains a root VisualElement object
                var root = rootVisualElement;
                RefreshSerializedObj();

                // Import UXML
                var visualTree =
                    AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/PostmanWindow.uxml");
                visualTree.CloneTree(root);
                root.AssignQueryResults(this);
                InitSidebarTabs();
                InitSidebar();
                InitReqSendButton();
                RefreshReqView();
                Bind();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void Bind()
        {
            rootVisualElement.Bind(serializedObject);
        }

        private void RefreshSerializedObj()
        {
            serializedObject = new SerializedObject(this);
            editorRequestsProp =
                serializedObject.FindProperty($"{nameof(data)}.{nameof(PostmanData.requests)}");
            playmodeRequestsProp =
                serializedObject.FindProperty($"{nameof(playmodeData)}.{nameof(PostmanData.requests)}");
        }

        private void InitSidebarTabs()
        {
            editorTabToggle.RegisterThisValueChangedCallback(evt =>
            {
                playmodeTabToggle.SetValueWithoutNotify(!evt.newValue);
                editorTabOpened = true;
                Refresh();
            });
            playmodeTabToggle.RegisterThisValueChangedCallback(evt =>
            {
                editorTabToggle.SetValueWithoutNotify(!evt.newValue);
                editorTabOpened = false;
                Refresh();
            });
            editorTabToggle.value = editorTabOpened;

            void Refresh()
            {
                reqList.itemsSource = Datasource.requests;
                reqList.Refresh();
                RefreshReqView();
            }
        }

        private void UpdateSelectedView()
        {
            var hasSelection = reqList.selectedIndex != -1;
            noSelectView.ToggleDisplayStyle(!hasSelection);
            exchangeView.ToggleDisplayStyle(hasSelection);
        }

        private void RefreshReqView()
        {
            Debug.Log($"Refresh Req View");
            UpdateSelectedView();
            var propName =
                //DatasourceRequestsProp[reqList.selectedIndex].propertyPath;
                GetRequestPropName(reqList.selectedIndex);
            reqTypeMenu.bindingPath = $"{propName}.{nameof(HttpExchange.type)}";
            reqUrlField.bindingPath = $"{propName}.url";
            reqBodyField.bindingPath = $"{propName}.body";

            var code = SelectedExchange?.lastResponse.statusCode ?? 0;
            resStatusLabel.text = $"({(int)code}) {code}";
            resBodyField.bindingPath = $"{propName}.lastResponse.payload";

            rootVisualElement.Unbind();
            rootVisualElement.Bind(serializedObject);
        }

        private void InitSidebar()
        {
            reqListCreateButton.Clicked(() =>
            {
                DatasourceRequestsProp.InsertArrayElementAtIndex(DatasourceRequestsProp.arraySize);
                serializedObject.ApplyModifiedProperties();
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
                Debug.Log($"Create element");
                var ve = new VisualElement();

                ve.Class("sidebar-req-el");
                ve.Add(new Label().Class("sidebar-req-el-type").BindingPath("type"));
                var renameField = new EditableLabel().Class("sidebar-req-el-name");
                ve.Add(renameField);
                return ve;
            };
            reqList.bindItem = (ve, index) =>
            {
                Debug.Log($"Bind element {index}");
                var propName = GetRequestPropName(index);
                var typeProp = serializedObject.FindProperty($"{propName}.type");
                var nameProp = serializedObject.FindProperty($"{propName}.name");
                var typeLabel = ve.Q<Label>(null, "sidebar-req-el-type");
                typeLabel.BindProperty(typeProp);
                var renameLabel = ve.Q<EditableLabel>(null, "sidebar-req-el-name");
                renameLabel.BindProperty(nameProp);
            };
            reqList.onSelectionChanged += list => RefreshReqView();
            reqList.itemsSource = Datasource?.requests;
            //reqList.selectedIndex = 0;
        }

        private void InitReqSendButton()
        {
            reqSendButton.clicked += async () =>
            {
                resStatusLabel.text = "Status: Loading...";
                await SelectedExchange.SendAsync();
                RefreshReqView();
            };
        }

        private string GetRequestPropName(int index) =>
            $"{DatasourcePropName}.requests.Array.data[{index}]";

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