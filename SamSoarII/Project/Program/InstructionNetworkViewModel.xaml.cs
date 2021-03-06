﻿using SamSoarII.Extend.LadderChartModel;
using SamSoarII.Extend.LogicGraph;
using SamSoarII.Extend.Utility;
using SamSoarII.LadderInstModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.Utility;
using SamSoarII.ValueModel;
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
using System.ComponentModel;

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
        
        private List<TextBlock> comments = new List<TextBlock>();
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
                foreach (TextBlock tb in comments)
                {
                    tb.Visibility = iscommentmode
                        ? Visibility.Visible
                        : Visibility.Hidden;
                }
            }
        }

        public bool IsMasked
        {
            get { return lnvmodel != null && lnvmodel.IsMasked; }
        }

        private bool ismodified;
        public bool IsModified
        {
            get { return !IsMasked && this.ismodified; }
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
            lnvmodel.PropertyChanged += OnNetworkPropertyChanged;
            Update();
        }
        
        public void Uninstall()
        {
            lnvmodel.ElementChanged -= OnElementChanged;
            lnvmodel.VerticalLineChanged -= OnVerticalLineChanged;
            lnvmodel.PropertyChanged -= OnNetworkPropertyChanged;
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
                Status = STATUS_ERROR;
                tberr.Text = App.CultureIsZH_CN() 
                    ? "找不到 Network。"
                    : "Cannot found network.";
                return;
            }
            if (IsMasked)
            {
                tberr.Background = Brushes.Gray;
                tberr.Text = String.Format(
                    App.CultureIsZH_CN() ? "Network {0:d} 已被屏蔽！" : "Network {0:d} has been masked!",
                    lnvmodel.NetworkNumber);
                return;
            }
            ismodified = false;
            this.lchart = GenerateHelper.CreateLadderChart(lnvmodel.GetElements().Union(lnvmodel.GetVerticalLines()));
            if (lchart.checkOpenCircuit())
            {
                Status = STATUS_OPEN;
                tberr.Text = String.Format(
                    App.CultureIsZH_CN() ? "Network {0:d} 的梯形图存在断路错误！" : "There have broken circuit in ladder of Network {0:d}.", 
                    lnvmodel.NetworkNumber);
                return;
            }
            this.lgraph = lchart.Generate();
            if (lgraph.checkShortCircuit())
            {
                Status = STATUS_SHORT;
                tberr.Text = String.Format(
                    App.CultureIsZH_CN() ? "Network {0:d} 的梯形图存在短路错误！" : "There have short circuit in ladder of Network {0:d}.",
                    lnvmodel.NetworkNumber);
                return;
            }
            if (lgraph.CheckFusionCircuit())
            {
                Status = STATUS_FUSION;
                tberr.Text = String.Format(
                    App.CultureIsZH_CN() ? "Network {0:d} 的梯形图存在混连错误！" : "There have fusion circuit in ladder of Network {0:d}.",
                    lnvmodel.NetworkNumber);
                return;
            }
            Status = STATUS_ACCEPT;
            List<PLCInstruction> _insts = lgraph.GenInst();
            SortedSet<int> prototypeids = new SortedSet<int>();     
            foreach (PLCInstruction inst in _insts)
            {
                insts.Add(inst.ToOrigin());
                if (inst.PrototypeID != -1)
                {
                    if (prototypeids.Contains(inst.PrototypeID))
                    {
                        Status = STATUS_FUSION;
                        tberr.Text = String.Format(
                            App.CultureIsZH_CN() ? "Network {0:d} 的梯形图存在混连错误！" : "There have fusion circuit in ladder of Network {0:d}.",
                            lnvmodel.NetworkNumber);
                        return;
                    }
                    prototypeids.Add(inst.PrototypeID);
                }
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
                    tb.Background = (rowid & 1) == 0 ? Brushes.AliceBlue : Brushes.LightCyan;
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
            UpdateComment();
        }
        
        public void UpdateComment()
        {
            foreach (TextBlock comment in comments)
            {
                if (G_Inst.Children.Contains(comment))
                {
                    G_Inst.Children.Remove(comment);
                }
            }
            comments.Clear();
            int rowid = 0;
            foreach (PLCOriginInst inst in insts)
            {
                BaseViewModel bvmodel = inst.ProtoType;
                if (bvmodel != null)
                {
                    TextBlock cmt = new TextBlock();
                    cmt.Visibility = iscommentmode
                        ? Visibility.Visible
                        : Visibility.Hidden;
                    cmt.Foreground = Brushes.Green;
                    cmt.Text = "//";
                    BaseModel bmodel = bvmodel.Model;
                    for (int i = 0; i < bmodel.ParaCount; i++)
                    {
                        IValueModel ivmodel = bmodel.GetPara(i);
                        cmt.Text += String.Format("{0:s}:{1:s}，",
                            ivmodel.ValueString,
                            ValueCommentManager.GetComment(ivmodel));
                    }
                    Grid.SetRow(cmt, rowid);
                    Grid.SetColumn(cmt, 7);
                    G_Inst.Children.Add(cmt);
                    comments.Add(cmt);
                }
                rowid++;
            }
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
            foreach (TextBlock cmt in comments)
            {
                cmt.Visibility = Visibility.Hidden;
            }
            int rowid = 0;
            foreach (PLCOriginInst inst in insts)
            {
                TextBlock tb = new TextBlock();
                tb.Text = inst.Message;
                tb.Background = inst.Status == PLCOriginInst.STATUS_ERROR 
                    ? Brushes.Red : Brushes.Yellow;
                Grid.SetRow(tb, rowid);
                Grid.SetColumn(tb, 7);
                G_Inst.Children.Add(tb);
                rowid++;
            }
        }

        private void OnElementChanged(object sender, LadderElementChangedArgs e)
        {
            ismodified = true;
        }

        private void OnVerticalLineChanged(object sender, LadderElementChangedArgs e)
        {
            ismodified = true;
        }
        
        private void OnNetworkPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsMasked":
                    Update();
                    break;
                case "NetworkMessage":
                    NetworkHeader.Text
                        = String.Format("Network {0:d}", lnvmodel.NetworkNumber);
                    Update();
                    break;
            }
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
