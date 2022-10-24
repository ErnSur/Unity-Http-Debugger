using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    [Serializable]
    internal partial class RequestConsole
    {
        public event Action<RequestData> SelectionChanged;
        private ListWithEvents<RequestData> _source;
        private ListWithEvents<RequestData> _filteredSource = new ListWithEvents<RequestData>();

        [SerializeField]
        private bool clearOnPlay;

        [SerializeField]
        private string searchText;

        private ListWithEvents<RequestData> Source => string.IsNullOrWhiteSpace(searchText) ? _source : _filteredSource;


        public RequestConsole()
        {
            EditorApplication.playModeStateChanged += PlayModeChanged;
        }

        private void PlayModeChanged(PlayModeStateChange newState)
        {
            if (clearOnPlay && newState == PlayModeStateChange.EnteredPlayMode)
                ClearConsole();
        }

        private void ClearConsole()
        {
            _source.Clear();
            requestList.Rebuild();
        }

        public void Init(VisualElement root)
        {
            AssignQueryResults(root);
            InitColumns();
            InitClearButton();
            InitSearchField();
#if UNITY_2022_2_OR_NEWER
            requestList.selectionChanged += items =>
#else
            requestList.onSelectionChange += items =>
#endif
            {
                SelectionChanged?.Invoke(items.FirstOrDefault() as RequestData);
            };
            HttpClientLoggerEditorWrapper.ExchangeLogged += _ =>
            {
                var autoScroll = false;
                var scroller = requestList.Q<ScrollView>().verticalScroller;
                if (scroller.highValue - scroller.value < requestList.fixedItemHeight || scroller.highValue == 0)
                    autoScroll = true;

                requestList.Rebuild();
                if (autoScroll)
                    requestList.ScrollToItem(-1);
            };
        }

        private void InitSearchField()
        {
            searchField.tooltip =
                "Search filters:\n\"res:\" by status code\n\"met:\" by method\n\"id:\" by id\n\"url:\" by url";
            searchField.value = searchText;
            searchField.RegisterValueChangedCallback(evt =>
            {
                searchText = evt.newValue;
                FilterAndRefresh();
            });
        }

        private void FilterSource(string searchText)
        {
            this.searchText = searchText;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredSource.Clear();
                return;
            }

            _filteredSource = new ListWithEvents<RequestData>(FilterInterpreter.Filter(_source, this.searchText));
        }

        private void FilterAndRefresh()
        {
            FilterSource(searchText);
            requestList.itemsSource = Source;
            requestList.Rebuild();
        }

        private void InitClearButton()
        {
            SetupDropdownMenu();
            clearButton.clicked += ClearConsole;
        }

        private void SetupDropdownMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Clear on Play"), clearOnPlay, () =>
            {
                clearOnPlay = !clearOnPlay;
                SetupDropdownMenu();
            });
            clearButton.DropdownMenu = menu;
        }

        private void InitColumns()
        {
            timeCol.makeCell = () => new Label();
            timeCol.bindCell = (element, i) =>
            {
                ((Label)element).text = ((ConsoleRequestData)Source[i]).timestamp.ToString();
            };

            resultCol.makeCell = () => new ResultCell();
            resultCol.bindCell = (element, i) =>
            {
                ((ResultCell)element).Setup(Source[i].lastResponse?.statusCode ?? 0);
                AddContextMenu(element, i);
            };

            methodCol.makeCell = () => new MethodCell();
            methodCol.bindCell = (element, i) =>
            {
                ((MethodCell)element).Setup(Source[i].type.ToString());
                AddContextMenu(element, i);
            };

            idCol.makeCell = () => new IdCell();
            idCol.bindCell = (element, i) =>
            {
                var id = Source[i].name;
                ((IdCell)element).Setup(id);
                AddContextMenu(element, i);
            };

            urlCol.makeCell = () => new UrlCell();
            urlCol.bindCell = (element, i) =>
            {
                ((UrlCell)element).Setup(Source[i].url);
                AddContextMenu(element, i);
            };
        }

        private void AddContextMenu(VisualElement element, int i)
        {
            element.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Save To Stash",
                    _ =>
                    {
                        StashDatabase.instance.requests.Add(RequestData.Create(Source[i]));
                        RequestStashWindow.Open();
                    });
                evt.menu.AppendAction("Save To Assets",
                    _ =>
                    {
                        var copy = RequestData.Create(Source[i]);
                        ProjectWindowUtil.CreateAsset(copy, $"Assets/{copy.name}.asset");
                    });
            }));
        }


        public void Setup(ListWithEvents<RequestData> itemSource)
        {
            _source = itemSource;
            FilterAndRefresh();
        }
    }
}