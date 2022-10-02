using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal partial class RequestConsole
    {
        private List<HDRequest> _source;

        public RequestConsole(VisualElement root)
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
            clearButton.clicked += () =>
            {
                _source.Clear();
                //     requestListProp?.serializedObject.ApplyModifiedProperties();
                requestList.Rebuild();
                //     RefreshReqView();
            };
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