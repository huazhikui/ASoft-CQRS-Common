using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections;

namespace ASoft
{
    /// <summary>
    /// 消息体
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code">消息代码</param>
        /// <param name="desc">错误说明</param>
        public Message(string code, string desc)
        {
            this.Code = code;
            this.Desc = desc;
        }

        /// <summary>
        /// 消息代码 
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// 消息说明
        /// </summary>
        public string Desc
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 应用于上下文错误传递的类
    /// </summary>
    public class ContextMessage
    {
        List<Message> msgs = new List<Message>();
        List<Message> emsg = new List<Message>();
        private readonly static ContextMessage curmsg = new ContextMessage();
        private const string CurMsgName = "CurMsgName";

        /// <summary>
        /// 获取当前系统中的上下文消息,这个只可以在单线程的应用程序和站点上使用,不建议在多线程应用程序上使用
        /// </summary>
        public static ContextMessage CurMsg
        {
            get
            {
                if (System.Web.HttpContext.Current != null)
                {
                    if (!System.Web.HttpContext.Current.Items.Contains(CurMsgName))
                    {
                        System.Web.HttpContext.Current.Items.Add(CurMsgName, new ContextMessage());
                    }
                    return (ContextMessage)System.Web.HttpContext.Current.Items[CurMsgName];
                }
                else
                {
                    return curmsg;
                }
            }
        }

        private bool valid = true;
        /// <summary>
        /// 返回一个值,指示结果是否正确
        /// </summary>
        public bool Valid
        {
            get
            {
                return this.valid;
            }
        }

        private bool hasmsg = false;
        /// <summary>
        /// 是否包含普通消息
        /// </summary>
        public bool HasMsg
        {
            get
            {
                return hasmsg;
            }
        }

        /// <summary>
        /// 重置上下文消息
        /// </summary>
        public void Reset()
        {
            this.msgs.Clear();
            this.emsg.Clear();
            this.valid = true;
        }

        /// <summary>
        /// 添加一个消息
        /// </summary>
        /// <param name="msg">格式化文本</param>
        public void AddInfo(string msg)
        {
            hasmsg = true;
            msgs.Add(new Message(string.Empty, msg));
        }

        /// <summary>
        /// 添加一个消息
        /// </summary>
        /// <param name="code">消息代码</param>
        /// <param name="msg">格式化文本</param>
        public void AddInfo(string code, string msg)
        {
            hasmsg = true;
            msgs.Add(new Message(code, msg));
        }

        /// <summary>
        /// 添加一个错误
        /// </summary>
        /// <param name="msg">格式化文本</param>
        public void AddErr(string msg)
        {
            this.valid = false;
            emsg.Add(new Message(string.Empty, msg));
        }

        /// <summary>
        /// 添加一个错误
        /// </summary>
        /// <param name="code">消息代码</param>
        /// <param name="msg">格式化文本</param>
        public void AddErr(string code, string msg)
        {
            this.valid = false;
            emsg.Add(new Message(code, msg));
        }

        /// <summary>
        /// 返回第一个消息
        /// </summary>
        public Message FirstMsg
        {
            get
            {
                return hasmsg ? this.msgs[0] : null;
            }
        }

        /// <summary>
        /// 返回第一个错误消息
        /// </summary>
        public Message FirstErr
        {
            get
            {
                return valid ? this.emsg[0] : null;
            }
        }

        /// <summary>
        /// 获取消息枚举
        /// </summary>
        public IEnumerable<Message> Msgs
        {
            get
            {
                foreach (Message msg in msgs)
                {
                    yield return msg;
                }
            }
        }

        /// <summary>
        /// 获取错误枚举
        /// </summary>
        public IEnumerable<Message> Errs
        {
            get
            {
                foreach (Message msg in emsg)
                {
                    yield return msg;
                }
            }
        }
    }
}