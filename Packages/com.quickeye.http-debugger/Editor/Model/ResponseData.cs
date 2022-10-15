using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    [Serializable]
    internal class ResponseData
    {
        public int statusCode;
        public string payload;
        public List<Header> headers;

        public ResponseData(int statusCode)
        {
            this.statusCode = statusCode;
        }

        public ResponseData(int statusCode, string payload, IEnumerable<Header> headers)
        {
            this.statusCode = statusCode;
            this.payload = payload;
            this.headers = new List<Header>(headers);
        }
    }
}