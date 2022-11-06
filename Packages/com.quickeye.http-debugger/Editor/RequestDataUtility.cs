using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace QuickEye.WebTools.Editor
{
    internal static class RequestDataUtility
    {
        public static async Task<T> FromHttpRequestMessage<T>(HttpRequestMessage req) where T : RequestData
        {
            var url = req.RequestUri.OriginalString;
            var type = HttpMethodTypeUtil.FromHttpMethod(req.Method);

            var headers = new List<Header>();
            headers.AddRange(EnumerateHeaders(req.Headers));
            if (req.Content?.Headers != null)
                headers.AddRange(EnumerateHeaders(req.Content.Headers));

            var result = ScriptableObject.CreateInstance<T>();
            result.url = url;
            result.type = type;
            result.headers = headers;
            if (req.Content != null)
                result.content = await req.Content.ReadAsStringAsync();
            return result;
        }

        public static async Task<T> FromHttpResponseMessage<T>(HttpResponseMessage res) where T : RequestData
        {
            var hdRequest = await FromHttpRequestMessage<T>(res.RequestMessage);
            hdRequest.lastResponse = await ResponseFromHttpResponseMessageAsync(res);
            return hdRequest;
        }

        public static T FromUnityWebRequest<T>(UnityWebRequest req) where T : RequestData
        {
            var result = ScriptableObject.CreateInstance<T>();
            result.url = req.url;
            result.type = HttpMethodTypeUtil.FromString(req.method);
            if (req.uploadHandler != null && req.uploadHandler.data != null)
            {
                result.headers.Add(new Header("Content-Type", req.uploadHandler.contentType));
                var content = Encoding.UTF8.GetString(req.uploadHandler.data);
                result.content = content;
            }

            result.lastResponse = ResponseFromUnityWebRequest(req);
            return result;
        }

        public static ResponseData ResponseFromUnityWebRequest(UnityWebRequest req)
        {
            var result = new ResponseData((int)req.responseCode);
            result.headers = HeadersToList(req.GetResponseHeaders());
            if (req.downloadHandler?.text != null)
                result.content = req.downloadHandler.text;

            return result;
        }

        public static async Task<ResponseData> ResponseFromHttpResponseMessageAsync(HttpResponseMessage res)
        {
            var result = new ResponseData((int)res.StatusCode);
            if (res.Content != null)
                result.content = await res.Content.ReadAsStringAsync();

            var headers = new List<Header>();
            headers.AddRange(EnumerateHeaders(res.Headers));
            if (res.Content?.Headers != null)
                headers.AddRange(EnumerateHeaders(res.Content.Headers));
            headers.AddRange(EnumerateHeaders(res.TrailingHeaders));
            result.headers = headers;
            return result;
        }

        private static IEnumerable<Header> EnumerateHeaders(HttpHeaders headerCollection)
        {
            return headerCollection?.Select(kvp =>
                new Header(kvp.Key, string.Join("; ", kvp.Value)));
        }

        private static List<Header> HeadersToList(Dictionary<string, string> dictionary)
        {
            if (dictionary == null)
                return null;
            return new List<Header>(dictionary
                .Select(p => new Header(p.Key, p.Value)));
        }
    }
}