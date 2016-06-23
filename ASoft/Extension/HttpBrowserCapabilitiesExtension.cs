using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASoft.Web;
using System.Web;

namespace ASoft.Extension
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpBrowserCapabilitiesExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="brower"></param>
        /// <returns></returns>
        public static BrowserType GetBrowserType(this HttpBrowserCapabilities brower)
        {
            return GetBrowserType(brower.Browser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="brower"></param>
        /// <returns></returns>
        public static BrowserType GetBrowserType(String brower)
        {
            brower = brower.ToUpper();
            switch (brower)
            {
                case "CHROME":
                    return BrowserType.Chrome;
                case "IE":
                    return BrowserType.InternetExplorer;
                case "INTERNETEXPLORER":
                    return BrowserType.InternetExplorer;
                case "OPERA":
                    return BrowserType.Opera;
                case "SAFARI":
                    return BrowserType.Safari;
                default:
                    return BrowserType.UnKnown;
            }
        }
    }
}
