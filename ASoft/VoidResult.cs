using System;
using System.Collections.Generic;
using System.Text;

namespace ASoft
{
    /// <summary>
    /// 结果基本信息,可用来标识返回void类型的结果
    /// </summary>
    public class VoidResult
    {
        #region 结果类型
        private int code = 0;
        /// <summary>
        /// 返回结果代码
        /// </summary>
        public int Code
        {
            get
            {
                return this.code;
            }
            set
            {
                this.code = value;
            }
        }
        #endregion

        /// <summary>
        /// 返回一个值,指示Code是否为0
        /// </summary>
        public bool Valid
        {
            get
            {
                return this.code == 0;
            }
        }

        #region 结果描述
        private string message = string.Empty;

        /// <summary>
        /// 结果描述
        /// </summary>
        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                this.message = value ?? string.Empty;
            }
        }
        #endregion

    }
}