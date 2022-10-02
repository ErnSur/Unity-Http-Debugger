using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal class RequestFilter
    {
        public string result;
        public string method;
        public string id;
        public string url;

        public RequestFilter(string filterString)
        {
            
        }
        
    }
    [Serializable]
    internal partial class RequestConsole
    {
        private List<HDRequest> _source;
        private List<HDRequest> _filteredSource = new List<HDRequest>();

        [SerializeField]
        private bool clearOnPlay;

        [SerializeField]
        private string _searchText;

        private List<HDRequest> Source => string.IsNullOrWhiteSpace(_searchText) ? _source : _filteredSource;


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

            requestList.selectionChanged += selection => { ExchangeInspectorWindow.Open(requestList.selectedIndex); };

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
            searchField.tooltip = "Search filters:\n\"res:\" search by status code\n\"met:\" search by method\n\"id:\" search by id";
            searchField.value = _searchText;
            searchField.RegisterValueChangedCallback(evt =>
            {
                _searchText = evt.newValue;
                FilterAndRefresh();
            });
        }

        private void FilterSource(string searchText)
        {
            _searchText = searchText;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredSource.Clear();
                return;
            }

            _filteredSource = FilterInterpreter.Filter(_source,_searchText).ToList();
        }

        private void FilterAndRefresh()
        {
            FilterSource(_searchText);
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
                ((StatusCodeCell)element).Setup(Source[i].lastResponse?.statusCode ?? 0);

            methodCol.makeCell = () => new MethodCell();
            methodCol.bindCell = (element, i) => ((MethodCell)element).Setup(Source[i].type.ToString());

            idCol.makeCell = () => new IdCell();
            idCol.bindCell = (element, i) => ((IdCell)element).Setup(Source[i].name);

            urlCol.makeCell = () => new UrlCell();
            urlCol.bindCell = (element, i) => ((UrlCell)element).Setup(Source[i].url);
        }

        public void Setup(List<HDRequest> itemSource)
        {
            _source = itemSource;
            FilterAndRefresh();
        }
    }
}