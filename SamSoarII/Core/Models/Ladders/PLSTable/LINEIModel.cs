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
            //line = new IntLine(this);
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            line.Dispose();
            base.Dispose();
        }

        public override SystemUnits Unit { get { return SystemUnits.PLS; } }

        private IntLine line;
        public IntLine Line { get { return this.line; } }

        public override void Save(XElement xele)
        {
            base.Save(xele);
            XElement xele_l = new XElement("Line");
            line.Save(xele_l);
            xele.Add(xele_l);
        }

        public override void Load(XElement xele)
        {
            base.Load(xele);
            XElement xele_l = xele.Element("Line");
            if (line == null) line = new IntLine(this);
            if (xele_l != null) line.Load(xele_l);
        }

        public override POLYLINEModel Clone()
        {
            LINEIModel that = (LINEIModel)(base.Clone());
            that.line = (IntLine)(this.line.Clone(that));
            return that;
        }
    }
}
