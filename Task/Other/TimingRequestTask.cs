using BaseTaskManager;
using LogManager;
using Newtonsoft.Json;
using SDK.MiDuo.CodeService.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;

namespace Task.Other
{
    public class TimingRequestTask : BaseTask
    {
        public override void Run()
        {
            var resultStr = Get($"{OnlineDomain}/manager/ResendBrandRedPacket");

            if (resultStr == "重发0条")
                LogHelper.Info($"没有重发数据");
            else
                LogHelper.Warn($"返回信息:{resultStr}");
        }

        public static string OnlineDomain = "http://10.66.199.20:10099";
        public static string Get(string url)
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
                    var rep = client.GetAsync(url).GetAwaiter().GetResult();
                    reStr = rep.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

                return reStr;
            }
            catch (Exception ex)
            {
                throw new Exception($"米多接口错误!错误信息:{ex.Message}");
            }
        }

    }
}
