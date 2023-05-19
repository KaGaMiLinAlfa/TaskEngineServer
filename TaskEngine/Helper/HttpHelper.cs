using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text.Json.Serialization;
using System.Text;
using System;
using Newtonsoft.Json;
using System.IO;

namespace TaskEngineServer.Helper
{
    public class HttpHelper
    {
        private static HttpClientHandler _handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip };

        public TResult Post<TResult>(string url, object input) where TResult : class
        {
            using (var client = new HttpClient(_handler))
            {
                client.Timeout = TimeSpan.FromSeconds(120);

                var requestJson = input == null ? "" : JsonConvert.SerializeObject(input);
                using (var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json"))
                using (var response = client.PostAsync(url, requestContent))
                {
                    var resultTask = response.Result.Content.ReadAsStringAsync();

                    if (Type.GetTypeCode(typeof(TResult)) == TypeCode.String)
                        return resultTask.Result as TResult;

                    try
                    {
                        return JsonConvert.DeserializeObject<TResult>(resultTask.Result);
                    }
                    catch (Exception e)
                    {
                        throw e.GetInnerMsg();
                    }
                }
            }
        }

        public TResult Get<TResult>(string url, string param) where TResult : class
        {
            if (url.LastOrDefault() != '?' && param.FirstOrDefault() != '&')
                url += '?';
            url += param;

            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip }))
            {
                client.Timeout = TimeSpan.FromSeconds(120);

                using (var response = client.GetAsync(url))
                {
                    var result = response.Result;
                    if (result.StatusCode != HttpStatusCode.OK)
                        throw new Exception(response.Result.ReasonPhrase ?? "消息为空");

                    var resultContent = result.Content.ReadAsStringAsync();

                    if (Type.GetTypeCode(typeof(TResult)) == TypeCode.String)
                        return resultContent.Result as TResult;

                    try
                    {
                        return JsonConvert.DeserializeObject<TResult>(resultContent.Result);
                    }
                    catch (Exception e)
                    {
                        throw e.GetInnerMsg();
                    }
                }
            }
        }

        public static string DownloadFile(string url, string savePath)
        {
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            WebClient client = new WebClient();
            var savePathName = savePath + Path.GetFileName(url);
            client.DownloadFile(url, savePathName);

            return savePathName;
        }
    }
}
