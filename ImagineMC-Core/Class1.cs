using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksum;


namespace ImagineMC_Core
{
    public class ImagineMain
    {
        public void DownloadFile(string URL, string filename) //DownloadFile Part,From Internet
        {
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    so.Write(by, 0, osize);
                    osize = st.Read(by, 0, (int)by.Length);
                }
                so.Close();
                st.Close();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public string CheckGame(string PackageFile)
        /*
         * PackageFile:the name of game resource file,such as “rapo.zip”.
         * The Package should be a zip file.
         */
        {
            if (System.IO.File.Exists(PackageFile))
            {
                return "Success";
            }
            else
            {
                return "Failed";
            }
        }
        public string CheckGame(string PackageFile, string URL, string FileName)
        /*
         * PackageFile:the name of game resource file,such as “rapo.zip”.
         * The Package should be a zip file.
         * URL:The URL of package,if you input URL here, the program will install the game from the Internet.
         */
        {
            if (System.IO.File.Exists(PackageFile))
            {
                return "Success";
            }
            else
            {
                string installResult = InstallGame(URL, PackageFile);
                if (installResult == "Installed")
                {
                    return "Success";
                    //Installed game package.
                }
                else
                {
                    return "Failed";
                    //Check the packet or Internet connection , and try again.
                }
            }
        }
        public string InstallGame(string URL, string FileName)
        {
            DownloadFile(URL, FileName);
            //Download the package from given URL.
            UnZip(FileName, @".\");
            //Unzip the package to lanuncher's path.
            string result = CheckGame(FileName);
            //Check again,is it installed?
            if (result == "Success")
            {
                return "Success";
            }
            else
            {
                return "Unreachable FileName";
                //Check your URL,is it point to the package?
                //Check your package,is it a zipped file?
                //Check your Internet connection?
            }
        }

        public string GetServerList(string URL)
        {
            if (Directory.Exists(@".\.minecraft"))
            {
                DownloadFile(URL, @".\.minecraft\servers.dat");
                //This file should be a json file.

                if (File.Exists(@".\.minecraft\servers.dat"))
                {
                    return "Success";
                    //Server list has already got now.
                }
                else
                {
                    return @"URL unreachable";
                    //Is the game installed?
                    //Check folder ".minecraft" exists or not.
                    //Is the URL point to a wrong file?
                    //Check your Internet connection.
                }
            }
            else
            {
                return ("Path \".\\.minecraft\" unreachable.");
                //Check the game package , is it installed?
                //Check the zipped file , is it include ".minecraft" folder?
                //Warning , zhe zipped file should be include ".minecraft" in the first dir.
            }
        }
        public string GetWindowsJava(string URL)
        {
            //This part only work in Windows Environment.
            //If you are running Linux,use GetLinuxJava().
            //If you are in other systems,you should download Java by yourself.
            if (File.Exists(".\\java\\bin\\java.exe"))
            {
                return ("Success");
            }
            else
            {
                DownloadFile(URL, @".\java.zip");
                //Java package should be a zipped file.And ".\bin\java.exe" must be exists.
                UnZip(@".\java.zip", @".\java");
                //Unzip Java package into folder ".\java".
                //File "java.exe" should be in ".\java\bin\".
                if (File.Exists(".\\java\\bin\\java.exe"))
                {
                    return "Success";
                    //Now you should run Minecraft as ".\java\bin\java.exe"
                }
                else
                {
                    return "Java : Env Unreachable";
                    //Check your package , does ".\bin\java.exe" exists?
                    //Check your given URL , does it point to a wrong file?
                    //Check your package , is it a zipped file?(not rar 7z or others)
                    //Check your Internet Connection .
                }
            }

        }
        public string GetLinuxJava(string URL)
        {
            //This part only work in Linux Environment.
            //If you are running Windows,use GetWindowsJava().
            //If you are in other systems,you should download Java by yourself.
            if (File.Exists(".\\java\\bin\\java"))
            {
                return ("Success");
            }
            else
            {
                DownloadFile(URL, @".\java.zip");
                //Java package should be a zipped file.And ".\bin\java" must be exists.
                UnZip(@".\java.zip", @".\java");
                //Unzip Java package into folder ".\java".
                //File "java.exe" should be in ".\java\bin\".
                if (File.Exists(".\\java\\bin\\java"))
                {
                    return "Success";
                    //Now you should run Minecraft as ".\java\bin\java"
                }
                else
                {
                    return "Java : Env Unreachable";
                    //Check your package , does ".\bin\java" exists?
                    //Check your given URL , does it point to a wrong file?
                    //Check your package , is it a zipped file?(not rar 7z or others)
                    //Check your Internet Connection .
                }
            }

        }
        private static bool ZipDirectory(string folderToZip, ZipOutputStream zipStream, string parentFolderName)
        {
            bool result = true;
            string[] folders, files;
            ZipEntry ent = null;
            FileStream fs = null;
            Crc32 crc = new Crc32();

            try
            {
                ent = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/"));
                zipStream.PutNextEntry(ent);
                zipStream.Flush();

                files = Directory.GetFiles(folderToZip);
                foreach (string file in files)
                {
                    fs = File.OpenRead(file);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    ent = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/" + Path.GetFileName(file)));
                    ent.DateTime = DateTime.Now;
                    ent.Size = fs.Length;

                    fs.Close();

                    crc.Reset();
                    crc.Update(buffer);

                    ent.Crc = crc.Value;
                    zipStream.PutNextEntry(ent);
                    zipStream.Write(buffer, 0, buffer.Length);
                }

            }
            catch
            {
                result = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }

            folders = Directory.GetDirectories(folderToZip);
            foreach (string folder in folders)
                if (!ZipDirectory(folder, zipStream, folderToZip))
                    return false;

            return result;
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="folderToZip">要压缩的文件夹路径</param>
        /// <param name="zipedFile">压缩文件完整路径</param>
        /// <param name="password">密码</param>
        /// <returns>是否压缩成功</returns>
        public static bool ZipDirectory(string folderToZip, string zipedFile, string password)
        {
            bool result = false;
            if (!Directory.Exists(folderToZip))
                return result;

            ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipedFile));
            zipStream.SetLevel(6);
            if (!string.IsNullOrEmpty(password)) zipStream.Password = password;

            result = ZipDirectory(folderToZip, zipStream, "");

            zipStream.Finish();
            zipStream.Close();

            return result;
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="folderToZip">要压缩的文件夹路径</param>
        /// <param name="zipedFile">压缩文件完整路径</param>
        /// <returns>是否压缩成功</returns>
        public static bool ZipDirectory(string folderToZip, string zipedFile)
        {
            bool result = ZipDirectory(folderToZip, zipedFile, null);
            return result;
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileToZip">要压缩的文件全名</param>
        /// <param name="zipedFile">压缩后的文件名</param>
        /// <param name="password">密码</param>
        /// <returns>压缩结果</returns>
        public static bool ZipFile(string fileToZip, string zipedFile, string password)
        {
            bool result = true;
            ZipOutputStream zipStream = null;
            FileStream fs = null;
            ZipEntry ent = null;

            if (!File.Exists(fileToZip))
                return false;

            try
            {
                fs = File.OpenRead(fileToZip);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                fs = File.Create(zipedFile);
                zipStream = new ZipOutputStream(fs);
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                ent = new ZipEntry(Path.GetFileName(fileToZip));
                zipStream.PutNextEntry(ent);
                zipStream.SetLevel(6);

                zipStream.Write(buffer, 0, buffer.Length);

            }
            catch
            {
                result = false;
            }
            finally
            {
                if (zipStream != null)
                {
                    zipStream.Finish();
                    zipStream.Close();
                }
                if (ent != null)
                {
                    ent = null;
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
            GC.Collect();
            GC.Collect(1);

            return result;
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileToZip">要压缩的文件全名</param>
        /// <param name="zipedFile">压缩后的文件名</param>
        /// <returns>压缩结果</returns>
        public static bool ZipFile(string fileToZip, string zipedFile)
        {
            bool result = ZipFile(fileToZip, zipedFile, null);
            return result;
        }

        /// <summary>
        /// 压缩文件或文件夹
        /// </summary>
        /// <param name="fileToZip">要压缩的路径</param>
        /// <param name="zipedFile">压缩后的文件名</param>
        /// <param name="password">密码</param>
        /// <returns>压缩结果</returns>
        public static bool Zip(string fileToZip, string zipedFile, string password)
        {
            bool result = false;
            if (Directory.Exists(fileToZip))
                result = ZipDirectory(fileToZip, zipedFile, password);
            else if (File.Exists(fileToZip))
                result = ZipFile(fileToZip, zipedFile, password);

            return result;
        }

        /// <summary>
        /// 压缩文件或文件夹
        /// </summary>
        /// <param name="fileToZip">要压缩的路径</param>
        /// <param name="zipedFile">压缩后的文件名</param>
        /// <returns>压缩结果</returns>
        public static bool Zip(string fileToZip, string zipedFile)
        {
            bool result = Zip(fileToZip, zipedFile, null);
            return result;

        }


        #region 解压

        /// <summary>
        /// 解压功能(解压压缩文件到指定目录)
        /// </summary>
        /// <param name="fileToUnZip">待解压的文件</param>
        /// <param name="zipedFolder">指定解压目标目录</param>
        /// <param name="password">密码</param>
        /// <returns>解压结果</returns>
        public static bool UnZip(string fileToUnZip, string zipedFolder, string password)
        {
            bool result = true;
            FileStream fs = null;
            ZipInputStream zipStream = null;
            ZipEntry ent = null;
            string fileName;

            if (!File.Exists(fileToUnZip))
                return false;

            if (!Directory.Exists(zipedFolder))
                Directory.CreateDirectory(zipedFolder);

            try
            {
                zipStream = new ZipInputStream(File.OpenRead(fileToUnZip));
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(ent.Name))
                    {
                        fileName = Path.Combine(zipedFolder, ent.Name);
                        fileName = fileName.Replace('/', '\\');//change by Mr.HopeGi

                        int index = ent.Name.LastIndexOf('/');
                        if (index != -1 || fileName.EndsWith("\\"))
                        {
                            string tmpDir = (index != -1 ? fileName.Substring(0, fileName.LastIndexOf('\\')) : fileName) + "\\";
                            if (!Directory.Exists(tmpDir))
                            {
                                Directory.CreateDirectory(tmpDir);
                            }
                            if (tmpDir == fileName)
                            {
                                continue;
                            }
                        }

                        fs = File.Create(fileName);
                        int size = 2048;
                        byte[] data = new byte[size];
                        while (true)
                        {
                            size = zipStream.Read(data, 0, data.Length);
                            if (size > 0)
                                fs.Write(data, 0, data.Length);
                            else
                                break;
                        }
                    }
                }
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return result;
        }

        /// <summary>
        /// 解压功能(解压压缩文件到指定目录)
        /// </summary>
        /// <param name="fileToUnZip">待解压的文件</param>
        /// <param name="zipedFolder">指定解压目标目录</param>
        /// <returns>解压结果</returns>
        public static bool UnZip(string fileToUnZip, string zipedFolder)
        {
            bool result = UnZip(fileToUnZip, zipedFolder, null);
            return result;
        }
        #endregion
    }

}
