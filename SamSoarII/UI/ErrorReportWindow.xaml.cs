using SamSoarII.AppMain.Project;
using SamSoarII.Extend.Utility;
using SamSoarII.LadderInstViewModel;
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

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// ErrorReportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ErrorReportWindow : UserControl, INotifyPropertyChanged
    {
        public ErrorReportWindow(InteractionFacade _parent)
        {
            InitializeComponent();
            DataContext = this;
            parent = _parent;
        }

        #region Numbers

        private InteractionFacade parent;

        private ObservableCollection<ErrorReportElement> items
            = new ObservableCollection<ErrorReportElement>();
        public IEnumerable<ErrorReportElement> Items
        {
            get
            {
                return items.Where(
                    (ErrorReportElement ele) =>
                    {
                        switch (ele.Status)
                        {
                            case PLCOriginInst.STATUS_WARNING:
                                return IsShowWarning;
                            case PLCOriginInst.STATUS_ERROR:
                                return IsShowError;
                            default:
                                return false;
                        }
                    }
                );
            }
        }

        private int errorcount;
        private int ErrorCount
        {
            get { return this.errorcount; }
            set
            {
                this.errorcount = value;
                H_Error.Text = String.Format("错误 {0:d}", value);
            }
        }

        private int warningcount;
        private int WarningCount
        {
            get { return this.warningcount; }
            set
            {
                this.warningcount = value;
                H_Warning.Text = String.Format("警告 {0:d}", value);
            }
        }

        private bool isshowerror;
        public bool IsShowError
        {
            get { return this.isshowerror; }
            set
            {
                this.isshowerror = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsShowError"));
                PropertyChanged(this, new PropertyChangedEventArgs("Items"));
            }
        }
        
        private bool isshowwarning;
        public bool IsShowWarning
        {
            get { return this.isshowwarning; }
            set
            {
                this.isshowwarning = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsShowWarning"));
                PropertyChanged(this, new PropertyChangedEventArgs("Items"));
            }
        }

        #endregion

        public void Update(IEnumerable<ErrorReportElement> insts)
        {
            items.Clear();
            int _errorcount = 0;
            int _warningcount = 0;
            foreach (ErrorReportElement inst in insts)
            {
                items.Add(inst);
                switch (inst.Status)
                {
                    case PLCOriginInst.STATUS_ERROR:
                        _errorcount++;
                        break;
                    case PLCOriginInst.STATUS_WARNING:
                        _warningcount++;
                        break;
                }
            }
            PropertyChanged(this, new PropertyChangedEventArgs("Items"));
            ErrorCount = _errorcount;
            WarningCount = _warningcount;
            IsShowError = true;
            IsShowWarning = true;
        }

        #region Event Handlers

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        private void DG_List_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (DG_List.SelectedIndex < 0) return;
            ErrorReportElement inst = (ErrorReportElement)DG_List.SelectedItem;
            BaseViewModel bvmodel = inst.Prototype;
            int x = bvmodel.X;
            int y = bvmodel.Y;
            int network = int.Parse(inst.Network);
            string diagram = inst.Diagram;
            NavigateToNetworkEventArgs _e = new NavigateToNetworkEventArgs(network, diagram, x, y);
            parent.NavigateToNetwork(_e);
        }

        #endregion

    }

    public class ErrorReportElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private PLCOriginInst inst;
        private LadderNetworkViewModel lnvmodel;
        private LadderDiagramViewModel ldvmodel;

        public ErrorReportElement
        (
            PLCOriginInst _inst,
            LadderNetworkViewModel _lnvmodel,
            LadderDiagramViewModel _ldvmodel
        )
        {
            this.inst = _inst;
            this.lnvmodel = _lnvmodel;
            this.ldvmodel = _ldvmodel;
        }

        public BaseViewModel Prototype
        {
            get { return inst.ProtoType; }
        }

        public int Status
        {
            get { return inst.Status; }
        }

        public Brush BrushFill
        {
            get
            {
                switch (inst.Status)
                {
                    case PLCOriginInst.STATUS_WARNING:
                        return Brushes.Yellow;
                    case PLCOriginInst.STATUS_ERROR:
                        return Brushes.Red;
                    default:
                        return Brushes.Transparent;
                }
            }
        }
         

        public string Detail
        {
            get { return inst.Message; }
        }

        public string InstText
        {
            get { return inst.Text; }
        }

        public string Network
        {
            get { return String.Format("{0:d}", lnvmodel.NetworkNumber); }
        }

        public string Diagram
        {
            get { return ldvmodel.ProgramName; }
        }
    }
}
