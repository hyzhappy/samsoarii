using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class LINEFModel : POLYLINEModel
    {
        public LINEFModel(LadderNetworkModel _parent)
            : base(_parent, Types.LINEF)
        {
            line = new FloatLine(this);
        }

        public LINEFModel(LadderNetworkModel _parent, XElement xele)
            : base(_parent, xele)
        {
            line = new FloatLine(this);
        }
        public override void Dispose()
        {
            line.Dispose();
            base.Dispose();
        }

        private FloatLine line;
        public FloatLine Line { get { return this.line; } }

        public override POLYLINEModel Clone()
        {
            LINEFModel that = (LINEFModel)(base.Clone());
            that.line = (FloatLine)(this.line.Clone(that));
            return that;
        }
    }
}
