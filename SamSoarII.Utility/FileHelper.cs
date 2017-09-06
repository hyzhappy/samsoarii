using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace SamSoarII.Utility
{
    public static class FileHelper
    {
        /// <summary>
        /// SamSoarII的文件类型
        /// </summary>
        public static string NewFileExtension
        {
            get
            {
                return "ssr";
            }
        }
        /// <summary>
        /// 第一版文件类型
        /// </summary>
        public static string OldFileExtension
        {
            get
            {
                return "ssp";
            }
        }
        /// <summary>
        /// 应用程序运行的根目录
        /// </summary>
        public static string AppRootPath
        {
            get
            {
                return Directory.GetParent(System.Windows.Forms.Application.ExecutablePath).FullName;
            }
        }
        /// <summary>
        /// Create a file in temporary path with specified postfix
        /// </summary>
        /// <param name="postfix"></param>
        /// <returns></returns>
        public static string GetTempFile(string postfix)
        {
            string newfile, result;
            while (true)
            {
                newfile = Path.GetTempFileName();
                result = Path.ChangeExtension(newfile, postfix);
                if (!File.Exists(result))
                {
                    File.Move(newfile, result);
                    break;
                }
                else File.Delete(newfile);
            }
            return result;
        }
        /// <summary>
        /// Change file extension
        /// </summary>
        /// <param name="file"></param>
        /// <param name="postfix"></param>
        /// <returns></returns>
        public static string ChangeExtension(string file, string extension)
        {
            return Path.ChangeExtension(file, extension);
        }
        public static string GetMD5(string filename)
        {
            try
            {
                FileStream file = new FileStream(filename, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
        /// <summary>
        /// 得到相应的文件大小（以字节为单位）
        /// </summary>
        /// <param name="fullFileName">文件的完整路径(包括文件名与后缀)</param>
        /// <returns></returns>
        public static long GetFileLength(string fullFileName)
        {
            if (File.Exists(fullFileName))
                return new FileInfo(fullFileName).Length;
            else return 0L;
        }

        public static string GetFullFileName(string filename,string extension)
        {
            return string.Format(@"{0}\rar\temp\{1}.{2}", StringHelper.RemoveSystemSeparator(AppRootPath), filename, extension);
        }

        public static string CompressFile(string fullFileName)
        {
            Process cmd = new Process();
            string exepath = string.Format(@"{0}\rar", StringHelper.RemoveSystemSeparator(AppRootPath));
            string CFName = string.Format(@"{0}\rar\temp\{1}.7z", StringHelper.RemoveSystemSeparator(AppRootPath), Path.GetFileNameWithoutExtension(fullFileName));
            cmd.StartInfo.FileName = string.Format(@"{0}\{1}",exepath,"HaoZipC.exe");
            cmd.StartInfo.Arguments = string.Format("a -t7z -pSamSoarII \"{0}\" \"{1}\"", CFName, fullFileName);
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();
            cmd.WaitForExit();
            cmd.Close();
            try
            {
                //由于某种原因，进程HaoZipC.exe可能未正常关闭，这里强制关闭进程
                foreach (var process in Process.GetProcessesByName("HaoZipC.exe"))
                    process.Kill();
            }
            catch (Exception)
            {

            }
            return CFName;
        }

        public static void DecompressFile(string fullFileName,string outPath)
        {
            Process cmd = new Process();
            string filepath = string.Format(@"{0}\{1}", StringHelper.RemoveSystemSeparator(AppRootPath), "rar");
            cmd.StartInfo.FileName = string.Format(@"{0}\{1}", filepath, "HaoZipC.exe");
            cmd.StartInfo.Arguments = string.Format("e -pSamSoarII \"{0}\" -o\"{1}\"", fullFileName, outPath);
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();
            cmd.WaitForExit();
            cmd.Close();
            try
            {
                //由于某种原因，进程HaoZipC.exe可能未正常关闭，这里强制关闭进程
                foreach (var process in Process.GetProcessesByName("HaoZipC.exe"))
                    process.Kill();
            }
            catch (Exception)
            {
                
            }
        }

        public static bool InvalidFileName(string fullFileName)
        {
            return fullFileName == null || fullFileName == string.Empty;
        }

        public static byte[] GetBytesByBinaryFile(string fullFileName)
        {
            List<byte> data = new List<byte>();
            BinaryReader br = new BinaryReader(
                new FileStream(fullFileName, FileMode.Open));
            while (br.BaseStream.CanRead)
            {
                try
                {
                    data.Add(br.ReadByte());
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }
            br.Close();
            return data.ToArray();
        }

        public static void GenerateBinaryFile(string fullFileName,byte[] data)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream(fullFileName, FileMode.Create,FileAccess.Write));
            for (int i = 0; i < data.Length; i++)
            {
                try
                {
                    writer.Write(data[i]);
                }
                catch (Exception)
                {
                    break;
                }
            }
            writer.Flush();
            writer.Close();
        }
    }
}
