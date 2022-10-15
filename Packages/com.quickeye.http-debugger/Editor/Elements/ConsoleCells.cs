using System;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal class StatusCodeCell : Label
    {
        public StatusCodeCell()
        {
            AddToClassList("cell-text");

            AddToClassList("code-cell");
            AddToClassList("status-code");
        }

        public void Setup(int statusCode)
        {
            text = statusCode.ToString();
            HttpStatusCodeUtil.ToggleStatusCodeClass(this, statusCode);
        }
    }

    internal class MethodCell : Label
    {
        public MethodCell()
        {
            AddToClassList("cell-text");

            AddToClassList("type-cell");
        }

        public void Setup(string method)
        {
            text = FormatHttpMethodType(method);
        }

        private string FormatHttpMethodType(string value)
        {
            return value[..3].ToUpperInvariant();
        }
    }

    internal class UrlCell : TextField
    {
        public UrlCell()
        {
            AddToClassList("cell-text");
            AddToClassList("url-cell");
            textInputBase.AddToClassList("url-cell--text-input");
            isReadOnly = true;
        }

        public void Setup(string url)
        {
            text = url;
        }
    }

    internal class IdCell : VisualElement
    {
        private readonly Label _label;
        private readonly Toggle _breakpointToggle;
        private string _id;

        public IdCell(Action<string, bool> onBreakpointValueChange)
        {
            Add(_label = new Label());
            Add(_breakpointToggle = new Toggle());
            _breakpointToggle.AddToClassList("breakpoint-toggle");
            _label.AddToClassList("cell-text");
            AddToClassList("id-cell");
            _breakpointToggle.RegisterValueChangedCallback(e => { onBreakpointValueChange?.Invoke(_id, e.newValue); });
            const string breakpointHoverClass = "breakpoint-toggle--hover";
            RegisterCallback<MouseEnterEvent>(evt =>
            {
                _breakpointToggle.EnableInClassList(breakpointHoverClass, true);
            });

            RegisterCallback<MouseLeaveEvent>(evt =>
            {
                _breakpointToggle.EnableInClassList(breakpointHoverClass, false);
            });
        }

        public void Setup(string id, bool hasBreakpoint)
        {
            _id = _label.text = id;
            _breakpointToggle.SetValueWithoutNotify(hasBreakpoint);
        }
    }
}