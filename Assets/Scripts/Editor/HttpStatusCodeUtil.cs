using System;
using System.Net;

namespace QuickEye.RequestWatcher
{
    internal static class HttpStatusCodeUtil
    {
        public static bool IsValidHttpStatusCode(int code)
        {
            return code >= 100 && code < 600;
        }
        private static StatusCodeType GetCodeType(HttpStatusCode code)
        {
            var codeInt = (int)code;
            if (codeInt < 200)
                return StatusCodeType.Info;
            if (codeInt < 300)
                return StatusCodeType.Successful;
            if (codeInt < 400)
                return StatusCodeType.Redirection;
            if (codeInt < 500)
                return StatusCodeType.ClientError;
            if (codeInt < 600)
                return StatusCodeType.ServerError;
            throw new ArgumentOutOfRangeException($"Unexpected HttpStatusCode value");
        }

        enum StatusCodeType
        {
            /*
            Informational responses (100–199)
            Successful responses (200–299)
            Redirection messages (300–399)
            Client error responses (400–499)
            Server error responses (500–599)
             */
            Info,
            Successful,
            Redirection,
            ClientError,
            ServerError
        }
    }
}