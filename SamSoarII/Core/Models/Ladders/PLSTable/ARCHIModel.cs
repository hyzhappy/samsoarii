using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class ARCHIModel : POLYLINEModel
    {
        public ARCHIModel(LadderNetworkModel _parent)
            : base(_parent, Types.ARCI)
        {
            arch = new IntArch(this);
        }

        public ARCHIModel(LadderNetworkModel _parent, XElement xele)
            : base(_parent, xele)
        {
            arch = new IntArch(this);
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            arch.Dispose();
            base.Dispose();
        }

        public override SystemUnits Unit { get { return SystemUnits.PLS; } }

        private IntArch arch;
        public IntArch Arch { get { return this.arch; } }

        public override void Save(XElement xele)
        {
            base.Save(xele);
            XElement xele_a = new XElement("Arch");
            arch.Save(xele_a);
            xele.Add(xele_a);
        }

        public override void Load(XElement xele)
        {
            base.Load(xele);
            arch.Load(xele.Element("Arch"));
        }

        public override POLYLINEModel Clone()
        {
            ARCHIModel that = (ARCHIModel)(base.Clone());
            that.arch = (IntArch)(this.arch.Clone(that));
            return that;
        }
    }
}
