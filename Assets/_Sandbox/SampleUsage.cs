using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using QuickEye.RequestWatcher;
using UnityEditor;
using UnityEngine;

public class SampleUsage : MonoBehaviour
{
    public HttpStatusCode code;

    [ContextMenu("Log")]
    public void Test()
    {
        Debug.Log($"MES: {code}");
    }

    [MenuItem("Test/SendSampleReq")]
    public static void SendSampleRequest()
    {
        SendRequestAsync();
    }

    private static async void SendRequestAsync()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("http://dummy.restapiexample.com/api/v1/employee/1"),
        };
        using (var response = await client.SendAsync($"Employee", request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Debug.Log(body);
        }
    }
}