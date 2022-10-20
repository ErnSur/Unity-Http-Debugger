using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    // HTTP status codes as per RFC 2616.
    public enum HttpStatusCode2
    {
        // Informational 1xx
        Continue = 100,
        SwitchingProtocols = 101,
        Processing = 102,
        EarlyHints = 103,
 
        // Successful 2xx
        OK = 200,
        Created = 201,
        Accepted = 202,
        NonAuthoritativeInformation = 203,
        NoContent = 204,
        ResetContent = 205,
        PartialContent = 206,
        MultiStatus = 207,
        AlreadyReported = 208,
 
        IMUsed = 226,
 
        // Redirection 3xx
        MultipleChoices = 300,
        Ambiguous = 300,
        MovedPermanently = 301,
        Moved = 301,
        Found = 302,
        Redirect = 302,
        SeeOther = 303,
        RedirectMethod = 303,
        NotModified = 304,
        UseProxy = 305,
        Unused = 306,
        TemporaryRedirect = 307,
        RedirectKeepVerb = 307,
        PermanentRedirect = 308,
 
        // Client Error 4xx
        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        ProxyAuthenticationRequired = 407,
        RequestTimeout = 408,
        Conflict = 409,
        Gone = 410,
        LengthRequired = 411,
        PreconditionFailed = 412,
        RequestEntityTooLarge = 413,
        RequestUriTooLong = 414,
        UnsupportedMediaType = 415,
        RequestedRangeNotSatisfiable = 416,
        ExpectationFailed = 417,
        // From https://github.com/dotnet/runtime/issues/15650:
        // "It would be a mistake to add it to .NET now. See golang/go#21326,
        // nodejs/node#14644, requests/requests#4238 and aspnet/HttpAbstractions#915".
        // ImATeapot = 418
 
        MisdirectedRequest = 421,
        UnprocessableEntity = 422,
        Locked = 423,
        FailedDependency = 424,
 
        UpgradeRequired = 426,
 
        PreconditionRequired = 428,
        TooManyRequests = 429,
 
        RequestHeaderFieldsTooLarge = 431,
 
        UnavailableForLegalReasons = 451,
 
        // Server Error 5xx
        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        HttpVersionNotSupported = 505,
        VariantAlsoNegotiates = 506,
        InsufficientStorage = 507,
        LoopDetected = 508,
 
        NotExtended = 510,
        NetworkAuthenticationRequired = 511
    }
    
    internal static class HttpStatusCodeUtil
    {
        public static bool IsValidHttpStatusCode(int code)
        {
            return code >= 100 && code < 600;
        }
        
        public static string StatusCodeToNiceString(int statusCode)
        {
            var isDefined = Enum.IsDefined(typeof(HttpStatusCode2), statusCode);

            var text = $"{statusCode}";
            if (isDefined)
                text += $" {ObjectNames.NicifyVariableName(((HttpStatusCode2)statusCode).ToString())}";
            return text;
        }
        
        private static StatusCodeType GetCodeType(int code)
        {
            if (code > 99 && code < 200)
                return StatusCodeType.Info;
            if (code < 300)
                return StatusCodeType.Successful;
            if (code < 400)
                return StatusCodeType.Redirection;
            if (code < 500)
                return StatusCodeType.ClientError;
            if (code < 600)
                return StatusCodeType.ServerError;
            else
                return StatusCodeType.Undefined;
        }

        public static void ToggleStatusCodeClass(VisualElement ve, int code)
        {
            var codeType = GetCodeType(code);
            ve.EnableInClassList("status-code-100",codeType == StatusCodeType.Info);
            ve.EnableInClassList("status-code-200",codeType == StatusCodeType.Successful);
            ve.EnableInClassList("status-code-300",codeType == StatusCodeType.Redirection);
            ve.EnableInClassList("status-code-400",codeType == StatusCodeType.ClientError);
            ve.EnableInClassList("status-code-500",codeType == StatusCodeType.ServerError);
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
            ServerError,
            Undefined
        }
    }
}