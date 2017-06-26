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
using Xceed.Wpf.AvalonDock.Global;

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

        public void Initialize()
        {
            items.Clear();
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

        private ObservableCollection<ErrorReportElement_FB> fitems
            = new ObservableCollection<ErrorReportElement_FB>();
        public IEnumerable<ErrorReportElement_FB> FItems
        {
            get
            {
                return fitems.Where(
                    (ErrorReportElement_FB ele) =>
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
                H_Error.Text = String.Format(Properties.Resources.ErrorReportWindow_Error + " {0:d}", value);
            }
        }

        private int warningcount;
        private int WarningCount
        {
            get { return this.warningcount; }
            set
            {
                this.warningcount = value;
                H_Warning.Text = String.Format(Properties.Resources.ErrorReportWindow_Warning + " {0:d}", value);
            }
        }

        public const int MODE_LADDER = 0x01;
        public const int MODE_FUNC = 0x02;
        private int mode;
        public int Mode
        {
            get { return this.mode; }
            set
            {
                this.mode = value;
                switch (value)
                {
                    case MODE_LADDER:
                        DG_List.Visibility = Visibility.Visible;
                        DG_FList.Visibility = Visibility.Hidden;
                        break;
                    case MODE_FUNC:
                        DG_List.Visibility = Visibility.Hidden;
                        DG_FList.Visibility = Visibility.Visible;
                        break;
                }
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
                PropertyChanged(this, new PropertyChangedEventArgs("FItems"));
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
                PropertyChanged(this, new PropertyChangedEventArgs("FItems"));
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

        public void Update(IEnumerable<ErrorReportElement_FB> eles)
        {
            fitems.Clear();
            int _errorcount = 0;
            int _warningcount = 0;
            foreach (ErrorReportElement_FB ele in eles)
            {
                fitems.Add(ele);
                switch (ele.Status)
                {
                    case PLCOriginInst.STATUS_ERROR:
                        _errorcount++;
                        break;
                    case PLCOriginInst.STATUS_WARNING:
                        _warningcount++;
                        break;
                }
            }
            PropertyChanged(this, new PropertyChangedEventArgs("FItems"));
            ErrorCount = _errorcount;
            WarningCount = _warningcount;
            IsShowError = true;
            IsShowWarning = true;
        }

        #region Event Handlers

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        private void DG_List_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (!parent.MainWindow.LAErrorList.IsFloat
             && !parent.MainWindow.LAErrorList.IsDock)
            {
                LayoutSetting.AddDefaultDockWidthAnchorable(
                    Properties.Resources.MainWindow_Error_List, parent.MainWindow.LAErrorList.AutoHideWidth.ToString());
                LayoutSetting.AddDefaultDockHeighAnchorable(
                    Properties.Resources.MainWindow_Error_List, parent.MainWindow.LAErrorList.AutoHideHeight.ToString());
                parent.MainWindow.LAErrorList.ToggleAutoHide();
            }
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
        
        private void DG_FList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (!parent.MainWindow.LAErrorList.IsFloat
             && !parent.MainWindow.LAErrorList.IsDock)
            {
                LayoutSetting.AddDefaultDockWidthAnchorable(
                    Properties.Resources.MainWindow_Error_List, parent.MainWindow.LAErrorList.AutoHideWidth.ToString());
                LayoutSetting.AddDefaultDockHeighAnchorable(
                    Properties.Resources.MainWindow_Error_List, parent.MainWindow.LAErrorList.AutoHideHeight.ToString());
                parent.MainWindow.LAErrorList.ToggleAutoHide();
            }
            if (DG_FList.SelectedIndex < 0) return;
            ErrorReportElement_FB ele = (ErrorReportElement_FB)DG_FList.SelectedItem;
            FuncBlockViewModel fbvmodel = ele.FBVModel;
            int line = ele.Line;
            int column = ele.Column;
            parent.NavigateToFuncBlock(fbvmodel, line, column);
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

    public class ErrorReportElement_FB : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public const int STATUS_WARNING = 0x01;
        public const int STATUS_ERROR = 0x02;
        public int Status { get; private set; }
        
        public Brush BrushFill
        {
            get
            {
                switch (Status)
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

        public string Message { get; private set; }

        private FuncBlockViewModel fbvmodel;
        public FuncBlockViewModel FBVModel
        {
            get { return this.fbvmodel; }
        }
        public string Program
        {
            get { return fbvmodel.ProgramName; }
        }
        
        public int Line { get; private set; }
        public int Column { get; private set; }
        public string Point
        {
            get { return String.Format("({0:d}, {1:d})", Line, Column); }
        }
        
        public ErrorReportElement_FB
        (
            int _status,
            string _message,
            FuncBlockViewModel _fbvmodel,
            int _line,
            int _column
        )
        {
            Status = _status;
            Message = _message;
            fbvmodel = _fbvmodel;
            Line = _line;
            Column = _column;
            PropertyChanged(this, new PropertyChangedEventArgs("Message"));
            PropertyChanged(this, new PropertyChangedEventArgs("Program"));
            PropertyChanged(this, new PropertyChangedEventArgs("Point"));
        }
    }
}
