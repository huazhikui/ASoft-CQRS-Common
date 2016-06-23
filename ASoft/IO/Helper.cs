using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Web;
namespace ASoft.IO
{
    /// <summary>
    /// IO操作类
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// 根据一个路径获取绝对路径(如果路径是绝对路径,不处理,否则,使用当前应用的根目录 + 参数路径)
        /// </summary>
        /// <param name="path">文件系统路径</param>
        /// <returns>文件系统绝对路径</returns>
        public static string GetAbsolutePath(string path)
        {
            if (path == null)
            {
                throw new Exception("参数不能为空");
            }
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else
            {
               
                    path = path.Trim().Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                 
            }
            return path;
        }

        /// <summary>
        /// 根据路径信息创建文件夹(如果是相关路径,则在当前应用程序的根目录下创建)
        /// </summary>
        /// <param name="path">要创建的文件夹的路径</param>
        /// <returns>返回创建的文件的全路径</returns>
        public static string CreateDirectory(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = GetAbsolutePath(path);
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        /// <summary>
        /// 根据指定的路径创建文件
        /// </summary>
        /// <param name="path">要创建的文件的路径</param>
        /// <returns>返回一个值,指示文件是否创建成功</returns>
        public static string CreateFile(string path)
        {
            path = GetAbsolutePath(path);
            CreateDirectory(Path.GetDirectoryName(path));
            File.Create(path);
            return path;
        }

        /// <summary>
        /// 创建一个临时文件
        /// </summary>
        /// <returns>临时文件的路径</returns>
        public static string CreateTempFile()
        {
            return Path.GetTempFileName();
        }

        /// <summary>
        /// 创建一个临时文件
        /// </summary>
        /// <returns>临时文件的路径</returns>
        public static string CreateTempFile(string extension)
        {
            string source = Path.GetTempFileName();
            string dest = Path.ChangeExtension(source, extension);
            File.Move(source, dest);
            return dest;
        }

        /// <summary>
        /// 按给定的路径获取文件系统信息(可能是文件,也可能是文件夹)
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>文件系统信息(如果不存在,返回null)</returns>
        public static FileSystemInfo GetFileSystemInfo(string path)
        {
            if (File.Exists(path))
                return new FileInfo(path);

            if (Directory.Exists(path))
                return new DirectoryInfo(path);

            return null;
        }

        /// <summary>
        /// 获取特定类型的文件夹的路径
        /// </summary>
        /// <param name="type">特定的文件夹类型</param>
        /// <returns>文件夹路径</returns>
        public static string GetSpecialPath(Environment.SpecialFolder type)
        {
            return Environment.GetFolderPath(type);
        }

        /// <summary>
        /// 获取.NET Framwork的安装路径(因为使用了注册表,所以只能运行在Windows上)
        /// </summary>
        public static string FrameworkPath
        {
            get
            {
                return Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework").GetValue("InstallRoot").ToString();
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="path">路径</param>
        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        /// <summary>
        /// 删除文件或文件夹
        /// </summary>
        /// <param name="path">路径</param>
        public static void Delete(string path)
        {
            FileSystemInfo fsi = GetFileSystemInfo(path);
            if (fsi != null)
            {
                if (fsi is FileInfo)
                {
                    File.Delete(path);
                }
                else
                {
                    Directory.Delete(path);
                }
            }
        }

        /// <summary>
        /// 根据一个文件路径信息获取文件名
        /// </summary>
        /// <param name="path">文件路径信息</param>
        /// <returns>文件名</returns>
        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// 根据一个文件路径信息获取文件扩展名
        /// </summary>
        /// <param name="path">文件路径信息</param>
        /// <returns>文件扩展名</returns>
        public static string GetFileExtention(string path)
        {
            return Path.GetExtension(path);
        }

        /// <summary>
        /// 根据一个文件路径信息获取文件名(不含扩展名)
        /// </summary>
        /// <param name="path">文件路径信息</param>
        /// <returns>文件名(不含扩展名)</returns>
        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// 更新文件的扩展名
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="extension">新的扩展名</param>
        /// <returns>文件修改后的路径</returns>
        public static string ChangeExtension(string path, string extension)
        {
            return Path.ChangeExtension(path, extension);
        }

        /// <summary>
        /// 检查文件名是否包含扩展名
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>返回一个值,指示文件是否包含扩展名</returns>
        public static bool HasExtension(string path)
        {
            return Path.HasExtension(path);
        }

        /// <summary>
        /// 获取系统临时文件夹路径
        /// </summary>
        /// <returns>系统临时文件夹路径</returns>
        public static string TempPath
        {
            get
            {
                return Path.GetTempPath();
            }
        }

        /// <summary>
        /// 获取或设置当前应用程序的工作目录
        /// </summary>
        /// <returns>当前应用程序的工作目录</returns>
        public static string WorkDirectory
        {
            get
            {
                return Directory.GetCurrentDirectory();
            }
            set
            {
                Directory.SetCurrentDirectory(value);
            }
        }

        /// <summary>
        /// 获取当前应用程序的根目录
        /// </summary>
        /// <returns></returns>
        public static string AppDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        /// <summary>
        /// 获取启动了当前应用程序的可执行文件的路径
        /// </summary>
        /// <returns>启动了当前应用程序的可执行文件的路径</returns>
        public static string SetupDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }
        }

        /// <summary>
        /// 获取启动了当前应用程序的可执行文件的名称
        /// </summary>
        /// <returns>启动了当前应用程序的可执行文件的名称</returns>
        public static string SetupName
        {
            get
            {
                return AppDomain.CurrentDomain.SetupInformation.ApplicationName;
            }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="logicDirectory"></param>
        /// <param name="prefix">Scan</param>
        /// <param name="fileType"></param>
        /// <param name="fileStream"></param>
        /// <param name="fileVirPath"></param>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public static bool UploadFile(String logicDirectory, String prefix, FileType fileType, Stream fileStream, out String fileVirPath, out long fileSize)
        {
            fileSize = 0;
            fileVirPath = "";
            if (fileStream == null)
            {
                return false;
            }

            //文件格式为\Upload\201501\05\docid\HttpPostedFile.FileName(时间+random(100))
            String virPath = String.Format("{0}/{1}/{2}", prefix, DateTime.Now.ToString("yyyyMM"), DateTime.Now.ToString("dd"));
            String saveDir = String.Format("{0}\\{1}", logicDirectory, virPath.Replace("/", "\\"));
            //时间戳+随机数   = 文件名
            String fileName = String.Format("{0}_{1}.{2}", ASoft.Rand.DateTimeTick, ASoft.Rand.Number(5), fileType.ToString());
            fileVirPath = virPath + "/" + fileName;
            String saveFullPath = saveDir + "\\" + fileName;
            int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];

            ASoft.IO.Helper.CreateDirectory(saveDir);

            using (FileStream fs = new FileStream(saveFullPath, FileMode.Create))
            {
                while (fileSize < fileStream.Length)
                {
                    //从输入流放进缓冲区
                    int bytes = fileStream.Read(buffer, 0, bufferSize);
                    fs.Write(buffer, 0, bytes);
                    fs.Flush(); // 字节写入文件流
                    fileSize += bytes;// 更新大小
                }
                fs.Close();
                fs.Dispose();
            }

            fileStream.Close();
            fileStream.Dispose();
            return true;
        }

        public static List<FileInfo> GetFilesByStartName(String logicDirectory, String fileStartName, String fileExtention = ".txt")
        {
            DirectoryInfo di = new DirectoryInfo(logicDirectory);
            FileInfo[] files = di.GetFiles();
            List<FileInfo> fileList = null;
            try
            {
                fileList = di.GetFiles().Where(f => f.Name.EndsWith(fileExtention) && f.Name.StartsWith(fileStartName)).ToList<FileInfo>();
            }
            catch
            {

            }
            return fileList;
        }
    }
}

public enum FileType
{
    JPG = 0,
    TXT = 1,
    DOC = 2,
    XLS = 3,
    DOCX = 4,
    XLSX = 5
}