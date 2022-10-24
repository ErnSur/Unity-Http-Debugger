using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    internal class MethodCell : Label
    {
        public MethodCell()
        {
            AddToClassList("cell-text");
            AddToClassList("method-cell");
        }

        public void Setup(string method)
        {
            text = FormatHttpMethodType(method);
        }

        private static string FormatHttpMethodType(string value)
        {
            return value[..3].ToUpperInvariant();
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

    internal class UrlCell : Label
    {
        public UrlCell()
        {
            AddToClassList("cell-text");
            AddToClassList("url-cell");
#if UNITY_2022_2_OR_NEWER
            selection.isSelectable = true;
#endif
        }

        public void Setup(string url)
        {
            text = url;
        }
    }

    internal class ResultCell : Label
    {
        public ResultCell()
        {
            AddToClassList("cell-text");
            AddToClassList("result-cell");
            AddToClassList("status-code");
        }

        public void Setup(int statusCode)
        {
            text = statusCode.ToString();
            HttpStatusCodeUtil.ToggleStatusCodeClass(this, statusCode);
        }
    }
}