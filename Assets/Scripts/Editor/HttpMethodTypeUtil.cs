using System;
using System.Net.Http;

namespace QuickEye.RequestWatcher
{
    internal static class HttpMethodTypeUtil
    {
        public static HttpMethod ToHttpMethod(this HttpMethodType type)
        {
            switch (type)
            {
                case HttpMethodType.Get:
                    return HttpMethod.Get;
                case HttpMethodType.Put:
                    return HttpMethod.Put;
                case HttpMethodType.Post:
                    return HttpMethod.Post;
                case HttpMethodType.Delete:
                    return HttpMethod.Delete;
                case HttpMethodType.Head:
                    return HttpMethod.Head;
                case HttpMethodType.Options:
                    return HttpMethod.Options;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public static HttpMethodType FromHttpMethod(HttpMethod method)
        {
            switch (method.Method)
            {
                case "GET":
                    return HttpMethodType.Get;
                case "PUT":
                    return HttpMethodType.Put;
                case "POST":
                    return HttpMethodType.Post;
                case "DELETE":
                    return HttpMethodType.Delete;
                case "HEAD":
                    return HttpMethodType.Head;
                case "OPTIONS":
                    return HttpMethodType.Options;
                default:
                    throw new ArgumentOutOfRangeException(nameof(HttpMethodType), method.Method, "No enum value to match");
            }
        }
    }
}