using System;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
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
        private readonly BreakpointToggle _breakpointToggle;

        public IdCell()
        {
            Add(_label = new Label());
            Add(_breakpointToggle = new BreakpointToggle());
            _label.AddToClassList("cell-text");
            AddToClassList("id-cell");
        }

        public void Setup(string id)
        {
            _label.text = id;
            _breakpointToggle.BreakpointName = id;
        }
    }
}