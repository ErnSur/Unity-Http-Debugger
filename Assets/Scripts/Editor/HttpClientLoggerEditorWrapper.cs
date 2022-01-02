﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

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
        }

        private static async Task Log(string name, HttpRequestMessage message)
        {
            var ex = await HDRequest.FromHttpRequestMessage(name, message);
            ExchangeLogged?.Invoke(ex);
            SerializePlaymodeLog(ex);
        }

        private static async Task Log(string name, HttpResponseMessage message)
        {
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