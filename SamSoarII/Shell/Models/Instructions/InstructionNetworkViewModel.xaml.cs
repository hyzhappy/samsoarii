using SamSoarII.Core.Generate;
using SamSoarII.Core.Models;
using SamSoarII.Threads;
using System;
using System.Collections.Generic;
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

namespace SamSoarII.Shell.Models
{
    /// <summary>
    /// InstructionNetworkViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class InstructionNetworkViewModel : UserControl, ILoadModel
    {
        public InstructionNetworkViewModel(InstructionNetworkModel _core)
        {
            InitializeComponent();
            DataContext = this;
            Core = _core;
        }

        public void Dispose()
        {
            Core = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Core

        private InstructionNetworkModel core;
        public InstructionNetworkModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                InstructionNetworkModel _core = core;
                this.core = value;
                if (_core != null)
                {
                    _core.PropertyChanged -= OnCorePropertyChanged;
                    if (_core.View != null) _core.View = null;
                }
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
                    if (core.View != this) core.View = this;
                }
            }
        }
        IModel IViewModel.Core
        {
            get { return core; }
            set { Core = (InstructionNetworkModel)value; }
        }

        public ValueManager ValueManager { get { return Core != null ? Core.Parent.Parent.Parent.Parent.MNGValue : null; } }

        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ID": PropertyChanged(this, new PropertyChangedEventArgs("Header")); break;
                case "IsMasked": Update(); break;
            }
        }

        public void FullLoad()
        {
            isfullloaded = true;
        }

        #endregion

        #region Shell

        public InstructionDiagramViewModel ViewParent { get { return core?.Parent.Parent?.Inst.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        #region Binding

        public string Header { get { return String.Format("Network {0:d}", core != null ? core.Parent.ID : 0); } }

        #endregion

        #region Load

        private bool isfullloaded;
        public bool IsFullLoaded { get { return this.isfullloaded; } }
        public ViewThreadManager ViewThread { get { return Core.Parent.Parent.Parent.Parent.ThMNGView; } }
        public IEnumerable<ILoadModel> LoadChildren { get { return new ILoadModel[] { }; } }

        public void UpdateFullLoadProgress() { }
        
        #endregion
        
        private List<TextBlock[]> insts = new List<TextBlock[]>();

        private void SetPosition(FrameworkElement ctrl, int row, int column = -1)
        {
            Canvas.SetTop(ctrl, row * 20 + 1);
            ctrl.Height = 18;
            switch (column)
            {
                case -1: Canvas.SetLeft(ctrl, 1); ctrl.Width = 600; break;
                case 0: Canvas.SetLeft(ctrl, 1); ctrl.Width = 38; break;
                case 1: Canvas.SetLeft(ctrl, 41); ctrl.Width = 78; break;
                case 2: Canvas.SetLeft(ctrl, 121); ctrl.Width = 78; break;
                case 3: Canvas.SetLeft(ctrl, 181); ctrl.Width = 78; break;
                case 4: Canvas.SetLeft(ctrl, 241); ctrl.Width = 78; break;
                case 5: Canvas.SetLeft(ctrl, 321); ctrl.Width = 78; break;
                case 6: Canvas.SetLeft(ctrl, 401); ctrl.Width = 78; break;
                case 7: Canvas.SetLeft(ctrl, 481); ctrl.Width = 238; break;
            }
        }

        public void Update()
        {
            RowDefinition rdef;
            CV_Inst.Children.Clear();
            CV_Inst.Height = 20;
            rdef = new RowDefinition();
            rdef.Height = new GridLength(20);
            TextBlock tberr = new TextBlock();
            tberr.Background = Brushes.Red;
            SetPosition(tberr, 0);
            CV_Inst.Children.Add(tberr);
            if (!isfullloaded)
            {
                tberr.Background = Brushes.Gray;
                tberr.Text = String.Format(
                    App.CultureIsZH_CH() ? "正在加载 Network {0:d}..." : "Loading Network {0:d}...",
                    Core.ID);
                return;
            }
            if (Core.IsMasked)
            {
                tberr.Background = Brushes.Gray;
                tberr.Text = String.Format(
                    App.CultureIsZH_CH() ? "Network {0:d} 已被屏蔽！" : "Network {0:d} has been masked!",
                    Core.ID);
                return;
            }
            if (Core.IsOpenCircuit)
            {
                tberr.Text = String.Format(
                    App.CultureIsZH_CH() ? "Network {0:d} 的梯形图存在断路错误！" : "There have broken circuit in ladder of Network {0:d}.",
                    Core.ID);
                return;
            }
            if (Core.IsShortCircuit)
            {
                tberr.Text = String.Format(
                    App.CultureIsZH_CH() ? "Network {0:d} 的梯形图存在短路错误！" : "There have short circuit in ladder of Network {0:d}.",
                    Core.ID);
                return;
            }
            if (Core.IsFusionCircuit)
            {
                tberr.Text = String.Format(
                    App.CultureIsZH_CH() ? "Network {0:d} 的梯形图存在混连错误！" : "There have fusion circuit in ladder of Network {0:d}.",
                    Core.ID);
                return;
            }
            CV_Inst.Children.Clear();
            CV_Inst.Height = Core.Insts.Count * 20;
            for (int rowid = 0; rowid < Core.Insts.Count; rowid++)
            {
                PLCOriginInst inst = Core.Insts[rowid];
                LadderUnitModel unit = inst.Inst.ProtoType;
                TextBlock[] tbs = (rowid < insts.Count())
                    ? insts[rowid] : new TextBlock[8];
                TextBlock tb = tbs[0] != null ? tbs[0] : (tbs[0] = new TextBlock());
                tb.Text = rowid.ToString();
                tb.Foreground = unit != null ? Brushes.Black : Brushes.Gray;
                tb.Background = (rowid & 1) == 0 ? Brushes.AliceBlue : Brushes.LightCyan;
                SetPosition(tb, rowid, 0);
                CV_Inst.Children.Add(tb);
                for (int colid = 1; colid <= 6; colid++)
                {
                    tb = tbs[colid] != null ? tbs[colid] : (tbs[colid] = new TextBlock());
                    tb.Text = inst[colid - 1];
                    tb.Foreground = unit != null ? Brushes.Black : Brushes.Gray;
                    tb.Background = (rowid & 1) == 0 ? Brushes.AliceBlue : Brushes.LightCyan;
                    SetPosition(tb, rowid, colid);
                    CV_Inst.Children.Add(tb);
                }
                tb = tbs[7] != null ? tbs[7] : (tbs[7] = new TextBlock());
                tb.Visibility = iscommentmode
                    ? Visibility.Visible
                    : Visibility.Hidden;
                tb.Foreground = Brushes.Green;
                StringBuilder tbtext = new StringBuilder("");
                if (unit != null)
                {
                    tbtext.Append("// ");
                    foreach (ValueModel value in unit.Children)
                    {
                        ValueInfo info = ValueManager[value];
                        tbtext.Append(String.Format("{0:s}:{1:s}, ",
                            value.Text, info.Comment));
                    }
                }
                tb.Text = tbtext.ToString();
                SetPosition(tb, rowid, 7);
                CV_Inst.Children.Add(tb);
                if (rowid >= insts.Count()) insts.Add(tbs);
            }
            Canvas.SetZIndex(Cursor, -1); 
            //CV_Inst.Children.Add(Cursor);
        }
        
        private LadderModes laddermode;
        public LadderModes LadderMode
        {
            get
            {
                return this.laddermode;
            }
            set
            {
                this.laddermode = value;
            }
        }

        private bool iscommentmode;
        public bool IsCommentMode
        {
            get
            {
                return this.iscommentmode;
            }
            set
            {
                this.iscommentmode = value;
                foreach (TextBlock[] tbs in insts)
                {
                    tbs[7].Visibility = iscommentmode
                        ? Visibility.Visible
                        : Visibility.Hidden;
                }
            }
        }

        #endregion

    }
}
