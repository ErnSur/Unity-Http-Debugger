using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEditor;
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

        public static void Log(string name, HttpRequestMessage message)
        {
            LoggedRequest?.Invoke(name, message);
        }

        public static void Log(string name, HttpResponseMessage message)
        {
            LoggedResponse?.Invoke(name, message);
        }
    }
}