using UnityEngine.Networking;
using System;
using System.Text;

namespace Anatidae {
    public class AnatidaeProxyWebRequest {
        # if UNITY_EDITOR
        static bool useProxy = false;
        # else
        static bool useProxy = true;
        # endif

        public static UnityWebRequest Get(string uri)
        {
            if (useProxy) uri = $"http://localhost:3000/proxy?url={new Uri(uri)}";
            UnityEngine.Debug.Log(uri);
            return new UnityWebRequest(uri, "GET", new DownloadHandlerBuffer(), null);
        }

        public static UnityWebRequest Post(string uri, string postData, string contentType)
        {
            if (useProxy) uri = $"http://localhost:3000/proxy?url={new Uri(uri)}";
            UnityEngine.Debug.Log(uri);
            UnityWebRequest request = new UnityWebRequest(uri, "POST");
            SetupPost(ref request, postData, contentType);
            return request;
        }

        private static void SetupPost(ref UnityWebRequest request, string postData, string contentType)
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", contentType);
            byte[] bytes = new UTF8Encoding().GetBytes(postData);
            request.uploadHandler = new UploadHandlerRaw(bytes)
            {
                contentType = contentType
            };
        }
    }
}