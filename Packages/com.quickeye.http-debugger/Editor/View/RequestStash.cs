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
        public event Action<SerializedProperty> SelectionChanged;

        private readonly VisualElement _root;
        private SerializedProperty _requestListProp;
        private SerializedArrayProperty _requestListPropList;
        private List<SerializedProperty> _searchResult;

        private SerializedProperty SelectedReq => stashList.selectedItem as SerializedProperty;

        public RequestStash(VisualElement root)
        {
            _root = root;
            AssignQueryResults(root);
            InitList();
            InitSearchField();
        }

        public void Setup(SerializedProperty requestListProp)
        {
            _requestListProp = requestListProp;
            _requestListPropList = new SerializedArrayProperty(requestListProp);
            Setup(_requestListPropList);
        }

        private void Setup(IList propList)
        {
            stashList.itemsSource = propList;
            _root.Bind(_requestListProp.serializedObject);

            stashList.Rebuild();
        }

        private void InitSearchField()
        {
            searchField.RegisterValueChangedCallback(evt =>
            {
                if (string.IsNullOrWhiteSpace(evt.newValue))
                {
                    Setup(_requestListPropList);
                    return;
                }

                _searchResult = (from prop in _requestListPropList
                    let name = prop.FindPropertyRelative(nameof(HDRequest.name)).stringValue
                    where name.ToLower().Contains(evt.newValue.ToLower())
                    select prop).ToList();

                Setup(_searchResult);
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
                    _requestListProp.DeleteArrayElementAtIndex(index);
                    _requestListProp.serializedObject.ApplyModifiedProperties();
                    stashList.Rebuild();
                    //RefreshReqView();
                };
                button.Duplicated = () =>
                {
                    _requestListProp.InsertArrayElementAtIndex(index);
                    _requestListProp.serializedObject.ApplyModifiedProperties();
                    stashList.Rebuild();
                    //RefreshReqView();
                };
            };
            stashList.selectionChanged += selection =>
            {
                SelectionChanged?.Invoke(((SerializedProperty)selection.FirstOrDefault()));
                //RefreshReqView();
            };

            createButton.clicked += () =>
            {
                _requestListProp.InsertArrayElementAtIndex(_requestListProp.arraySize);
                _requestListProp.serializedObject.ApplyModifiedProperties();
                stashList.Rebuild();
            };
        }
    }
}