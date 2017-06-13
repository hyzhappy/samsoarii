using ICSharpCode.AvalonEdit;
using SamSoarII.AppMain.Project;
using SamSoarII.Extend.FuncBlockModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Simulation.UI.Breakpoint
{
    public class FuncBrpoElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private FuncBlockViewModel fbvmodel;
        public FuncBlockViewModel FBVModel
        {
            get
            {
                return this.fbvmodel;
            }
            set
            {
                this.fbvmodel = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ProgramName"));
            }
        }
        public string ProgramName
        {
            get
            {
                return fbvmodel != null ? fbvmodel.ProgramName : "null";
            }
        }

        private FuncBlock fblock;
        public FuncBlock FBlock
        {
            get
            {
                return this.fblock;
            }
            set
            {
                this.fblock = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Position"));
            }
        }
        public string Position
        {
            get
            {
                TextViewPosition? pos = fbvmodel.GetPosition(fblock.IndexEnd);
                if (!pos.HasValue)
                    return String.Empty;
                return String.Format("({0}, {1})",
                    pos.Value.Line, pos.Value.Column);
            }
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}",
                ProgramName, Position);
        }

    }
}
