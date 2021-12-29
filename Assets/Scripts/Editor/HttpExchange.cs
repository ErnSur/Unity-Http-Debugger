using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    [Serializable]
    internal class PostmanData
    {
        public List<HttpExchange> requests = new List<HttpExchange>
        {
            new HttpExchange
            {
            }
        };
    }

    [Serializable]
    internal class HttpExchange
    {
        public string name = "New Request";
        public string url = "https://";
        public HttpMethodType type;

        [Multiline(100)]
        public string body;

        public Response lastResponse;

        public static async Task<HttpExchange> FromHttpRequestMessage(string name, HttpRequestMessage req)
        {
            return new HttpExchange
            {
                name = name,
                url = req.RequestUri.OriginalString,
                type = HttpMethodTypeUtil.FromHttpMethod(req.Method),
                body = await req.Content.ReadAsStringAsync()
            };
        }

        public static async Task<HttpExchange> FromHttpResponseMessage(string name, HttpResponseMessage res)
        {
            var exchange = await FromHttpRequestMessage(name, res.RequestMessage);
            exchange.lastResponse = new Response
            {
                statusCode = res.StatusCode,
                payload = await res.Content.ReadAsStringAsync()
            };
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
            lastResponse = new Response
            {
                statusCode = res.StatusCode,
                payload = new JsonFormatter(await res.Content.ReadAsStringAsync()).Format()
            };
            return res;
        }

        [Serializable]
        internal class Response
        {
            public HttpStatusCode statusCode;
            public string payload;
        }
    }
}