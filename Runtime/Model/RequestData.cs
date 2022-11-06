using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


namespace QuickEye.WebTools
{
    [CreateAssetMenu]
    public class RequestData : ScriptableObject, IDisposable
    {
        private static readonly HttpClient _Client = new HttpClient();
        internal const string NamePropertyName = "m_Name";
        internal event Action Modified;
        public string url;
        public HttpMethodType type;
        public string content;

        public List<Header> headers = new List<Header>();

        public ResponseData lastResponse;

        [HideInInspector]
        public bool isReadOnly;

        protected RequestData() { }

        public static RequestData Create() => CreateInstance<RequestData>();
        public static RequestData Create(RequestData prototype) => Create<RequestData>(prototype);
        protected static T Create<T>() where T : RequestData => CreateInstance<T>();

        protected static T Create<T>(RequestData prototype) where T : RequestData
        {
            var i = CreateInstance<T>();
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(prototype), i);
            i.name = prototype.name;
            return i;
        }

        public HttpRequestMessage ToHttpRequestMessage()
        {
            var result = new HttpRequestMessage(type.ToHttpMethod(), url);

            var contentType = headers
                .Where(h => h.enabled)
                .Where(h => h.name == "Content-Type").ToArray();

            foreach (var (name, value) in headers.Where(header => header.enabled).Except(contentType))
                result.Headers.Add(name, value);
            if (!string.IsNullOrWhiteSpace(content) && contentType.Length > 0)
                result.Content = new StringContent(content, Encoding.UTF8, contentType.Last().value);
            return result;
        }

        public UnityWebRequest ToUnityWebRequest()
        {
            var result = new UnityWebRequest();
            result.method = type.ToString();
            result.url = url;
            result.downloadHandler = new DownloadHandlerBuffer();
            var contentType = headers
                .Where(h => h.enabled)
                .LastOrDefault(h => h.name == "Content-Type")?.value;
            if (contentType != null)
            {
                var bytes = Encoding.UTF8.GetBytes(content);
                result.uploadHandler = new UploadHandlerRaw(bytes);
                result.uploadHandler.contentType = contentType;
            }

            foreach (var (name, value) in headers.Where(h => h.enabled))
            {
                result.SetRequestHeader(name, value);
            }

            return result;
        }

        public void Dispose()
        {
#if UNITY_EDITOR
            DestroyImmediate(this);
#else
            Destroy(this);
#endif
        }

        protected virtual void OnValidate()
        {
            Modified?.Invoke();
        }
    }
}