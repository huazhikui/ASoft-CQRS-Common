using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace ASoft
{
    /// <summary>
    /// 使用反射执行对象上的方法
    /// </summary>
    public static class Reflect
    {
        #region 创建对象
        /// <summary>
        /// 创建指定类型的一个默认实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>指定类型的一个实例</returns>
        public static T CreateInstance<T>()
            where T : class
        {
            return typeof(T).Assembly.CreateInstance(typeof(T).FullName) as T;
        }

        /// <summary>
        /// 创建指定类型的一个默认实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns>指定类型的一个实例</returns>
        public static T CreateInstance<T>(bool ignoreCase)
            where T : class
        {
            return typeof(T).Assembly.CreateInstance(typeof(T).FullName, ignoreCase) as T;
        }

        /// <summary>
        /// 创建指定类型的一个默认实例
        /// </summary>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="bindingAttr">方法的类型</param>
        /// <param name="binder"></param>
        /// <param name="activationAttributes"></param>
        /// <param name="Params">构造函数的参数</param>
        /// <param name="culture"></param>
        /// <returns>指定类型的一个实例</returns>
        public static T CreateInstance<T>(bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
            where T : class
        {
            return typeof(T).Assembly.CreateInstance(typeof(T).FullName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes) as T;
        }

        /// <summary>
        /// 加载指定的程序集,并创建一个指定类型的对象
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="className">要创建的对象的类名</param>
        /// <returns>返回创建的对象</returns>
        public static object CreateInstance(string assemblyName, string className)
        {
            return Assembly.Load(assemblyName).CreateInstance(className);
        }

        /// <summary>
        /// 加载指定的程序集,并创建一个指定类型的对象
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="className">要创建的对象的类名</param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns>返回创建的对象</returns>
        public static object CreateInstance(string assemblyName, string className, bool ignoreCase)
        {
            return Assembly.Load(assemblyName).CreateInstance(className, ignoreCase);
        }

        /// <summary>
        /// 加载指定的程序集,并创建一个指定类型的对象
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="className">要创建的对象的类名</param>
        /// <param name="Params">构造函数的参数</param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <param name="activationAttributes"></param>
        /// <param name="binder"></param>
        /// <param name="bindingAttr"></param>
        /// <param name="culture"></param>
        /// <returns>返回创建的对象</returns>
        public static object CreateInstance(string assemblyName, string className, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
        {
            return Assembly.Load(assemblyName).CreateInstance(className, ignoreCase, bindingAttr, binder, args, culture, activationAttributes);
        }


        #endregion

        #region 方法反射

        /// <summary>
        /// 检查一个对象是否包含某个指定的方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">方法名</param>
        /// <returns>返回一个值,指示参数给定的对象是否包含指定的方法</returns>
        public static bool HasMethod(object o, string name)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetMethod(name) != null;
            }
            return o.GetType().GetMethod(name) != null;
        }

        /// <summary>
        /// 检查一个对象是否包含某个指定的方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">方法名</param>
        /// <param name="bindingAttr">方法的类型</param>
        /// <returns>返回一个值,指示参数给定的对象是否包含指定的方法</returns>
        public static bool HasMethod(object o, string name, BindingFlags bindingAttr)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetMethod(name, bindingAttr) != null;
            }
            return o.GetType().GetMethod(name, bindingAttr) != null;
        }

        /// <summary>
        /// 使用反射执行对象上的方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">方法名</param>
        /// <returns>方法执行后的返回值</returns>
        public static object ExecMethod(object o, string name)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetMethod(name).Invoke(o, null);
            }
            return o.GetType().GetMethod(name).Invoke(o, null);
        }

        /// <summary>
        /// 使用反射执行对象上的方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">方法名</param>
        /// <param name="parameters">方法调用的参数</param>
        /// <returns>方法执行后的返回值</returns>
        public static object ExecMethod(object o, string name, object[] parameters)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetMethod(name).Invoke(o, parameters);
            }
            return o.GetType().GetMethod(name).Invoke(o, parameters);
        }

        /// <summary>
        /// 使用反射执行对象上的方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">方法名</param>
        /// <param name="bindingAttr">方法的类型</param>
        /// <returns>方法执行后的返回值</returns>
        public static object ExecMethod(object o, string name, BindingFlags bindingAttr)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetMethod(name, bindingAttr).Invoke(o, null);
            }
            return o.GetType().GetMethod(name, bindingAttr).Invoke(o, null);
        }

        /// <summary>
        /// 使用反射执行对象上的方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">方法名</param>
        /// <param name="bindingAttr">方法的类型</param>
        /// <param name="parameters">方法调用的参数</param>
        /// <returns>方法执行后的返回值</returns>
        public static object ExecMethod(object o, string name, BindingFlags bindingAttr, object[] parameters)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetMethod(name, bindingAttr).Invoke(o, parameters);
            }
            return o.GetType().GetMethod(name, bindingAttr).Invoke(o, parameters);
        }
        #endregion

        #region 字段反射
        /// <summary>
        /// 检查对象是否包含指定名称的字段
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">字段名称</param>
        /// <returns>返回一个值,指示是否包含指定名称的字段</returns>
        public static bool HasField(object o, string name)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetField(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) != null;
            }
            return o.GetType().GetField(name) != null;
        }

        /// <summary>
        /// 检查对象是否包含指定名称的字段
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">字段名称</param>
        /// <param name="bindingAttr">属性的类型</param>
        /// <returns>返回一个值,指示是否包含指定名称的字段</returns>
        public static bool HasField(object o, string name, BindingFlags bindingAttr)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetField(name, bindingAttr) != null;
            }
            return o.GetType().GetField(name, bindingAttr) != null;
        }

        /// <summary>
        /// 使用反射获取字段的值
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">字段名称</param>
        /// <returns>字段值</returns>
        public static object GetField(object o, string name)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetField(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).GetValue(null);
            }
            return o.GetType().GetField(name).GetValue(o);
        }

        /// <summary>
        /// 使用反射获取字段的值
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">字段名称</param>
        /// <param name="bindingAttr">属性的类型</param>
        /// <returns>字段值</returns>
        public static object GetField(object o, string name, BindingFlags bindingAttr)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetField(name, bindingAttr).GetValue(null);
            }
            return o.GetType().GetField(name, bindingAttr).GetValue(o);
        }

        /// <summary>
        /// 使用反射为字段设置值
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">字段名称</param>
        /// <param name="value">value</param>
        public static void SetField(object o, string name, object value)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                ((Type)o).GetField(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).SetValue(value, null);
            }
            o.GetType().GetField(name).SetValue(o, value);
        }

        /// <summary>
        /// 使用反射为字段设置值
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">字段名称</param>
        /// <param name="bindingAttr">属性的类型</param>
        /// <param name="value">属性的值</param>
        /// <returns></returns>
        public static void SetField(object o, string name, BindingFlags bindingAttr, string value)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                ((Type)o).GetField(name, bindingAttr).SetValue(value, null);
            }
            o.GetType().GetField(name, bindingAttr).SetValue(o, value);
        }

        #endregion

        #region 属性
        /// <summary>
        /// 检查某个指定的属性是否包含get方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">属性名称</param>
        /// <returns>返回一个值,指示指定的属性是否包含指定属性</returns>
        public static bool HasProperty(object o, string name)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetProperty(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) != null;
            }
            return o.GetType().GetProperty(name) != null;

        }

        /// <summary>
        /// 检查某个指定的属性是否包含get方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">属性名称</param>
        /// <returns>返回一个值,指示指定的属性是否包含get方法</returns>
        /// <param name="bindingAttr">属性的类型</param>
        public static bool HasProperty(object o, string name, BindingFlags bindingAttr)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetProperty(name, bindingAttr) != null;
            }
            return o.GetType().GetProperty(name, bindingAttr) != null;
        }

        /// <summary>
        /// 检查某个指定的属性是否包含get方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">属性名称</param>
        /// <returns>返回一个值,指示指定的属性是否包含get方法</returns>
        public static bool HasGetProperty(object o, string name)
        {
            PropertyInfo pi = null;
            if (o.GetType().FullName == "System.RuntimeType")
            {
                pi = ((Type)o).GetProperty(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            }
            else
            {
                pi = o.GetType().GetProperty(name);
            }
            if (pi != null)
            {
                return pi.GetGetMethod() != null || pi.GetGetMethod(true) != null;
            }
            return false;
        }

        /// <summary>
        /// 检查某个指定的属性是否包含get方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">属性名称</param>
        /// <returns>返回一个值,指示指定的属性是否包含get方法</returns>
        /// <param name="bindingAttr">属性的类型</param>
        public static bool HasGetProperty(object o, string name, BindingFlags bindingAttr)
        {
            PropertyInfo pi = null;
            if (o.GetType().FullName == "System.RuntimeType")
            {
                pi = ((Type)o).GetProperty(name, bindingAttr);
            }
            else
            {
                pi = o.GetType().GetProperty(name, bindingAttr);
            }
            if (pi != null)
            {
                return pi.GetGetMethod() != null || pi.GetGetMethod(true) != null;
            }
            return false;
        }

        /// <summary>
        /// 检查某个指定的属性是否包含set方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">属性名称</param>
        /// <returns>返回一个值,指示指定的属性是否包含set方法</returns>
        public static bool HasSetProperty(object o, string name)
        {
            PropertyInfo pi = null;
            if (o.GetType().FullName == "System.RuntimeType")
            {
                pi = ((Type)o).GetProperty(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            }
            else
            {
                pi = o.GetType().GetProperty(name);
            }
            if (pi != null)
            {
                return pi.GetSetMethod() != null || pi.GetSetMethod(true) != null;
            }
            return false;
        }

        /// <summary>
        /// 检查某个指定的属性是否包含set方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">属性名称</param>
        /// <param name="bindingAttr">属性的类型</param>
        /// <returns>返回一个值,指示指定的属性是否包含set方法</returns>
        public static bool HasSetProperty(object o, string name, BindingFlags bindingAttr)
        {
            PropertyInfo pi = null;
            if (o.GetType().FullName == "System.RuntimeType")
            {
                pi = ((Type)o).GetProperty(name, bindingAttr);
            }
            else
            {
                pi = o.GetType().GetProperty(name, bindingAttr);
            }
            if (pi != null)
            {
                return pi.GetSetMethod() != null || pi.GetSetMethod(true) != null;
            }
            return false;
        }

        /// <summary>
        /// 使用反射执行Get访问器
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">属性名称</param>
        /// <param name="bindingAttr">属性的类型</param>
        /// <returns>属性的返回值</returns>
        public static object GetProperty(object o, string name)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetGetMethod().Invoke(null, null);
            }
            if (o.GetType().GetProperty(name) == null)
            {
                return null;
            }
            return o.GetType().GetProperty(name).GetValue(o, null);
        }

        /// <summary>
        /// 使用反射执行Get访问器
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">属性名称</param>
        /// <returns>属性的返回值</returns>
        public static object GetProperty(object o, string name, BindingFlags bindingAttr)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetProperty(name, bindingAttr).GetGetMethod().Invoke(null, null);
            }
            return o.GetType().GetProperty(name, bindingAttr).GetValue(o, null);
        }

        /// <summary>
        /// 使用反射执行Set访问器
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">属性名称</param>
        /// <param name="value">属性值</param>
        public static void SetProperty(object o, string name, object value)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                ((Type)o).GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetSetMethod().Invoke(null, null);
                return;
            }
            o.GetType().GetProperty(name).SetValue(o, value, null);
        }

        /// <summary>
        /// 使用反射执行Set访问器
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">属性名称</param>
        /// <param name="value">属性值</param>
        /// <param name="bindingAttr">属性的类型</param>
        public static void SetProperty(object o, string name, object value, BindingFlags bindingAttr)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                ((Type)o).GetProperty(name, bindingAttr).GetSetMethod().Invoke(null, new object[] { value });
                return;
            }
            o.GetType().GetProperty(name, bindingAttr).SetValue(o, value, null);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 检查对象是否包含指定事件
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">事件名称</param>
        public static bool HasEvent(object o, string name)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetType().GetEvent(name) != null;
            }
            return o.GetType().GetEvent(name) != null;
        }

        /// <summary>
        /// 检查对象是否包含指定事件
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">事件名称</param>
        /// <param name="bindingAttr">属性的类型</param>
        public static bool HasEvent(object o, string name, BindingFlags bindingAttr)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                return ((Type)o).GetEvent(name, bindingAttr) != null;
            }
            return o.GetType().GetEvent(name, bindingAttr) != null;
        }

        /// <summary>
        /// 为指定事件添加代理方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">事件名称</param>
        /// <param name="method">代理方法</param>
        public static void AddEventHandler(object o, string name, Delegate method)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                ((Type)o).GetEvent(name).AddEventHandler(o, method);
            }
            o.GetType().GetEvent(name).AddEventHandler(o, method);
        }

        /// <summary>
        /// 为指定事件添加代理方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">事件名称</param>
        /// <param name="bindingAttr">属性的类型</param>
        /// <param name="method">代理方法</param>
        public static void AddEventHandler(object o, string name, BindingFlags bindingAttr, Delegate method)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                ((Type)o).GetEvent(name, bindingAttr).AddEventHandler(o, method);
            }
            o.GetType().GetEvent(name, bindingAttr).AddEventHandler(o, method);
        }

        /// <summary>
        /// 为指定事件移除代理方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">事件名称</param>
        /// <param name="method">代理方法</param>
        public static void RemoveEventHandler(object o, string name, Delegate method)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                ((Type)o).GetEvent(name).RemoveEventHandler(o, method);
            }
            o.GetType().GetEvent(name).RemoveEventHandler(o, method);
        }

        /// <summary>
        /// 为指定事件移除代理方法
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="name">事件名称</param>
        /// <param name="bindingAttr">属性的类型</param>
        /// <param name="method">代理方法</param>
        public static void RemoveEventHandler(object o, string name, BindingFlags bindingAttr, Delegate method)
        {
            if (o.GetType().FullName == "System.RuntimeType")
            {
                ((Type)o).GetEvent(name, bindingAttr).RemoveEventHandler(o, method);
            }
            o.GetType().GetEvent(name, bindingAttr).RemoveEventHandler(o, method);
        }
        #endregion

        #region 特性
        /// <summary>
        /// 返回一个值,指示类型是否包含特性
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <typeparam name="A">特性名称</typeparam>
        /// <returns>类型是否包含特性</returns>
        public static bool HasAttribute<T, A>()
            where T : Attribute
        {
            return typeof(T).IsDefined(typeof(A), true);
        }

        /// <summary>
        /// 返回一个值,指示一个对象是否包含特性
        /// </summary>
        /// <typeparam name="A">特性名称</typeparam>
        /// <param name="o">对象</param>
        /// <returns>对象是否包含特性</returns>
        public static bool HasAttribute<A>(MemberInfo o)
            where A : Attribute
        {
            return HasAttribute<A>(o, true);
        }

        /// <summary>
        /// 返回一个值,指示类型是否包含特性
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <typeparam name="A">特性名称</typeparam>
        /// <param name="inherit">是否包含继承特性</param>
        /// <returns>类型是否包含特性</returns>
        public static bool HasAttribute<T, A>(bool inherit)
            where A : Attribute
        {
            return typeof(T).IsDefined(typeof(A), inherit);
        }

        /// <summary>
        /// 返回一个值,指示一个对象是否包含特性
        /// </summary>
        /// <typeparam name="A">特性名称</typeparam>
        /// <param name="o">对象</param>
        /// <param name="inherit">是否来自继承</param>
        /// <returns>对象是否包含特性</returns>
        public static bool HasAttribute<A>(MemberInfo o, bool inherit)
            where A : Attribute
        {
            return o.IsDefined(typeof(A), true);
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <typeparam name="A">特性名称</typeparam>
        /// <returns>类型特性</returns>
        public static A GetAttribute<T, A>()
            where A : Attribute
        {
            return (A)typeof(T).GetCustomAttributes(typeof(A), true)[0];
        }

        /// <summary>
        /// 获取指定对象特性
        /// </summary>
        /// <typeparam name="A">特性名称</typeparam>
        /// <param name="o">对象</param>
        /// <returns>指定对象第一个特性</returns>
        public static A GetAttribute<A>(MemberInfo o)
            where A : Attribute
        {
            return (A)o.GetCustomAttributes(typeof(A), true)[0];
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <typeparam name="A">特性名称</typeparam>
        /// <param name="inherit">是否包含继承特性</param>
        /// <returns>类型特性</returns>
        public static A GetAttribute<T, A>(bool inherit)
            where A : Attribute
        {
            return (A)typeof(T).GetCustomAttributes(typeof(A), inherit)[0];
        }

        /// <summary>
        /// 获取指定对象特性
        /// </summary>
        /// <typeparam name="A">特性名称</typeparam>
        /// <param name="o">对象</param>
        /// <param name="inherit">是否包含继承特性</param>
        /// <returns>指定对象特性</returns>
        public static A GetAttribute<A>(MemberInfo o, bool inherit)
            where A : Attribute
        {
            return (A)o.GetCustomAttributes(typeof(A), inherit)[0];
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <typeparam name="A">特性名称</typeparam>
        /// <returns>类型特性</returns>
        public static A[] GetAttributes<T, A>()
            where A : Attribute
        {
            return typeof(T).GetCustomAttributes(typeof(A), true) as A[];
        }

        /// <summary>
        /// 获取指定对象特性
        /// </summary>
        /// <typeparam name="A">特性名称</typeparam>
        /// <param name="o">对象</param>
        /// <returns>指定对象特性</returns>
        public static A[] GetAttributes<A>(MemberInfo o)
            where A : Attribute
        {
            return GetAttributes<A>(o, true);
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <typeparam name="A">特性名称</typeparam>
        /// <param name="inherit">是否包含继承特性</param>
        /// <returns>类型特性</returns>
        public static A[] GetAttributes<T, A>(bool inherit)
            where A : Attribute
        {
            return typeof(T).GetCustomAttributes(typeof(A), inherit) as A[];
        }

        /// <summary>
        /// 获取指定对象特性
        /// </summary>
        /// <typeparam name="A">特性名称</typeparam>
        /// <param name="o">对象</param>
        /// <param name="inherit">是否包含继承特性</param>
        /// <returns>指定对象特性</returns>
        public static A[] GetAttributes<A>(MemberInfo o, bool inherit)
            where A : Attribute
        {
            return o.GetCustomAttributes(typeof(A), inherit) as A[];
        }

        /// <summary>
        /// 获取指定类型的某个成员的特性
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <typeparam name="A">特性名称</typeparam>
        /// <param name="name">成员名称</param>
        /// <returns>成员特性列表的第一项</returns>
        public static A GetAttribute<T, A>(string name)
            where A : Attribute
        {
            return (A)typeof(A).GetMember(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)[0].GetCustomAttributes(typeof(A), true)[0];
        }

        /// <summary>
        /// 获取指定对象的某个成员的特性
        /// </summary>
        /// <typeparam name="A">特性名称</typeparam>
        /// <param name="o">对象</param>
        /// <param name="name">成员名称</param>
        /// <returns>成员特性列表的第一项</returns>
        public static A GetAttribute<A>(MemberInfo o, string name)
            where A : Attribute
        {
            return (A)o.GetCustomAttributes(typeof(A), true)[0];
        }

        /// <summary>
        /// 获取指定类型的某个成员的特性
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <typeparam name="A">特性名称</typeparam>
        /// <param name="name">成员名称</param>
        /// <returns>成员特性列表</returns>
        public static A[] GetAttributes<T, A>(string name)
            where A : Attribute
        {
            return typeof(T).GetMember(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)[0].GetCustomAttributes(typeof(A), true) as A[];
        }

        /// <summary>
        /// 获取指定对象的某个成员的特性
        /// </summary>
        /// <typeparam name="A">特性名称</typeparam>
        /// <param name="o">对象</param>
        /// <param name="name">成员名称</param>
        /// <returns>成员特性列表</returns>
        public static A[] GetAttributes<A>(MemberInfo o, string name)
           where A : Attribute
        {
            return o.GetCustomAttributes(typeof(A), true) as A[];
        }

        #endregion
    }
}
