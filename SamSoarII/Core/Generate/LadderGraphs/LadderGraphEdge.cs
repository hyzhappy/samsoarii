using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Generate
{
    public class LadderGraphEdge : IDisposable
    {
        public LadderGraphEdge(LadderChartNode _prototype, LadderGraphVertex _source, LadderGraphVertex _dest)
        {
            prototype = _prototype;
            source = _source;
            dest = _dest;
            expr = null;
        }

        public void Dispose()
        {
            prototype = null;
            source = null;
            dest = null;
            expr = null;
        }

        #region Numbers

        private LadderChartNode prototype;
        public LadderChartNode Prototype { get { return this.prototype; } }

        private LadderGraphVertex source;
        public LadderGraphVertex Source { get { return this.source; } }

        private LadderGraphVertex dest;
        public LadderGraphVertex Destination { get { return this.dest; } }

        private string expr;
        public string Expr
        {
            get { return this.expr; }
            set { this.expr = value; }
        }

        private int[] flags;
        public int this[int index]
        {
            get
            {
                return this.flags[index];
            }
            set
            {
                this.flags[index] = value; 
            }
        }
        public int FlagCount
        {
            get { return this.flags.Length; }
            set { this.flags = new int[value]; }
        }

        #endregion
    }
}
