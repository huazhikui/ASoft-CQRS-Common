using System;
using System.Collections.Generic;
using System.Text;

namespace ASoft
{
    /// <summary>
    /// 上下文结果
    /// </summary>
    /// <typeparam name="T">结果的类型</typeparam>
    public class ValueResult<T> : VoidResult
    {
        /// <summary>
        /// 默认的构造函数
        /// </summary>
        public ValueResult()
        {
            value = default(T);
        }

        /// <summary>
        /// 生成一个值为参数指定值的对象
        /// </summary>
        /// <param name="value">对象的值</param>
        public ValueResult(T value)
        {
            this.value = value;
        }

        #region 结果
        private T value;
        /// <summary>
        /// 结果
        /// </summary>
        public T Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }
        #endregion

    }
}
