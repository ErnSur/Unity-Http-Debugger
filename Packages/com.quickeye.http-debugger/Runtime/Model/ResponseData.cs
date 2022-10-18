using System;
using System.Collections.Generic;

namespace QuickEye.WebTools
{
    [Serializable]
    public class ResponseData
    {
        public int statusCode;
        public string content;
        public List<Header> headers;

        public ResponseData(int statusCode)
        {
            this.statusCode = statusCode;
        }

        public ResponseData(int statusCode, string content, IEnumerable<Header> headers)
        {
            this.statusCode = statusCode;
            this.content = content;
            this.headers = new List<Header>(headers);
        }
    }
}