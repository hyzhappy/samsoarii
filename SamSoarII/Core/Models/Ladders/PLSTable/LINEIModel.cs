using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class LINEIModel : POLYLINEModel
    {
        public LINEIModel(LadderNetworkModel _parent)
            : base(_parent, Types.LINEI)
        {
            line = new IntLine(this);
        }

        public LINEIModel(LadderNetworkModel _parent, XElement xele)
            : base(_parent, xele)
        {
            line = new IntLine(this);
        }

        public override void Dispose()
        {
            line.Dispose();
            base.Dispose();
        }

        private IntLine line;
        public IntLine Line { get { return this.line; } }

        public override POLYLINEModel Clone()
        {
            LINEIModel that = (LINEIModel)(base.Clone());
            that.line = (IntLine)(this.line.Clone(that));
            return that;
        }
    }
}
