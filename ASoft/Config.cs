using System;
using System.Configuration;
using System.Text;
using System.Collections.Specialized;
using System.Web.Configuration;

namespace ASoft
{ 
    

    /// <summary>
    /// 对应用程序中的.config文件的操作
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// 根据key的值从config设置文件中得到相对应的值.
        /// <para>
        /// 如果没有找到,返回null;
        /// </para>
        /// <para>
        /// 如果在value中发现有|DataDirectory|值,则使用网站根目录的App_Data目录地址来替换.
        /// </para>
        /// </summary>
        /// <param name="key">要查找的AppSettings键值对中的键</param>
        /// <returns>要查找的AppSettings键值对中的值</returns>
        public static string GetAppSettings(string key)
        {
            if (string.IsNullOrEmpty(key) || ConfigurationManager.AppSettings[key] == null)
            {
                return null;
            }
            if (!ConfigurationManager.AppSettings[key].Contains("|DataDirectory|"))
            {
                return ConfigurationManager.AppSettings[key];
            }
            return ConfigurationManager.AppSettings[key].Replace("|DataDirectory|", App_Data);
        }

        /// <summary>
        /// 获取应用程序App_Data目录的绝对路径,最后没有目录分隔符
        /// </summary>
        public static string App_Data
        {
            get
            {
                return System.IO.Path.Combine((System.Web.HttpContext.Current != null ? System.Web.HttpContext.Current.Server.MapPath("~") : AppDomain.CurrentDomain.BaseDirectory), "App_Data");
            }
        }

        /// <summary>
        /// 根据key的值从config设置文件中得到相对应的值,如果没有找到,返回null;
        /// </summary>
        /// <param name="key">要查找的ConnectionString键值对中的键</param>
        /// <returns>要查找的ConnectionString键值对中的值</returns>
        public static ConnectionStringSettings GetConnectionSettings(string key)
        {
            if (string.IsNullOrEmpty(key) || ConfigurationManager.ConnectionStrings[key] == null)
            {
                return null;
            }

            ConnectionStringSettings conns = ConfigurationManager.ConnectionStrings[key];
            if (conns.ConnectionString.Contains("|DataDirectory|"))
            {
                return new ConnectionStringSettings(conns.Name, conns.ConnectionString.Replace("|DataDirectory|", System.IO.Path.Combine((System.Web.HttpContext.Current != null ? System.Web.HttpContext.Current.Server.MapPath("~") : AppDomain.CurrentDomain.BaseDirectory), "App_Data")), conns.ProviderName);
            }
            return conns;
        }

        /// <summary>
        /// 在配置文件的appSettings节保存信息(如果有则更新,没有则添加)
        /// </summary>
        /// <param name="key">要更新的配置文件的key</param>
        /// <param name="value">要更新的配置文件的key的value值</param>
        public static void SaveAppSetting(string key, string value)
        {
            Configuration config = null;
            if (System.Web.HttpContext.Current != null)
            {
                config = WebConfigurationManager.OpenWebConfiguration("~");
            }
            else
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            if (config.AppSettings.Settings[key] == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                config.AppSettings.Settings[key].Value = value;
            }
            config.Save(System.Configuration.ConfigurationSaveMode.Modified);
            System.Configuration.ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }

        /// <summary>
        /// 在配置文件connectionSettings节保存信息(如果有则更新,没有则添加)
        /// </summary>
        /// <param name="name">数据库连接名称</param>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerName">数据库连接提供程序</param>
        public static void SaveConnectionSetting(string name, string connectionString, string providerName)
        {
            Configuration config = null;
            if (System.Web.HttpContext.Current != null)
            {
                config = WebConfigurationManager.OpenWebConfiguration("~");
            }
            else
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            ConnectionStringSettings connectionSettings;
            if (providerName != null)
            {
                connectionSettings = new ConnectionStringSettings(name, connectionString, providerName);
            }
            else
            {
                connectionSettings = new ConnectionStringSettings(name, connectionString);
            }
            if (config.ConnectionStrings.ConnectionStrings[name] == null)
            {
                config.ConnectionStrings.ConnectionStrings.Add(connectionSettings);
            }
            else
            {
                config.ConnectionStrings.ConnectionStrings[name].ConnectionString = connectionSettings.ConnectionString;
                if (providerName != null)
                {
                    config.ConnectionStrings.ConnectionStrings[name].ProviderName = connectionSettings.ProviderName;
                }
            }
            config.Save(System.Configuration.ConfigurationSaveMode.Modified);
            System.Configuration.ConfigurationManager.RefreshSection(config.ConnectionStrings.SectionInformation.Name);
        }
    }
}
