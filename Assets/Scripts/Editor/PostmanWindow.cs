using System;
using System.Linq;
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
        [Q("sidebar-editor")]
        private VisualElement sidebarEditor;
        [Q("editor-create-button")]
        private ToolbarButton editorCreateButton;
        [Q("editor-searchField")]
        private ToolbarSearchField editorSearchField;
        [Q("editor-list")]
        private ListView editorList;
        [Q("sidebar-playmode")]
        private VisualElement sidebarPlaymode;
        [Q("playmode-clear-button")]
        private ToolbarButton playmodeClearButton;
        [Q("playmode-searchField")]
        private ToolbarSearchField playmodeSearchField;
        [Q("playmode-list")]
        private ListView playmodeList;
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
            ActiveListView.selectedIndex < 0 ? null : Datasource.requests[ActiveListView.selectedIndex];

        private const string PrefsKey = "postmanData";

        [SerializeField]
        private PostmanData editorData;

        [SerializeField]
        private PostmanData playmodeData;

        [SerializeField]
        private bool editorTabOpened;

        private ListView ActiveListView => editorTabOpened ? editorList : playmodeList;
        
        private PostmanData Datasource => editorTabOpened ? editorData : playmodeData;
        private string DatasourcePropName => editorTabOpened ? nameof(editorData) : nameof(playmodeData);

        private SerializedProperty DatasourceRequestsProp =>
            editorTabOpened ? editorRequestsProp : playmodeRequestsProp;

        private SerializedProperty editorRequestsProp;
        private SerializedProperty playmodeRequestsProp;

        private SerializedObject serializedObject;

        private void LoadData()
        {
            var json = EditorPrefs.GetString(PrefsKey, JsonUtility.ToJson(new PostmanData()));
            editorData = JsonUtility.FromJson<PostmanData>(json);
        }

        private void SaveData()
        {
            EditorPrefs.SetString(PrefsKey, JsonUtility.ToJson(editorData));
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
                serializedObject.FindProperty($"{nameof(editorData)}.{nameof(PostmanData.requests)}");
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
                ActiveListView.Refresh();
                RefreshReqView();
            }
        }

        private void UpdateSelectedView()
        {
            var hasSelection = ActiveListView.selectedIndex != -1;
            noSelectView.ToggleDisplayStyle(!hasSelection);
            exchangeView.ToggleDisplayStyle(hasSelection);

            sidebarEditor.ToggleDisplayStyle(editorTabOpened);
            sidebarPlaymode.ToggleDisplayStyle(!editorTabOpened);
        }

        private void RefreshReqView()
        {
            Debug.Log($"Refresh Req View");
            UpdateSelectedView();
            var propName =
                //DatasourceRequestsProp[reqList.selectedIndex].propertyPath;
                GetRequestPropName(ActiveListView.selectedIndex);
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
            editorCreateButton.Clicked(() =>
            {
                DatasourceRequestsProp.InsertArrayElementAtIndex(DatasourceRequestsProp.arraySize);
                serializedObject.ApplyModifiedProperties();
                editorList.Refresh();
            });
            InitEditorList();
            InitPlaymodeList();
        }

        private void InitEditorList()
        {
            editorList.makeItem = () => new RequestButtonBig();
            editorList.bindItem = (ve, index) =>
            {
                var propName = GetRequestPropName(index);
                var typeProp = serializedObject.FindProperty($"{propName}.type");
                var nameProp = serializedObject.FindProperty($"{propName}.name");
                var button = ve.As<RequestButtonBig>();
                button.BindProperties(typeProp,nameProp);
                button.Deleted = () =>
                {
                    DatasourceRequestsProp.DeleteArrayElementAtIndex(index);
                    serializedObject.ApplyModifiedProperties();
                    editorList.Refresh();
                    RefreshReqView();
                };
                button.Duplicated = () =>
                {
                    DatasourceRequestsProp.InsertArrayElementAtIndex(index);
                    serializedObject.ApplyModifiedProperties();
                    editorList.Refresh();
                    RefreshReqView();
                };
            };
            editorList.onSelectionChanged += list => RefreshReqView();
            editorList.itemsSource = editorData?.requests;
        }
        
        private void InitPlaymodeList()
        {
            playmodeList.makeItem = () => new RequestButtonSmall();
            playmodeList.bindItem = (ve, index) =>
            {
                var propName = GetRequestPropName(index);
                var typeProp = serializedObject.FindProperty($"{propName}.type");
                var nameProp = serializedObject.FindProperty($"{propName}.name");
                var button = ve.As<RequestButtonSmall>();
                button.BindProperties(typeProp,nameProp);
            };
            playmodeList.onSelectionChanged += list => RefreshReqView();
            playmodeList.itemsSource = playmodeData?.requests;

            playmodeClearButton.Clicked(() =>
            {
                playmodeRequestsProp.ClearArray();
                serializedObject.ApplyModifiedProperties();
                playmodeList.Refresh();
                RefreshReqView();
            });
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
    }

    public static class FluentVisualElementExtensions
    {
        public static T As<T>(this VisualElement ve) where T : VisualElement
        {
            return (T)ve;
        }
    }
}