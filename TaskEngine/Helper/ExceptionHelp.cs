using System;

namespace TaskEngineServer.Helper
{
    public static class ExceptionHelp
    {
        public static Exception GetInnerMsg(this Exception ex)
        {
            return ex.InnerException == null ? ex : ex.GetInnerMsg();
        }
    }
}
