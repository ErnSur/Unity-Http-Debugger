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
        private float requestInterval;

        private float _remainingTimeToRequest;

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
            
            
            using (var response = await client.SendAsync("Sample Get Request",request, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
            }
        }
    }
}