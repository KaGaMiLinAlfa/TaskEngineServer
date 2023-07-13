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


namespace Task.CodeHandle
{
    public class CodeHandleService : BaseTask
    {
        static string connectionString = "server=localhost;port=3306;database=TaskDB;uid=root;pwd=123123;";
        static IFreeSql fsql = new FreeSql.FreeSqlBuilder().UseConnectionString(FreeSql.DataType.MySql, connectionString).Build();


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
            //客户码包文件地址
            //var packerfileurl = "D:\\MiDuo\\Code\\TaskEngineServer\\test\\bin\\Debug\\netcoreapp3.1\\temp\\test.txt";
            var packerfileurl = item.HandlePackPath;

            //var tmepPackerFile = DownloadFilePath(packerfileurl);
            //var tmepPackerFile = packerfileurl;

            //var firstLine = GetCodePackageFirstLine(tmepPackerFile);

            //var firstSplit = firstLine.Split(',');

            ////这里未来添加类型判断
            //if (firstSplit.Length != 2)
            //    throw new Exception("码包不是两种码");

            //var serialNumber = firstSplit[0];
            //if (!Regex.IsMatch(serialNumber, @"^\d+$"))
            //    throw new Exception("第一列码不为全数字流水号");

            //var qrcodeUrl = firstSplit[1];
            //if (!Regex.IsMatch(qrcodeUrl, @"^http") && qrcodeUrl.IndexOf('/') > 0)
            //    throw new Exception("第二列不为二维码连接");

            //var qrcode = qrcodeUrl.Substring(qrcodeUrl.LastIndexOf('/') + 1);


            var analyze = CodePackgeAnalyze.Build(packerfileurl);

            var handleType = analyze.Analyze();
            QueryOrderApplyDto bacthInfo = analyze.BacthInfo;

            后关联乱顺序_流水号_二维码链接 handler;
            switch (handleType)
            {
                case 1: handler = new 后关联乱顺序_流水号_二维码链接(bacthInfo); break;
                default: throw new Exception("未知的处理类型");
            }

            if (analyze.IsReverse)
                handler.SetReverse(analyze.IsReverse);

            handler.Handle(packerfileurl);

            //var checker = CodePackageFirstLineCheck.Build(packerfileurl);


            //checker.Check();

            //var qrcode = checker.GetFirstQrCode();

            //var firstCodeInfo = CodeServiceSDK.CheckCode(qrcode);

            //var bacthCode = firstCodeInfo?.Return_data?.CodeBatch;
            //var segmentCode = firstCodeInfo?.Return_data?.CodeSegment;

            //if (!string.IsNullOrEmpty(bacthCode) || !string.IsNullOrEmpty(segmentCode))
            //    throw new Exception("查码无批次信息返回,可能假码或接口异常");

            //var applyOrder = CodeServiceSDK.QueryOrderApply(new QueryOrderApplyRequest
            //{
            //    CodeSegment = segmentCode,
            //    CodeBatch = bacthCode,
            //});

            //var bacthInfo = applyOrder?.Return_data?.Results?.FirstOrDefault();
            //if (bacthInfo == null)
            //    throw new Exception("码信息为空");

            //if (bacthInfo.LogisticsRelatedType != 1)//SerialNoType
            //    throw new Exception("批次不是后关联");



            var bacthPackRar = CodeServiceSDK.GetCloudFileUrl(bacthInfo.FilePath);




            var bacthPackRarPassword = bacthInfo.ZipPwd;
            var tempBacthPackFileRar = DownloadFilePath(bacthPackRar.Return_data);

            var extractPath = Path.Combine(AppContext.BaseDirectory, "Temp", "Extract", $"{bacthInfo.CodeSegment}-{bacthInfo.CodeBatch}");
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


            BaseResponseModelV1<ImportFileResult> importResult = null;
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


        public bool Handle(string clientPackerFile)
        {
            var tuple = GetImprotAndErrorDic(clientPackerFile);

            var improtDic = tuple.Item1;
            var errorDic = tuple.Item2;

            var codeRelationCount = CodeServiceSDK.GetCodeRelationCount(new CodeRelation
            {
                MemberLogin = _bacthInfo.Memberlogin,
                Batch = _bacthInfo.CodeBatch,
                Segment = _bacthInfo.CodeSegment,
                BigSerialIsNull = true,
                StorageState = 0
            });

            var type = GetCodeRelationDataStats(codeRelationCount, _bacthInfo, improtDic.Count);
            if (type == 0)
                throw new Exception("状态验证异常");



            if ((type & ImportOperatee.Delete) != 0)
            {
                var delectCount = CodeServiceSDK.DeleteCodeRelation(new CodeRelation
                {
                    MemberLogin = _bacthInfo.Memberlogin,
                    Batch = _bacthInfo.CodeBatch,
                    Segment = _bacthInfo.CodeSegment,
                    BigSerialIsNull = false,
                    StorageState = -1
                });
            }

            var importFile = WriteImprotFile(improtDic);
            BaseResponseModelV1<ImportFileResult> importResult = null;
            if ((type & ImportOperatee.Import) != 0)
            {
                var uploadUrl = CodeServiceSDK.UploadFileWithParameters(importFile, _bacthInfo.Memberlogin).Result;

                importResult = CodeServiceSDK.ImportFangcuanRelation(new ImportRelationRequest
                {
                    Path = uploadUrl.Return_data,
                    dateCount = improtDic.Count,
                    codeType = 3,//后关联
                    type = _bacthInfo.CodeType,
                    id = _bacthInfo.BatchId,
                    Memberlogin = _bacthInfo.Memberlogin
                });
            }

            return true;
        }

        private ImportOperatee GetCodeRelationDataStats(BaseResponseModel<int> codeRelationCount, QueryOrderApplyDto bacthInfo, int improtCount)
        {

            if (codeRelationCount.Return_data == 0)
                return ImportOperatee.Import;//导入新数据

            if (bacthInfo.CodeCount == codeRelationCount.Return_data)
                return ImportOperatee.Import | ImportOperatee.Delete; //删除数据,导入新数据

            if ((codeRelationCount.Return_data + improtCount) <= bacthInfo.CodeCount)
                return ImportOperatee.Import;//导入新数据

            return 0;
        }

        private Tuple<Dictionary<string, ImportLine>, Dictionary<string, ImportLine>> GetImprotAndErrorDic(string clientPackerFile)
        {
            var bdeCodes = GetBDEPackgeCodeDic();

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



        Dictionary<string, string> GetBDEPackgeCodeDic()
        {
            var codePackgePath = SingleDownload.GetSingleCodePackge(_bacthInfo.CodeSegment, _bacthInfo.CodeBatch, _bacthInfo.FilePath, _bacthInfo.ZipPwd);

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
            firstLine = GetCodePackageFirstLine(codePackagePath);

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


            var firstCodeInfo = CodeServiceSDK.CheckCode(firstqrCode);
            bacthCode = firstCodeInfo?.Return_data?.CodeBatch;
            segmentCode = firstCodeInfo?.Return_data?.CodeSegment;
            if (!string.IsNullOrEmpty(bacthCode) || !string.IsNullOrEmpty(segmentCode))
                throw new Exception("查码无批次信息返回,可能假码或接口异常");

            var applyOrder = CodeServiceSDK.QueryOrderApply(new QueryOrderApplyRequest
            {
                CodeSegment = segmentCode,
                CodeBatch = bacthCode,
            });

            QueryOrderApplyDto bacthInfo = applyOrder?.Return_data?.Results?.FirstOrDefault();
            if (bacthInfo == null)
                throw new Exception("码信息为空");

            BacthInfo = bacthInfo;

            switch (handleType)
            {
                case 1: break;
                case 2: if (bacthInfo.LogisticsRelatedType != 1) throw new Exception("批次信息不是后关联"); break;

                default: throw new Exception("解析出未知的处理类型");
            }

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



