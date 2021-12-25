using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    [CreateAssetMenu]
    internal class HttpRequestSettings : ScriptableObject
    {
        public string url;
        public HttpReqType type;

        [Multiline(100)]
        public string body;

        public async Task<HttpResponseMessage> SendAsync()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var request = new HttpRequestMessage(type.ToHttpMethod(), url);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            return await client.SendAsync(request);
        }
    }
}