#define HTTP_DEBUGGER_DEV
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

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

    public class LogMessageHandler : DelegatingHandler
    {
        private readonly (string domain, string alias)[] domainAliases;

        public LogMessageHandler(HttpMessageHandler innerHandler, string id) : base(innerHandler)
        {
            domainAliases = new (string domain, string alias)[]
            {
                ("http", id)
            };
        }

        public LogMessageHandler(HttpMessageHandler innerHandler, IEnumerable<(string domain, string alias)> aliases) :
            base(innerHandler)
        {
            domainAliases = aliases.OrderByDescending(t => t.domain).ToArray();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            var url = request.RequestUri.OriginalString;
            if (!TryGetId(url, out var id))
                id = url;
            HttpClientLogger.Log(id, response);
            return response;
        }

        private bool TryGetId(string url, out string id)
        {
            foreach (var (domain, alias) in domainAliases)
            {
                if (!url.StartsWith(domain))
                    continue;
                id = alias;
                return true;
            }

            id = default;
            return false;
        }
    }
}