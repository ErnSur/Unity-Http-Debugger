﻿#define HTTP_DEBUGGER_DEV
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[assembly: InternalsVisibleTo("QuickEye.RequestWatcher.Editor")]

namespace QuickEye.RequestWatcher
{
    /// <summary>
    /// Runtime API for game to log their HTTP calls. Those will be visible in Request Watcher window in "Playmode" tab
    /// </summary>
    public static class HttpClientLogger
    {
        internal static event Action<string, HttpRequestMessage> LoggedRequest;
        internal static event Action<string, HttpResponseMessage> LoggedResponse;
        internal static event Action<string, UnityWebRequest> LoggedUnityRequest;

        public static void Log(string name, HttpRequestMessage message)
        {
            LoggedRequest?.Invoke(name, message);
        }

        public static void Log(string name, HttpResponseMessage message)
        {
            LoggedResponse?.Invoke(name, message);
        }

        public static async Task<HttpResponseMessage> SendAsync(this HttpClient c, string id,
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await c.SendAsync(request, cancellationToken);
            LoggedResponse?.Invoke(id, result);
            return result;
        }

        public static async Task<HttpResponseMessage> SendAsync(this HttpClient c, string id,
            HttpRequestMessage request)
        {
            var result = await c.SendAsync(request);
            LoggedResponse?.Invoke(id, result);
            return result;
        }

        public static async Task<HttpResponseMessage> SendAsync(this HttpMessageInvoker c, string id,
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await c.SendAsync(request, cancellationToken);
            LoggedResponse?.Invoke(id, result);
            return result;
        }

        public static UnityWebRequest Log(this UnityWebRequest unityRequest, string id)
        {
            try
            {
                LoggedUnityRequest?.Invoke(id, unityRequest);
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