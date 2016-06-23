using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace ASoft.IO
{
    /// <summary>
    /// 共享文件连接类
    /// </summary>
    public class FileConnection : IDisposable
    {
        #region win32 API
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername,
                                             string lpszDomain,
                                             string lpszPassword,
                                             int dwLogonType,
                                             int dwLogonProvider,
                                             ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool DuplicateToken(IntPtr existingTokenHandle,
                                                 int SECURITY_IMPERSONATION_LEVEL,
                                                 ref IntPtr duplicateTokenHandle);


        // logon types
        const int LOGON32_LOGON_INTERACTIVE = 2;
        const int LOGON32_LOGON_NETWORK = 3;
        const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

        // logon providers
        const int LOGON32_PROVIDER_DEFAULT = 0;
        const int LOGON32_PROVIDER_WINNT50 = 3;
        const int LOGON32_PROVIDER_WINNT40 = 2;
        const int LOGON32_PROVIDER_WINNT35 = 1;

        #endregion

        WindowsIdentity newIdentity;
        WindowsImpersonationContext impersonatedUser;
        IntPtr token = IntPtr.Zero;
        private bool isConnected = false;
        private string server;
        private string username;
        private string password;

        /// <summary>
        /// 服务器
        /// </summary>
        public string Server
        {
            get
            {
                return this.server;
            }
            set
            {
                this.server = value;
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get
            {
                return this.username;
            }
            set
            {
                this.username = value;
            }
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
            }
        }

        /// <summary>
        /// 是否连接中
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public FileConnection(string server, string username, string password)
        {
            this.server = server;
            this.username = username;
            this.password = password;
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            if (string.IsNullOrWhiteSpace(server))
            {
                return true;
            }
            isConnected = LogonUser(username, server, password, LOGON32_LOGON_NEW_CREDENTIALS, LOGON32_PROVIDER_DEFAULT, ref token);
            newIdentity = new WindowsIdentity(token);
            impersonatedUser = newIdentity.Impersonate();
            return IsConnected;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (IsConnected)
            {
                if (impersonatedUser != null)
                {
                    impersonatedUser.Undo();

                }
                if (token != IntPtr.Zero)
                {
                    CloseHandle(token);
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }
    }
}
