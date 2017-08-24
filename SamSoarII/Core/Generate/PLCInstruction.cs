using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SamSoarII.Core.Generate
{
    public class PLCInstruction
    {
        /// <summary> 私有成员 </summary>
        protected LadderUnitModel prototype;
        protected int prototypeid;
        protected string[] args;
        protected string oldinstname;
        protected string enbit = String.Empty;
        protected string stackcalc = String.Empty;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="text">指令文本</param>
        public PLCInstruction(string _text)
        {
            ProtoType = null;
            args = _text.Split(' ');
            oldinstname = args[0];
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="instname">指令名称</param>
        /// <param name="prototype">指令原型</param>
        public PLCInstruction(string _instname, LadderUnitModel _prototype)
        {
            oldinstname = _instname;
            args = new string[] { _instname };
            ProtoType = _prototype;
        }
        /// <summary>
        /// 原型
        /// </summary>
        public LadderUnitModel ProtoType
        {
            get
            {
                return this.prototype;
            }
            set
            {
                if (prototype == value) return;
                LadderUnitModel _prototype = prototype;
                this.prototype = null;
                if (_prototype != null && _prototype.Inst != null)
                    _prototype.Inst = null;
                this.prototype = value;
                if (prototype != null)
                {
                    if (prototype.Inst != this)
                        prototype.Inst = this;
                    Analyze();
                }
                
            }
        }
        /// <summary>
        /// 原型的ID
        /// </summary>
        public int PrototypeID
        {
            get { return this.prototypeid; }
            set { this.prototypeid = value; }
        }

        /// <summary>
        /// 用于显示的指令
        /// </summary>
        private PLCOriginInst origin;
        public PLCOriginInst Origin
        {
            get
            {
                return this.origin;
            }
            set
            {
                if (origin == value) return;
                PLCOriginInst _origin = origin;
                this.origin = null;
                if (_origin != null && _origin.Inst != null)
                    _origin.Inst = null;
                this.origin = value;
                if (origin != null && origin.Inst != this)
                    origin.Inst = this;
            }
        }
        public string OldInstname { get { return this.oldinstname; } }

        /// <summary>
        /// 当这个指令把元素加入栈中时（LD类指令），需要知道这个元素是如何与上一个元素计算合并的
        /// 已知的合并方式有三种，分别为ANDB，ORB和POP
        /// 这里用于线路转换指令（INV, MEP, MEF）的当前信号的计算
        /// </summary>
        public string StackCalc
        {
            get { return this.stackcalc; }
            set { this.stackcalc = value; }
        }
        /// <summary>
        /// 根据已知条件，重新分析该指令
        /// </summary>
        public void Analyze()
        {
            enbit = "";
            // 根据指令类型的参数结构来分类，并转化参数的格式
            if (prototype != null)
            {
                args = new string[] { oldinstname };
                args = args.Concat(new string[prototype.Children.Count]).ToArray();
                switch (args[0])
                {
                    case "LDIM":
                    case "ANDIM":
                    case "ORIM":
                    case "LDIIM":
                    case "ANDIIM":
                    case "ORIIM":
                        args = new string[] { args[0], args[1], "" };
                        args[2] = ToCIndex(prototype.Children[0]);
                        break;
                    case "PLSNEXT":
                    case "PLSSTOP":
                        args[1] = ToCIndex(prototype.Children[0]);
                        break;
                    case "OUTIM":
                        args = new string[] { args[0], args[1], "" };
                        args[2] = ToCIndex(prototype.Children[0]);
                        break;
                    case "SETIM":
                    case "RSTIM":
                        args = new string[] { args[0], args[1], args[2], "" };
                        args[3] = ToCIndex(prototype.Children[0]);
                        break;
                    case "TON":
                    case "TONR":
                    case "TOF":
                    case "HCNT":
                        args[1] = ToCIndex(prototype.Children[0]);
                        break;
                    case "CTU":
                    case "CTD":
                    case "CTUD":
                        args = new string[] { args[0], args[1], args[2], "" };
                        // C + 地址
                        args[3] = String.Format("CBit[{0:s}]", ToCIndex(prototype.Children[0]));
                        break;
                    // (rW, wB)
                    case "PLSF":
                    case "DPLSF":
                        args[2] = ToCIndex(prototype.Children[1]);
                        break;
                    // (rW, rW, wB)
                    case "PWM":
                    case "PLSY":
                    case "PLSR":
                    case "ZRN":
                    case "DPWM":
                    case "DPLSY":
                    case "DPLSR":
                    case "DZRN":
                        args[3] = ToCIndex(prototype.Children[2]);
                        break;
                    case "DRVI":
                    case "DRVA":
                    case "DDRVI":
                    case "DDRVA":
                    case "PLSRD":
                    case "PLSA":
                    case "DPLSRD":
                    case "DPLSA":
                        args[3] = ToCIndex(prototype.Children[2]);
                        args[4] = ToCIndex(prototype.Children[3]);
                        break;
                    case "PTO":
                        args[2] = ToCIndex(prototype.Children[1]);
                        args[3] = ToCIndex(prototype.Children[2]);
                        break;
                }
                for (int i = 1; i <= prototype.Children.Count; i++)
                    if (args[i] == null) args[i] = ToCStyle(prototype.Children[i - 1]);
                /*
                 * 注意如果是复位(RST)了计数器的位(C)的话
                 * 会影响对应标号的计数器值
                 * 所以标号需要记录到另外的参数
                 */
                if (args[0].Length > 2 && args[0].Substring(0, 3).Equals("RST") && (args[1][0] == 'C' || args[1][0] == 'T'))
                    args = args.Concat(new string[] { prototype.Children[0].Text.Substring(1), prototype.Children[1].Store.Value.ToString() }).ToArray();
                if (args[0].Length > 2 && args[0].Substring(0, 2).Equals("CT"))
                    args = args.Concat(new string[] { prototype.Children[0].Text.Substring(2) }).ToArray();
            }
        }
        
        /// <summary>
        /// 指令参数数量
        /// </summary>
        public int Count
        {
            get { return args.Count(); }
        }
        /// <summary>
        /// 指令参数（0为类型，1-5分别对应五个参数）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string this[int id]
        {
            get { return args[id]; }
            set { args[id] = value; }
        }
        /// <summary>
        /// 输出写入使能（仿真模式下使用）
        /// </summary>
        public string EnBit
        {
            get { return this.enbit; }
        }
        /// <summary>
        /// 将常量/变量名称转变为c语言可识别的形式
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="mode">访问变量的权限（r：读，w：写，rw：读写）</param>
        /// <param name="ctype">要转换成的c语言类型</param>
        /// <returns>c语言格式</returns>
        private string ToCStyle(ValueModel vmodel)
        {
            ValueFormat vformat = vmodel.Format;
            // 如果该参数可写，需要附带写入使能
            if (vformat.CanWrite)
            {
                string _enbit = ToCEnable(vmodel);
                enbit = (enbit.Length == 0) ? _enbit : enbit + "||" + _enbit;
            }
            switch (vmodel.Type)
            {
                case ValueModel.Types.BOOL:
                    switch (vmodel.Base)
                    {
                        case ValueModel.Bases.X:
                        case ValueModel.Bases.Y:
                            if (vmodel.IsExtend)
                                return String.Format("E{0:s}Bit[{1:s}-512]",
                                    ValueModel.NameOfBases[(int)(vmodel.Base)],
                                    ToCIndex(vmodel));
                            else
                                return String.Format("{0:s}Bit[{1:s}]",
                                    ValueModel.NameOfBases[(int)(vmodel.Base)],
                                    ToCIndex(vmodel));
                        case ValueModel.Bases.S:
                        case ValueModel.Bases.M:
                        case ValueModel.Bases.C:
                        case ValueModel.Bases.T:
                            return String.Format("{0:s}Bit[{1:s}]", 
                                ValueModel.NameOfBases[(int)(vmodel.Base)], 
                                ToCIndex(vmodel));
                        case ValueModel.Bases.D:
                        case ValueModel.Bases.V:
                        case ValueModel.Bases.Z:
                            if (vformat.CanWrite) args[0] = "WB" + args[0];
                            return String.Format(
                                vformat.CanWrite 
                                    ? "_set_wbit({0:s}Word+({1:s}>>4), {1:s}&15,"
                                    : "_get_wbit({0:s}Word+({1:s}>>4), {1:s}&15)",
                                ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel));
                    }
                    break;
                case ValueModel.Types.WORD:
                    switch (vmodel.Base)
                    {
                        case ValueModel.Bases.D:
                        case ValueModel.Bases.AI:
                        case ValueModel.Bases.AO:
                        case ValueModel.Bases.V:
                        case ValueModel.Bases.Z:
                        case ValueModel.Bases.TV:
                            return String.Format("{0:s}Word[{1:s}]",
                                ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel));
                        case ValueModel.Bases.CV:
                            return vmodel.Offset < 200
                                ? String.Format("CVWord[{0:s}]", ToCIndex(vmodel))
                                : String.Format("*((_WORD)(&CV32DoubleWord[{0:s}-200]))", ToCIndex(vmodel));
                        case ValueModel.Bases.X:
                        case ValueModel.Bases.Y:
                        case ValueModel.Bases.M:
                        case ValueModel.Bases.S:
                            if (vformat.CanWrite) args[0] = "BW" + args[0];
                            return String.Format(
                                vformat.CanWrite
                                    ? "_set_bword({0:s}Bit+{1:s}, {2:d},"
                                    : "_get_bword({0:s}Bit+{1:s}, {2:d})",
                                ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel), vmodel.Size);
                        case ValueModel.Bases.K:
                        case ValueModel.Bases.H:
                            return vmodel.Store.Value.ToString();
                    }
                    break;
                case ValueModel.Types.DWORD:
                    switch (vmodel.Base)
                    {
                        case ValueModel.Bases.D:
                            if (vmodel.IsPulseCount)
                            {
                                if (vformat.CanWrite) args[0] = "PC" + args[0];
                                return String.Format(
                                    !vformat.CanWrite
                                        ? "(UpdatePulseCount(({0:s}-8140)>>1),*((D_WORD)(&DWord[{0:s}])))"
                                        : "WritePulseCount(({0:s}-8140)>>1,",
                                    ToCIndex(vmodel));
                            }
                            return String.Format("*((D_WORD)(&DWord[{0:s}]))", ToCIndex(vmodel));
                        case ValueModel.Bases.AI:
                        case ValueModel.Bases.AO:
                        case ValueModel.Bases.V:
                        case ValueModel.Bases.Z:
                        case ValueModel.Bases.TV:
                            return String.Format("*((D_WORD)(&{0:s}Word[{1:s}]))",
                                ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel));
                        case ValueModel.Bases.CV:
                            return vmodel.Offset < 200
                                ? String.Format("*((D_WORD)(&CVWord[{0:s}]))", ToCIndex(vmodel))
                                : String.Format("CV32DoubleWord[{0:s}-200]", ToCIndex(vmodel));
                        case ValueModel.Bases.X:
                        case ValueModel.Bases.Y:
                        case ValueModel.Bases.M:
                        case ValueModel.Bases.S:
                            if (vformat.CanWrite) args[0] = "BD" + args[0];
                            return String.Format(
                                vformat.CanWrite
                                    ? "_set_bdword({0:s}Bit+{1:s}, {2:d},"
                                    : "_get_bdword({0:s}Bit+{1:s}, {2:d})",
                                ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel), vmodel.Size);
                        case ValueModel.Bases.K:
                        case ValueModel.Bases.H:
                            return vmodel.Store.Value.ToString();
                    }
                    break;
                case ValueModel.Types.FLOAT:
                    switch (vmodel.Base)
                    {
                        case ValueModel.Bases.D:
                        case ValueModel.Bases.AI:
                        case ValueModel.Bases.AO:
                        case ValueModel.Bases.V:
                        case ValueModel.Bases.Z:
                        case ValueModel.Bases.TV:
                            return String.Format("*((_FLOAT)(&{0:s}Word[{1:s}]))",
                                ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel));
                        case ValueModel.Bases.CV:
                            return vmodel.Offset < 200
                                ? String.Format("*((_FLOAT)(&CVWord[{0:s}]))", ToCIndex(vmodel))
                                : String.Format("*((_FLOAT)(&CV32DoubleWord[{0:s}-200]))", ToCIndex(vmodel));
                        case ValueModel.Bases.K:
                        case ValueModel.Bases.H:
                            return vmodel.Store.Value.ToString();
                    }
                    break;
            }
            return vmodel.Text;
        }

        private string ToCIndex(ValueModel vmodel)
        {
            int intratime = vmodel.IsWordBit ? 16 : 1;
            switch (vmodel.Intra)
            {
                case ValueModel.Bases.V: return String.Format("({0:d}+VWord[{1:d}]*{2:d})", vmodel.Offset, vmodel.IntraOffset, intratime);
                case ValueModel.Bases.Z: return String.Format("({0:d}+ZWord[{1:d}]*{2:d})", vmodel.Offset, vmodel.IntraOffset, intratime);
                default: return String.Format("({0:d})", vmodel.Offset);
            }
        }
        
        private string ToCAddr(ValueModel vmodel)
        {
            return vmodel.IsWordBit
                ? String.Format("({0:s}>>4)", ToCIndex(vmodel))
                : ToCIndex(vmodel);
        }

        private string ToCEnable(ValueModel vmodel)
        {
            return String.Format("{0:s}Enable[{1:s}]", ValueModel.NameOfBases[(int)(vmodel.Base)], ToCAddr(vmodel));
        }
        
        public string ToCEnable(int id)
        {
            ValueModel vmodel = prototype.Children[id - 1];
            return ToCEnable(vmodel);
        }

        public string ToCParas(int id)
        {
            ValueModel vmodel = prototype.Children[id-1];
            ValueFormat vformat = vmodel.Format;
            if (vmodel.IsWordBit)
                return String.Format(
                    "{0:s}Word+({1:d}>>4), {1:d}&15",
                    ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel));
            if (vmodel.IsBitWord || vmodel.IsBitDoubleWord)
                return String.Format(
                    "{0:s}Bit+{1:d}, {2:d}",
                    ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel), vmodel.Size);
            return ToCStyle(vmodel);
        }
        
        public PLCOriginInst ToOrigin(InstructionNetworkModel _parent)
        {
           return new PLCOriginInst(_parent, this);
        }
    }
}
