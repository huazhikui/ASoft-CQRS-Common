using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
using ASoft;
 

namespace ASoft
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseHttpHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public  void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json"; 
            string action = context.Request["action"];
            if(String.IsNullOrEmpty(action))
            {
                context.Response.Write("action参数不能为空");
                return;
            }
            String msg = "";
           
            System.Reflection.MethodInfo methodInfo = this.GetType().GetMethod(action);
            if (methodInfo != null)
            {
                methodInfo.Invoke(this, new object[] { context });
            }
        }
 

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
