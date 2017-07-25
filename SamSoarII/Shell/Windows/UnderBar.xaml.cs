using SamSoarII.Core.Models;
using SamSoarII.Shell.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SamSoarII.Shell.Windows
{
    public enum UnderBarStatus
    {
        Normal, Loading, Simulate, Monitor, Error
    }
    /// <summary>
    /// UnderBar.xaml 的交互逻辑
    /// </summary>
    public partial class UnderBar : StatusBar, IWindow, INotifyPropertyChanged
    {
        #region Resources

        public readonly static SolidColorBrush NormalBrush;
        public readonly static SolidColorBrush SimulateBrush;
        public readonly static SolidColorBrush MonitorBrush;
        public readonly static SolidColorBrush LoadingBrush;
        public readonly static SolidColorBrush ErrorBrush;

        static UnderBar()
        {
            Color color;
            //#FFC0C0C0
            color = new Color();
            color.A = 0xff;
            color.R = 0xd0;
            color.G = 0xd0;
            color.B = 0xd0;
            NormalBrush = new SolidColorBrush(color);
            //#FF4CB2FA
            color = new Color();
            color.A = 255;
            color.R = 0x4c;
            color.G = 0xb2;
            color.B = 0xfa;
            MonitorBrush = new SolidColorBrush(color);
            //#FFF46622
            color = new Color();
            color.A = 255;
            color.R = 0xf4;
            color.G = 0x66;
            color.B = 0x22;
            SimulateBrush = new SolidColorBrush(color);
            //#FFA92AD7
            color = new Color();
            color.A = 255;
            color.R = 0xa9;
            color.G = 0x2a;
            color.B = 0xd7;
            LoadingBrush = new SolidColorBrush(color);
            //#FF812135
            color = new Color();
            color.A = 0xff;
            color.R = 0x81;
            color.G = 0x21;
            color.B = 0x35;
            ErrorBrush = new SolidColorBrush(color);
        }

        #endregion

        public UnderBar(InteractionFacade _ifparent)
        {
            InitializeComponent();
            DataContext = this;
            ifparent = _ifparent;
            ifparent.PostIWindowEvent += OnReceiveIWindowEvent;
            status = UnderBarStatus.Normal;
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private InteractionFacade ifparent;
        public InteractionFacade IFParent
        {
            get
            {
                return this.ifparent;
            }
        }

        private UnderBarStatus status;
        public UnderBarStatus Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
                PropertyChanged(this, new PropertyChangedEventArgs("BarColor"));
                PropertyChanged(this, new PropertyChangedEventArgs("FontColor"));
            }
        }

        public Brush BarColor
        {
            get
            {
                switch (status)
                {
                    case UnderBarStatus.Normal: return NormalBrush;
                    case UnderBarStatus.Loading: return LoadingBrush;
                    case UnderBarStatus.Simulate: return SimulateBrush;
                    case UnderBarStatus.Monitor: return MonitorBrush;
                    case UnderBarStatus.Error: return ErrorBrush;
                    default: return NormalBrush;
                }
            }
        }
        public Brush FontColor
        {
            get
            {
                switch (status)
                {
                    case UnderBarStatus.Normal: return Brushes.Black;
                    default: return Brushes.White;
                }
            }
        }

        private ProjectModel project;
        public ProjectModel Project
        {
            get
            {
                return this.project;
            }
            set
            {
                if (project == value) return;
                ProjectModel _project = project;
                project = value;
                if (_project != null)
                {
                    _project.PropertyChanged -= OnProjectPropertyChanged;
                    _project.Modified -= OnProjectModified;
                }
                if (project != null)
                {
                    project.PropertyChanged += OnProjectPropertyChanged;
                    project.Modified += OnProjectModified;
                    TB_Item4.Text = String.Format("{0:s}:{1:s}", Properties.Resources.Project, project.ProjName);
                }
            }
        }
        
        private object current;
        public object Current
        {
            get
            {
                return this.current;
            }
            private set
            {
                if (current == value) return;
                object _current = current;
                this.current = value;
                if (_current != null)
                {
                    if (_current is LadderDiagramViewModel)
                    {
                        LadderDiagramViewModel ldvmodel = (LadderDiagramViewModel)_current;
                        ldvmodel.SelectionChanged -= OnLadderSelectionChanged;
                        ldvmodel.SelectionRect.Core.PropertyChanged -= OnLadderSelectionRectPropertyChanged;
                    }
                    if (_current is FuncBlockViewModel)
                    {
                        FuncBlockViewModel fbvmodel = (FuncBlockViewModel)_current;
                        fbvmodel.PropertyChanged -= OnFuncBlockPropertyChanged;
                    }
                    if (_current is ModbusTableViewModel)
                    {

                    }
                }
                if (current != null)
                {
                    if (current is LadderDiagramViewModel)
                    {
                        LadderDiagramViewModel ldvmodel = (LadderDiagramViewModel)current;
                        ldvmodel.SelectionChanged += OnLadderSelectionChanged;
                        ldvmodel.SelectionRect.Core.PropertyChanged += OnLadderSelectionRectPropertyChanged;
                    }
                    if (current is FuncBlockViewModel)
                    {
                        FuncBlockViewModel fbvmodel = (FuncBlockViewModel)current;
                        fbvmodel.PropertyChanged += OnFuncBlockPropertyChanged;
                    }
                    if (current is ModbusTableViewModel)
                    {

                    }
                }
                if (project != null && !ifparent.IsWaitForKey)
                    TB_Header.Text = Properties.Resources.Ready;
            }
        }
        
        #endregion

        #region Update

        public void Reset()
        {
            Project = null;
            Current = null;
            Status = UnderBarStatus.Normal;
            TB_Header.Text = "";
            TB_Item1.Text = "";
            TB_Item2.Text = "";
            TB_Item3.Text = "";
            TB_Item4.Text = "";
        }

        public void ResetMessage()
        {
            if (Status == UnderBarStatus.Error)
            {
                Status = UnderBarStatus.Normal;
                TB_Header.Text = Properties.Resources.Ready;
            }
        }
        
        public void Update(LadderDiagramViewModel view)
        {
            Current = view;
            TB_Item3.Text = String.Format("{0:s}:{1:s}", Properties.Resources.Routine, view.Core.Name);
            switch (view.SelectionStatus)
            {
                case SelectStatus.Idle:
                    TB_Item2.Text = "";
                    TB_Item1.Text = "";
                    break;
                case SelectStatus.SingleSelected:
                    SelectRectCore rect = view.SelectionRect.Core;
                    TB_Item2.Text = (rect.Parent.Brief.Length > 0)
                        ? String.Format("{0:s} {1:d} - {2:s}", Properties.Resources.Network, rect.Parent.ID, rect.Parent.Brief)
                        : String.Format("{0:s} {1:d}", Properties.Resources.Network, rect.Parent.ID);
                    TB_Item1.Text = String.Format("(X={0:d},Y={1:d})", rect.X, rect.Y);
                    break;
                case SelectStatus.MultiSelecting:
                case SelectStatus.MultiSelected:
                    LadderNetworkViewModel network = view.SelectStartNetwork;
                    if (network == null)
                    {
                        TB_Item2.Text = Properties.Resources.MainWindow_Select_All;
                        TB_Item1.Text = "";
                        break;
                    }
                    switch (view.CrossNetState)
                    {
                        case CrossNetworkState.NoCross:
                            TB_Item2.Text = String.Format("{0:s} {1:d}", Properties.Resources.Network, network.Core.ID);
                            TB_Item1.Text = String.Format("(X1={0:d},X2={1:d},Y1={2:d},Y2={3:d})",
                                Math.Min(network.SelectAreaFirstX, network.SelectAreaSecondX),
                                Math.Max(network.SelectAreaFirstX, network.SelectAreaSecondX),
                                Math.Min(network.SelectAreaFirstY, network.SelectAreaSecondY),
                                Math.Max(network.SelectAreaFirstY, network.SelectAreaSecondY));
                            break;
                        case CrossNetworkState.CrossUp:
                            TB_Item2.Text = String.Format("{0:s} ({1:d}~{2:d})", 
                                Properties.Resources.Network,
                                view.SelectAllNetworks.Count() > 0
                                    ? view.SelectAllNetworks.Select(n => n.Core.ID).Min()
                                    : view.SelectStartNetwork.Core.ID,
                                view.SelectStartNetwork.Core.ID);
                            TB_Item1.Text = "";
                            break;
                        case CrossNetworkState.CrossDown:
                            TB_Item2.Text = String.Format("{0:s} ({1:d}~{2:d})",
                                Properties.Resources.Network,
                                view.SelectStartNetwork.Core.ID,
                                view.SelectAllNetworks.Count() > 0
                                    ? view.SelectAllNetworks.Select(n => n.Core.ID).Max()
                                    : view.SelectStartNetwork.Core.ID);
                            TB_Item1.Text = "";
                            break;
                    }
                    break;
            }
            ResetMessage();
        }

        public void Update(FuncBlockViewModel view)
        {
            Current = view;
            TB_Item3.Text = String.Format("{0:s}:{1:s}", Properties.Resources.FuncBlock, view.Core.Name);
            TB_Item2.Text = String.Format("({0:d},{1:d})", view.Line, view.Column);
            TB_Item1.Text = "";
            ResetMessage();
        }

        public void Update(ModbusTableViewModel view)
        {
            Current = view;
            TB_Item3.Text = view.Current != null
                ? String.Format("{0:s}:{1:s}", Properties.Resources.Modbus_Table, view.Current.Name)
                : String.Empty;
            TB_Item2.Text = "";
            TB_Item1.Text = "";
            ResetMessage();
        }

        #endregion

        #region Event Handler

        public event IWindowEventHandler Post = delegate { };

        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {
            if (sender is MainTabControl && e is MainTabControlEventArgs)
            {
                MainTabControlEventArgs e1 = (MainTabControlEventArgs)e;
                if (e1.Action == TabAction.ACTIVE)
                {
                    if (e1.Tab is MainTabDiagramItem)
                        Update(((MainTabDiagramItem)(e1.Tab)).LDVModel);
                    if (e1.Tab is FuncBlockViewModel)
                        Update((FuncBlockViewModel)(e1.Tab));
                    if (e1.Tab is ModbusTableViewModel)
                        Update((ModbusTableViewModel)(e1.Tab));
                }
            }
            if (e is UnderBarEventArgs)
            {
                UnderBarEventArgs e2 = (UnderBarEventArgs)e;
                Status = e2.Status;
                TB_Header.Text = e2.Message;
            }
        }
        
        private void OnProjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsModified") && project.IsModified == false)
                TB_Header.Text = Properties.Resources.Project_Saved;
        }

        private void OnProjectModified(object sender, RoutedEventArgs e)
        {
            if (sender is LadderDiagramModel)
                TB_Header.Text = Properties.Resources.Ladder_Changed;
            else if (sender is FuncBlockModel)
                TB_Header.Text = Properties.Resources.Func_Changed;
            else if (sender is ModbusTableModel || sender is ModbusModel || sender is ModbusItem)
                TB_Header.Text = Properties.Resources.ModBus_Changed;
            else if (sender is ProjectPropertyParams)
                TB_Header.Text = Properties.Resources.Project_Config_Changed;
            else
                TB_Header.Text = Properties.Resources.Project_Changed;
        }

        private void OnLadderSelectionChanged(object sender, RoutedEventArgs e)
        {
            Update((LadderDiagramViewModel)current);
        }

        private void OnLadderSelectionRectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Update((LadderDiagramViewModel)current);
        }
        
        private void OnFuncBlockPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Update((FuncBlockViewModel)current);
        }

        #endregion

    }

    public class UnderBarEventArgs : IWindowEventArgs
    {
        private UnderBar barStatus;
        public UnderBar BARStatus { get { return this.barStatus; } }
        object IWindowEventArgs.TargetedObject { get { return this.barStatus; } }

        private UnderBarStatus status;
        public UnderBarStatus Status { get { return this.status; } }
        int IWindowEventArgs.Flags { get { return (int)status; } }

        private string message;
        public string Message { get { return this.message; } }
        object IWindowEventArgs.RelativeObject { get { return this.message; } }

        public UnderBarEventArgs(UnderBar _barStatus, UnderBarStatus _status, string _message)
        {
            barStatus = _barStatus;
            status = _status;
            message = _message;
        }
    }
}
