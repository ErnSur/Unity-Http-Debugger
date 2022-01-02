using System.Collections.Generic;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal class StashView : VisualElement,IRequestListView
    {
        [Q("sidebar")]
        private VisualElement sidebar;
        [Q("stash-create--button")]
        private ToolbarButton stashCreateButton;
        [Q("stash--search-field")]
        private ToolbarSearchField stashSearchField;
        [Q("stash--list")]
        private ListView stashList;
        [Q("exchange-view")]
        private QuickEye.RequestWatcher.ExchangeView exchangeView;

        private SerializedProperty requestListProp;

        private SerializedProperty SelectedReq =>
            stashList.selectedIndex < 0 ? null : requestListProp.GetArrayElementAtIndex(stashList.selectedIndex);
        
        public StashView()
        {
            this.InitFromUxml();
            InitSidebar();
            RefreshReqView();
        }

        public void Init(SerializedProperty requestListProp, List<HDRequest> requests)
        {
            this.requestListProp = requestListProp;
            stashList.itemsSource = requests;
            this.Bind(requestListProp.serializedObject);

            stashList.Refresh();
            RefreshReqView();
        }

        private void InitSidebar()
        {
            stashCreateButton.Clicked(() =>
            {
                requestListProp.InsertArrayElementAtIndex(requestListProp.arraySize);
                requestListProp.serializedObject.ApplyModifiedProperties();
                stashList.Refresh();
            });
            InitList();
        }

        private void InitList()
        {
            stashList.makeItem = () => new RequestButtonBig();
            stashList.bindItem = (ve, index) =>
            {
                var reqProp = requestListProp.GetArrayElementAtIndex(index);
                var typeProp = reqProp.FindPropertyRelative(nameof(HDRequest.type));
                var nameProp = reqProp.FindPropertyRelative(nameof(HDRequest.name));
                var button = ve.As<RequestButtonBig>();
                button.BindProperties(typeProp, nameProp);
                button.Deleted = () =>
                {
                    requestListProp.DeleteArrayElementAtIndex(index);
                    requestListProp.serializedObject.ApplyModifiedProperties();
                    stashList.Refresh();
                    RefreshReqView();
                };
                button.Duplicated = () =>
                {
                    requestListProp.InsertArrayElementAtIndex(index);
                    requestListProp.serializedObject.ApplyModifiedProperties();
                    stashList.Refresh();
                    RefreshReqView();
                };
            };
            stashList.onSelectionChanged += list => RefreshReqView();
        }

        public void RefreshReqView()
        {
            exchangeView.SetupView(SelectedReq);
        }
        private class UxmlFactory : UxmlFactory<StashView>{}

        public int GetSelectedIndex()
        {
            return stashList.selectedIndex;
        }
    }
}