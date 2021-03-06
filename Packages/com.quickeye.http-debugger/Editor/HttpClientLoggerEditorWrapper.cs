using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace QuickEye.RequestWatcher
{
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
            ExchangeLogged?.Invoke(ex);
            SerializePlaymodeLog(ex);
        }
        
        private static void Log(string name, UnityWebRequest message)
        {
            if (message == null)
                return;
            var ex =  HDRequest.FromUnityRequest(name, message);
            ExchangeLogged?.Invoke(ex);
            SerializePlaymodeLog(ex);
        }

        private static async Task Log(string name, HttpResponseMessage message)
        {
            if (message == null)
                return;
            var ex = await HDRequest.FromHttpResponseMessage(name, message);
            ExchangeLogged?.Invoke(ex);
            SerializePlaymodeLog(ex);
        }

        private static void SerializePlaymodeLog(HDRequest exchange)
        {
            var json = EditorPrefs.GetString(PlaymodePrefsKey, JsonUtility.ToJson(new PostmanData()));
            var data = JsonUtility.FromJson<PostmanData>(json);
            data.requests.Add(exchange);
            EditorPrefs.SetString(PlaymodePrefsKey, JsonUtility.ToJson(data));
        }
    }
}