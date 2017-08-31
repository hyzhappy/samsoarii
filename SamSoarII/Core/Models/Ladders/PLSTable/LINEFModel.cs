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
            if (IsDisposed) return;
            line.Dispose();
            base.Dispose();
        }

        public override SystemUnits Unit { get { return SystemUnits.MM; } }

        private FloatLine line;
        public FloatLine Line { get { return this.line; } }

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
            line.Load(xele.Element("Line"));
        }

        public override POLYLINEModel Clone()
        {
            LINEFModel that = (LINEFModel)(base.Clone());
            that.line = (FloatLine)(this.line.Clone(that));
            return that;
        }
    }
}
