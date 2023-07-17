using BaseTaskManager;
using LogManager;
using SDK.MiDuo.CodeService;
using SDK.MiDuo.CodeService.Model;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using SharpCompress.Compressors.LZMA;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Web;
using Task.CodeHandle.EntityModel;
using static FreeSql.Internal.GlobalFilter;

namespace Task.CodeHandle
{
    public class CodeHandleService : BaseTask
    {
        static string connectionString = "server=localhost;port=3306;database=TaskDB;uid=root;pwd=123123;";
        public static IFreeSql fsql = new FreeSql.FreeSqlBuilder().UseConnectionString(FreeSql.DataType.MySql, connectionString).Build();

        public static int TempHandleId;

        public static void HandleInfoLog(string msg)
        {
            fsql.Insert(new CodeHandleLog
            {
                HandleId = TempHandleId,
                CreateTime = DateTime.Now,
                Message = msg,
                Type = 1
            }).ExecuteAffrows();
        }

        public static void HandleErrorLog(string msg)
        {
            fsql.Insert(new CodeHandleLog
            {
                HandleId = TempHandleId,
                CreateTime = DateTime.Now,
                Message = msg,
                Type = 3
            }).ExecuteAffrows();
        }

        public override void Run()
        {
            LogHelper.Info("任务开始执行");

            try
            {
                var handleList = fsql.Select<EntityModel.CodeHandle>().Where(x => x.Stats == 1).ToList();

                foreach (var item in handleList)
                    Handle(item);

            }
            catch (Exception ex)
            {
                LogHelper.Error("任务异常");
            }

            LogHelper.Info("任务结束");
        }

        public void Handle(EntityModel.CodeHandle item)
        {
            TempHandleId = item.Id;

            HandleInfoLog($"开始处理:{item.Id},文件地址:{item.HandlePackPath}");
            //客户码包文件地址
            var packerfileurl = item.HandlePackPath;



            var analyze = CodePackgeAnalyze.Build(packerfileurl);

            var handleType = analyze.Analyze();
            QueryOrderApplyDto bacthInfo = analyze.BacthInfo;

            OrderChaos_NumberAndCQLink handler;
            switch (handleType)
            {
                case 1: handler = new OrderChaos_NumberAndCQLink(bacthInfo); break;
                default: throw new Exception("未知的处理类型");
            }

            if (analyze.IsReverse)
                handler.SetReverse(analyze.IsReverse);

            handler.Handle(packerfileurl, item.Id);


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
    }



    /// <summary>
    /// 后关联乱顺序_流水号_二维码链接
    /// </summary>
    public class OrderChaos_NumberAndCQLink
    {
        string bacthCode;
        string segmentCode;
        string _BacthFilePath;
        QueryOrderApplyDto _bacthInfo;
        bool _isReverse;

        public OrderChaos_NumberAndCQLink(QueryOrderApplyDto bacthInfo)
        {
            _bacthInfo = bacthInfo;
        }



        public void SetReverse(bool isReverse)
        {
            _isReverse = isReverse;
        }


        public bool Handle(string clientPackerFile, int handleId)
        {
            CodeHandleService.HandleInfoLog($"开始规则处理码包-HandleId:{handleId}");

            var tuple = GetImprotAndErrorDic(clientPackerFile);

            var improtDic = tuple.Item1;
            var errorDic = tuple.Item2;

            CodeHandleService.HandleInfoLog($"解析出导入码量:{improtDic.Count} ;错误匹配码量:{errorDic.Count}");

            var errorFile = WriteErrorFile(errorDic);

            if (!string.IsNullOrEmpty(errorFile))
                CodeHandleService.HandleInfoLog($"写入匹配错误码文件:{errorFile}");

            var codeRelationCount = CodeServiceSDK.GetCodeRelationCount(new CodeRelation
            {
                MemberLogin = _bacthInfo.Memberlogin,
                Batch = _bacthInfo.CodeBatch,
                Segment = _bacthInfo.CodeSegment,
                BigSerialIsNull = true,
                StorageState = 0
            });

            CodeHandleService.HandleInfoLog($"查询码关系数:{codeRelationCount?.Return_data ?? -1}");

            var type = GetCodeRelationDataStats(codeRelationCount, _bacthInfo, improtDic.Count);
            if (type == 0)
                throw new Exception("状态验证异常");

            CodeHandleService.HandleInfoLog($"判断得执行处理方式:{Convert.ToString(type, 2)}");

            if ((type & 0b10) != 0)
            {
                var delectCount = CodeServiceSDK.DeleteCodeRelation(new CodeRelation
                {
                    MemberLogin = _bacthInfo.Memberlogin,
                    Batch = _bacthInfo.CodeBatch,
                    Segment = _bacthInfo.CodeSegment,
                    BigSerialIsNull = false,
                    StorageState = -1
                });

                CodeHandleService.HandleInfoLog($"删除码关系数量:{delectCount}");
            }

            var importFile = WriteImprotFile(improtDic);
            CodeHandleService.HandleInfoLog($"写入导入的码处理文件:{importFile}");

            BaseResponseModelV1<ImportFileResult> importResult = null;
            if ((type & 0b01) != 0)
            {
                var uploadUrl = CodeServiceSDK.UploadFileWithParameters(importFile, _bacthInfo.Memberlogin).Result;

                CodeHandleService.HandleInfoLog($"成功上传码包文件:{uploadUrl}");

                importResult = CodeServiceSDK.ImportFangcuanRelation(new ImportRelationRequest
                {
                    Path = uploadUrl.Return_data,
                    dateCount = improtDic.Count,
                    codeType = 3,//后关联
                    type = _bacthInfo.CodeType,
                    id = _bacthInfo.BatchId,
                    Memberlogin = _bacthInfo.Memberlogin
                });

                CodeHandleService.HandleInfoLog($"成功导入码包; 成功数:{importResult?.parameter?.sucCount ?? -1};失败数:{importResult?.parameter?.errCount ?? -1}");
            }


            if (!string.IsNullOrEmpty(errorFile))
                CodeHandleService.fsql.Update<EntityModel.CodeHandle>()
                    .Set(x => x.ErrorFilePath, errorFile)
                    .Where(x => x.Id == handleId).ExecuteAffrows();

            if (importResult?.parameter?.sucCount > 0 && importResult?.parameter?.errCount <= 0)
            {
                CodeHandleService.fsql.Update<EntityModel.CodeHandle>()
                .Set(x => x.Stats, 4)
                .Where(x => x.Id == handleId).ExecuteAffrows();
            }
            else
            {
                CodeHandleService.fsql.Update<EntityModel.CodeHandle>()
               .Set(x => x.Stats, 3)
               .Where(x => x.Id == handleId).ExecuteAffrows();
            }

            return true;
        }

        private int GetCodeRelationDataStats(BaseResponseModel<int> codeRelationCount, QueryOrderApplyDto bacthInfo, int improtCount)
        {

            if (codeRelationCount.Return_data == 0)
                return 0b01;//导入新数据

            if (bacthInfo.CodeCount == codeRelationCount.Return_data)
                return 0b11; //删除数据,导入新数据

            if ((codeRelationCount.Return_data + improtCount) <= bacthInfo.CodeCount)
                return 0b01;//导入新数据

            return 0;
        }

        private Tuple<Dictionary<string, ImportLine>, Dictionary<string, ImportLine>> GetImprotAndErrorDic(string clientPackerFile)
        {
            CodeHandleService.HandleInfoLog($"开始匹配码包 PackerFile:{clientPackerFile}");

            var bdeCodes = GetBDEPackgeCodeDic();

            CodeHandleService.HandleInfoLog($"BDE码数:{bdeCodes.Count}");

            var line = string.Empty;
            var newDictionary = new Dictionary<string, ImportLine>();
            var ErrorDictionary = new Dictionary<string, ImportLine>();

            using (StreamReader reader = new StreamReader(clientPackerFile))
            {
                var logisticIndex = 0;
                var cqcodeurlIndex = 1;

                if (_isReverse)
                {
                    logisticIndex = 1;
                    cqcodeurlIndex = 0;
                }

                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var lineItems = line.Split(',');

                    if (lineItems.Length < 2)
                        throw new Exception("客户码包列数少于2！");
                    var cqcodeurl = lineItems[cqcodeurlIndex];

                    if (bdeCodes.ContainsKey(cqcodeurl) && !newDictionary.ContainsKey(cqcodeurl))
                    {
                        if (lineItems[logisticIndex].Length != 8)
                            throw new Exception("流水长度异常！");

                        var importLine = new ImportLine
                        {
                            LogisticNo = lineItems[logisticIndex],
                            //QrCode = lineItems[1],
                            SerialNo = bdeCodes[cqcodeurl]
                        };

                        newDictionary.Add(cqcodeurl, importLine);
                    }
                    else
                    {
                        var importLine = new ImportLine
                        {
                            LogisticNo = lineItems[logisticIndex],
                            //QrCode = lineItems[1],
                            SerialNo = ""
                        };

                        ErrorDictionary.Add(cqcodeurl, importLine);
                    }
                }
            }

            return new Tuple<Dictionary<string, ImportLine>, Dictionary<string, ImportLine>>(newDictionary, ErrorDictionary);
        }


        private string WriteImprotFile(Dictionary<string, ImportLine> writeDic)
        {
            var inputPackfilePath = Path.Combine(AppContext.BaseDirectory, "Temp", "InputFile");
            if (!Directory.Exists(inputPackfilePath))
                Directory.CreateDirectory(inputPackfilePath);

            var inputPackfileName = Path.Combine(inputPackfilePath, $"Input-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}-{segmentCode}-{bacthCode}.txt");

            var codeCount = writeDic.Count;
            using (var fs = new FileStream(inputPackfileName, FileMode.Create))
            using (var sw = new StreamWriter(fs))
                foreach (var codeInfo in writeDic)
                {
                    sw.WriteLine($"{codeInfo.Value.LogisticNo},{codeInfo.Value.SerialNo}");
                    sw.Flush();
                }

            return inputPackfileName;
        }

        private string WriteErrorFile(Dictionary<string, ImportLine> writeDic)
        {
            if (writeDic.Count <= 0)
                return string.Empty;

            var inputPackfilePath = Path.Combine(AppContext.BaseDirectory, "Temp", "InputFile");
            if (!Directory.Exists(inputPackfilePath))
                Directory.CreateDirectory(inputPackfilePath);

            var inputPackfileName = Path.Combine(inputPackfilePath, $"Error-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}-{segmentCode}-{bacthCode}.txt");

            var codeCount = writeDic.Count;
            using (var fs = new FileStream(inputPackfileName, FileMode.Create))
            using (var sw = new StreamWriter(fs))
                foreach (var codeInfo in writeDic)
                {
                    sw.WriteLine($"{codeInfo.Value.LogisticNo},{codeInfo.Value.SerialNo}");
                    sw.Flush();
                }

            return inputPackfileName;
        }



        Dictionary<string, string> GetBDEPackgeCodeDic()
        {
            CodeHandleService.HandleInfoLog($"开始下载BDE码包 FilePath:{_bacthInfo.FilePath}");

            var codePackgePath = SingleDownload.GetSingleCodePackge(_bacthInfo.CodeSegment, _bacthInfo.CodeBatch, _bacthInfo.FilePath, _bacthInfo.ZipPwd);

            CodeHandleService.HandleInfoLog($"BDE码包路径:{codePackgePath}");

            var line = string.Empty;
            var bdeCodes = new Dictionary<string, string>();
            using (StreamReader reader = new StreamReader(codePackgePath))
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

            return bdeCodes;
        }
    }

    public class CodePackgeAnalyze
    {
        private string codePackagePath;
        private string firstLine;
        private string firstqrCode;
        private int packageColumnCount;
        public bool IsReverse;
        string bacthCode;
        string segmentCode;

        public QueryOrderApplyDto BacthInfo;

        //1:后关联(乱顺序)-流水号-二维码链接;2:后关联(乱顺序)-二维码-二维码链接
        private int handleType;

        public static CodePackgeAnalyze Build(string path)
        {
            return new CodePackgeAnalyze(path);
        }

        public CodePackgeAnalyze(string path)
        {
            codePackagePath = path;
        }
        public int Analyze()
        {
            CodeHandleService.HandleInfoLog($"开始验证码包文件");

            firstLine = GetCodePackageFirstLine(codePackagePath);

            CodeHandleService.HandleInfoLog($"第一行内容:{firstLine}");

            if (string.IsNullOrEmpty(firstLine))
                throw new Exception("文件内容为空");

            var firstSplit = firstLine.Split(',');
            packageColumnCount = firstSplit.Length;

            switch (firstSplit.Length)
            {
                case 2: TwoLengthAnalyze(firstSplit); break;
                default: throw new Exception("码包第一行存在未知的列数");
            }

            //不反转默认第二列才是二维码链接,也就是主码包
            if (!IsReverse)
                firstqrCode = firstSplit[1].Substring(firstSplit[1].LastIndexOf('/') + 1);
            else
                firstqrCode = firstSplit[0].Substring(firstSplit[0].LastIndexOf('/') + 1);

            if (IsReverse)
                CodeHandleService.HandleInfoLog($"反转码包");

            var firstCodeInfo = CodeServiceSDK.CheckCode(firstqrCode);
            bacthCode = firstCodeInfo?.Return_data?.CodeBatch;
            segmentCode = firstCodeInfo?.Return_data?.CodeSegment;

            CodeHandleService.HandleInfoLog($"查询到的信息-CodeSegment:{segmentCode};CodeBatch:{bacthCode}");

            if (!string.IsNullOrEmpty(bacthCode) || !string.IsNullOrEmpty(segmentCode))
                throw new Exception("查码无批次信息返回,可能假码或接口异常");

            var applyOrder = CodeServiceSDK.QueryOrderApply(new QueryOrderApplyRequest
            {
                CodeSegment = segmentCode,
                CodeBatch = bacthCode,
            });

            QueryOrderApplyDto bacthInfo = applyOrder?.Return_data?.Results?.FirstOrDefault();

            CodeHandleService.HandleInfoLog($"查询到码段信息-Id:{bacthInfo?.BatchId}");

            if (bacthInfo == null)
                throw new Exception("码信息为空");

            BacthInfo = bacthInfo;

            switch (handleType)
            {
                case 1: break;
                case 2: if (bacthInfo.LogisticsRelatedType != 1) throw new Exception("批次信息不是后关联"); break;

                default: throw new Exception("解析出未知的处理类型");
            }

            CodeHandleService.HandleInfoLog($"解析出处理类型为:{handleType}");

            return handleType;
        }


        private void TwoLengthAnalyze(string[] firstSplit)
        {
            var first = firstSplit[0];
            var second = firstSplit[1];

            var firstType = 0b0001;
            var secondType = 0b0001;

            if (Regex.IsMatch(first, @"^\d+$"))
                firstType = 0b0010;
            else if (Regex.IsMatch(first, @"^[a-zA-Z0-9]+$"))
                firstType = 0b0100;
            else if (Regex.IsMatch(second, @"^http") && second.IndexOf('/') > 0)
                firstType = 0b1000;

            if (Regex.IsMatch(first, @"^\d+$"))
                secondType = 0b0010;
            else if (Regex.IsMatch(first, @"^[a-zA-Z0-9]+$"))
                secondType = 0b0100;
            else if (Regex.IsMatch(second, @"^http") && second.IndexOf('/') > 0)
                secondType = 0b1000;

            var analyzeHandleType = 0;

            switch (firstType | secondType)
            {
                case 0b1010: analyzeHandleType = 1; break;//流水号-二维码链接
                case 0b1100: analyzeHandleType = 2; break;//二维码-二维码链接
                default: throw new Exception("码包第一行存在未知的组合");
            }

            //是否翻转
            switch (firstType | secondType)
            {
                case 0b1010:
                case 0b1100: if (firstType == 0b1000) IsReverse = true; break;
            }

            //不是没有指定类型和与解析的类型不对应则异常
            if (handleType != 0 && handleType != analyzeHandleType)
                throw new Exception("指定的处理类型不匹配码包数据结构");

            handleType = analyzeHandleType;
        }



        private string GetCodePackageFirstLine(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("文件不存在");

            //读取txt第一行
            using (StreamReader reader = new StreamReader(filePath))
                return reader.ReadLine();
        }
    }


    public static class SingleDownload
    {

        public static string GetSingleCodePackge(string segmentCode, string bacthCode, string filePath, string rarPassword)
        {
            var bacthPackRar = CodeServiceSDK.GetCloudFileUrl(filePath);

            var bacthPackRarPassword = rarPassword;
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

            return firstFilePath;
        }


        private static string GetFirstFile(string path)
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

        private static string DownloadFilePath(string url)
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

    internal class ImportLine
    {
        public string SerialNo { get; set; }

        public string LogisticNo { get; set; }

    }
}



