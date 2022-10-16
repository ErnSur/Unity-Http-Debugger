using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    [InitializeOnLoad]
    public static class HttpClientLoggerEditorWrapper
    {
        internal static event Action<RequestData> ExchangeLogged;

        static HttpClientLoggerEditorWrapper()
        {
            HttpClientLogger.LoggedRequest += async (n, m, s) => await Log(n, m, s);
            HttpClientLogger.LoggedResponse += async (n, m, s) => await Log(n, m, s);
            HttpClientLogger.LoggedUnityRequest += Log;
        }

        private static void HandleBreakpoint(string reqName)
        {
            if (RequestConsoleDatabase.instance.breakpoints.Contains(reqName))
            {
                // show mock window
                Debug.Break();
                RequestConsoleWindow.Open().ShowNotification(new GUIContent($"Breakpoint: {reqName}"));
            }
        }

        private static async Task Log(string name, HttpRequestMessage message, string stackTrace)
        {
            if (message == null)
                return;
            var req = await RequestDataUtility.FromHttpRequestMessage<ConsoleRequestData>(message);
            req.name = name;
            req.stackTrace = RemoveFirstTwoLines(stackTrace);
            SerializePlaymodeLog(req);
            ExchangeLogged?.Invoke(req);
            HandleBreakpoint(name);
        }

        private static void Log(string name, UnityWebRequest message, string stackTrace)
        {
            if (message == null)
                return;
            var req = RequestDataUtility.FromUnityRequest<ConsoleRequestData>(message);
            req.name = name;
            req.stackTrace = RemoveFirstTwoLines(stackTrace);
            SerializePlaymodeLog(req);
            ExchangeLogged?.Invoke(req);
            HandleBreakpoint(name);
        }

        private static async Task Log(string name, HttpResponseMessage message, string stackTrace)
        {
            if (message == null)
                return;
            var req = await RequestDataUtility.FromHttpResponseMessage<ConsoleRequestData>(message);
            req.name = name;
            req.stackTrace = RemoveFirstTwoLines(stackTrace);
            SerializePlaymodeLog(req);
            ExchangeLogged?.Invoke(req);
            HandleBreakpoint(name);
        }

        private static void SerializePlaymodeLog(RequestData exchange)
        {
            RequestConsoleDatabase.instance.requests.Add(exchange);
        }

        private static string RemoveFirstTwoLines(string text)
        {
            using (var sr = new StringReader(text))
            {
                sr.ReadLine();
                sr.ReadLine();
                return sr.ReadToEnd();
            }
        }
    }
}