using System.Linq;

namespace QuickEye.WebTools.Editor
{
    internal static class RequestDataUtils
    {
        public const string JsonContentType = "application/json";
        public const string XmlContentType = "application/xml";

        internal static bool IsContentType(this RequestData data, string type)
        {
            return data.headers.Exists(h => h.name == HeaderNames.ContentType && h.value.Contains(type));
        }

        internal static void SetContentType(this RequestData data, string type)
        {
            var contentHeader = data.headers.FirstOrDefault(h => h.name == HeaderNames.ContentType);
            if (contentHeader == null)
            {
                data.headers.Insert(0, new Header(HeaderNames.ContentType, type));
                return;
            }
            if (!contentHeader.value.Contains(type))
                contentHeader.value = type;
        }

        internal static string GetContentType(this RequestData data)
        {
            return data.headers.FirstOrDefault(h => h.name == HeaderNames.ContentType)?.value;
        }

        internal static string GetContentType(this ResponseData data)
        {
            return data?.headers?.FirstOrDefault(h => h.name == HeaderNames.ContentType)?.value;
        }

        internal static string GetFormattedContent(this RequestData data)
        {
            return GetFormattedContent(data.content, GetContentType(data));
        }

        internal static string GetFormattedResponseContent(this RequestData data)
        {
            return GetFormattedContent(data.lastResponse.content, GetContentType(data.lastResponse));
        }

        private static string GetFormattedContent(string content, string contentType)
        {
            if (content == null || contentType == null)
                return content;
            if (contentType.Contains(JsonContentType))
                return JsonFormatter.Format(content);
            if (contentType.Contains(XmlContentType))
                return content; // TODO: implemented XML formatter
            return content;
        }
    }
}