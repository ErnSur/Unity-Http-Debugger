#define HTTP_DEBUGGER_DEV
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

[assembly: InternalsVisibleTo("QuickEye.WebTools.Editor")]

namespace QuickEye.WebTools
{
    /// <summary>
    /// Runtime API for game to log their HTTP calls. Those will be visible in Request Console Window
    /// </summary>
    public static class HttpClientLogger
    {
        internal static event Action<string, HttpRequestMessage, string> LoggedRequest;
        internal static event Action<string, HttpResponseMessage, string> LoggedResponse;
        internal static event Action<string, UnityWebRequest, string> LoggedUnityRequest;
  

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

        public static void Log(string name, HttpRequestMessage message)
        {
            LoggedRequest?.Invoke(name, message, StackTraceUtility.ExtractStackTrace());
        }

        public static void Log(string name, HttpResponseMessage message)
        {
            LoggedResponse?.Invoke(name, message, StackTraceUtility.ExtractStackTrace());
        }

        public static UnityWebRequest Log(this UnityWebRequest unityRequest, string id)
        {
            try
            {
                LoggedUnityRequest?.Invoke(id, unityRequest, StackTraceUtility.ExtractStackTrace());
            }
            catch (Exception e)
            {
#if HTTP_DEBUGGER_DEV
                Debug.LogException(e);
#endif
            }

            return unityRequest;
        }
    }
}