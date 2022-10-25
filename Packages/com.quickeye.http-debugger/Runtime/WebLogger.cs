#define HTTP_DEBUGGER_DEV
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace QuickEye.WebTools
{
    /// <summary>
    /// Runtime API for game to log their HTTP calls. Those will be visible in Request Console Window
    /// </summary>
    public static partial class WebLogger
    {
        internal static event Action<string, HttpRequestMessage, string> LoggedRequest;
        internal static event Action<string, HttpResponseMessage, string> LoggedResponse;
        internal static event Action<string, UnityWebRequest, string> LoggedUnityRequest;

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