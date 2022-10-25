using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace QuickEye.WebTools
{
    public static partial class WebLogger
    {
        public static void Log(string name, HttpRequestMessage message)
        {
            LoggedRequest?.Invoke(name, message, StackTraceUtility.ExtractStackTrace());
        }

        public static void Log(string name, HttpResponseMessage message)
        {
            LoggedResponse?.Invoke(name, message, StackTraceUtility.ExtractStackTrace());
        }

        public static async Task<HttpResponseMessage> SendAsync(this HttpClient c, string id,
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await c.SendAsync(request, cancellationToken);
            LoggedResponse?.Invoke(id, result, StackTraceUtility.ExtractStackTrace());
            return result;
        }

        public static async Task<HttpResponseMessage> SendAsync(this HttpClient c, string id,
            HttpRequestMessage request)
        {
            var result = await c.SendAsync(request);
            LoggedResponse?.Invoke(id, result, StackTraceUtility.ExtractStackTrace());
            return result;
        }

        public static async Task<HttpResponseMessage> SendAsync(this HttpMessageInvoker c, string id,
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await c.SendAsync(request, cancellationToken);
            LoggedResponse?.Invoke(id, result, StackTraceUtility.ExtractStackTrace());
            return result;
        }
    }
}