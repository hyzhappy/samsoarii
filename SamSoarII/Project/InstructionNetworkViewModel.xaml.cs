﻿using SamSoarII.Extend.LadderChartModel;
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

namespace SamSoarII.AppMain.Project
{
    /// <summary>
    /// InstructionViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class InstructionNetworkViewModel : UserControl, ITabItem
    {
        public string TabHeader
        {
            get;
            set;
        }

        protected double actualheight;
        double ITabItem.ActualHeight
        {
            get
            {
                //return this.actualheight;
                if (insts == null) return 20;
                return 20 + 20 * insts.Count();
            }

            set
            {
                this.actualheight = value;
            }
        }

        protected double actualwidth;
        double ITabItem.ActualWidth
        {
            get
            {
                return this.actualwidth;
            }

            set
            {
                this.actualwidth = value;
            }
        }

        private LadderNetworkViewModel lnvmodel;
        private LadderChart lchart;
        private LGraph lgraph;
        private List<PLCOriginInst> insts;
        private Dictionary<LadderNetworkViewModel, List<PLCInstruction>> networkInstsDict;
        private Dictionary<IntPoint, TextBlock> instTextDict;

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
            networkInstsDict = new Dictionary<LadderNetworkViewModel, List<PLCInstruction>>();
            instTextDict = new Dictionary<IntPoint, TextBlock>();
        }
        
        public void Setup(LadderNetworkViewModel _lnvmodel)
        {
            if (lnvmodel != null)
                Uninstall();
            this.lnvmodel = _lnvmodel;
            lnvmodel.ElementChanged += OnElementChanged;
            lnvmodel.VerticalLineChanged += OnVerticalLineChanged;
            if (!networkInstsDict.ContainsKey(lnvmodel))
            {
                networkInstsDict.Add(lnvmodel, null);
            }
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
                tberr.Text = "找不到 Network。";
                return;
            }
            this.lchart = GenerateHelper.CreateLadderChart(lnvmodel.GetElements().Union(lnvmodel.GetVerticalLines()));
            if (lchart.checkOpenCircuit())
            {
                tberr.Text = String.Format("Network {0:d} 的梯形图存在断路错误！", lnvmodel.NetworkNumber);
                networkInstsDict[lnvmodel] = null;
                return;
            }
            this.lgraph = lchart.Generate();
            if (lgraph.checkShortCircuit())
            {
                tberr.Text = String.Format("Network {0:d} 的梯形图存在短路错误！", lnvmodel.NetworkNumber);
                networkInstsDict[lnvmodel] = null;
                return;
            }
            if (lgraph.CheckFusionCircuit())
            {
                tberr.Text = String.Format("Network {0:d} 的梯形图存在混连错误！", lnvmodel.NetworkNumber);
                networkInstsDict[lnvmodel] = null;
                return;
            }
            List<PLCInstruction> _insts = lgraph.GenInst();
            networkInstsDict[lnvmodel] = _insts;
            foreach (PLCInstruction inst in _insts)
            {
                insts.Add(inst.ToOrigin());
            }
            G_Inst.RowDefinitions.Clear();
            G_Inst.Children.Clear();
            instTextDict.Clear();
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
                    IntPoint ip = new IntPoint();
                    ip.X = colid;
                    ip.Y = rowid;
                    instTextDict.Add(ip, tb);
                }
                rowid++;
            }
            rdef = new RowDefinition();
            rdef.Height = new GridLength(1, GridUnitType.Star);
            G_Inst.RowDefinitions.Add(rdef);
            G_Inst.Children.Add(Cursor);
        }

        private void OnElementChanged(object sender, LadderElementChangedArgs e)
        {
            Update();
        }

        private void OnVerticalLineChanged(object sender, LadderElementChangedArgs e)
        {
            Update();
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
