using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo("Sandbox.Editor")]

namespace QuickEye.RequestWatcher
{
    internal class RequestData : ScriptableObject, IDisposable
    {
        private static readonly HttpClient _Client = new HttpClient();
        internal const string NamePropertyName = "m_Name";
        public event Action Modified;
        public string url;
        public HttpMethodType type;
        public string body;
        public List<Header> headers;
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

        //TODO: Add SendUnityRequest
        public async Task<HttpResponseMessage> SendAsync()
        {
            _Client.DefaultRequestHeaders.Accept.Clear();
            _Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage(type.ToHttpMethod(), url);
            foreach (var header in headers.Where(header => header.enabled))
            {
                request.Headers.Add(header.name, header.value);
            }

            if (type == HttpMethodType.Post)
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var res = await _Client.SendAsync(request);
            return res;
        }

        public void Dispose()
        {
            DestroyImmediate(this);
        }

        private void OnValidate()
        {
            Modified?.Invoke();
        }
    }
}