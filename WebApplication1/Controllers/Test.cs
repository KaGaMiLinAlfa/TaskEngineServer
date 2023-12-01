using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;
using Worker2.Comm;
using System.IO;
using Ubiety.Dns.Core;

namespace Worker2.Controllers
{
    public class TestController : BaseController
    {
        private static int concount = 0;
        [HttpGet]
        public async Task GetNotReadCount()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await Echo(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        private async Task Echo(WebSocket webSocket)
        {
            concount++;
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            Task.Run(() =>
            {
                while (!result.CloseStatus.HasValue && !webSocket.CloseStatus.HasValue)
                {
                    var serverMsg = Encoding.UTF8.GetBytes($"服务器时间: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")};总连接:{concount}");

                    if (webSocket.State == WebSocketState.Closed || webSocket.State == WebSocketState.CloseSent)
                    {
                        break;
                    }
                    webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), result.MessageType, result.EndOfMessage, CancellationToken.None).GetAwaiter().GetResult();

                    System.Threading.Thread.Sleep(1000);
                }
            }).GetAwaiter();

            while (!result.CloseStatus.HasValue)
            {

                if (buffer.Length > result.Count)
                {
                    var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (msg == "end")
                    {
                        //await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, result.CloseStatusDescription, CancellationToken.None);
                        break;
                    }
                }





                var serverMsg = Encoding.UTF8.GetBytes($"服务端收到信息: {Encoding.UTF8.GetString(buffer)}");
                await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "None Msg", CancellationToken.None);
            concount--;
        }

        [HttpGet]
        public async Task EventSteam()
        {
            Response.ContentType = "text/event-stream";
            var randomString = GenerateRandomString(50);
            int index = 0;
            while (index < randomString.Length)
            {
                var num = new Random().Next(1, 5);
                var fiveChars = randomString.Substring(index, Math.Min(num, randomString.Length - index));
                index += num;
                await Response.WriteAsync(fiveChars);
                await Task.Delay(100);
            }
        }

        static string GenerateRandomString(int length)
        {
            return "123456789123456789123456789123456789123456789123456789";
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] randomArray = new char[length];

            for (int i = 0; i < length; i++)
                randomArray[i] = chars[random.Next(chars.Length)];

            return new string(randomArray);
        }


        [Route("getFileById")]
        public FileResult getFileById(int fileId)
        {
            return PhysicalFile($"C:/movies/", "application/octet-stream", enableRangeProcessing: true);
        }
    }
}
