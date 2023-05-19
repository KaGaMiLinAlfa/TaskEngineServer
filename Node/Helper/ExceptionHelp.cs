using System;
using System.Collections.Generic;
using System.Text;

namespace Node.Helper
{
    public static class ExceptionHelp
    {
        public static Exception GetInnerMsg(this Exception ex)
        {
            return ex.InnerException == null ? ex : ex.GetInnerMsg();
        }
    }
}
