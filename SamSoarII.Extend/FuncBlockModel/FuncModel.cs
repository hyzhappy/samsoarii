using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Namespace : SamSoarII.Extend
/// ClassName : FuncModel
/// Version   : 1.0
/// Date      : 2017/4/17
/// Author    : Morenan
/// </summary>
/// <remarks>
/// 表示用户自定义的函数方法的模型类
/// </remarks>

namespace SamSoarII.Extend.FuncBlockModel
{
    public class FuncModel
    {
        #region Number

        /// <summary>
        /// 函数名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 所有参数的类型
        /// </summary>
        private string[] argtypes = new string[4] { String.Empty, String.Empty, String.Empty, String.Empty };
        /// <summary>
        /// 所有参数的名称
        /// </summary>
        private string[] argnames = new string[4] { String.Empty, String.Empty, String.Empty, String.Empty };
        /// <summary>
        /// 获取参数的类型
        /// </summary>
        /// <param name="id">参数id</param>
        /// <returns></returns>
        public string GetArgType(int id)
        {
            return argtypes[id];
        }
        /// <summary>
        /// 设置参数的类型
        /// </summary>
        /// <param name="id">参数id</param>
        /// <param name="_type">参数新类型</param>
        public void SetArgType(int id, string _type)
        {
            argtypes[id] = _type;
        }
        /// <summary>
        /// 获取参数的名称
        /// </summary>
        /// <param name="id">参数id</param>
        /// <returns></returns>
        public string GetArgName(int id)
        {
            return argnames[id];
        }
        /// <summary>
        /// 设置参数的名称
        /// </summary>
        /// <param name="id">参数id</param>
        /// <param name="_name">参数新名称</param>
        public void SetArgName(int id, string _name)
        {
            argnames[id] = _name; 
        }

        #endregion
    }
}
