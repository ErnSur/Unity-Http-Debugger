using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[assembly: InternalsVisibleTo("Sandbox.Editor")]

namespace QuickEye.RequestWatcher
{
    [Serializable]
    internal class HDRequest : ScriptableObject, IDisposable
    {
        private static HttpClient _client = new HttpClient();
        public string id;
        public string url;
        public HttpMethodType type;
        public string body;
        public List<Header> headers;
        public HDResponse lastResponse;
        public bool isReadOnly;

        private HDRequest() { }

        public static HDRequest Create(string id, string url, HttpMethodType type, string body,
            IEnumerable<Header> headers)
        {
            var i = CreateInstance<HDRequest>();
            i.hideFlags = HideFlags.DontSaveInEditor;
            i.name = i.id = id;
            i.url = url;
            i.type = type;
            i.body = body;
            i.headers = headers == null ? new List<Header>() : new List<Header>(headers);
            return i;
        }

        public static HDRequest Create(HDRequest request)
        {
            return Instantiate(request);
        }

        //TODO: Add SendUnityRequest
        public async Task<HttpResponseMessage> SendAsync()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage(type.ToHttpMethod(), url);
            foreach (var header in headers.Where(header => header.enabled))
            {
                request.Headers.Add(header.name, header.value);
            }

            if (type == HttpMethodType.Post)
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var res = await _client.SendAsync(request);
            return res;
        }

        public static async Task<HDRequest> FromHttpRequestMessage(string name, HttpRequestMessage req)
        {
            var url = req.RequestUri.OriginalString;
            var type = HttpMethodTypeUtil.FromHttpMethod(req.Method);
            var headers = ContentHeadersToList(req.Content?.Headers);
            var e = Create(name, url, type, null, headers);
            if (req.Content != null)
                e.body = JsonFormatter.Format(await req.Content.ReadAsStringAsync());
            return e;
        }

        public static async Task<HDRequest> FromHttpResponseMessage(string name, HttpResponseMessage res)
        {
            var hdRequest = await FromHttpRequestMessage(name, res.RequestMessage);
            hdRequest.lastResponse = new HDResponse((int)res.StatusCode);
            if (res.Content != null)
                hdRequest.lastResponse.payload = JsonFormatter.Format(await res.Content.ReadAsStringAsync());
            hdRequest.lastResponse.headers = ContentHeadersToList(res.Content?.Headers);
            return hdRequest;
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

        public static HDRequest FromUnityRequest(string name, UnityWebRequest req)
        {
            var hdRequest = Create(name, req.url, HttpMethodTypeUtil.FromString(req.method), null, null);
            if (req.uploadHandler != null)
            {
                var textPayload = Encoding.UTF8.GetString(req.uploadHandler.data);
                hdRequest.body = new JsonFormatter(textPayload).Format();
            }

            hdRequest.headers = ContentHeadersToList(req.GetRequestHeaders());
            hdRequest.lastResponse = new HDResponse((int)req.responseCode);
            hdRequest.lastResponse.headers = ContentHeadersToList(req.GetResponseHeaders());
            if (req.downloadHandler?.text != null)
                hdRequest.lastResponse.payload = new JsonFormatter(req.downloadHandler.text).Format();

            return hdRequest;
        }

        private static string ArrayToString(IEnumerable<string> enumerable)
        {
            return string.Join("; ", enumerable.ToArray());
        }

        public void Dispose()
        {
            DestroyImmediate(this);
        }

        private void OnValidate()
        {
            name = id;
        }
    }
}