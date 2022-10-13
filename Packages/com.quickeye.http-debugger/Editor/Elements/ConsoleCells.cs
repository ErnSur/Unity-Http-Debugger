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

    internal class IdCell : Label
    {
        public IdCell()
        {
            AddToClassList("cell-text");

            AddToClassList("id-cell");
        }

        public void Setup(string id)
        {
            text = id;
        }
    }
}