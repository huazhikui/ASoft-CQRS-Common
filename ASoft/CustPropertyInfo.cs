using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASoft
{
    /**/
    /// <summary>
    /// 自定义的属性信息类型。
    /// </summary>
    public class CustPropertyInfo
    {
        private string propertyName;
        private string type;

        /**/
        /// <summary>
        /// 空构造。
        /// </summary>
        public CustPropertyInfo() { }

        /**/
        /// <summary>
        /// 根据属性类型名称,属性名称构造实例。
        /// </summary>
        /// <param name="type">属性类型名称。</param>
        /// <param name="propertyName">属性名称。</param>
        public CustPropertyInfo(string type, string propertyName)
        {
            this.type = type;
            this.propertyName = propertyName;
        }

        /**/
        /// <summary>
        /// 获取或设置属性类型名称。
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /**/
        /// <summary>
        /// 获取或设置属性名称。
        /// </summary>
        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }

        /**/
        /// <summary>
        /// 获取属性字段名称。
        /// </summary>
        public string FieldName
        {
            get
            {
                if (propertyName.Length < 1)
                    return "";
                return propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
            }
        }

        /**/
        /// <summary>
        /// 获取属性在IL中的Set方法名。
        /// </summary>
        public string SetPropertyMethodName
        {
            get { return "set_" + PropertyName; }
        }

        /**/
        /// <summary>
        ///  获取属性在IL中的Get方法名。
        /// </summary>
        public string GetPropertyMethodName
        {
            get { return "get_" + PropertyName; }
        }
    }
}
