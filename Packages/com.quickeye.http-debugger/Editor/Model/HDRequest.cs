using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

[assembly: InternalsVisibleTo("Sandbox.Editor")]

namespace QuickEye.RequestWatcher
{
    [Serializable]
    internal class HDRequest
    {
        private static HttpClient client = new HttpClient();
        public string name;
        public string url;
        public HttpMethodType type;
        public string body;
        public string headers;
        public HDResponse lastResponse;

        public async Task<HttpResponseMessage> SendAsync()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage(type.ToHttpMethod(), url);

            if (type == HttpMethodType.Post)
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var res = await client.SendAsync(request);
            return res;
        }

        public static async Task<HDRequest> FromHttpRequestMessage(string name, HttpRequestMessage req)
        {
            var url = req.RequestUri.OriginalString;
            var type = HttpMethodTypeUtil.FromHttpMethod(req.Method);
            var headers = HeadersToString(req.Content?.Headers);
            var e = new HDRequest
            {
                name = name,
                url = url,
                type = type,
                headers = headers
            };
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
            hdRequest.lastResponse.headers = HeadersToString(res.Content?.Headers);
            return hdRequest;
        }

        public static HDRequest FromUnityRequest(string name, UnityWebRequest req)
        {
            var hdRequest = new HDRequest
            {
                name = name,
                url = req.url,
                type = HttpMethodTypeUtil.FromString(req.method),
            };
            var textPayload = Encoding.UTF8.GetString(req.uploadHandler.data);
            hdRequest.body = new JsonFormatter(textPayload).Format();
            //hdRequest.headers = req.GetRequestHeader()
            hdRequest.lastResponse = new HDResponse((int)req.responseCode);
            hdRequest.lastResponse.headers = string.Join("\n",
                req.GetResponseHeaders().Select(kvp => $"{kvp.Key} :: {kvp.Value}").ToArray());
            if (req.downloadHandler.text != null)
                hdRequest.lastResponse.payload = new JsonFormatter(req.downloadHandler.text).Format();

            return hdRequest;
        }

        private static string HeadersToString(HttpHeaders headers)
        {
            if (headers == null)
                return "";
            var headersKvps = headers.Select(kvp => $"{kvp.Key} :: {ArrayToString(kvp.Value)}").ToArray();
            return string.Join("\n", headersKvps);
        }

        private static string ArrayToString(IEnumerable<string> enumerable)
        {
            return string.Join("; ", enumerable.ToArray());
        }
    }
}