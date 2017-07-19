using SamSoarII.Core.Generate;
using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace SamSoarII.Shell.Models
{
    public class InstructionRowViewModel : IResource
    {
        #region IResource

        private int resourceid;
        public int ResourceID
        {
            get { return this.resourceid; }
            set { this.resourceid = value; }
        }

        public IResource Create(params object[] args)
        {
            return new InstructionRowViewModel((PLCOriginInst)args[0], (int)args[1]);
        }
        
        public void Recreate(params object[] args)
        {
            core = (PLCOriginInst)args[0];
            id = (int)args[1];
            if (core == null) return;
            LadderUnitModel unit = core.Inst.ProtoType;
            for (int i = 0; i < 7; i++)
            {
                tbs[i].Background = (i & 1) == 0 ? Brushes.AliceBlue : Brushes.LightCyan;
                tbs[i].Foreground = unit != null ? Brushes.Black : Brushes.Gray;
                tbs[i].Text = i == 0 ? id.ToString() : core[i - 1];
            }
            StringBuilder tbtext = new StringBuilder("");
            if (unit != null)
            {
                tbtext.Append("// ");
                foreach (ValueModel value in unit.Children)
                {
                    ValueInfo info = value.ValueManager[value];
                    tbtext.Append(String.Format("{0:s}:{1:s}, ", value.Text, info.Comment));
                }
            }
            tbs[7].Text = tbtext.ToString();
        }

        #endregion

        public InstructionRowViewModel(PLCOriginInst _core, int _id)
        {
            tbs = new TextBlock[8];
            for (int i = 0; i < 8; i++)
                tbs[i] = new TextBlock();
            tbs[7].Foreground = Brushes.Green;
            Recreate(_core, _id);
        }
        
        public void Dispose()
        {
            core = null;
            AllResourceManager.Dispose(this);
        }

        #region Number

        private PLCOriginInst core;
        public PLCOriginInst Core { get { return this.core; } }

        private int id;
        public int ID { get { return this.id; } }

        private TextBlock[] tbs;
        public IList<TextBlock> TextBlocks { get { return this.tbs; } }
        
        #endregion


    }
}
