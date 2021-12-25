using System;
using System.Net.Http;

namespace QuickEye.RequestWatcher
{
    internal static class HttpReqTypeExtensions
    {
        public static HttpMethod ToHttpMethod(this HttpReqType type)
        {
            switch (type)
            {
                case HttpReqType.Get:
                    return HttpMethod.Get;
                case HttpReqType.Put:
                    return HttpMethod.Put;
                case HttpReqType.Post:
                    return HttpMethod.Post;
                case HttpReqType.Delete:
                    return HttpMethod.Delete;
                case HttpReqType.Head:
                    return HttpMethod.Head;
                case HttpReqType.Options:
                    return HttpMethod.Options;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}