using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;

namespace QuickEye.RequestWatcher
{
    public class SerializedArrayProperty : IReadOnlyList<SerializedProperty>, IList
    {
        public SerializedProperty Property { get; }
        public string RelativePath { get; }

        public SerializedArrayProperty(SerializedProperty property, string relativePath = null)
        {
            Property = property;
            RelativePath = relativePath;
        }

        public IEnumerator<SerializedProperty> GetEnumerator()
        {
            for (int i = 0; i < Property.arraySize; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        int IList.Add(object value)
        {
            Property.arraySize++;
            Property.serializedObject.ApplyModifiedProperties();
            return Count - 1;
        }

        public void Clear() => Property.ClearArray();
        bool IList.Contains(object value)
        {
            return ((IList)this).IndexOf(value) != -1;
        }

        int IList.IndexOf(object value)
        {
            if (value is SerializedProperty prop)
            {
                return IndexOf(prop);
            }
            return -1;
        }

        public int IndexOf(SerializedProperty prop)
        {
            if (IsArrayElement(prop.propertyPath))
            {
                return GetElementIndex(prop.propertyPath);
            }
            return -1;
        }

        private bool IsArrayElement(string propPath)
        {
            var re = Regex.Escape(Property.propertyPath) +
                     @"\.Array\.data\[\d+\]$";
            return Regex.IsMatch(propPath, re);
        }
        private static int GetElementIndex(string propPath)
        {
            var match =Regex.Match(propPath, @"data\[\d+\]$");
            var index = match.Value.Substring("data[".Length);
            index = index.Remove(index.Length-1,1);
            return int.Parse(index);
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        void IList.Remove(object value)
        {
            Property.arraySize--;
            Property.serializedObject.ApplyModifiedProperties();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count => Property.arraySize;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot { get; } = new object();

        public void Add() => Insert(Count);

        public void Insert(int index)
        {
            Property.InsertArrayElementAtIndex(index);
        }

        public void RemoveAt(int index)
        {
            Property.DeleteArrayElementAtIndex(index);
        }

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => true;

        object IList.this[int index]
        {
            get => this[index];
            set => throw new NotImplementedException();
        }

        public SerializedProperty this[int index] => GetPropertyAtIndex(index);

        public SerializedProperty this[int index, string relativePath] =>
            GetPropertyAtIndex(index).FindPropertyRelative(relativePath);

        private SerializedProperty GetPropertyAtIndex(int index)
        {
            var prop = Property.GetArrayElementAtIndex(index);
            if (!string.IsNullOrWhiteSpace(RelativePath))
                prop = prop.FindPropertyRelative(RelativePath);
            return prop;
        }
    }
}