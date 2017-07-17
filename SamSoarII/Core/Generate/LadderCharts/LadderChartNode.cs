using SamSoarII.Core.Models;
using SamSoarII.Global;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Generate
{
    public class LadderChartNode : IDisposable
    {
        public LadderChartNode(LadderUnitModel _prototype)
        {
            prototype = _prototype;
            if (prototype.Type == LadderUnitModel.Types.VLINE)
            {
                VAccess = true;
                HAccess = false;
            }
            else
            {
                HAccess = true;
                VAccess = false;
            }
        }

        public void Dispose()
        {
            prototype = null;
            Left = Right = Up = Down = LeDo = LeUp = RiDo = RiUp = null;
        }

        #region Numbers

        /// <summary> 原型 </summary>
        private LadderUnitModel prototype;
        /// <summary> 原型 </summary>
        public LadderUnitModel Prototype { get { return this.prototype; } }
        
        /// <summary> 类型 </summary>
        public string Type { get { return prototype.InstName; } }
        /// <summary> X坐标 </summary>
        public int X { get { return prototype.X; } }
        /// <summary> Y坐标 </summary>
        public int Y { get { return prototype.Y; } }
        /// <summary> 是否为起点 </summary>
        public bool IsStart { get { return X == 0; } }
        /// <summary> 是否为终点 </summary>
        public bool IsTerminate { get { return X == GlobalSetting.LadderXCapacity - 1; } }
        /// <summary> 根据下标返回或设置相应的值（0：指令名称，1~5：指令参数）</summary>
        public string this[int id] { get { return id == 0 ? Type : prototype.Children[id - 1].Text; } }
        
        /// <summary> 编号 </summary>
        private int id;
        /// <summary> 编号 </summary>
        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary> 左节点 </summary>
        public LadderChartNode Left { get; set; }
        /// <summary> 右节点 </summary>
        public LadderChartNode Right { get; set; }
        /// <summary> 上节点 </summary>
        public LadderChartNode Up { get; set; }
        /// <summary> 下节点 </summary>
        public LadderChartNode Down { get; set; }
        /// <summary> 左上节点 </summary>
        public LadderChartNode LeUp { get; set; }
        /// <summary> 左下节点 </summary>
        public LadderChartNode LeDo { get; set; }
        /// <summary> 右上节点 </summary>
        public LadderChartNode RiUp { get; set; }
        /// <summary> 右下节点 </summary> 
        public LadderChartNode RiDo { get; set; }
        /// <summary> 横向联通性 </summary>
        public bool HAccess { get; set; }
        /// <summary> 纵向联通性 </summary>
        public bool VAccess { get; set; }

        /// <summary> 当梯形图转换为逻辑图后，这个元件左边相邻的线路所对应的图中的节点 </summary> 
        public int LNodeID { get; set; }
        /// <summary> 当梯形图转换为逻辑图后，这个元件右边相邻的线路所对应的图中的节点 </summary> 
        public int RNodeID { get; set; }

        /// <summary>
        /// 该元件的表达式模式（限逻辑元件）
        /// </summary>        
        public string Expr
        {
            get
            {
                switch (Type)
                {
                    case "LD": return String.Format("[{0:d}]{1:s}", id, this[1]);
                    case "LDI": return String.Format("[{0:d}]!{1:s}", id, this[1]);
                    case "LDIM": return String.Format("[{0:d}]im{1:s}", id, this[1]);
                    case "LDIIM": return String.Format("[{0:d}]!im{1:s}", id, this[1]);
                    case "LDP": return String.Format("[{0:d}]ue{1:s}", id, this[1]);
                    case "LDF": return String.Format("[{0:d}]de{1:s}", id, this[1]);
                    case "MEP": return String.Format("[{0:d}]ue", id);
                    case "MEF": return String.Format("[{0:d}]de", id);
                    case "INV": return String.Format("[{0:d}]!", id);
                    case "LDWEQ": return String.Format("[{0:d}]{1:s}w={2:s}", id, this[1], this[2]);
                    case "LDDEQ": return String.Format("[{0:d}]{1:s}d={2:s}", id, this[1], this[2]);
                    case "LDFEQ": return String.Format("[{0:d}]{1:s}f={2:s}", id, this[1], this[2]);
                    case "LDWNE": return String.Format("[{0:d}]{1:s}w<>{2:s}", id, this[1], this[2]);
                    case "LDDNE": return String.Format("[{0:d}]{1:s}d<>{2:s}", id, this[1], this[2]);
                    case "LDFNE": return String.Format("[{0:d}]{1:s}f<>{2:s}", id, this[1], this[2]);
                    case "LDWGE": return String.Format("[{0:d}]{1:s}w>={2:s}", id, this[1], this[2]);
                    case "LDDGE": return String.Format("[{0:d}]{1:s}d>={2:s}", id, this[1], this[2]);
                    case "LDFGE": return String.Format("[{0:d}]{1:s}f>={2:s}", id, this[1], this[2]);
                    case "LDWLE": return String.Format("[{0:d}]{1:s}w<={2:s}", id, this[1], this[2]);
                    case "LDDLE": return String.Format("[{0:d}]{1:s}d<={2:s}", id, this[1], this[2]);
                    case "LDFLE": return String.Format("[{0:d}]{1:s}f<={2:s}", id, this[1], this[2]);
                    case "LDWG": return String.Format("[{0:d}]{1:s}w>{2:s}", id, this[1], this[2]);
                    case "LDDG": return String.Format("[{0:d}]{1:s}d>{2:s}", id, this[1], this[2]);
                    case "LDFG": return String.Format("[{0:d}]{1:s}f>{2:s}", id, this[1], this[2]);
                    case "LDWL": return String.Format("[{0:d}]{1:s}w<{2:s}", id, this[1], this[2]);
                    case "LDDL": return String.Format("[{0:d}]{1:s}d<{2:s}", id, this[1], this[2]);
                    case "LDFL": return String.Format("[{0:d}]{1:s}f<{2:s}", id, this[1], this[2]);
                    default: return String.Format("[{0:d}]1", id);
                }
            }
        }

        public string ToShowString(string profix = "")
        {
            switch (Prototype.Shape)
            {
                case LadderUnitModel.Shapes.Input:
                    return String.Format("{0:s}{1:s}", profix, Prototype.ToInstString());
                default:
                    return Prototype.ToInstString();
            }
        }
        
        /// <summary>
        /// 生成该元件对应的指令
        /// </summary> 
        public void GenInst(List<PLCInstruction> insts, int flag = 0)
        {
            string profix = "LD";
            if ((flag & 0x04) != 0)
                profix = "A";
            if ((flag & 0x08) != 0)
                profix = "OR";
            PLCInstruction inst = new PLCInstruction(ToShowString(profix));
            inst.PrototypeID = id;
            inst.ProtoType = Prototype;
            insts.Add(inst);
        }
        
        #endregion

    }
}
