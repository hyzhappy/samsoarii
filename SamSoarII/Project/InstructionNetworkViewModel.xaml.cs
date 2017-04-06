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

        public InstructionNetworkViewModel()
        {
            InitializeComponent();
            lnvmodel = null;
            networkInstsDict = new Dictionary<LadderNetworkViewModel, List<PLCInstruction>>();
            inputbox = new TextBox();
            inputbox.KeyUp += OnInputBoxKeyUp;
            instTextDict = new Dictionary<IntPoint, TextBlock>();
            InputStatus = INPUTSTATUS_FREE;
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
            NetworkHeader.Text = String.Format("Network{0:d}", lnvmodel.NetworkNumber);
            G_Inst.RowDefinitions.Clear();
            G_Inst.Children.Clear();
            if (lnvmodel == null)
            {
                return;
            }
            this.lchart = GenerateHelper.CreateLadderChart(lnvmodel.GetElements().Union(lnvmodel.GetVerticalLines()));
            if (lchart.checkOpenCircuit())
            {
                networkInstsDict[lnvmodel] = null;
                return;
            }
            this.lgraph = lchart.Generate();
            if (lgraph.checkShortCircuit())
            {
                networkInstsDict[lnvmodel] = null;
                return;
            }
            if (lgraph.CheckFusionCircuit())
            {
                networkInstsDict[lnvmodel] = null;
                return;
            }
            List<PLCInstruction> _insts = lgraph.GenInst();
            networkInstsDict[lnvmodel] = _insts;
            insts = new List<PLCOriginInst>();
            foreach (PLCInstruction inst in _insts)
            {
                insts.Add(inst.ToOrigin());
            }

            int rowid = 0;
            RowDefinition rdef;
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
            G_Inst.Children.Add(inputbox);
        }

        private void OnElementChanged(object sender, LadderElementChangedArgs e)
        {
            Update();
        }

        private void OnVerticalLineChanged(object sender, LadderElementChangedArgs e)
        {
            Update();
        }

        #region Instruction Modification

        private const int INPUTSTATUS_FREE = 0x00;
        private const int INPUTSTATUS_INPUT = 0x01;
        private const int INPUTSTATUS_INPUTALL = 0x02;
        private int inputstatus;
        private TextBox inputbox;
        private int inputrow;
        private int inputcolumn;
        private string inputtext;
        private int inputnumber;

        private int InputStatus
        {
            get { return this.inputstatus; }
            set
            {
                if (insts != null)
                {
                    if (value != INPUTSTATUS_INPUTALL)
                    {
                        PLCOriginInst inst = insts[InputRow];
                        int rowid = InputRow;
                        int colid = 1;
                        for (; colid <= 6; colid++)
                        {
                            IntPoint ip = new IntPoint();
                            ip.X = colid;
                            ip.Y = rowid;
                            TextBlock tb = instTextDict[ip];
                            tb.Text = inst[colid - 1];
                            tb.Foreground = Brushes.Black;
                        }
                    }
                    else
                    {
                        IntPoint ip = new IntPoint();
                        ip.Y = InputRow;
                        ip.X = InputColumn;
                        TextBlock tb = instTextDict[ip];
                        tb.Text = inputbox.Text;
                        tb.Foreground = Brushes.Blue;
                    }
                }
                this.inputstatus = value;
                switch (value)
                {
                    case INPUTSTATUS_FREE:
                        inputbox.Visibility = Visibility.Hidden;
                        break;
                    default:
                        inputbox.Visibility = Visibility.Visible;
                        inputbox.Text = String.Empty;
                        inputbox.Focus();
                        break;
                }
            }
        }
        private int InputRow
        {
            get { return this.inputrow; }
            set
            {
                this.inputrow = value;
                Grid.SetRow(inputbox, value);
            }
        }
        private int InputColumn
        {
            get { return this.inputcolumn; }
            set
            {
                this.inputcolumn = value;
                Grid.SetColumn(inputbox, value);
            }
        }
        
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
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
                if (inst[colid].Equals(String.Empty))
                    return;
                InputStatus = INPUTSTATUS_INPUT;
                InputRow = rowid;
                InputColumn = colid;
            }
        }

        private void OnInputBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (InputStatus == INPUTSTATUS_INPUT)
            {
                if (e.Key == Key.Enter || e.Key == Key.Space)
                {
                    string text = inputbox.Text;
                    if (InputColumn == 1)
                    {
                        if (!LadderInstViewModelPrototype.CheckInstructionName(text))
                        {
                            MessageBox.Show("输入的指令不存在!");
                            InputStatus = INPUTSTATUS_INPUT;
                            return;
                        }
                        inputtext = text;
                        inputnumber = PLCInstruction.FlagNumber(text);
                        InputStatus = INPUTSTATUS_INPUTALL;
                        InputColumn++;
                    }
                    else
                    {
                        try
                        {
                            insts[InputRow] = insts[InputRow].ReplaceFlag(InputColumn - 1, text).ToOrigin();
                            insts[InputRow].UpdatePrototype();
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.Message);
                            InputStatus = INPUTSTATUS_INPUT;
                            return;
                        }
                        InputStatus = INPUTSTATUS_FREE;
                    }
                }
            }
            else if (InputStatus == INPUTSTATUS_INPUTALL)
            {
                if (e.Key == Key.Enter || e.Key == Key.Space)
                {
                    string text = inputbox.Text;
                    inputtext += String.Format(" {0:s}", text);
                    if (InputColumn - 1 == inputnumber)
                    {
                        try
                        {
                            PLCOriginInst _inst = new PLCOriginInst(inputtext);
                            _inst.ProtoType = insts[InputRow].ProtoType;
                            insts[InputRow] = _inst;
                            _inst.UpdatePrototype();
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.Message);
                            InputStatus = INPUTSTATUS_INPUT;
                            InputColumn = 1;
                            return;
                        }
                        InputStatus = INPUTSTATUS_FREE;
                    }
                    else
                    {
                        InputStatus = INPUTSTATUS_INPUTALL;
                        InputColumn++;
                    }
                }
            }
        }
        
        #endregion

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
        
        #endregion
    }
}
