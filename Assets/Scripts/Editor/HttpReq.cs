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
        public List<HttpReq> requests = new List<HttpReq>
        {
            new HttpReq
            {
            }
        };
    }

    [Serializable]
    internal class HttpReq
    {
        public string name = "New Request";
        public string url = "https://";
        public HttpReqType type;

        [Multiline(100)]
        public string body;

        public HttpRes lastResponse;
        
        public async Task<HttpResponseMessage> SendAsync()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var request = new HttpRequestMessage(type.ToHttpMethod(), url);
            if (type == HttpReqType.Post)
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var res= await client.SendAsync(request);
            lastResponse = new HttpRes
            {
                statusCode = res.StatusCode,
                payload = new JsonFormatter(await res.Content.ReadAsStringAsync()).Format()
            };
            return res;
        }
    }

    [Serializable]
    internal class HttpRes
    {
        public HttpStatusCode statusCode;
        public string payload;
    }
}