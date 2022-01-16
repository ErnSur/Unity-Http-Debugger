using System;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    [Serializable]
    internal class HDResponse
    {
        public int statusCode;
        public string payload;
        public string headers;

        public HDResponse(int statusCode)
        {
            this.statusCode = statusCode;
        }

        public HDResponse(int statusCode, string payload, string headers)
        {
            this.statusCode = statusCode;
            this.payload = payload;
            this.headers = headers;
        }
    }
}