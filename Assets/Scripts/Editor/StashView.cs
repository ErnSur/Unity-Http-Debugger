using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private SerializedArrayProperty requestListPropList;

        private SerializedProperty SelectedReq =>stashList.selectedItem as SerializedProperty;
        private List<SerializedProperty> searchResult;

        public StashView()
        {
            this.InitFromUxml();
            InitList();
            InitSearchField();
            RefreshReqView();
        }
        
        private void InitSearchField()
        {
            stashSearchField.RegisterValueChangedCallback(evt =>
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
            stashList.itemsSource = propList;
            this.Bind(requestListProp.serializedObject);

            stashList.Refresh();
            RefreshReqView();
        }

        private void InitList()
        {
            stashList.makeItem = () => new RequestButtonBig();
            stashList.bindItem = (ve, index) =>
            {
                var reqProp = (SerializedProperty)stashList.itemsSource[index];
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
            
            stashCreateButton.Clicked(() =>
            {
                var prop = SelectedReq;
                Debug.Log($"Array: {requestListProp.propertyPath}");
                Debug.Log($"Elelemt: {prop.propertyPath}");
                
                return;
                
                requestListProp.InsertArrayElementAtIndex(requestListProp.arraySize);
                requestListProp.serializedObject.ApplyModifiedProperties();
                stashList.Refresh();
            });
        }

        public void RefreshReqView()
        {
            exchangeView.SetupView(SelectedReq);
        }
        private class UxmlFactory : UxmlFactory<StashView>{}

        public int GetSelectedIndex()
        {
            return SelectedReq != null ? requestListPropList.IndexOf(SelectedReq) : -1;
        }
    }
}