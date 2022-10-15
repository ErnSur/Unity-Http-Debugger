using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal partial class RequestStash
    {
        public event Action<RequestData> SelectionChanged;

        private readonly VisualElement _root;
        private RequestList _requestList;
        private RequestList _searchResult;

        private string searchText;

        private RequestList Source => string.IsNullOrWhiteSpace(searchText) ? _requestList : _searchResult;

        private RequestData SelectedReq => (RequestData)stashList.selectedItem;

        public RequestStash(VisualElement root)
        {
            _root = root;
            AssignQueryResults(root);
            InitList();
            InitSearchField();
        }

        public void Setup(RequestList requestList)
        {
            UnregisterListEvents(_requestList);
            _requestList = requestList;
            RegisterListEvents(_requestList);

            SetupList(requestList);
        }

        private void RegisterListEvents(RequestList requestList)
        {
            requestList.Added += OnRequestListModification;
            requestList.Removed += OnRequestListModification;
            requestList.AfterClear += stashList.Rebuild;
        }

        private void UnregisterListEvents(RequestList requestList)
        {
            if (requestList == null)
                return;
            requestList.Added -= OnRequestListModification;
            requestList.Removed -= OnRequestListModification;
            requestList.AfterClear -= stashList.Rebuild;
        }

        private void OnRequestListModification(RequestData obj)
        {
            stashList.Rebuild();
        }
        
        private void SetupList(IList propList)
        {
            stashList.itemsSource = propList;
            stashList.Rebuild();
        }

        private void InitSearchField()
        {
            searchField.RegisterValueChangedCallback(evt =>
            {
                if (string.IsNullOrWhiteSpace(evt.newValue))
                {
                    SetupList(_requestList);
                    return;
                }

                _searchResult = new RequestList(from req in _requestList
                    let name = req.name
                    where name.ToLower().Contains(evt.newValue.ToLower())
                    select req);

                SetupList(_searchResult);
            });
        }

        private void InitList()
        {
            stashList.makeItem = () => new RequestButtonBig();
            stashList.bindItem = (ve, index) =>
            {
                var serObj = new SerializedObject(Source[index]);
                var typeProp = serObj.FindProperty(nameof(RequestData.type));
                var nameProp = serObj.FindProperty(RequestData.NamePropertyName);
                var button = ve.As<RequestButtonBig>();
                button.BindProperties(typeProp, nameProp);
                button.Deleted = () =>
                {
                    _requestList.RemoveAt(index);
                    stashList.Rebuild();
                };
                button.Duplicated = () =>
                {
                    _requestList.Insert(index, RequestData.Create(Source[index]));
                    stashList.Rebuild();
                };
            };
            stashList.selectionChanged += selection =>
            {
                SelectionChanged?.Invoke((RequestData)selection.FirstOrDefault());
            };

            createButton.clicked += () =>
            {
                var newRequest = RequestData.Create();
                newRequest.name = "New Request";
                newRequest.type = HttpMethodType.Get;
                newRequest.body = "{ }";
                
                _requestList.Add(newRequest);
                stashList.Rebuild();
            };
        }
    }
}