using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    [Serializable]
    internal partial class RequestConsole
    {
        public event Action<RequestData> SelectionChanged;
        private RequestList _source;
        private RequestList _filteredSource = new RequestList();

        [SerializeField]
        private bool clearOnPlay;

        [SerializeField]
        private string searchText;

        private RequestList Source => string.IsNullOrWhiteSpace(searchText) ? _source : _filteredSource;


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

            requestList.selectionChanged += items => { SelectionChanged?.Invoke(items.FirstOrDefault() as RequestData); };
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

            _filteredSource = new RequestList(FilterInterpreter.Filter(_source, this.searchText));
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
            resultCol.makeCell = () => new StatusCodeCell();
            resultCol.bindCell = (element, i) =>
            {
                ((StatusCodeCell)element).Setup(Source[i].lastResponse?.statusCode ?? 0);
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
                ((IdCell)element).Setup(Source[i].name);
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
                    _ => { StashDatabase.instance.requests.Add(RequestData.Create(Source[i])); });
            }));
        }


        public void Setup(RequestList itemSource)
        {
            _source = itemSource;
            FilterAndRefresh();
        }
    }
}