using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    [Serializable]
    internal class ListWithEvents<T> : IList<T>, IList
    {
        public event Action<T> Added;
        public event Action<T> Removed;
        public event Action BeforeClear;
        public event Action AfterClear;
        public event Action Modified;

        [SerializeField]
        private List<T> items;

        public ListWithEvents() => items = new List<T>();

        public ListWithEvents(IEnumerable<T> collection)
        {
            items = new List<T>(collection);
        }

        void IList.Clear() => Clear();

        bool IList.Contains(object value) => Contains((T)value);

        int IList.IndexOf(object value) => IndexOf((T)value);

        void IList.Insert(int index, object value) => Insert(index, (T)value);

        int IList.Add(object value)
        {
            Add((T)value);
            return Count - 1;
        }

        void IList.Remove(object value) => Remove((T)value);

        public void Add(T item)
        {
            items.Add(item);
            Added?.Invoke(item);
            Modified?.Invoke();
        }

        public void Clear()
        {
            BeforeClear?.Invoke();
            items.Clear();
            AfterClear?.Invoke();
            Modified?.Invoke();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (item == null)
                return false;
            var res = items.Remove(item);
            Removed?.Invoke(item);
            Modified?.Invoke();
            return res;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count => items.Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        public bool IsReadOnly => false;

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
        public int IndexOf(T item) => items.IndexOf(item);

        public void Insert(int index, T item)
        {
            items.Insert(index, item);
            Added?.Invoke(item);
            Modified?.Invoke();
        }

        public void RemoveAt(int index)
        {
            var item = this[index];
            items.RemoveAt(index);
            Removed?.Invoke(item);
            Modified?.Invoke();
        }

        bool IList.IsFixedSize => false;

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }

        public T this[int index]
        {
            get => items[index];
            set
            {
                items[index] = value;
                Modified?.Invoke();
            }
        }
    }
}