using System;
using System.Collections.Generic;
using System.Text;

namespace ASoft.Model
{
    /// <summary>
    /// 用户信息的基类
    /// </summary>
    public class UserInfo : BaseModel
    {
         

        #region 是否登录
        private bool isLogin = false;
        /// <summary>
        /// 是否登录
        /// </summary>
        public bool IsLogin
        {
            get
            {
                return this.isLogin;
            }
            set
            {
                this.isLogin = value;
            }
        }
        #endregion

        #region 用户登录名
        private string loginName = string.Empty;
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string LoginName
        {
            get
            {
                return this.loginName ?? string.Empty;
            }
            set
            {
                this.loginName = value;
            }
        }
        #endregion

        #region 用户登录密码
        private string loginPwd = string.Empty;
        /// <summary>
        /// 用户登录密码
        /// </summary>
        public string LoginPwd
        {
            get
            {
                return this.loginPwd ?? string.Empty;
            }
            set
            {
                this.loginPwd = value;
            }
        }

        #endregion

        #region 用户姓名
        private string userName = "匿名用户";
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName
        {
            get
            {
                return this.userName ?? string.Empty;
            }
            set
            {
                this.userName = value;
            }

        }
        #endregion
    }
}
