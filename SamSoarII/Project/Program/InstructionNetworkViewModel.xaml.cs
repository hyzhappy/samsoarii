using SamSoarII.Extend.LadderChartModel;
using SamSoarII.Extend.LogicGraph;
using SamSoarII.Extend.Utility;
using SamSoarII.LadderInstViewModel;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
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
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace SamSoarII.AppMain.Project
{
    /// <summary>
    /// InstructionViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class InstructionNetworkViewModel : UserControl
    {
        public const int STATUS_ACCEPT = 0x00;
        public const int STATUS_OPEN = 0x01;
        public const int STATUS_SHORT = 0x02;
        public const int STATUS_FUSION = 0x03;
        public const int STATUS_WARNING = 0x04;
        public const int STATUS_ERROR = 0x05;
        public int Status { get; private set; }
        
        private LadderNetworkViewModel lnvmodel;
        public LadderNetworkViewModel LNVModel
        {
            get { return this.lnvmodel; }
        }
        
        private bool ismodified;
        public bool IsModified
        {
            get { return this.ismodified; }
        }

        private LadderChart lchart;

        private LadderGraph lgraph;

        private List<PLCOriginInst> insts;

        public IEnumerable<PLCOriginInst> Insts
        {
            get { return this.insts; }
        }
        
        public LadderNetworkViewModel LadderNetwork
        {
            get { return this.lnvmodel; }
        }

        public BaseViewModel CurrentViewModel
        {
            get
            {
                if (Cursor.Visibility == Visibility.Hidden)
                    return null;
                int rowid = Grid.GetRow(Cursor);
                if (rowid < 0 || rowid >= insts.Count())
                    return null;
                PLCOriginInst inst = insts[rowid];
                return inst.ProtoType;
            }
        }

        public InstructionNetworkViewModel()
        {
            InitializeComponent();
            lnvmodel = null;
        }
        
        public void Setup(LadderNetworkViewModel _lnvmodel)
        {
            if (lnvmodel != null)
                Uninstall();
            this.lnvmodel = _lnvmodel;
            lnvmodel.ElementChanged += OnElementChanged;
            lnvmodel.VerticalLineChanged += OnVerticalLineChanged;
            Update();
        }

        public void Uninstall()
        {
            lnvmodel.ElementChanged -= OnElementChanged;
            lnvmodel.VerticalLineChanged -= OnVerticalLineChanged;
            this.lnvmodel = null;
        }

        public void Update()
        {
            ismodified = false;
            int rowid = 0;
            RowDefinition rdef;
            NetworkHeader.Text = String.Format("Network {0:d}", lnvmodel?.NetworkNumber);
            insts = new List<PLCOriginInst>();
            G_Inst.RowDefinitions.Clear();
            G_Inst.Children.Clear();
            rdef = new RowDefinition();
            rdef.Height = new GridLength(20);
            TextBlock tberr = new TextBlock();
            tberr.Background = Brushes.Red;
            Grid.SetRow(tberr, 0);
            Grid.SetColumn(tberr, 0);
            Grid.SetColumnSpan(tberr, 6);
            G_Inst.RowDefinitions.Add(rdef);
            G_Inst.Children.Add(tberr);
            if (lnvmodel == null)
            {
                Status = STATUS_ERROR;
                tberr.Text = App.CultureIsZH_CH() 
                    ? "找不到 Network。"
                    : "Cannot found network.";
                return;
            }
            this.lchart = GenerateHelper.CreateLadderChart(lnvmodel.GetElements().Union(lnvmodel.GetVerticalLines()));
            if (lchart.checkOpenCircuit())
            {
                Status = STATUS_OPEN;
                tberr.Text = String.Format(
                    App.CultureIsZH_CH() ? "Network {0:d} 的梯形图存在断路错误！" : "There have broken circuit in ladder of Network {0:d}", 
                    lnvmodel.NetworkNumber);
                return;
            }
            this.lgraph = lchart.Generate();
            if (lgraph.checkShortCircuit())
            {
                Status = STATUS_SHORT;
                tberr.Text = String.Format(
                    App.CultureIsZH_CH() ? "Network {0:d} 的梯形图存在短路错误！" : "There have broken circuit in ladder of Network {0:d}",
                    lnvmodel.NetworkNumber);
                return;
            }
            if (lgraph.CheckFusionCircuit())
            {
                Status = STATUS_FUSION;
                tberr.Text = String.Format("Network {0:d} 的梯形图存在混连错误！", lnvmodel.NetworkNumber);
                return;
            }
            Status = STATUS_ACCEPT;
            List<PLCInstruction> _insts = lgraph.GenInst();
            foreach (PLCInstruction inst in _insts)
            {
                insts.Add(inst.ToOrigin());
            }
            G_Inst.RowDefinitions.Clear();
            G_Inst.Children.Clear();
            foreach (PLCOriginInst inst in insts)
            {
                rdef = new RowDefinition();
                rdef.Height = new GridLength(20);
                G_Inst.RowDefinitions.Add(rdef);
                TextBlock tb = new TextBlock();
                tb.Text = rowid.ToString();
                tb.Foreground = inst.ProtoType != null ? Brushes.Black : Brushes.Gray;
                tb.Background = (rowid & 1) == 0 ? Brushes.AliceBlue : Brushes.LightCyan;
                Grid.SetRow(tb, rowid);
                Grid.SetColumn(tb, 0);
                G_Inst.Children.Add(tb);
                for (int colid = 1; colid <= 6; colid++)
                {
                    tb = new TextBlock();
                    tb.Text = inst[colid-1];
                    tb.Foreground = inst.ProtoType != null ? Brushes.Black : Brushes.Gray;
                    tb.Background = (rowid&1) == 0 ? Brushes.AliceBlue : Brushes.LightCyan;
                    Grid.SetRow(tb, rowid);
                    Grid.SetColumn(tb, colid);
                    G_Inst.Children.Add(tb);
                }
                rowid++;
            }
            rdef = new RowDefinition();
            rdef.Height = new GridLength(1, GridUnitType.Star);
            G_Inst.RowDefinitions.Add(rdef);
            G_Inst.Children.Add(Cursor);
        }

        public void UpdateCheck()
        {
            if (Status != STATUS_ACCEPT) return;
            foreach (PLCOriginInst inst in insts)
            {
                switch (inst.Status)
                {
                    case PLCOriginInst.STATUS_WARNING:
                        Status = STATUS_WARNING;
                        break;
                    case PLCOriginInst.STATUS_ERROR:
                        Status = STATUS_ERROR;
                        break;
                }
                if (Status == STATUS_ERROR)
                {
                    break;
                }
            }
            int rowid = 0;
            RowDefinition rdef;
            G_Inst.RowDefinitions.Clear();
            G_Inst.Children.Clear();
            foreach (PLCOriginInst inst in insts)
            {
                rdef = new RowDefinition();
                rdef.Height = new GridLength(20);
                G_Inst.RowDefinitions.Add(rdef);
                TextBlock tb = new TextBlock();
                tb.Text = rowid.ToString();
                tb.Foreground = inst.ProtoType != null ? Brushes.Black : Brushes.Gray;
                tb.Background = (rowid & 1) == 0 ? Brushes.AliceBlue : Brushes.LightCyan;
                Grid.SetRow(tb, rowid);
                Grid.SetColumn(tb, 0);
                G_Inst.Children.Add(tb);
                for (int colid = 1; colid <= 6; colid++)
                {
                    tb = new TextBlock();
                    tb.Text = inst[colid - 1];
                    tb.Foreground = inst.ProtoType != null ? Brushes.Black : Brushes.Gray;
                    tb.Background = (rowid & 1) == 0 ? Brushes.AliceBlue : Brushes.LightCyan;
                    switch (inst.Status)
                    {
                        case PLCOriginInst.STATUS_WARNING:
                            tb.Background = Brushes.LightYellow;
                            break;
                        case PLCOriginInst.STATUS_ERROR:
                            tb.Background = Brushes.Red;
                            break;
                    }
                    Grid.SetRow(tb, rowid);
                    Grid.SetColumn(tb, colid);
                    G_Inst.Children.Add(tb);
                }
                tb = new TextBlock();
                tb.Text = inst.Message;
                //tb.Background = (rowid & 1) == 0 ? Brushes.AliceBlue : Brushes.LightCyan;
                Grid.SetRow(tb, rowid);
                Grid.SetColumn(tb, 7);
                G_Inst.Children.Add(tb);
                rowid++;
            }
            rdef = new RowDefinition();
            rdef.Height = new GridLength(1, GridUnitType.Star);
            G_Inst.RowDefinitions.Add(rdef);
            G_Inst.Children.Add(Cursor);
        }

        private void OnElementChanged(object sender, LadderElementChangedArgs e)
        {
            ismodified = true;
        }

        private void OnVerticalLineChanged(object sender, LadderElementChangedArgs e)
        {
            ismodified = true;
        }
        
        #region Cursor
        public bool CatchCursor(BaseViewModel bvmodel)
        {
            if (insts == null)
                return false;
            Cursor.Visibility = Visibility.Hidden;
            int rowid = 0;
            foreach (PLCOriginInst inst in insts)
            {
                if (inst.ProtoType == bvmodel)
                {
                    Grid.SetRow(Cursor, rowid);
                    Cursor.Visibility = Visibility.Visible;
                    return true;
                }
                rowid++;
            }
            return false;
        }

        public bool CatchCursorTop()
        {
            if (insts == null)
                return false;
            for (int rowid = 0; rowid < insts.Count(); rowid++)
            {
                PLCOriginInst inst = insts[rowid];
                if (inst.ProtoType != null)
                {
                    Grid.SetRow(Cursor, rowid);
                    Cursor.Visibility = Visibility.Visible;
                    return true;
                }
            }
            return false;
        }
        
        public bool CatchCursorBottom()
        {
            if (insts == null)
                return false;
            for (int rowid = insts.Count()-1; rowid >= 0; rowid--)
            {
                PLCOriginInst inst = insts[rowid];
                if (inst.ProtoType != null)
                {
                    Grid.SetRow(Cursor, rowid);
                    Cursor.Visibility = Visibility.Visible;
                    return true;
                }
            }
            return false;
        }

        public bool CursorUp()
        {
            if (insts == null)
                return false;
            for (int rowid = Grid.GetRow(Cursor)-1; rowid >= 0; rowid--)
            {
                PLCOriginInst inst = insts[rowid];
                if (inst.ProtoType != null)
                {
                    Grid.SetRow(Cursor, rowid);
                    Cursor.Visibility = Visibility.Visible;
                    return true;
                }
            }
            return false;
        }

        public bool CursorDown()
        {
            if (insts == null)
                return false;
            for (int rowid = Grid.GetRow(Cursor)+1; rowid < insts.Count(); rowid++)
            {
                PLCOriginInst inst = insts[rowid];
                if (inst.ProtoType != null)
                {
                    Grid.SetRow(Cursor, rowid);
                    Cursor.Visibility = Visibility.Visible;
                    return true;
                }
            }
            return false;
        }

        public event RoutedEventHandler CursorChanged = delegate { };
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            Point p = e.GetPosition(this);
            int rowid = (int)((p.Y - 20) / 20);
            int colid = (int)((p.X - 40) / 80) + 1;
            if (insts == null)
            {
                return;
            }
            if (rowid >= 0 && rowid < insts.Count() &&
                colid >= 1 && colid <= 6)
            {
                PLCOriginInst inst = insts[rowid];
                if (inst.ProtoType == null)
                    return;
                Grid.SetRow(Cursor, rowid);
                Cursor.Visibility = Visibility.Visible;
                CursorChanged(this, new RoutedEventArgs());
            }
        }
        
        public event RoutedEventHandler CursorEdit = delegate { };
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            CursorEdit(this, new RoutedEventArgs());
        }
        #endregion
    }
}
