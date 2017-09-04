using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class ARCHFModel : POLYLINEModel
    {
        public ARCHFModel(LadderNetworkModel _parent)
            : base(_parent, Types.ARCF)
        {
            arch = new FloatArch(this);
        }

        public ARCHFModel(LadderNetworkModel _parent, XElement xele)
            : base(_parent, xele)
        {
            //arch = new FloatArch(this);
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            arch.Dispose();
            base.Dispose();
        }

        public override SystemUnits Unit { get { return SystemUnits.MM; } }

        private FloatArch arch;
        public FloatArch Arch { get { return this.arch; } }

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
            XElement xele_a = xele.Element("Arch");
            if (arch == null) arch = new FloatArch(this);
            if (xele_a != null) arch.Load(xele_a);
        }

        public override POLYLINEModel Clone()
        {
            ARCHFModel that = (ARCHFModel)(base.Clone());
            that.arch = (FloatArch)(this.arch.Clone(that));
            return that;
        }
    }
}
