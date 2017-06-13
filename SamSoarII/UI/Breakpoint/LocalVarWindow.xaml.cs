using SamSoarII.Extend.FuncBlockModel;
using SamSoarII.Extend.Utility;
using SamSoarII.Simulation.Core;
using SamSoarII.Simulation.Core.Event;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamSoarII.Simulation.UI.Breakpoint
{
    /// <summary>
    /// LocalVarWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LocalVarWindow : UserControl, INotifyPropertyChanged
    {
        public LocalVarWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ObservableCollection<LocalVarElement> items
            = new ObservableCollection<LocalVarElement> ();
        public IEnumerable<LocalVarElement> Items
        {
            get { return this.items; }
        }

        unsafe
        private void GetValue(LocalVarElement lvele, FuncBlock_Assignment assign, int addr)
        {
            if (assign.Type.EndsWith("*"))
            {
                lvele.Value = String.Format("0x{0:x8}",
                    (*((Int32*)(addr))));
            }
            else 
            {
                switch (assign.Type)
                {
                    case "WORD":
                        lvele.Value = String.Format("{0}", 
                            (Int16)(*((Int32*)(addr))));
                        break;
                    case "UWORD":
                        lvele.Value = String.Format("{0}",
                            (UInt16)(*((UInt32*)(addr))));
                        break;
                    case "DWORD":
                        lvele.Value = String.Format("{0}",
                            (Int32)(*((Int64*)(addr))));
                        break;
                    case "UDWORD":
                        lvele.Value = String.Format("{0}",
                            (UInt32)(*((UInt64*)(addr))));
                        break;
                    case "FLOAT":
                        lvele.Value = String.Format("{0}",
                            (float)(*((double*)(addr))));
                        break;
                    case "BIT":
                        lvele.Value = *((Int32*)(addr)) == 1
                            ? "ON" : "OFF";
                        break;
                }

            }
        }
        
        private void DG_Main_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DG_Main_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }

        unsafe
        private void OnBreakpointPause(object sender, BreakpointPauseEventArgs e)
        {
            items.Clear();
            FuncBlock fblock = BreakPointManager.GetFBlock(e.Address);
            void* rbp = SimulateDllModel.GetRBP();
            int laddr = (int)rbp + 0x4;
            int paddr = (int)rbp - 0x8;
            if (fblock != null)
            {
                IList<FuncBlock_Assignment> localvars = fblock.LocalVars;
                IList<FuncBlock_Assignment> parameters = fblock.Parameters;
                LocalVarElement_ForFuncBlock lvelement = null;
                foreach (FuncBlock_Assignment localvar in localvars)
                {
                    lvelement = new LocalVarElement_ForFuncBlock(localvar);
                    GetValue(lvelement, localvar, laddr);
                    items.Add(lvelement);
                    laddr -= localvar.Sizeof;
                }
                foreach (FuncBlock_Assignment parameter in parameters)
                {
                    lvelement = new LocalVarElement_ForFuncBlock(parameter);
                    GetValue(lvelement, parameter, paddr);
                    items.Add(lvelement);
                    paddr += parameter.Sizeof;
                }
            }
            PropertyChanged(this, new PropertyChangedEventArgs("Items"));
        }

        private void OnBreakpointResume(object sender, BreakpointPauseEventArgs e)
        {
            items.Clear();
            PropertyChanged(this, new PropertyChangedEventArgs("Items"));
        }
    }

    public class LocalVarElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            protected set
            {
                this.name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        private string describe;
        public string Describe
        {
            get
            {
                return this.describe;
            }
            private set
            {
                this.describe = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Describe"));
            }
        }

        private string value;
        public string Value
        {
            get
            {
                return this.Value;
            }
            set
            {
                this.value = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        private string type;
        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Type"));
            }
        }
    }

    public class LocalVarElement_ForFuncBlock : LocalVarElement
    {
        private FuncBlock_Assignment assign;
        public FuncBlock_Assignment Assign
        {
            get
            {
                return this.assign;
            }
            private set
            {
                this.assign = value;
                Name = assign.Name;
                Type = assign.Type;
            }
        }

        public LocalVarElement_ForFuncBlock(FuncBlock_Assignment _assign)
        {
            Assign = _assign;
        }
    }
}
