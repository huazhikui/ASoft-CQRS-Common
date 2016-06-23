using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using ASoft.Db;

namespace ASoft.Model
{
    [Serializable]
    /// <summary>
    /// 实体的基本信息
    /// </summary>
    public class BaseModel
    {
        #region 实体标识 
        /// <summary>
        /// 实体标识
        /// </summary>  
        public string GetUniqueID(string keyValue = "")
        {
            EntityInfo ea = this.GetEntityInfo();
            if (ea == null
                || ea.Identifier == null
                || ea.EntityProperty == null
                || ea.EntityProperty[ea.Identifier] == null
                || ea.EntityProperty[ea.Identifier].PropertyInfo == null)
            {
                return null;
            }
            if (keyValue == null || keyValue.Length == 0)
            {
                //主键的值
                keyValue = ea.EntityProperty[ea.Identifier].PropertyInfo.GetValue(this, null).ToString();
            }

            if (keyValue == null)
            {
                return null;
            }
            string result = $"{ea.EntityFullName}:{keyValue}:{ea.Identifier}";

            return result;

        }

        /// <summary>
        /// 获取主键的数据库字段名称
        /// </summary>
        /// <returns></returns>
        public string GetIdentifierField()
        {
            EntityInfo ea = this.GetEntityInfo();
            if (ea == null
                || ea.Identifier == null
                || ea.EntityProperty == null
                || ea.EntityProperty[ea.Identifier] == null
                || ea.EntityProperty[ea.Identifier].PropertyAttribute == null)
            {
                return null;
            }

            return ea.EntityProperty[ea.Identifier].PropertyAttribute.Field;
        }

        #endregion


        /// <summary>
        /// 需要更新的字段
        /// </summary>
        public List<string> Updates { private set; get; }

        /// <summary>
        /// 添加更新字段
        /// </summary>
        /// <param name="propertyName"></param>
        public void PropertyUpdate(string propertyName)
        {
            if (this.Updates == null)
            {
                this.Updates = new List<string>();
            }
            if (!this.Updates.Contains(propertyName))
            {
                this.Updates.Add(propertyName);
            }
        }

        [NonSerialized]
        private Dictionary<FilterGroup, List<String>> _filterDict = null;

        [NonSerialized]
        private String _filter = "";
        /// <summary>
        /// where查询条件
        /// </summary>
        public String Filter
        {
            get
            {
                if (String.IsNullOrEmpty(_filter) && _filterDict != null)
                {
                    foreach (var key in _filterDict.Keys)
                    {
                        if (_filterDict[key] != null && _filterDict[key].Count > 0)
                        {
                            if (!String.IsNullOrEmpty(_filter))
                            {
                                _filter += " and ";
                            }
                            _filter += String.Format(" ({0}) ", ASoft.Text.StringUtils.Join(" " + key.ToString() + " ", _filterDict[key]));
                        }
                    }
                }
                return _filter;
            }
        }

        /// <summary>
        /// 添加where查询条件
        /// </summary>
        /// <param name="propertyName">字段</param>
        /// <param name="filter"></param>
        public void AddFilter(String propertyName, String filter, FilterGroup filterGroup)
        {
            try
            {
                if (_filterDict == null)
                {
                    _filterDict = new Dictionary<FilterGroup, List<string>>();
                    _filterDict[FilterGroup.AND] = new List<string>();
                    _filterDict[FilterGroup.OR] = new List<string>();
                }

                String filedName = this.GetEntityInfo().EntityProperty[propertyName].PropertyAttribute.Field;
                _filterDict[filterGroup].Add(filedName + " " + filter);
            }
            catch
            {
            }
        }

        [NonSerialized]
        private EntityInfo _ea = null;

        public EntityInfo GetEntityInfo()
        {

            if (_ea == null)
            {
                _ea = EntityHelper.GetEntityAttribute(this);
            }
            return _ea;

        }

        public IEnumerable<KeyValuePair<string, EntityPropertyInfo>> GetEntityPropertyInfos()
        {
            var entityPropertys = this.GetEntityInfo().EntityProperty;
            foreach (var item in entityPropertys)
            {
                yield return item;
            }
        }
    }
    public enum FilterGroup
    {
        OR = 0,
        AND = 1
    }
}
