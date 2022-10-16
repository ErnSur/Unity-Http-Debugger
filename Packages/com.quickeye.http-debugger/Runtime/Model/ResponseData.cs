using System;
using System.Collections.Generic;

namespace QuickEye.WebTools
{
    [Serializable]
    public class ResponseData
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