using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.Extend.LadderChartModel;
/// <summary>
/// ClassName : LCNode
/// Version : 1.0
/// Date : 2017/2/23
/// Author : morenan
/// </summary>
/// <remarks>
/// 逻辑图中的边
/// </remarks>

namespace SamSoarII.Extend.LogicGraph
{
    public class LGEdge
    {
        private bool enable;

        private int id;
        private int flag1, flag2, flag3, flag4;

        private LCNode plcinfo;
        private LGVertex source;
        private LGVertex dest;

        private string expr;

        public LGEdge(LCNode _info, LGVertex _s, LGVertex _d)
        {
            this.id = 0;
            this.PLCInfo = _info;
            this.Source = _s;
            this.Destination = _d;
        }

        public bool Enable
        {
            get { return this.enable; }
            set { this.enable = value; }
        }

        public LCNode PLCInfo
        {
            get { return this.plcinfo; }
            set { this.plcinfo = value; }
        }
        
        public LGVertex Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        public LGVertex Destination
        {
            get { return this.dest; }
            set { this.dest = value; }
        }

        /// <summary>
        /// 根据下标返回或设置相应的值
        /// 0：指令类型
        /// 1,2,3,4：指令的操作数和信息
        /// </summary>
        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return this.id;
                    case 1: return this.flag1;
                    case 2: return this.flag2;
                    case 3: return this.flag3;
                    case 4: return this.flag4;
                    default: return 0;
                }
            }
            set
            {
                switch (index)
                {
                    case 0: this.id = value; break;
                    case 1: this.flag1 = value; break;
                    case 2: this.flag2 = value; break;
                    case 3: this.flag3 = value; break;
                    case 4: this.flag4 = value; break;
                }
            }
        }

        public string Expr
        {
            get { return this.expr; }
            set { this.expr = value; }
        }

    }
}
