using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace QuickEye.RequestWatcher
{
    public class SerializedArrayProperty : IReadOnlyList<SerializedProperty>
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

        public void Clear() => Property.ClearArray();

        public int Count => Property.arraySize;

        public void Add() => Insert(Count);

        public void Insert(int index)
        {
            Property.InsertArrayElementAtIndex(index);
        }

        public void RemoveAt(int index)
        {
            Property.DeleteArrayElementAtIndex(index);
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