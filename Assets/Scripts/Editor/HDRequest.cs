using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[assembly:InternalsVisibleTo("Sandbox.Editor")]
namespace QuickEye.RequestWatcher
{
    [Serializable]
    internal class PostmanData
    {
        public List<HDRequest> requests = new List<HDRequest>
        {
            new HDRequest
            {
            }
        };
    }

    [Serializable]
    internal struct HDRequest
    {
        public string name;
        public string url;
        public HttpMethodType type;
        public string body;

        public HDResponse lastResponse;

        public static async Task<HDRequest> FromHttpRequestMessage(string name, HttpRequestMessage req)
        {
            var e = new HDRequest
            {
                name = name,
                url = req.RequestUri.OriginalString,
                type = HttpMethodTypeUtil.FromHttpMethod(req.Method),
            };
            if (req.Content != null)
                e.body = new JsonFormatter(await req.Content.ReadAsStringAsync()).Format();
            return e;
        }

        public static async Task<HDRequest> FromHttpResponseMessage(string name, HttpResponseMessage res)
        {
            var exchange = await FromHttpRequestMessage(name, res.RequestMessage);
            exchange.lastResponse = new HDResponse((int)res.StatusCode);
            if (res.Content != null)
                exchange.lastResponse.payload = new JsonFormatter(await res.Content.ReadAsStringAsync()).Format();
            return exchange;
        }

        public async Task<HttpResponseMessage> SendAsync()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var request = new HttpRequestMessage(type.ToHttpMethod(), url);
            if (type == HttpMethodType.Post)
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var res = await client.SendAsync(request);
            lastResponse = new HDResponse(statusCode: (int)res.StatusCode,
                payload: new JsonFormatter(await res.Content.ReadAsStringAsync()).Format());
            return res;
        }
    }

    [Serializable]
    internal struct HDResponse
    {
        public int statusCode;
        public string payload;

        public HDResponse(int statusCode) : this()
        {
            this.statusCode = statusCode;
        }

        public HDResponse(int statusCode, string payload)
        {
            this.statusCode = statusCode;
            this.payload = payload;
        }
    }
}