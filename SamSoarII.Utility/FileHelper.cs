using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace SamSoarII.Utility
{
    public static class FileHelper
    {
        /// <summary>
        /// Create a file in temporary path with specified postfix
        /// </summary>
        /// <param name="postfix"></param>
        /// <returns></returns>
        public static string GetTempFile(string postfix)
        {
            var newfile = Path.GetTempFileName();
            var result = Path.ChangeExtension(newfile, postfix);
            File.Move(newfile, result);
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
    }
}
