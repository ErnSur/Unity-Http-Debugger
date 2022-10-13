using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    [Serializable]
    internal class RequestList : IList<HDRequest>, IList
    {
        public event Action<HDRequest> Added;
        public event Action<HDRequest> Removed;
        public event Action BeforeClear;

        [SerializeField]
        private List<HDRequest> requests;

        public RequestList() => requests = new List<HDRequest>();

        public RequestList(IEnumerable<HDRequest> collection)
        {
            requests = new List<HDRequest>(collection);
        }

        void IList.Clear() => Clear();

        bool IList.Contains(object value) => Contains((HDRequest)value);

        int IList.IndexOf(object value) => IndexOf((HDRequest)value);

        void IList.Insert(int index, object value) => Insert(index, (HDRequest)value);

        int IList.Add(object value)
        {
            Add((HDRequest)value);
            return Count - 1;
        }

        void IList.Remove(object value) => Remove((HDRequest)value);

        public void Add(HDRequest request)
        {
            requests.Add(request);
            Added?.Invoke(request);
        }

        public void Clear()
        {
            BeforeClear?.Invoke();
            requests.Clear();
        }

        public void CopyTo(HDRequest[] array, int arrayIndex)
        {
            requests.CopyTo(array, arrayIndex);
        }

        public bool Remove(HDRequest item)
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

        public bool Contains(HDRequest item)
        {
            return requests.Contains(item);
        }

        public IEnumerator<HDRequest> GetEnumerator() => requests.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => requests.GetEnumerator();
        public int IndexOf(HDRequest item) => requests.IndexOf(item);

        public void Insert(int index, HDRequest item) => requests.Insert(index, item);

        public void RemoveAt(int index) => requests.RemoveAt(index);

        bool IList.IsFixedSize => false;

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (HDRequest)value;
        }

        public HDRequest this[int index]
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