using System;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Xml;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
namespace ASoft
{
    /// <summary>
    /// 对象序列化与反序列化类
    /// </summary>
    public static class Serialize
    {
        #region 二进制序列化

        /// <summary>
        /// 把一个对象进行二进序列化后存入到一个数据流中
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <returns>对象序列化后的数据流</returns>
        public static MemoryStream BinarySerilize(object o)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
           formatter.Serialize(stream, o);
            return stream;
        }

        /// <summary>
        /// 把一个对象进行二进序列化后存入到一个数据流中
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="stream">对象序列化后的数据流</param>
        public static void BinarySerilize(object o, Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, o);
        }

        /// <summary>
        /// 把一个二进制数据流的内容进行反序列化
        /// </summary>
        /// <param name="stream">反序列化的数据流</param>
        /// <returns>反序列化后的对象</returns>
        public static T BinaryDeserilize<T>(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return (T)formatter.Deserialize(stream);
        }

        #endregion

        #region Soap序列化与反序列化

        /// <summary>
        /// 把一个对象进行Soap序列化并返回序列化后的字符串
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <returns>对象序列化后的数据流</returns>
        public static string SoapSerilize(object o)
        {
            return SoapSerilize(o, (Header[])null);
        }

        /// <summary>
        /// 把一个对象进行Soap序列化并返回序列化后的字符串
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="headers">头</param>
        /// <returns>对象序列化后的数据流</returns>
        public static string SoapSerilize(object o, Header[] headers)
        {
            MemoryStream stream = new MemoryStream();
            new SoapFormatter().Serialize(stream, o, headers);
            stream.Seek(0, SeekOrigin.Begin);
            return new StreamReader(stream).ReadToEnd();
        }

        /// <summary>
        /// 把一个对象进行Soap序列化并返回序列化后的字符串
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="stream">对象序列化后的数据流</param>
        public static void SoapSerilize(object o, Stream stream)
        {
            SoapSerilize(o, (Header[])null, stream);
        }

        /// <summary>
        /// 把一个对象进行Soap序列化并返回序列化后的字符串
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="headers">头</param>
        /// <param name="stream">对象序列化后的数据流</param>
        public static void SoapSerilize(object o, Header[] headers, Stream stream)
        {
            long pos = stream.Seek(0, SeekOrigin.Current);
            new SoapFormatter().Serialize(stream, o, headers);
            stream.Seek(pos, SeekOrigin.Begin);
        }

        /// <summary>
        /// 把一个Soap数据流的内容进行反序列化
        /// </summary>
        /// <param name="stream">Soap数据流</param>
        /// <returns>反序列化后的对象</returns>
        public static T SoapDeserilize<T>(Stream stream)
        {
            SoapFormatter formatter = new SoapFormatter();
            return (T)formatter.Deserialize(stream);
        }

        /// <summary>
        /// 把一个Soap数据流的内容进行反序列化
        /// </summary>
        /// <param name="soap">Soap字符串</param>
        /// <returns>反序列化后的对象</returns>
        public static T SoapDeserilize<T>(string soap)
        {
            return SoapDeserilize<T>(soap, Encoding.UTF8);
        }

        /// <summary>
        /// 把一个Soap数据流的内容进行反序列化
        /// </summary>
        /// <param name="soap">Soap字符串</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>反序列化后的对象</returns>
        public static T SoapDeserilize<T>(string soap, Encoding encoding)
        {
            MemoryStream ms = new MemoryStream(encoding.GetBytes(soap));
            ms.Seek(0, SeekOrigin.Begin);
            SoapFormatter formatter = new SoapFormatter();
            return (T)formatter.Deserialize(ms);
        }
        #endregion

        #region XML序列化与反序列化

        /// <summary>
        /// 把一个对象进行XML序列化 
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <returns>序列化后的数据流</returns>
        public static string XmlSerilize(object o)
        {
            return XmlSerilize(o, (XmlSerializerNamespaces)null);
        }

        /// <summary>
        /// 把一个对象进行XML序列化 
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="useDefaultNamespace">是否使用默认的命名空间</param>
        /// <returns>序列化后的数据流</returns>
        public static string XmlSerilize(object o, bool useDefaultNamespace)
        {
            if (useDefaultNamespace)
            {
                return XmlSerilize(o, (XmlSerializerNamespaces)null);
            }
            else
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);
                return XmlSerilize(o, ns);
            }
        }

        /// <summary>
        /// 把一个对象进行XML序列化 
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="encoding">序列化后的XML流的encoding</param>
        /// <returns>序列化后的数据流</returns>
        public static string XmlSerilize(object o, Encoding encoding)
        {
            return XmlSerilize(o, (XmlSerializerNamespaces)null, encoding);
        }

        /// <summary>
        /// 把一个对象进行XML序列化 
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="namespaces">命名空间</param>
        /// <returns>序列化后的数据流</returns>
        public static string XmlSerilize(object o, XmlSerializerNamespaces namespaces)
        {
            return XmlSerilize(o, namespaces, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 把一个对象进行XML序列化 
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="namespaces">命名空间</param>
        /// <param name="encoding">序列化后的XML流的encoding</param>
        /// <returns>序列化后的数据流</returns>
        public static string XmlSerilize(object o, XmlSerializerNamespaces namespaces, Encoding encoding)
        {
            XmlSerializer formatter = new XmlSerializer(o.GetType());
            MemoryStream ms = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(ms, encoding))
            {
                formatter.Serialize(writer, o, namespaces);
                writer.Close();
            }
            return encoding.GetString(ms.ToArray()).Trim();
        }

        /// <summary>
        /// 把一个对象进行XML序列化 
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="doc">序列化后的XML文档对象</param>
        public static void XmlSerilize(object o, XmlDocument doc)
        {
            XmlSerilize(o, doc, null);
        }

        /// <summary>
        /// 把一个对象进行XML序列化 
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="doc">序列化后的XML文档对象</param>
        /// <param name="namespaces">命名空间</param>
        public static void XmlSerilize(object o, XmlDocument doc, XmlSerializerNamespaces namespaces)
        {
            XmlSerilize(o, doc, namespaces, Encoding.UTF8);
        }

        /// <summary>
        /// 把一个对象进行XML序列化 
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="doc">序列化后的XML文档对象</param>
        /// <param name="namespaces">命名空间</param>
        /// <param name="encoding">编码</param>
        public static void XmlSerilize(object o, XmlDocument doc, XmlSerializerNamespaces namespaces, Encoding encoding)
        {
            XmlSerializer formatter = new XmlSerializer(o.GetType());
            MemoryStream ms = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(ms, encoding))
            {
                formatter.Serialize(writer, o, namespaces);
                ms.Position = 0;
                doc.Load(ms);
                ms.Close();
            }
        }

        /// <summary>
        /// 把一个对象进行XML序列化 
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="serializer">格式化对象</param>
        /// <param name="doc">序列化后的XML文档对象</param>
        public static void XmlSerilize(object o, XmlSerializer serializer, XmlDocument doc)
        {
            XmlSerilize(o, serializer, doc, null);
        }

        /// <summary>
        /// 把一个对象进行XML序列化 
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="serializer">格式化对象</param>
        /// <param name="doc">序列化后的XML文档对象</param>
        /// <param name="namespaces">命名空间</param>
        public static void XmlSerilize(object o, XmlSerializer serializer, XmlDocument doc, XmlSerializerNamespaces namespaces)
        {
            XmlSerilize(o, serializer, doc, namespaces, Encoding.UTF8);
        }

        /// <summary>
        /// 把一个对象进行XML序列化 
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="serializer">格式化对象</param>
        /// <param name="doc">序列化后的XML文档对象</param>
        /// <param name="namespaces">命名空间</param>
        /// <param name="encoding">编码</param>
        public static void XmlSerilize(object o, XmlSerializer serializer, XmlDocument doc, XmlSerializerNamespaces namespaces, Encoding encoding)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }
            MemoryStream ms = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(ms, encoding))
            {
                serializer.Serialize(writer, o, namespaces);
                ms.Position = 0;
                doc.Load(ms);
                ms.Close();
            }
        }

        /// <summary>
        /// 把一个XML格式的数据反序列化,返回反序列化后的对象
        /// </summary>
        /// <param name="xml">要反序列化的XML数据</param>
        /// <returns>反序列化后的对象</returns>
        public static T XmlDeserilize<T>(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNodeReader xmlReader = new XmlNodeReader(doc.DocumentElement);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(xmlReader);
        }

        /// <summary>
        /// 把一个XML格式的数据反序列化,返回反序列化后的对象
        /// </summary>
        /// <param name="xmlDoc">要反序列化的XML数据文档</param>
        /// <returns>反序列化后的对象</returns>
        public static T XmlDeserilize<T>(XmlDocument xmlDoc)
        {
            XmlNodeReader xmlReader = new XmlNodeReader(xmlDoc.DocumentElement);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(xmlReader);
        }

        /// <summary>
        /// 把一个XML数据流反序列化,返回反序列化后的对象
        /// </summary>
        /// <param name="stream">要反序列化的对象</param>
        /// <returns>反序列化后的对象</returns>
        public static T XmlDeserilize<T>(Stream stream)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            return (T)formatter.Deserialize(stream);
        }

        /// <summary>
        /// 把一个XML格式的数据反序列化,返回反序列化后的对象
        /// </summary>
        /// <param name="reader">要反序列化的XML数据</param>
        /// <returns>反序列化后的对象</returns>
        public static T XmlDeserilize<T>(TextReader reader)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            return (T)formatter.Deserialize(reader);
        }

        /// <summary>
        /// 把一个XML格式的数据反序列化,返回反序列化后的对象
        /// </summary>
        /// <param name="reader">要反序列化的XML数据</param>
        /// <returns>反序列化后的对象</returns>
        public static T XmlDeserilize<T>(XmlReader reader)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            return (T)formatter.Deserialize(reader);
        }

        /// <summary>
        /// 反序列化指定 System.xml.XmlReader 和编码样式包含的 XML 文档。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="reader">包含要反序列化的 XML 文档的 System.xml.XmlReader</param>
        /// <param name="encodingStyle">序列化的 XML 的编码样式</param>
        /// <returns>反序列化的对象</returns>
        public static T XmlDeserilize<T>(XmlReader reader, string encodingStyle)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            return (T)formatter.Deserialize(reader, encodingStyle);
        }

        /// <summary>
        /// 反序列化指定 System.xml.XmlReader 和编码样式包含的 XML 文档。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="reader">包含要反序列化的 XML 文档的 System.xml.XmlReader</param>
        /// <param name="events">System.Xml.Serialization.XmlDeserializationEvents 类的实例</param>
        /// <returns>反序列化的对象</returns>
        public static T XmlDeserilize<T>(XmlReader reader, XmlDeserializationEvents events)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            return (T)formatter.Deserialize(reader, events);

        }

        /// <summary>
        /// 反序列化指定 System.xml.XmlReader 和编码样式包含的 XML 文档。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="reader">包含要反序列化的 XML 文档的 System.xml.XmlReader</param>
        /// <param name="encodingStyle">序列化的 XML 的编码样式</param>
        /// <param name="events">System.Xml.Serialization.XmlDeserializationEvents 类的实例</param>
        /// <returns>反序列化的对象</returns>
        public static T XmlDeserilize<T>(XmlReader reader, string encodingStyle, XmlDeserializationEvents events)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            return (T)formatter.Deserialize(reader, encodingStyle, events);
        }

        #endregion

        #region JSON序列化与反序列化

        /// <summary>
        /// 把一个对象进行JSON序列化
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <returns>序列化后的对象</returns>
        public static string JsonSerilize(object o)
        {
            //using (var output = new StringWriter())
            //{
            //    Jil.JSON.Serialize(o, output);
            //    output.Close();
            //    return output.ToString();
            //}

            String result = Newtonsoft.Json.JsonConvert.SerializeObject(o);
            result = result.Replace(@"\/Date(-62135596800000+0800)\/", "");
            result = ConvertJsonDateToDateString(result);
            return result;
        }


        /// <summary>
        /// 把一个对象进行JSON序列化
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="formatting">格式化样式,缩进或不缩进</param>
        /// <returns>序列化后的对象</returns>
        public static string JsonSerilize(object o, Newtonsoft.Json.Formatting formatting)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(o, formatting);
        }

        /// <summary>
        /// 把一个对象进行JSON序列化
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="converters"></param>
        /// <returns>序列化后的对象</returns>
        public static string JsonSerilize(object o, params Newtonsoft.Json.JsonConverter[] converters)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(o, converters);
        }

        /// <summary>
        /// 把一个对象进行JSON序列化
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="formatting">格式化样式,缩进或不缩进</param>
        /// <param name="settings"></param>
        /// <returns>序列化后的对象</returns>
        public static string JsonSerilize(object o, Newtonsoft.Json.Formatting formatting, Newtonsoft.Json.JsonSerializerSettings settings)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(o, formatting, settings);
        }

        /// <summary>
        /// 把一个对象进行JSON序列化
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="formatting">格式化样式,缩进或不缩进</param>
        /// <param name="converters"></param>
        /// <returns>序列化后的对象</returns>
        public static string JsonSerilize(object o, Newtonsoft.Json.Formatting formatting, params Newtonsoft.Json.JsonConverter[] converters)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(o, formatting, converters);
        }

        /// <summary>
        /// 把一个XML节点对象进行JSON序列化
        /// </summary>
        /// <param name="node">要序列化的XML节点对象</param>
        /// <returns>序列化后的对象</returns>
        public static string JsonXmlNodeSerilize(XmlNode node)
        {
            return Newtonsoft.Json.JsonConvert.SerializeXmlNode(node);
        }

        /// <summary>
        /// 把一个XML节点对象进行JSON序列化
        /// </summary>
        /// <param name="node">要序列化的XML节点对象</param>
        /// <param name="formatting">格式化样式,缩进或不缩进</param>
        /// <returns>序列化后的对象</returns>
        public static string JsonXmlNodeSerilize(XmlNode node, Newtonsoft.Json.Formatting formatting)
        {
            return Newtonsoft.Json.JsonConvert.SerializeXmlNode(node, formatting);
        }

        /// <summary>
        /// 把一个字符串进行JSON反序列化
        /// </summary>
        /// <typeparam name="T">要反序列化的对象的类型</typeparam>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <returns>反序列化后的对象</returns>
        public static T JsonDeserilize<T>(string value)
        {
           value = ConvertDateStringToJsonDate(value);
            return (T)Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 把一个字符串进行JSON反序列化
        /// </summary>
        /// <typeparam name="T">要反序列化的对象的类型</typeparam>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <param name="settings"></param>
        /// <returns>反序列化后的对象</returns>
        public static T JsonDeserilize<T>(string value, Newtonsoft.Json.JsonSerializerSettings settings)
        {
            return (T)Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value, settings);
        }

        /// <summary>
        /// 把一个字符串进行JSON反序列化
        /// </summary>
        /// <typeparam name="T">要反序列化的对象的类型</typeparam>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <param name="converters"></param>
        /// <returns>反序列化后的对象</returns>
        public static T JsonDeserilize<T>(string value, params Newtonsoft.Json.JsonConverter[] converters)
        {
            return (T)Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value, converters);
        }

        /// <summary>
        /// 把一个字符串进行JSON反序列化
        /// </summary>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <returns>反序列化后的对象</returns>
        public static object JsonDeserilize(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value);
        }

        /// <summary>
        /// 把一个字符串进行JSON反序列化
        /// </summary>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <param name="type">要反序列化的对象的类型</param>
        /// <returns>反序列化后的对象</returns>
        public static object JsonDeserilize(string value, Type type)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
        }

        /// <summary>
        /// 把一个字符串进行JSON反序列化
        /// </summary>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <param name="type">要反序列化的对象的类型</param>
        /// <param name="settings"></param>
        /// <returns>反序列化后的对象</returns>
        public static object JsonDeserilize(string value, Type type, Newtonsoft.Json.JsonSerializerSettings settings)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type, settings);
        }

        /// <summary>
        /// 把一个字符串进行JSON反序列化
        /// </summary>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <param name="type">要反序列化的对象的类型</param>
        /// <param name="converters"></param>
        /// <returns>反序列化后的对象</returns>
        public static object JsonDeserilize(string value, Type type, params Newtonsoft.Json.JsonConverter[] converters)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type, converters);
        }

        /// <summary>
        /// 把一个字符串进行反序列化为一个XmlNode
        /// </summary>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <returns>反序列化后的对象</returns>
        public static XmlNode JsonXmlNodeDeserilize(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeXmlNode(value);
        }

        /// <summary>
        /// 把一个字符串进行反序列化为一个XmlNode
        /// </summary>
        /// <param name="value">要反序列化的对象的原始字符串</param>
        /// <param name="root">根节点名称</param>
        /// <returns>反序列化后的对象</returns>
        public static XmlNode JsonXmlNodeDeserilize(string value, string root)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeXmlNode(value, root);
        }

        #endregion
        #region
        public static String ConvertDateStringToJsonDate(String jsonString)
        {
            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"//Date(1294499956278+0800)//"格式    
            string p = @"/d{4}-/d{2}-/d{2}/s/d{2}:/d{2}:/d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);     
            return jsonString;
        }

        /// <summary>
        ///  将Json序列化的时间由/Date(1294499956278+0800)转为字符串
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static String ConvertJsonDateToDateString(String jsonString)
        {
             
            //替换Json的Date字符串    
            string p = @"\\/Date\(((-)?\d+)\+\d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            return jsonString;
        }

        /// <summary>    
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串    
        /// </summary>    
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }
        /// <summary>    
        /// 将时间字符串转为Json时间    
        /// </summary>    
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("///Date({0}+0800)///", ts.TotalMilliseconds);
            return result;
        }    
        #endregion
    }
}
