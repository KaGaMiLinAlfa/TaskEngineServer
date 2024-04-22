using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Worker2.Comm;

namespace Worker2.Controllers
{
    public class MiDuoToolController: BaseController
    {



        [HttpGet]
        public string GetVJIFenInfo(string code, string RegionCode)
        {
            var url = $"{CodeService_FW_Domain}/api/v1/Manager/GetVJIFenInfo?code={code}&RegionCode={RegionCode}";
            return Get(url);
        }
        
        [HttpGet]
        public string GetMiDuoCodeInfo(string code)
        {
            var url = $"{CodeService_FW_Domain}/api/v2/validate/checkcode";
            return Post(url,new {code,Ip="123"});
        }

  
        private static readonly string CodeService_FW_Domain = "http://10.66.199.20:20112";

        private static string Get(string url)
        {
            var reStr = string.Empty;

            var proxyUri = new Uri("http://212.64.92.183:9808");
            var proxy = new WebProxy(proxyUri);
            proxy.Credentials = new NetworkCredential("yaofan", "zxqd0S4Mt4");
            var httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
                UseProxy = true,
            };

            try
            {
                using var client = new HttpClient(httpClientHandler);
                var rep = client.GetAsync(url).GetAwaiter().GetResult();
                reStr = rep.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return reStr;
            }
            catch (Exception ex)
            {
                throw new Exception($"米多接口错误!错误信息:{ex.Message}");
            }
        }
        
        public static string Post(string url, object data)
        {
            var reStr = string.Empty;

            var proxyUri = new Uri("http://212.64.92.183:9808");
            var proxy = new WebProxy(proxyUri);
            proxy.Credentials = new NetworkCredential("yaofan", "zxqd0S4Mt4");
            var httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
                UseProxy = true,
            };

            try
            {
                using (var client = new HttpClient(httpClientHandler))
                {
                    var rep = client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"))
                        .GetAwaiter().GetResult();

                    reStr = rep.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLogs($"米多接口错误，接口:{url},参数:{JsonConvert.SerializeObject(data, jsonSetting)},返回数据:{reStr}", _module);
                throw new Exception($"米多接口错误!错误信息:{ex.Message}");
            }

            return reStr;
        }

    }
}