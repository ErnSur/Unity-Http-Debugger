using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    [Serializable]
    internal partial class RequestConsole
    {
        private List<HDRequest> _source;
        [SerializeField]
        private bool clearOnPlay;

        public RequestConsole()
        {
            EditorApplication.playModeStateChanged += PlayModeChanged;
        }

        private void PlayModeChanged(PlayModeStateChange newState)
        {
            if(clearOnPlay && newState == PlayModeStateChange.EnteredPlayMode)
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
                ((StatusCodeCell)element).Setup(_source[i].lastResponse?.statusCode ?? 0);

            methodCol.makeCell = () => new MethodCell();
            methodCol.bindCell = (element, i) => ((MethodCell)element).Setup(_source[i].type.ToString());
            
            idCol.makeCell = () => new IdCell();
            idCol.bindCell = (element, i) => ((IdCell)element).Setup(_source[i].name);

            urlCol.makeCell = () => new UrlCell();
            urlCol.bindCell = (element, i) => ((UrlCell)element).Setup(_source[i].url);
        }

        public void Setup(List<HDRequest> itemSource)
        {
            requestList.itemsSource = _source = itemSource;
            requestList.Rebuild();
        }
    }
}