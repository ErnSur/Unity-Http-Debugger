using System;
using System.Collections;
using System.Collections.Generic;
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
        public event Action<IEnumerable<HDRequest>> ItemsChosen;
        private RequestList _source;
        private RequestList _filteredSource = new RequestList();

        [SerializeField]
        private bool clearOnPlay;

        [SerializeField]
        private string _searchText;

        private RequestList Source => string.IsNullOrWhiteSpace(_searchText) ? _source : _filteredSource;


        public RequestConsole()
        {
            EditorApplication.playModeStateChanged += PlayModeChanged;
        }

        private void OpenRequestScript()
        {
            InternalEditorUtility.OpenFileAtLineExternal("path", 1);
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

            requestList.itemsChosen += items => { ItemsChosen?.Invoke(items.Cast<HDRequest>()); };
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

            _filteredSource = new RequestList(FilterInterpreter.Filter(_source, _searchText));
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
            idCol.bindCell = (element, i) => ((IdCell)element).Setup(Source[i].id);

            urlCol.makeCell = () => new UrlCell();
            urlCol.bindCell = (element, i) => ((UrlCell)element).Setup(Source[i].url);
        }

        public void Setup(RequestList itemSource)
        {
            _source = itemSource;
            FilterAndRefresh();
        }
    }
}