using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal class PlaymodeView : VisualElement, IRequestListView
    {
        [Q("sidebar-playmode")]
        private VisualElement sidebarPlaymode;

        [Q("playmode-clear-button")]
        private ToolbarButton playmodeClearButton;

        [Q("playmode-searchField")]
        private ToolbarSearchField playmodeSearchField;

        [Q("playmode-list")]
        private ListView playmodeList;

        [Q("exchange-view")]
        private QuickEye.RequestWatcher.ExchangeView exchangeView;

        private SerializedProperty requestListProp;
        private SerializedArrayProperty requestListPropList;

        private SerializedProperty SelectedReq => playmodeList.selectedItem as SerializedProperty;

        private List<SerializedProperty> searchResult;

        public PlaymodeView()
        {
            this.InitResources();
            InitList();
            InitSearchField();
            RefreshReqView();
        }

        private void InitSearchField()
        {
            playmodeSearchField.RegisterValueChangedCallback(evt =>
            {
                if (string.IsNullOrWhiteSpace(evt.newValue))
                {
                    Setup(requestListPropList);
                    return;
                }

                searchResult = (from prop in requestListPropList
                    let name = prop.FindPropertyRelative("name").stringValue
                    where name.ToLower().Contains(evt.newValue.ToLower())
                    select prop).ToList();

                Setup(searchResult);
            });
        }

        public void Setup(SerializedProperty requestListProp)
        {
            this.requestListProp = requestListProp;
            requestListPropList = new SerializedArrayProperty(requestListProp);
            Setup(requestListPropList);
        }

        private void Setup(IList propList)
        {
            playmodeList.itemsSource = propList;
            this.Bind(requestListProp.serializedObject);

            playmodeList.Refresh();
            RefreshReqView();
        }

        private void InitList()
        {
            playmodeList.makeItem = () => new RequestButtonSmall();
            playmodeList.bindItem = (ve, index) =>
            {
                var reqProp = (SerializedProperty)playmodeList.itemsSource[index];
                ve.userData = requestListPropList.IndexOf(reqProp);
                var typeProp = reqProp.FindPropertyRelative(nameof(HDRequest.type));
                var nameProp = reqProp.FindPropertyRelative(nameof(HDRequest.name));
                var codeProp = reqProp.FindPropertyRelative(nameof(HDRequest.lastResponse))
                    .FindPropertyRelative(nameof(HDResponse.statusCode));
                var button = ve.As<RequestButtonSmall>();
                button.SetBindingPaths(typeProp.propertyPath,
                    nameProp.propertyPath,
                    codeProp.propertyPath);
                button.Unbind();
                button.Bind(requestListProp.serializedObject);
            };

            playmodeList.onSelectionChanged += _ => RefreshReqView();

            playmodeClearButton.Clicked(() =>
            {
                requestListProp?.ClearArray();
                requestListProp?.serializedObject.ApplyModifiedProperties();
                playmodeList.Refresh();
                RefreshReqView();
            });
        }

        public void Refresh()
        {
            playmodeList.Refresh();
            RefreshReqView();
        }

        private void RefreshReqView()
        {
            exchangeView.SetupView(SelectedReq);
        }

        private class UxmlFactory : UxmlFactory<PlaymodeView>
        {
        }

        public int GetSelectedIndex()
        {
            return SelectedReq != null ? requestListPropList.IndexOf(SelectedReq) : -1;
        }
    }
}