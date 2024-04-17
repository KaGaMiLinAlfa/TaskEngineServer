using System.Collections.Generic;

namespace Worker2.ApiModel.WordComb
{
    public class CreateWordCombGroupModel
    {
        //public int GroupId { get; set; }
        public string GroupName { get; set; }


        public Dictionary<string,string > Params { get; set; }

        //public string ParamName { get; set; }
        //public string ParamVal { get; set; }
    }

    public class CreateWordCombParamsModel
    {
        public string ParamName { get; set; }

        public string ReplaceStr { get; set; }
    }

    public class CreateWordCombContentModel
    {
        public string Title { get; set; }

        public string WordContent { get; set; }
    }
}
