using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace BaseFramework
{
    public delegate void ResultCallback(string retCode, object data, object paging);
    public delegate void RequestErrorCallback();

    public class WebRequestHelper : MonoSingleton<WebRequestHelper>
    {
        public string AuthorizationToken = "";

        private string serverHost = "http://192.168.110.194:9901/api/v1/system/service";

        private IEnumerator API_Post(string query, string postData, ResultCallback callback = null, int retryCount = 3, RequestErrorCallback errorCallback = null)
        {
            var url = $"{serverHost}/{query}";

            var request = UnityWebRequest.Post(url, "POST");
            request.timeout = 10;
            request.downloadHandler = new DownloadHandlerBuffer();
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(postData))
            {
                contentType = "application/json"
            };

            if (!string.IsNullOrEmpty(AuthorizationToken))
            {
                request.SetRequestHeader("Authorization", AuthorizationToken);
            }

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                MDebug.LogError(request.error);

                if (retryCount > 0)
                {
                    retryCount--;
                    yield return API_Post(query, postData, callback, retryCount);
                }
                else
                {
                    errorCallback?.Invoke();
                }

                yield break;
            }
            WebRequestResultData resultData = JsonConvert.DeserializeObject<WebRequestResultData>(request.downloadHandler.text);

            MDebug.Log(request.downloadHandler.text);

            callback?.Invoke(resultData.retCode, resultData.data, resultData.paging);
        }

        public void CallPostApi(string query, string postData, ResultCallback callback = null, int retryCount = 3, RequestErrorCallback errorCallback = null)
        {
            StartCoroutine(API_Post(query, postData, callback, retryCount, errorCallback));
        }

        private IEnumerator API_Get(string query, Dictionary<string, string> param = null, ResultCallback callback = null, int retryCount = 3, RequestErrorCallback errorCallback = null)
        {
            var url = $"{serverHost}/{query}";

            if (param != null)
            {
                string paramUrl = "";
                foreach (var child in param.Keys)
                {
                    paramUrl += $"{child}={param[child]}&";
                }

                url = $"{url}?{paramUrl}";
            }

            var request = UnityWebRequest.Get(url);
            request.timeout = 10;

            if (!string.IsNullOrEmpty(AuthorizationToken))
            {
                request.SetRequestHeader("Authorization", AuthorizationToken);
            }

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                MDebug.LogError(request.error);

                if (retryCount > 0)
                {
                    retryCount--;
                    yield return API_Get(query, param, callback, retryCount);
                }
                else
                {
                    errorCallback?.Invoke();
                }

                yield break;
            }
            WebRequestResultData resultData = JsonConvert.DeserializeObject<WebRequestResultData>(request.downloadHandler.text);

            MDebug.Log(request.downloadHandler.text);

            callback?.Invoke(resultData.retCode, resultData.data, resultData.paging);
        }

        public void CallGetApi(string query, Dictionary<string, string> param = null, ResultCallback callback = null, int retryCount = 3, RequestErrorCallback errorCallback = null)
        {
            StartCoroutine(API_Get(query, param, callback, retryCount, errorCallback));
        }
    }

    public class WebRequestResultData
    {
        public string retCode;
        public string retMessage;
        public object data;
        public object paging;
    }
}
