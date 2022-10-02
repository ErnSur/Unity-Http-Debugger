using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal partial class RequestStash
    {
        private VisualElement _root;
        public event Action<SerializedProperty> SelectionChanged;

        private SerializedProperty requestListProp;
        private SerializedArrayProperty requestListPropList;

        private SerializedProperty SelectedReq => stashList.selectedItem as SerializedProperty;
        private List<SerializedProperty> searchResult;

        public RequestStash(VisualElement root)
        {
            _root = root;
            AssignQueryResults(root);
            InitList();
            InitSearchField();
            
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
            _root.Bind(requestListProp.serializedObject);

            stashList.Rebuild();
            //RefreshReqView();
        }

        private void InitSearchField()
        {
            searchField.RegisterValueChangedCallback(evt =>
            {
                if (string.IsNullOrWhiteSpace(evt.newValue))
                {
                    Setup(requestListPropList);
                    return;
                }

                searchResult = (from prop in requestListPropList
                    let name = prop.FindPropertyRelative(nameof(HDRequest.name)).stringValue
                    where name.ToLower().Contains(evt.newValue.ToLower())
                    select prop).ToList();

                Setup(searchResult);
            });
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
                    stashList.Rebuild();
                    RefreshReqView();
                };
                button.Duplicated = () =>
                {
                    requestListProp.InsertArrayElementAtIndex(index);
                    requestListProp.serializedObject.ApplyModifiedProperties();
                    stashList.Rebuild();
                    RefreshReqView();
                };
            };
            stashList.selectionChanged += _ =>
            {
                Debug.Log($"selection changed");
                RefreshReqView();
            };

            createButton.clicked += () =>
            {
                requestListProp.InsertArrayElementAtIndex(requestListProp.arraySize);
                requestListProp.serializedObject.ApplyModifiedProperties();
                stashList.Rebuild();
            };
        }

        public void RefreshReqView()
        {
            ExchangeInspectorWindow.Open(SelectedReq);
        }

        public int GetSelectedIndex()
        {
            return SelectedReq != null ? requestListPropList.IndexOf(SelectedReq) : -1;
        }
    }
}