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
        public event Action<HDRequest> SelectionChanged;

        private readonly VisualElement _root;
        private RequestList _requestList;
        private RequestList _searchResult;

        [SerializeField]
        private string searchText;

        private RequestList Source => string.IsNullOrWhiteSpace(searchText) ? _requestList : _searchResult;

        private HDRequest SelectedReq => (HDRequest)stashList.selectedItem;

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

        private void OnRequestListModification(HDRequest obj)
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
                    let name = req.id
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
                var typeProp = serObj.FindProperty(nameof(HDRequest.type));
                var nameProp = serObj.FindProperty(nameof(HDRequest.id));
                var button = ve.As<RequestButtonBig>();
                button.BindProperties(typeProp, nameProp);
                button.Deleted = () =>
                {
                    _requestList.RemoveAt(index);
                    stashList.Rebuild();
                    //RefreshReqView();
                };
                button.Duplicated = () =>
                {
                    _requestList.Insert(index, HDRequest.Create(Source[index]));
                    stashList.Rebuild();
                    //RefreshReqView();
                };
            };
            stashList.selectionChanged += selection =>
            {
                SelectionChanged?.Invoke((HDRequest)selection.FirstOrDefault());
                //RefreshReqView();
            };

            createButton.clicked += () =>
            {
                _requestList.Add(HDRequest.Create("New Request", null, HttpMethodType.Get, "{ }", null));
                stashList.Rebuild();
            };
        }
    }
}