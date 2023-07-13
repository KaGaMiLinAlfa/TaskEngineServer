using SDK.MiDuo.CodeService.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using System.IO;

namespace SDK.MiDuo.CodeService
{
    public class CodeServiceSDK
    {
        public static string Domain = "http://localhost:13509";
        //public static string OnlineDomain = "http://10.66.199.20:20112";
        public static string OnlineDomain = "http://localhost:41393";
        //public static string MiDuo_MemberLogin = ConfigurationManager.AppSettings["MiDuo_MemberLogin"].ToString();

        public static BaseResponseModel<QueryOrderApplyResponse> QueryOrderApply(QueryOrderApplyRequest request)
        {
            return Post<QueryOrderApplyResponse>(OnlineDomain + "/api/v2/ApplyAndPackage/QueryOrderApply", request);
        }

        public static BaseResponseModel<string> GetCloudFileUrl(string url)
        {
            return Post<string>(OnlineDomain + "/api/v2/CodeHandle/GetCloudFileUrl", url);
        }

        public static BaseResponseModel<ValidateCodeResponse> CheckCode(string code)
        {
            return Post<ValidateCodeResponse>(OnlineDomain + "/api/v2/Validate/CheckCode", new { IP = "123", Code = code });
        }

        public static BaseResponseModelV1<ImportFileResult> ImportFangcuanRelation(ImportRelationRequest request)
        {
            return PostV1<ImportFileResult>(OnlineDomain + "/api/v1/CodeImport/ImportFangcuanRelation", request);
        }




        public static BaseResponseModel<int> GetCodeRelationCount(CodeRelation request)
        {
            return Post<int>(OnlineDomain + "/api/v2/CodeHandle/GetCodeRelationCount", request);
        }

        public static BaseResponseModel<int> DeleteCodeRelation(CodeRelation request)
        {
            return Post<int>(OnlineDomain + "/api/v2/CodeHandle/DeleteCodeRelation", request);
        }

        public static async Task<BaseResponseModel<string>> UploadFileWithParameters(string path, string memberLogin)
        {
            using (var client = new HttpClient())
            {
                var reStr = string.Empty;
                using var formData = new MultipartFormDataContent();
                using var fileStream = File.Open(path, FileMode.Open);
                var fileContent = new StreamContent(fileStream);
                formData.Add(fileContent, "file", Path.GetFileName(path));
                formData.Add(new StringContent("memberLogin2"), "memberLogin");


                try
                {
                    var rep = await client.PostAsync(OnlineDomain + "/api/v2/CodeHandle/Upload", formData);

                    reStr = rep.Content.ReadAsStringAsync().GetAwaiter().GetResult();


                    var resultModel = JsonConvert.DeserializeObject<BaseResponseModel<string>>(reStr);

                    //if (resultModel?.Return_code != 0 && resultModel?.Return_msg != "OK")
                    //    LogHelper.WriteLogs($"米多接口数据异常，接口:{url},参数:{JsonConvert.SerializeObject(data, jsonSetting)},返回数据:{reStr}", _module);

                    return resultModel;
                }
                catch (Exception ex)
                {
                    //LogHelper.WriteLogs($"米多接口错误，接口:{url},参数:{JsonConvert.SerializeObject(data, jsonSetting)},返回数据:{reStr}", _module);
                    throw new Exception($"米多接口错误!错误信息:{ex.Message}");
                }
            }
        }

        public static BaseResponseModelV1<T> PostV1<T>(string url, object data)
        {
            var reStr = string.Empty;

            //var proxyUri = new Uri("http://212.64.92.183:9808");
            //var proxy = new WebProxy(proxyUri);
            //proxy.Credentials = new NetworkCredential("yaofan", "zxqd0S4Mt4");
            //var httpClientHandler = new HttpClientHandler
            //{
            //    Proxy = proxy,
            //    UseProxy = true,
            //};

            try
            {
                //using (var client = new HttpClient(httpClientHandler))
                using (var client = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(data);

                    var rep = client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"))
                    .GetAwaiter().GetResult();

                    reStr = rep.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

                var resultModel = JsonConvert.DeserializeObject<BaseResponseModelV1<T>>(reStr);

                //if (resultModel?.Return_code != 0 && resultModel?.Return_msg != "OK")
                //    LogHelper.WriteLogs($"米多接口数据异常，接口:{url},参数:{JsonConvert.SerializeObject(data, jsonSetting)},返回数据:{reStr}", _module);

                return resultModel;
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLogs($"米多接口错误，接口:{url},参数:{JsonConvert.SerializeObject(data, jsonSetting)},返回数据:{reStr}", _module);
                throw new Exception($"米多接口错误!错误信息:{ex.Message}");
            }
        }

        public static BaseResponseModel<T> Post<T>(string url, object data)
        {
            var reStr = string.Empty;

            //var proxyUri = new Uri("http://212.64.92.183:9808");
            //var proxy = new WebProxy(proxyUri);
            //proxy.Credentials = new NetworkCredential("yaofan", "zxqd0S4Mt4");
            //var httpClientHandler = new HttpClientHandler
            //{
            //    Proxy = proxy,
            //    UseProxy = true,
            //};

            try
            {
                //using (var client = new HttpClient(httpClientHandler))
                using (var client = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(data);

                    var rep = client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"))
                    .GetAwaiter().GetResult();

                    reStr = rep.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

                var resultModel = JsonConvert.DeserializeObject<BaseResponseModel<T>>(reStr);

                //if (resultModel?.Return_code != 0 && resultModel?.Return_msg != "OK")
                //    LogHelper.WriteLogs($"米多接口数据异常，接口:{url},参数:{JsonConvert.SerializeObject(data, jsonSetting)},返回数据:{reStr}", _module);

                return resultModel;
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLogs($"米多接口错误，接口:{url},参数:{JsonConvert.SerializeObject(data, jsonSetting)},返回数据:{reStr}", _module);
                throw new Exception($"米多接口错误!错误信息:{ex.Message}");
            }
        }

        public static BaseResponseModel<T> PostText<T>(string url, object data)
        {
            var reStr = string.Empty;

            //var proxyUri = new Uri("http://212.64.92.183:9808");
            //var proxy = new WebProxy(proxyUri);
            //proxy.Credentials = new NetworkCredential("yaofan", "zxqd0S4Mt4");
            //var httpClientHandler = new HttpClientHandler
            //{
            //    Proxy = proxy,
            //    UseProxy = true,
            //};

            try
            {
                //using (var client = new HttpClient(httpClientHandler))
                using (var client = new HttpClient())
                {
                    var zxc = @"zxcasd";

                    var content = new StringContent(zxc.ToString(), Encoding.UTF8, "text/json");


                    var rep = client.PostAsync(url, content).GetAwaiter().GetResult();

                    reStr = rep.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

                var resultModel = JsonConvert.DeserializeObject<BaseResponseModel<T>>(reStr);

                //if (resultModel?.Return_code != 0 && resultModel?.Return_msg != "OK")
                //    LogHelper.WriteLogs($"米多接口数据异常，接口:{url},参数:{JsonConvert.SerializeObject(data, jsonSetting)},返回数据:{reStr}", _module);

                return resultModel;
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLogs($"米多接口错误，接口:{url},参数:{JsonConvert.SerializeObject(data, jsonSetting)},返回数据:{reStr}", _module);
                throw new Exception($"米多接口错误!错误信息:{ex.Message}");
            }
        }

    }
}
