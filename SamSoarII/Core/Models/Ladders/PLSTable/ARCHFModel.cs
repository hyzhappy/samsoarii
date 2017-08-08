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
            arch = new FloatArch(this);
        }

        public override void Dispose()
        {
            arch.Dispose();
            base.Dispose();
        }

        private FloatArch arch;
        public FloatArch Arch { get { return this.arch; } }

        public override POLYLINEModel Clone()
        {
            ARCHFModel that = (ARCHFModel)(base.Clone());
            that.arch = (FloatArch)(this.arch.Clone(that));
            return that;
        }
    }
}
