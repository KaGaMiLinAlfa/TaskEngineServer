using System.ComponentModel.DataAnnotations;

namespace Worker2.EntityModel
{
    public class WordCombGroup
    {
        //主键
        [Key]
        public int Id { get; set; }

        public string GroupName { get; set; }

        public string ParamName { get; set; }
        public string ParamVal { get; set; }
    }

    //public class WordCombParams
    //{
    //    //主键
    //    [Key]
    //    public int Id { get; set; }

    //    //参数名
    //    public string ParamName { get; set; }

    //    //参数替换字符
    //    public string ReplaceStr { get; set; }
    //}

    public class WordCombContent
    {
        //主键
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }

        public string WordContent { get; set; }
    }
}
