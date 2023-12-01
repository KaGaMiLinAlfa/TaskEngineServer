using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.IO;
using System.Text;

namespace ClassLibrary2
{
    public class Class1
    {
        public void Run()
        {
            try
            {
                var ArchiveEncoding = new ArchiveEncoding();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

            //    var options = new ReaderOptions()
            //    {
            //        Password = "5ywQjF",
            //    };

            //    options.ArchiveEncoding.Default = Encoding.GetEncoding("gb2312");
            //    using (Stream stream = File.OpenRead("C:\\Users\\KaGaMi\\Desktop\\t\\rr.rar"))
            //    using (var archive = ReaderFactory.Open(stream, options))
            //        while (archive.MoveToNextEntry())
            //            if (!archive.Entry.IsDirectory)
            //            {
            //                Console.WriteLine(archive.Entry.Key);
            //                archive.WriteEntryToDirectory("C:\\Users\\KaGaMi\\Desktop\\t", new ExtractionOptions()
            //                {
            //                    ExtractFullPath = true,
            //                    Overwrite = true
            //                });
            //            }
        }
    }
}
