using System.Collections;
using UnityEngine;

namespace QuickEye.WebTools.LoggingSample
{
    public class RequestSender : MonoBehaviour
    {
        [SerializeField]
        private float requestInterval = 3;

        [SerializeField]
        private RequestData requestData;

        private float _remainingTimeToRequest;

        private void Update()
        {
            _remainingTimeToRequest -= Time.deltaTime;
            if (!(_remainingTimeToRequest <= 0))
                return;

            StartCoroutine(SendUnityRequest());
            _remainingTimeToRequest = requestInterval;
        }
        
        private IEnumerator SendUnityRequest()
        {
            var r = requestData.ToUnityWebRequest();
            yield return r.SendWebRequest();
            r.Log(requestData.name);
        }
    }
}