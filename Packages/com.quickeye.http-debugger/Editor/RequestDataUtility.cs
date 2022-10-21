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
            var headers = ContentHeadersToList(req.Content?.Headers);
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
            hdRequest.lastResponse = new ResponseData((int)res.StatusCode);
            if (res.Content != null)
                hdRequest.lastResponse.content = await res.Content.ReadAsStringAsync();
            hdRequest.lastResponse.headers = ContentHeadersToList(res.Content?.Headers);
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
            

            result.headers.AddRange(ContentHeadersToList(req.GetRequestHeaders()));
            result.lastResponse = ResponseFromUnityWebRequest(req);
            return result;
        }

        public static ResponseData ResponseFromUnityWebRequest(UnityWebRequest req)
        {
            var result = new ResponseData((int)req.responseCode);
            result.headers = ContentHeadersToList(req.GetResponseHeaders());
            if (req.downloadHandler?.text != null)
                result.content = req.downloadHandler.text;

            return result;
        }

        private static List<Header> ContentHeadersToList(HttpContentHeaders headerCollection)
        {
            if (headerCollection == null)
                return null;
            return new List<Header>(headerCollection
                .Select(p => new Header(p.Key, string.Join("; ", p))));
        }

        private static List<Header> ContentHeadersToList(Dictionary<string, string> dictionary)
        {
            if (dictionary == null)
                return null;
            return new List<Header>(dictionary
                .Select(p => new Header(p.Key, p.Value)));
        }
    }
}