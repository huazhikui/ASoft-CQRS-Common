using System;
using System.Collections.Generic;
using System.Text;

namespace ASoft
{
    /// <summary>
    /// 带有分页参数的列表对象
    /// </summary>
    /// <typeparam name="T">列表中对象的类型</typeparam>
    public class ListResult<T> : List<T>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ListResult()
        {
        }

        /// <summary>
        /// 生成一个指定了每页数量和页码的列表对象
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        public ListResult(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = PageSize;
        }

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize
        {
            get;
            set;
        }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount
        {
            get;
            set;
        }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get;
            set;
        }
    }
}
