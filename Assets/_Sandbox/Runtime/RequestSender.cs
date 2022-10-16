using System;
using System.Collections;
using System.Net.Http;
using System.Threading;
using UnityEditor;
using UnityEngine;
using QuickEye.WebTools;

namespace QuickEye.RequestWatcher.LoggingSample
{
    public class RequestSender : MonoBehaviour
    {
        [SerializeField]
        private float requestInterval = 3;

        private float _remainingTimeToRequest;
        private static int _logIndex = -1;

        private void Update()
        {
            _remainingTimeToRequest -= Time.deltaTime;
            if (!(_remainingTimeToRequest <= 0))
                return;

            StartCoroutine(SendUnityRequest());
            //SendSampleRequest();
            _remainingTimeToRequest = requestInterval;
        }

        [MenuItem("Debug Requests/Send Get Request")]
        public static void SendSampleRequest()
        {
            SendRequestAsync();
        }

        private static async void SendRequestAsync(CancellationToken cancellationToken = default)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://dummy.restapiexample.com/api/v1/employee/1"),
            };

            _logIndex = ++_logIndex % 3;
            using (var response = await client.SendAsync($"Req {_logIndex}", request, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
            }
        }

        private static IEnumerator SendUnityRequest()
        {
            var r = new UnityWebRequest("http://dummy.restapiexample.com/api/v1/employee/1", "GET");
            r.SetRequestHeader("Header1", "Test");
            yield return r.SendWebRequest();
            var localIndex = _logIndex = ++_logIndex % 3;

            r.Log($"Unity Req {localIndex}");
        }
    }
}