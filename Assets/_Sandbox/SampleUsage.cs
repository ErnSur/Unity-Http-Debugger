using System;
using System.Net.Http;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace QuickEye.RequestWatcher.LoggingSample
{
    public class SampleUsage : MonoBehaviour
    {
        [SerializeField]
        private float requestInterval=3;

        private float _remainingTimeToRequest;
        private static int _logIndex = -1;

        private void Update()
        {
            _remainingTimeToRequest -= Time.deltaTime;
            if (!(_remainingTimeToRequest <= 0))
                return;
            SendSampleRequest();
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

        private static int Repeat(int value, int maxValue)
        {
            return value % maxValue;
        }
    }
}