using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Worker2.ApiModel.CodeHandle;
using Worker2.ApiModel.Task;
using Worker2.Comm;
using Worker2.EntityModel;
using Worker2.ApiModel.WordComb;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Worker.ApiModel.User;

namespace Worker2.Controllers
{
    public class UserController : BaseController
    {
        private readonly IFreeSql _freesql;

        public UserController(IFreeSql freeSql)
        {
            _freesql = freeSql;
        }


        #region Query
        //userlist
        [HttpGet]
        public async Task<ListResultModel<UserInfo>> GetUserList([FromQuery] GetUserListInModel input)
        {
            var query = _freesql.Select<UserInfo>();

            if (!string.IsNullOrEmpty(input?.UserName))
                query = query.Where(x => x.UserName.Contains(input.UserName));

            if (!string.IsNullOrEmpty(input?.NickName))
                query = query.Where(x => x.NickName.Contains(input.NickName));

            var total = query.CountAsync();
            var list = query.OrderByDescending(x => x.Id).Page(input.PageIndex, input.PageSize).ToListAsync();

            return new ListResultModel<UserInfo> { List = await list, Total = await total };
        }

        #endregion

        #region Update

        //登录接口
        [HttpPost]
        public async Task<UserInfo> Login(LoginModel input)
        {
            // var user = await _freesql.Select<UserInfo>().Where(x => x.UserName == input.UserName && x.Password == input.Password).FirstAsync();
            //
            // if (user == null)
            var user = new UserInfo()
            {
                Id = 0,
                UserName = "admin",
                Password = "admin",
                NickName = "管理员",
            };

            // 创建一个 cookie
            var cookie = new CookieOptions();

            // 设置 cookie 的属性
            // cookie.Domain = "task.kagamikun.com";
            // cookie.Path = "/";
            cookie.Expires = DateTime.Now.AddDays(1);
            // cookie.Secure = true;
            // cookie.HttpOnly = true;

            // 将 cookie 添加到响应头
            HttpContext.Response.Cookies.Append("LoginCookie", GenerateCookie(user), cookie);

            return user;
        }

        //logintest
        [CookieFilter]
        [HttpGet]
        public async Task<string> LoginTest()
        {
            return "zxc";
        }

        #endregion

        #region Private

        //生成cookie
        private string GenerateCookie(UserInfo user)
        {
            var cookie = $"Login_{user.Id}_{DateTime.Now.Ticks}";
            return cookie;
        }

        #endregion
    }
}