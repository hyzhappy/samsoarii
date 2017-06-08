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
        static private string[] CALLMTYPES = {"BIT*", "WORD*", "DWORD*", "FLOAT*"}; 

        #region Number
        /// <summary>
        /// 文本坐标
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// 函数名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标题注释
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// 返回类型
        /// </summary>
        public string ReturnType { get; set;}
        /// <summary>
        /// 所有参数的类型
        /// </summary>
        private string[] argtypes = new string[0];
        /// <summary>
        /// 所有参数的名称
        /// </summary>
        private string[] argnames = new string[0];
        /// <summary>
        /// 参数个数
        /// </summary>
        public int ArgCount
        {
            get { return argtypes.Length; }
            set
            {
                argtypes = new string[value];
                argnames = new string[value];
            }
        }
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

        public bool CanCALLM()
        {
            if (ArgCount > 4)
            {
                return false;
            }
            for (int i = 0; i < ArgCount; i++)
            {
                if (!CALLMTYPES.Contains(GetArgType(i)))
                    return false;
            }
            return true;
        }

        public string[] GetMessageList()
        {
            string[] ret = new string[ArgCount * 2 + 3];
            ret[0] = ReturnType;
            ret[1] = Name;
            ret[2] = Comment;
            for (int i = 0; i < ArgCount; i++)
            {
                ret[i * 2 + 3] = GetArgType(i);
                ret[i * 2 + 4] = GetArgName(i);
            }
            return ret;
        }

        #endregion
    }
}
