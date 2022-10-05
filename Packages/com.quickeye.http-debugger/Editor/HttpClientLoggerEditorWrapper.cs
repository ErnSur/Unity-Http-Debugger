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
        private const string PlaymodePrefsKey = "PostmanPlaymode";

        internal static event Action<HDRequest> ExchangeLogged;

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
            var ex = await HDRequest.FromHttpRequestMessage(name, message);
            SerializePlaymodeLog(ex);
            ExchangeLogged?.Invoke(ex);
        }
        
        private static void Log(string name, UnityWebRequest message)
        {
            if (message == null)
                return;
            var ex =  HDRequest.FromUnityRequest(name, message);
            SerializePlaymodeLog(ex);
            ExchangeLogged?.Invoke(ex);
        }

        private static async Task Log(string name, HttpResponseMessage message)
        {
            if (message == null)
                return;
            var ex = await HDRequest.FromHttpResponseMessage(name, message);
            SerializePlaymodeLog(ex);
            ExchangeLogged?.Invoke(ex);
        }

        private static void SerializePlaymodeLog(HDRequest exchange)
        {
            var json = EditorPrefs.GetString(PlaymodePrefsKey, JsonUtility.ToJson(new RequestCollection()));
            var data = JsonUtility.FromJson<RequestCollection>(json);
            data.requests.Add(exchange);
            Database.instance.playmodeRequests.Add(exchange);
            EditorPrefs.SetString(PlaymodePrefsKey, JsonUtility.ToJson(data));
        }
    }
}