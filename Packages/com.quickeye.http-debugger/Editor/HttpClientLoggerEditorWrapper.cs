using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace QuickEye.RequestWatcher
{
    [InitializeOnLoad]
    public static class HttpClientLoggerEditorWrapper
    {
        internal static event Action<RequestData> ExchangeLogged;

        static HttpClientLoggerEditorWrapper()
        {
            HttpClientLogger.LoggedRequest += async (n, m) => await Log(n, m);
            HttpClientLogger.LoggedResponse += async (n, m) => await Log(n, m);
            HttpClientLogger.LoggedUnityRequest += Log;
        }

        private static async Task Log(string name, HttpRequestMessage message)
        {
            if (message == null)
                return;
            var ex = await RequestDataUtility.FromHttpRequestMessage<ConsoleRequestData>(message);
            ex.name = name;
            SerializePlaymodeLog(ex);
            ExchangeLogged?.Invoke(ex);
        }

        private static void Log(string name, UnityWebRequest message)
        {
            if (message == null)
                return;
            var ex = RequestDataUtility.FromUnityRequest<ConsoleRequestData>(message);
            ex.name = name;
            SerializePlaymodeLog(ex);
            ExchangeLogged?.Invoke(ex);
        }

        private static async Task Log(string name, HttpResponseMessage message)
        {
            if (message == null)
                return;
            var ex = await RequestDataUtility.FromHttpResponseMessage<ConsoleRequestData>(message);
            ex.name = name;
            SerializePlaymodeLog(ex);
            ExchangeLogged?.Invoke(ex);
        }

        private static void SerializePlaymodeLog(RequestData exchange)
        {
            RequestConsoleDatabase.instance.requests.Add(exchange);
        }
    }
}