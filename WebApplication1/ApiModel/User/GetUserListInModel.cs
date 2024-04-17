namespace Worker.ApiModel.User
{
    public class GetUserListInModel
    {
        public string UserName { get; set; }
        public string NickName { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        
    }
}