using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SamSoarII.Core.Models
{
    public class FuncModel
    {
        #region Resources

        static readonly private string[] CALLMTYPES = { "BIT*", "WORD*", "DWORD*", "FLOAT*" };

        static readonly private Regex[] BitRegexs = { ValueModel.VerifyBitRegex3 };
        static readonly private Regex[] WordRegexs = { ValueModel.VerifyWordRegex2 };
        static readonly private Regex[] DoubleWordRegexs = { ValueModel.VerifyDoubleWordRegex1 };
        static readonly private Regex[] FloatRegexs = { ValueModel.VerifyFloatRegex };

        #endregion

        #region Number
        /// <summary> 文本坐标 </summary>
        public int Offset { get; set; }
        /// <summary> 函数名称 </summary>
        public string Name { get; set; }
        /// <summary> 标题注释 </summary>
        public string Comment { get; set; }
        /// <summary> 返回类型 </summary>
        public string ReturnType { get; set; }
        /// <summary> 所有参数的类型 </summary>
        private string[] argtypes = new string[0];
        /// <summary> 所有参数的名称 </summary>
        private string[] argnames = new string[0];
        /// <summary> 参数个数 </summary>
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
        /// <summary>
        /// 是否能被CALLM调用
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 获取对应的值模型的表
        /// </summary>
        /// <param name="_parent">梯形图元件</param>
        /// <returns></returns>
        public IList<ValueModel> GetValueModels(LadderUnitModel _parent)
        {
            ValueModel[] vmodels = new ValueModel[ArgCount];
            for (int i = 0; i < ArgCount; i++)
            {
                switch (GetArgType(i))
                {
                    case "BIT*":
                        vmodels[i] = new ValueModel(_parent, new ValueFormat(
                            GetArgName(i), ValueModel.Types.BOOL, true, true, i + 1, BitRegexs));
                        break;
                    case "WORD*":
                        vmodels[i] = new ValueModel(_parent, new ValueFormat(
                            GetArgName(i), ValueModel.Types.WORD, true, true, i + 1, WordRegexs));
                        break;
                    case "DWORD*":
                        vmodels[i] = new ValueModel(_parent, new ValueFormat(
                            GetArgName(i), ValueModel.Types.DWORD, true, true, i + 1, DoubleWordRegexs));
                        break;
                    case "FLOAT*":
                        vmodels[i] = new ValueModel(_parent, new ValueFormat(
                            GetArgName(i), ValueModel.Types.FLOAT, true, true, i + 1, FloatRegexs));
                        break;
                    default:
                        return null;
                }
            }
            return vmodels;
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

