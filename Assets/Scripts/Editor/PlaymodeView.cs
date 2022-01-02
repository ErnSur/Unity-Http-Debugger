using System.Collections.Generic;
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

        private SerializedProperty SelectedReq =>
            playmodeList.selectedIndex < 0 ? null : requestListProp.GetArrayElementAtIndex(playmodeList.selectedIndex);

        public PlaymodeView()
        {
            this.InitFromUxml();
            InitSidebar();
            RefreshReqView();
        }

        public void Init(SerializedProperty requestListProp, List<HDRequest> requests)
        {
            this.requestListProp = requestListProp;
            playmodeList.itemsSource = requests;
            playmodeList.Refresh();
            RefreshReqView();
        }

        private void InitSidebar()
        {
            playmodeClearButton.Clicked(() =>
            {
                requestListProp.InsertArrayElementAtIndex(requestListProp.arraySize);
                requestListProp.serializedObject.ApplyModifiedProperties();
                playmodeList.Refresh();
            });
            InitList();
        }

        private void InitList()
        {
            playmodeList.makeItem = () => new RequestButtonSmall();
            playmodeList.bindItem = (ve, index) =>
            {
                var reqProp = requestListProp.GetArrayElementAtIndex(index);
                var typeProp = reqProp.FindPropertyRelative(nameof(HDRequest.type));
                var nameProp = reqProp.FindPropertyRelative(nameof(HDRequest.name));
                var button = ve.As<RequestButtonSmall>();
                button.BindProperties(typeProp, nameProp);
            };

            playmodeClearButton.Clicked(() =>
            {
                requestListProp.ClearArray();
                requestListProp.serializedObject.ApplyModifiedProperties();
                playmodeList.Refresh();
                RefreshReqView();
            });
            playmodeList.onSelectionChanged += list => RefreshReqView();
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
            return playmodeList.selectedIndex;
        }
    }
}