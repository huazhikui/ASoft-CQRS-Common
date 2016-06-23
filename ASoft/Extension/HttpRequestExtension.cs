using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace ASoft.Extension
{
    /// <summary>
    /// HttpRequest扩展
    /// </summary>
    public static class HttpRequestExtension
    {
        public static int GetQueryInt(this HttpRequest request, string name)
        {
            string value = request[name];
            return value.ToInt();
        }

        public static int GetQueryInt(this HttpRequest request, string name, int defaultValue)
        {
            string value = request[name];
            return value.ToInt(defaultValue);
        }

        public static long GetQueryLong(this HttpRequest request, string name)
        {
            string value = request[name];
            return value.ToLong();
        }

        public static long GetQueryLong(this HttpRequest request, string name, long defaultValue)
        {
            string value = request[name];
            return value.ToLong(defaultValue);
        }

        public static double GetQueryDouble(this HttpRequest request, string name)
        {
            string value = request[name];
            return value.ToDouble();
        }

        public static double GetQueryDouble(this HttpRequest request, string name, double defaultValue)
        {
            string value = request[name];
            return value.ToDouble(defaultValue);
        }

        public static float GetQueryFloat(this HttpRequest request, string name)
        {
            string value = request[name];
            return value.ToFloat();
        }

        public static float GetQueryFloat(this HttpRequest request, string name, float defaultValue)
        {
            string value = request[name];
            return value.ToFloat(defaultValue);
        }

        public static Guid GetQueryGuid(this HttpRequest request, string name)
        {
            string value = request[name];
            return value.ToGuid();
        }

        public static Guid GetQueryGuid(this HttpRequest request, string name,Guid defaultValue)
        {
            string value = request[name];
            return value.ToGuid(defaultValue);
        }

        public static DateTime GetQueryDate(this HttpRequest request, string name)
        {
            string value = request[name];
            return value.ToDate();
        }

        public static DateTime GetQueryDate(this HttpRequest request, string name,DateTime defaultValue)
        {
            string value = request[name];
            return value.ToDate();
        }
    }
}
