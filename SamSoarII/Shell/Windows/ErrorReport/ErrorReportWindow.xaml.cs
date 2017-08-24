using SamSoarII.Core.Generate;
using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

namespace SamSoarII.Shell.Windows
{
    /// <summary>
    /// ErrorReportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ErrorReportWindow : UserControl, IWindow, INotifyPropertyChanged
    {
        public ErrorReportWindow(InteractionFacade _ifParent)
        {
            InitializeComponent();
            DataContext = this;
            ifParent = _ifParent;
            ifParent.PostIWindowEvent += OnReceiveIWindowEvent;
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void Initialize()
        {
            foreach (ErrorReportElement element in items)
                element.Dispose();
            items.Clear();
            foreach (ErrorReportElement_FB element in fitems)
                element.Dispose();
            fitems.Clear();
        }

        #region Numbers
        
        private InteractionFacade ifParent;
        public InteractionFacade IFParent { get { return this.ifParent; } }

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
            Initialize();
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
            Initialize();
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

        public event IWindowEventHandler Post = delegate { };

        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {

        }
        
        private void DataGridCell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!ifParent.WNDMain.LAErrorList.IsFloat
             && !ifParent.WNDMain.LAErrorList.IsDock)
            {
                LayoutSetting.AddDefaultDockWidthAnchorable(
                    Properties.Resources.MainWindow_Error_List, ifParent.WNDMain.LAErrorList.AutoHideWidth.ToString());
                LayoutSetting.AddDefaultDockHeighAnchorable(
                    Properties.Resources.MainWindow_Error_List, ifParent.WNDMain.LAErrorList.AutoHideHeight.ToString());
                ifParent.WNDMain.LAErrorList.ToggleAutoHide();
            }
            if (sender is DataGridCell)
            {
                DataGridCell dgcell = (DataGridCell)sender;
                if (dgcell.DataContext is ErrorReportElement)
                {
                    ErrorReportElement ele = (ErrorReportElement)(dgcell.DataContext);
                    LadderUnitModel unit = ele.Prototype;
                    ifParent.Navigate(unit);
                }
                if (dgcell.DataContext is ErrorReportElement_FB)
                {
                    ErrorReportElement_FB ele = (ErrorReportElement_FB)(dgcell.DataContext);
                    FuncBlockModel fbmodel = ele.FuncBlock;
                    int line = ele.Line;
                    int column = ele.Column;
                    ifParent.Navigate(fbmodel, line, column);
                }
            }
        }

        #endregion

    }
}
