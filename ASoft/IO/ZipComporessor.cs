using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;

namespace ASoft.IO
{
    /// <summary>
    /// zip压缩帮助类
    /// </summary>
    public sealed class ZipComporessor
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Compress(string input)
        {
            string result = string.Empty;
            byte[] buffer = Encoding.UTF8.GetBytes(input);
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (BZip2OutputStream zipStream = new BZip2OutputStream(outputStream))
                {
                    zipStream.Write(buffer, 0, buffer.Length);
                    zipStream.Close();
                }
                return Convert.ToBase64String(outputStream.ToArray());
            }
        }
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decompress(string input)
        {
            string result = string.Empty;
            byte[] buffer = Convert.FromBase64String(input);
            using (Stream inputStream = new MemoryStream(buffer))
            {
                BZip2InputStream zipStream = new BZip2InputStream(inputStream);

                using (StreamReader reader = new StreamReader(zipStream, Encoding.UTF8))
                {
                    //输出
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }

        #region 压缩文件
        /// <summary>
        /// 功能：压缩文件（暂时只压缩文件夹下一级目录中的文件，文件夹及其子级被忽略）
        /// </summary>
        /// <param name="dirPath">被压缩的文件夹夹路径</param>
        /// <param name="zipFilePath">生成压缩文件的路径，为空则默认与被压缩文件夹同一级目录，名称为：文件夹名+.zip</param>
        /// <param name="password">解压密码</param>
        /// <param name="err">出错信息</param>
        /// <returns>是否压缩成功</returns>
        public bool ZipFile(string dirPath, string zipFilePath,String password, out string err)
        {
            err = "";
            if (dirPath == string.Empty)
            {
                err = "要压缩的文件夹不能为空！";
                return false;
            }
            if (!Directory.Exists(dirPath))
            {
                err = "要压缩的文件夹不存在！";
                return false;
            }
            //压缩文件名为空时使用文件夹名＋.zip
            if (zipFilePath == string.Empty)
            {
                if (dirPath.EndsWith("\\"))
                {
                    dirPath = dirPath.Substring(0, dirPath.Length - 1);
                }
                zipFilePath = dirPath + ".zip";
            }

            try
            {
                string[] filenames = Directory.GetFiles(dirPath);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
                {
                    s.SetLevel(9);
                    if (password != null && password != "")
                    {
                        s.Password = password;
                    }
                    byte[] buffer = new byte[4096];
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
            return true;
        }


        /// <summary>  
        /// 压缩单个文件  
        /// </summary>  
        /// <param name="FileToZip">被压缩的文件名称(包含文件路径)</param>  
        /// <param name="ZipedFile">压缩后的文件名称(包含文件路径)</param>  
        /// <param name="CompressionLevel">压缩率0（无压缩）-9（压缩率最高）</param>   
        /// <param name="password">压缩密码</param>   
        public bool ZipFile(string FileToZip, string ZipedFile, int CompressionLevel, String password, out string err)
        {
            err = "";
            //如果文件没有找到，则报错   
            if (!System.IO.File.Exists(FileToZip))
            {
                throw new System.IO.FileNotFoundException("文件：" + FileToZip + "没有找到！");
            }

            if (ZipedFile == string.Empty)
            {
                ZipedFile = Path.GetFileNameWithoutExtension(FileToZip) + ".zip";
            }

            if (Path.GetExtension(ZipedFile) != ".zip")
            {
                ZipedFile = ZipedFile + ".zip";
            }

            ////如果指定位置目录不存在，创建该目录  
            //string zipedDir = ZipedFile.Substring(0,ZipedFile.LastIndexOf("/"));  
            //if (!Directory.Exists(zipedDir))  
            //    Directory.CreateDirectory(zipedDir);  

            //被压缩文件名称  
            string fileName = FileToZip.Substring(FileToZip.LastIndexOf("\\") + 1);

            System.IO.FileStream StreamToZip = new System.IO.FileStream(FileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.FileStream ZipFile = System.IO.File.Create(ZipedFile);
            ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
            ZipEntry ZipEntry = new ZipEntry(fileName); 
            if (password!=null && password!="")
            {
                ZipStream.Password = password;
            }
            ZipStream.PutNextEntry(ZipEntry);
            ZipStream.SetLevel(CompressionLevel);
            

            byte[] buffer = new byte[2048];
            System.Int32 size = StreamToZip.Read(buffer, 0, buffer.Length);
            ZipStream.Write(buffer, 0, size);
            try
            {
                while (size < StreamToZip.Length)
                {
                    int sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                    ZipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
            }
            catch (System.Exception ex)
            {
                err = ex.Message;
                return false;
            }
            finally
            {
                ZipStream.Finish();
                ZipStream.Close();
                StreamToZip.Close();
            }
            return true;
        }


        #endregion

        #region 解压缩
        /// <summary>
        /// 功能：解压zip格式的文件。
        /// </summary>
        /// <param name="zipFilePath">压缩文件路径</param>
        /// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>
        /// <param name="err">出错信息</param>
        /// <returns>解压是否成功</returns>
        public bool UnZipFile(string zipFilePath, string unZipDir, out string err)
        {
            err = "";
            if (zipFilePath == string.Empty)
            {
                err = "压缩文件不能为空！";
                return false;
            }
            if (!File.Exists(zipFilePath))
            {
                err = "压缩文件不存在！";
                return false;
            }
            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹
            if (unZipDir == string.Empty)
                unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
            if (!unZipDir.EndsWith("\\"))
                unZipDir += "\\";
            if (!Directory.Exists(unZipDir))
                Directory.CreateDirectory(unZipDir);

            try
            {
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
                {

                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name);
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(unZipDir + directoryName);
                        }
                        if (!directoryName.EndsWith("\\"))
                            directoryName += "\\";
                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                            {

                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }//while
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
            return true;
        }//解压结束
        #endregion
    }
}
