using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace BaseFramework
{
    public delegate void ResultCallback(int status, string data);
    public delegate void RequestErrorCallback();
    public class WebRequestHelper : MonoSingleton<WebRequestHelper>
    {
        private string serverHost = "";

        private IEnumerator API(string query, ResultCallback callback = null, Dictionary<string, string> formData = null, int retryCount = 3, RequestErrorCallback errorCallback = null)
        {
            if (formData == null)
            {
                formData = new Dictionary<string, string>();
            }

            var url = $"{serverHost}/api/{query}/";

            var request = UnityWebRequest.Post(url, formData);
            request.timeout = 10;

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                MDebug.LogError(request.error);

                if (retryCount > 0)
                {
                    retryCount--;
                    yield return API(query, callback, formData, retryCount);
                }
                else
                {
                    errorCallback?.Invoke();
                }

                yield break;
            }

            WebRequestResultData resultData = JsonConvert.DeserializeObject<WebRequestResultData>(request.downloadHandler.text);
            callback?.Invoke(resultData.status, resultData.data);
        }
    }

    public class WebRequestResultData
    {
        public int status;
        public string data;
    }
}
