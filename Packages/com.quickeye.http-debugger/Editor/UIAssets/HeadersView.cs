using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    internal partial class HeadersView
    {
        private SerializedProperty _headersProperty;
        private SerializedArrayProperty _serializedArrayProperty;
        private bool _inReadOnlyMode;

        public HeadersView(VisualElement root)
        {
            AssignQueryResults(root);
            headerList.showAddRemoveFooter = true;
            toggleCol.makeCell = () =>
            {
                var ve = new VisualElement();
                ve.Add(new Toggle());
                ve.style.flexGrow = 1;
                ve.style.alignItems = Align.Center;
                ve.style.justifyContent = Justify.Center;
                return ve;
            };
            toggleCol.bindCell = (element, i) =>
            {
                var prop = _serializedArrayProperty[i].FindPropertyRelative(nameof(Header.enabled));
                element.Q<Toggle>().BindProperty(prop);
            };

            nameCol.makeCell = () => new TextField();
            nameCol.bindCell = (element, i) =>
            {
                var prop = _serializedArrayProperty[i].FindPropertyRelative(nameof(Header.name));
                var field = (TextField)element;
                field.BindProperty(prop);
                field.isReadOnly = _inReadOnlyMode;
            };

            valueCol.makeCell = () => new TextField();
            valueCol.bindCell = (element, i) =>
            {
                var prop = _serializedArrayProperty[i].FindPropertyRelative(nameof(Header.value));
                var field = (TextField)element;
                field.BindProperty(prop);
                field.isReadOnly = _inReadOnlyMode;
            };
        }

        public void Setup(SerializedProperty headersProperty)
        {
            _headersProperty = headersProperty;
            _serializedArrayProperty = new SerializedArrayProperty(_headersProperty);

            headerList.itemsSource = _serializedArrayProperty;
            headerList.Rebuild();
        }

        public void ToggleReadOnlyMode(bool enabled)
        {
            _inReadOnlyMode = enabled;
            headerList.showAddRemoveFooter = !_inReadOnlyMode;
            if (_inReadOnlyMode)
                headerList.columns.Remove(toggleCol);
            else if (!headerList.columns.Contains(toggleCol))
                headerList.columns.Add(toggleCol);
            headerList.Rebuild();
        }
    }
}