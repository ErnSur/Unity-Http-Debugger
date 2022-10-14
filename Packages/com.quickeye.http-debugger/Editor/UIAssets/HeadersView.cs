using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal partial class HeadersView
    {
        private SerializedProperty _headersProperty;
        private SerializedArrayProperty _serializedArrayProperty;
        public HeadersView(VisualElement root)
        {
            AssignQueryResults(root);
            headerList.showAddRemoveFooter = true;
            toggleCol.makeCell = () => new Toggle();
            toggleCol.bindCell = (element, i) =>
            {
                if (_headersProperty == null)
                    return;
                var prop = _serializedArrayProperty[i].FindPropertyRelative(nameof(Header.enabled));
                ((Toggle)element).BindProperty(prop);
            };

            nameCol.makeCell = () => new TextField();
            nameCol.bindCell = (element, i) =>
            {
                if (_headersProperty == null)
                    return;
                var prop = _serializedArrayProperty[i].FindPropertyRelative(nameof(Header.name));
                ((TextField)element).BindProperty(prop);
            };

            valueCol.makeCell = () => new TextField();
            valueCol.bindCell = (element, i) =>
            {
                if (_headersProperty == null)
                    return;
                var prop = _serializedArrayProperty[i].FindPropertyRelative(nameof(Header.value));
                ((TextField)element).BindProperty(prop);
            };
        }

        public void Setup(SerializedProperty headersProperty)
        {
            _headersProperty = headersProperty;
            _serializedArrayProperty = new SerializedArrayProperty(_headersProperty);
            
            headerList.itemsSource = _serializedArrayProperty;
            headerList.Rebuild();
        }
    }
}