using BaseTaskManager;
using SDK.MiDuo.CodeService;
using SDK.MiDuo.CodeService.Model;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Web;

namespace Task.CodeHandle
{
    public class CodeHandleService : BaseTask
    {
        public override void Run()
        {
            throw new NotImplementedException();
        }


        public void Handle()
        {
            //客户码包文件地址
            var packerfileurl = "D:\\MiDuo\\Code\\TaskEngineServer\\test\\bin\\Debug\\netcoreapp3.1\\temp\\test.txt";

            //var tmepPackerFile = DownloadFilePath(packerfileurl);
            var tmepPackerFile = packerfileurl;

            var firstLine = GetCodePackageFirstLine(tmepPackerFile);

            var firstSplit = firstLine.Split(',');

            //这里未来添加类型判断
            if (firstSplit.Length != 2)
                throw new Exception("码包不是两种码");

            var serialNumber = firstSplit[0];
            if (!Regex.IsMatch(serialNumber, @"^\d+$"))
                throw new Exception("第一列码不为全数字流水号");

            var qrcodeUrl = firstSplit[1];
            if (!Regex.IsMatch(qrcodeUrl, @"^http") && qrcodeUrl.IndexOf('/') > 0)
                throw new Exception("第二列不为二维码连接");

            var qrcode = qrcodeUrl.Substring(qrcodeUrl.LastIndexOf('/') + 1);

            var firstCodeInfo = CodeServiceSDK.CheckCode(qrcode);
            //这里需要添加假码处理


            var bacthCode = firstCodeInfo.Return_data.CodeBatch;
            var segmentCode = firstCodeInfo.Return_data.CodeSegment;

            var applyOrder = CodeServiceSDK.QueryOrderApply(new QueryOrderApplyRequest
            {
                CodeSegment = segmentCode,
                CodeBatch = bacthCode,
            });

            var bacthInfo = applyOrder?.Return_data?.Results?.FirstOrDefault();
            if (bacthInfo == null)
                throw new Exception("码信息为空");

            if (bacthInfo.LogisticsRelatedType != 1)//SerialNoType
                throw new Exception("批次不是后关联");


            var testurl1 = HttpUtility.UrlEncode(bacthInfo.FilePath);
            var testurl2 = HttpUtility.UrlEncode(HttpUtility.UrlEncode(bacthInfo.FilePath));

            var asdas = HttpUtility.UrlDecode("%252fMiCodeUploadCodePackage%252f10003911%252f0605-157-%25e6%259d%25a5%25e5%25ae%25a2(%25e6%2595%25b0%25e9%2587%258f%25ef%25bc%259a10000%25e6%259e%259a)%25e3%2580%2590%25e6%2599%25ba%25e8%2583%25bd%25e8%2590%25a5%25e9%2594%2580%25e7%25a0%2581%25e2%2597%258f%25e9%2598%25b2%25e7%25aa%259c%25e7%2589%25a9%25e6%25b5%2581%25e5%2590%258e%25e5%2585%25b3%25e8%2581%2594%25e3%2580%2591.rar");

            var bacthPackRar = CodeServiceSDK.GetCloudFileUrl(bacthInfo.FilePath);




            var bacthPackRarPassword = bacthInfo.ZipPwd;
            var tempBacthPackFileRar = DownloadFilePath(bacthPackRar.Return_data);

            var extractPath = Path.Combine(AppContext.BaseDirectory, "Temp", "Extract", $"{segmentCode}-{bacthCode}");
            if (!Directory.Exists(extractPath))
                Directory.CreateDirectory(extractPath);


            using (var archive = ArchiveFactory.Open(tempBacthPackFileRar, new ReaderOptions() { Password = bacthPackRarPassword }))
                foreach (var entry in archive.Entries)
                    if (!entry.IsDirectory)
                        entry.WriteToDirectory(extractPath, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });


            var firstFilePath = GetFirstFile(extractPath);

            var line = string.Empty;
            var bdeCodes = new Dictionary<string, string>();
            using (StreamReader reader = new StreamReader(firstFilePath))
            {
                int i = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var code = line.ToString().Split(',');

                    if (code.Length < 2)
                        throw new Exception("码包列数少于2！");

                    var codeindex = code.Length - 1;
                    var serialIndex = code.Length - 2;

                    if (!bdeCodes.ContainsKey(code[codeindex]))
                    {
                        if (i == 0)
                            i++;
                        else if (code[serialIndex].Length != 8)
                            throw new Exception("流水长度异常！");

                        bdeCodes.Add(code[codeindex], code[serialIndex]);
                    }
                }
            }


            var newDictionary = new Dictionary<string, ImportLine>();
            var ErrorDictionary = new Dictionary<string, ImportLine>();
            using (StreamReader reader = new StreamReader(tmepPackerFile))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var lineItems = line.Split(',');

                    if (lineItems.Length < 2)
                        throw new Exception("客户码包列数少于2！");

                    var cqcodeurl = lineItems[1];

                    if (bdeCodes.ContainsKey(cqcodeurl) && !newDictionary.ContainsKey(cqcodeurl))
                    {
                        if (lineItems[0].Length != 8)
                            throw new Exception("流水长度异常！");

                        var importLine = new ImportLine
                        {
                            LogisticNo = lineItems[0],
                            //QrCode = lineItems[1],
                            SerialNo = bdeCodes[cqcodeurl]
                        };

                        newDictionary.Add(cqcodeurl, importLine);
                    }
                    else
                    {
                        var importLine = new ImportLine
                        {
                            LogisticNo = lineItems[0],
                            //QrCode = lineItems[1],
                            SerialNo = ""
                        };

                        ErrorDictionary.Add(cqcodeurl, importLine);
                    }
                }
            }


            //如果有错误的码,则在这里警报,中断处理
            if (ErrorDictionary.Count > 0)
            {

            }
            ErrorDictionary = null;

            var inputPackfilePath = Path.Combine(AppContext.BaseDirectory, "Temp", "InputFile");
            if (!Directory.Exists(inputPackfilePath))
                Directory.CreateDirectory(inputPackfilePath);

            var inputPackfileName = Path.Combine(inputPackfilePath,
                $"Input-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}-{segmentCode}-{bacthCode}.txt");

            var codeCount = newDictionary.Count;
            using (var fs = new FileStream(inputPackfileName, FileMode.Create))
            using (var sw = new StreamWriter(fs))
                foreach (var codeInfo in newDictionary)
                {
                    sw.WriteLine($"{codeInfo.Value.LogisticNo},{codeInfo.Value.SerialNo}");
                    sw.Flush();
                }






            var codeRelationCount = CodeServiceSDK.GetCodeRelationCount(new CodeRelation
            {
                MemberLogin = bacthInfo.Memberlogin,
                Batch = bacthInfo.CodeBatch,
                Segment = bacthInfo.CodeSegment,
                BigSerialIsNull = true,
                StorageState = 0
            });

            //bacthInfo.Memberlogin, bacthInfo.CodeBatch, bacthInfo.CodeSegment, true, 0
            var type = GetCodeRelationDataStats(codeRelationCount, bacthInfo, newDictionary);
            newDictionary = null;//释放内存占用


            if (type == 0)
                throw new Exception("状态验证异常");


            if ((type & ImportOperatee.Delete) != 0)
            {
                var delectCount = CodeServiceSDK.DeleteCodeRelation(new CodeRelation
                {
                    MemberLogin = bacthInfo.Memberlogin,
                    Batch = bacthInfo.CodeBatch,
                    Segment = bacthInfo.CodeSegment,
                    BigSerialIsNull = false,
                    StorageState = -1
                });
            }


            BaseResponseModelV1<ValidateCodeResponse> importResult = null;
            if ((type & ImportOperatee.Import) != 0)
            {
                var uploadUrl = CodeServiceSDK.UploadFileWithParameters(inputPackfileName, bacthInfo.Memberlogin).Result;

                importResult = CodeServiceSDK.ImportFangcuanRelation(new ImportRelationRequest
                {
                    Path = uploadUrl.Return_data,
                    dateCount = codeCount,
                    codeType = 3,//后关联
                    type = bacthInfo.CodeType,
                    id = bacthInfo.BatchId,
                    Memberlogin = bacthInfo.Memberlogin
                });
            }










            //检查批次数据情况
            //1.检查批次数量
            //3.检查导入的码包+现有批次有没有超过上限
            //4.检查前十个码添加时间是否与码批次时间相吻合
            //2.检查是否有出入库(如果数量+现有批次不超过,则当第二次)
            //5.码包上传

            //6.上传调用处理
            //7.检查码包上传情况
            //8.回写日志





            //获取码包第一条信息
            //判断类型(添加类型判断)
            //获取第一个二维码
            //获取批次号
            //0.获取批次码包信息,判断处理方式
            //1.获取需要处理的批次码包
            //2.处理码包(需要识别跨码包)

            //3.检查批次码数据情况(多种情况)
            //4.上传码包
            //5.检查码包上传情况
            //6.完成
        }

        private ImportOperatee GetCodeRelationDataStats(BaseResponseModel<int> codeRelationCount, QueryOrderApplyDto bacthInfo, Dictionary<string, ImportLine> newDictionary)
        {

            if (codeRelationCount.Return_data == 0)
                return ImportOperatee.Import;//导入新数据

            if (bacthInfo.CodeCount == codeRelationCount.Return_data)
                return ImportOperatee.Import | ImportOperatee.Delete; //删除数据,导入新数据

            if ((codeRelationCount.Return_data + newDictionary.Count) <= bacthInfo.CodeCount)
                return ImportOperatee.Import;//导入新数据

            return 0;
        }

        public string GetFirstFile(string path)
        {
            if (File.Exists(path))
                return path;

            if (!Directory.Exists(path))
                return null;

            var files = Directory.GetFiles(path);

            if (files.Length > 0)
                return files[0];

            foreach (string subdirectory in Directory.GetDirectories(path))
                return GetFirstFile(subdirectory);

            return null;
        }

        //获取码包文件第一行数据
        public string GetCodePackageFirstLine(string filePath)
        {
            //读取txt第一行
            using (StreamReader reader = new StreamReader(filePath))
                return reader.ReadLine() ?? "";
        }


        public string DownloadFilePath(string url)
        {
            var tempUrl = Path.Combine(AppContext.BaseDirectory, "Temp", "Download");
            if (!Directory.Exists(tempUrl))
                Directory.CreateDirectory(tempUrl);

            var filePath = Path.Combine(tempUrl, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss fff") + ".rar");

            using (WebClient webClient = new WebClient())
                webClient.DownloadFile(url, filePath);

            return filePath;
        }






    }

    public enum ImportOperatee
    {
        /// <summary>
        /// 导入新数据
        /// </summary>
        Import = 1 << 0,
        ///                      
        /// <summary>
        /// 删除数据,导入新数据
        /// </summary>
        Delete = 1 << 1,
    }
    internal class ImportLine
    {
        public string SerialNo { get; set; }

        public string LogisticNo { get; set; }

        //public string QrCode { get; set; }
    }
}



