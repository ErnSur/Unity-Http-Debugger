using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    [Serializable]
    internal class RequestList : IList<RequestData>, IList
    {
        // TODO: To avoid disposing request on add/remove
        // before unreferencing any request add it to "Unused" set
        // and remove form it if you are assigning it again
        // add DisposeUnunsed method
        public event Action<RequestData> Added;
        public event Action<RequestData> Removed;
        public event Action BeforeClear;
        public event Action AfterClear;

        [SerializeField]
        private List<RequestData> requests;

        public RequestList() => requests = new List<RequestData>();

        public RequestList(IEnumerable<RequestData> collection)
        {
            requests = new List<RequestData>(collection);
        }

        void IList.Clear() => Clear();

        bool IList.Contains(object value) => Contains((RequestData)value);

        int IList.IndexOf(object value) => IndexOf((RequestData)value);

        void IList.Insert(int index, object value) => Insert(index, (RequestData)value);

        int IList.Add(object value)
        {
            Add((RequestData)value);
            return Count - 1;
        }

        void IList.Remove(object value) => Remove((RequestData)value);

        public void Add(RequestData request)
        {
            requests.Add(request);
            Added?.Invoke(request);
        }

        public void Clear()
        {
            BeforeClear?.Invoke();
            requests.Clear();
            AfterClear?.Invoke();
        }

        public void CopyTo(RequestData[] array, int arrayIndex)
        {
            requests.CopyTo(array, arrayIndex);
        }

        public bool Remove(RequestData item)
        {
            if (item == null)
                return false;
            var res = requests.Remove(item);
            Removed?.Invoke(item);
            return res;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count => requests.Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        public bool IsReadOnly => false;

        public bool Contains(RequestData item)
        {
            return requests.Contains(item);
        }

        public IEnumerator<RequestData> GetEnumerator() => requests.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => requests.GetEnumerator();
        public int IndexOf(RequestData item) => requests.IndexOf(item);

        public void Insert(int index, RequestData item) => requests.Insert(index, item);

        public void RemoveAt(int index) => requests.RemoveAt(index);

        bool IList.IsFixedSize => false;

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (RequestData)value;
        }

        public RequestData this[int index]
        {
            get => requests[index];
            set
            {
                //TODO: destroy unreferenced object?
                requests[index] = value;
            }
        }
    }
}