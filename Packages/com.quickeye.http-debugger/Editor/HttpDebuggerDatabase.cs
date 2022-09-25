using System.Collections.Generic;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    [CreateAssetMenu]
    internal class HttpDebuggerDatabase : ScriptableObject
    {
        public static HttpDebuggerDatabase Instance => GetInstance();
        public List<HDRequest> playmodeRequests;
        private static HttpDebuggerDatabase GetInstance()
        {
            return Resources.Load<HttpDebuggerDatabase>("Database");
        }
    }
}