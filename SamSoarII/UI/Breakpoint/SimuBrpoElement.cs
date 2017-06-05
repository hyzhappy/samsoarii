using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Simulation.UI.Breakpoint
{
    public class SimuBrpoElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private bool isactive;
        public bool IsActive
        {
            get
            {
                return this.isactive;
            }
            set
            {
                this.isactive = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ActiveInfo"));
            }
        }
        public string ActiveInfo
        {
            get
            {
                return IsActive ? "启用" : "禁用";
            }
        }

        private LadderDiagramViewModel ldvmodel;
        public LadderDiagramViewModel LDVModel
        {
            get
            {
                return this.ldvmodel;
            }
            set
            {
                this.ldvmodel = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ProgramName"));
            }
        }
        public string ProgramName
        {
            get
            {
                return this.ldvmodel.ProgramName;
            }
        }

        private LadderNetworkViewModel lnvmodel;
        public LadderNetworkViewModel LNVModel
        {
            get
            {
                return this.lnvmodel;
            }
            set
            {
                this.lnvmodel = value;

            }
        }
        public string NetworkNumber
        {
            get
            {
                return LNVModel.NetworkNumber.ToString();
            }
        }

        private BaseViewModel bvmodel;
        public BaseViewModel BVModel
        {
            get
            {
                return this.bvmodel;
            }
            set
            {
                this.bvmodel = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Instruction"));
            }
        }
        public string Instruction
        {
            get
            {
                return BVModel.ToInstString();
            }
        }

        private string condition;
        public string Condition
        {
            get
            {
                return this.condition;
            }
            set
            {
                this.condition = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Condition"));
            }
        }

        private string breaktime;
        public string BreakTime
        {
            get
            {
                return this.breaktime;
            }
            set
            {
                this.breaktime = value;
                PropertyChanged(this, new PropertyChangedEventArgs("BreakTime"));
            }
        }

    }
}
