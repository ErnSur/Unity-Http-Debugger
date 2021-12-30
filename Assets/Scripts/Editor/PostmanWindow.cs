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
        private ListView stashList;
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
        private PostmanData stashData;

        [SerializeField]
        private PostmanData playmodeData;

        [SerializeField]
        private bool stashTabOpened;

        private ListView ActiveListView => stashTabOpened ? stashList : playmodeList;
        
        private PostmanData Datasource => stashTabOpened ? stashData : playmodeData;
        private string DatasourcePropName => stashTabOpened ? nameof(stashData) : nameof(playmodeData);

        private SerializedProperty DatasourceRequestsProp =>
            stashTabOpened ? stashRequestsProp : playmodeRequestsProp;

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
            stashRequestsProp =
                serializedObject.FindProperty($"{nameof(stashData)}.{nameof(PostmanData.requests)}");
            playmodeRequestsProp =
                serializedObject.FindProperty($"{nameof(playmodeData)}.{nameof(PostmanData.requests)}");
        }

        private void InitSidebarTabs()
        {
            editorTabToggle.RegisterThisValueChangedCallback(evt =>
            {
                playmodeTabToggle.SetValueWithoutNotify(!evt.newValue);
                stashTabOpened = true;
                Refresh();
            });
            playmodeTabToggle.RegisterThisValueChangedCallback(evt =>
            {
                editorTabToggle.SetValueWithoutNotify(!evt.newValue);
                stashTabOpened = false;
                Refresh();
            });
            editorTabToggle.value = stashTabOpened;

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

            sidebarEditor.ToggleDisplayStyle(stashTabOpened);
            sidebarPlaymode.ToggleDisplayStyle(!stashTabOpened);
        }

        private void RefreshReqView()
        {
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
                stashList.Refresh();
            });
            InitEditorList();
            InitPlaymodeList();
        }

        private void InitEditorList()
        {
            stashList.makeItem = () => new RequestButtonBig();
            stashList.bindItem = (ve, index) =>
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
                    stashList.Refresh();
                    RefreshReqView();
                };
                button.Duplicated = () =>
                {
                    DatasourceRequestsProp.InsertArrayElementAtIndex(index);
                    serializedObject.ApplyModifiedProperties();
                    stashList.Refresh();
                    RefreshReqView();
                };
            };
            stashList.onSelectionChanged += list => RefreshReqView();
            stashList.itemsSource = stashData?.requests;
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