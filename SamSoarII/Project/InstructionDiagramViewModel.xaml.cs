using SamSoarII.AppMain.UI;
using SamSoarII.Extend.FuncBlockModel;
using SamSoarII.Extend.Utility;
using SamSoarII.LadderInstModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// InstructionDiagramViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class InstructionDiagramViewModel : UserControl
    {
        protected LadderDiagramViewModel ldvmodel;

        protected List<InstructionNetworkViewModel> invmodels;

        protected Dictionary<LadderNetworkViewModel, InstructionNetworkViewModel> invmodeldict;

        protected InstructionNetworkViewModel invmodelcursor;

        protected static Dictionary<string, PLCOriginInst> cvdict
            = new Dictionary<string, PLCOriginInst>();

        protected static Dictionary<string, PLCOriginInst> tvdict
            = new Dictionary<string, PLCOriginInst>();

        protected static Dictionary<string, PLCOriginInst> lbdict
            = new Dictionary<string, PLCOriginInst>();

        protected static Dictionary<string, PLCOriginInst> outdict
            = new Dictionary<string, PLCOriginInst>();

        public InstructionDiagramViewModel()
        {
            InitializeComponent();
            ldvmodel = null;
            invmodels = new List<InstructionNetworkViewModel>();
            invmodeldict = new Dictionary<LadderNetworkViewModel, InstructionNetworkViewModel>();
        }
        
        public void Setup(LadderDiagramViewModel _ldvmodel)
        {
            if (ldvmodel != _ldvmodel)
            {
                invmodeldict.Clear();
                if (ldvmodel != null)
                {
                    ldvmodel.SelectionRect.NetworkParentChanged -= OnLadderCursorChanged;
                    ldvmodel.SelectionRect.XChanged -= OnLadderCursorChanged;
                    ldvmodel.SelectionRect.YChanged -= OnLadderCursorChanged;
                }
                if (_ldvmodel != null)
                {
                    _ldvmodel.SelectionRect.NetworkParentChanged += OnLadderCursorChanged;
                    _ldvmodel.SelectionRect.XChanged += OnLadderCursorChanged;
                    _ldvmodel.SelectionRect.YChanged += OnLadderCursorChanged;
                }
            }
            this.ldvmodel = _ldvmodel;
            invmodels.Clear();
            MainStack.Children.Clear();
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                InstructionNetworkViewModel invmodel = null;
                if (invmodeldict.ContainsKey(lnvmodel))
                {
                    invmodel = invmodeldict[lnvmodel];
                }
                else
                {
                    invmodel = new InstructionNetworkViewModel();
                    invmodel.CursorChanged += OnNetworkCursorChanged;
                    invmodel.CursorEdit += OnNetworkCursorEdit;
                    lnvmodel.INVModel = invmodel;
                    //invmodel.Setup(lnvmodel);
                    invmodeldict.Add(lnvmodel, invmodel);
                }
                MainStack.Children.Add(invmodel);
                invmodels.Add(invmodel);
            }
        }
       
        public void Setup(LadderNetworkViewModel lnvmodel)
        {
            invmodels.Clear();
            InstructionNetworkViewModel invmodel = new InstructionNetworkViewModel();
            lnvmodel.INVModel = invmodel;
            //invmodel.Setup(lnvmodel);
            MainStack.Children.Add(invmodel);
            invmodels.Add(invmodel);
        }
        
        static public void CheckInitialize()
        {
            cvdict.Clear();
            tvdict.Clear();
            lbdict.Clear();
            outdict.Clear();
        }
        
        public IEnumerable<ErrorReportElement> Check()
        {
            int forcount = 0;
            int stkcount = 0;
            int outcount = 0;
            Match match = null;

            foreach (InstructionNetworkViewModel invmodel in invmodels)
            {
                stkcount = 0;
                outcount = 0;
                if (invmodel.Status == InstructionNetworkViewModel.STATUS_OPEN
                 || invmodel.Status == InstructionNetworkViewModel.STATUS_SHORT
                 || invmodel.Status == InstructionNetworkViewModel.STATUS_FUSION)
                {
                    continue;
                }
                foreach (PLCOriginInst inst in invmodel.Insts)
                {
                    if (inst.ProtoType is OutputBaseViewModel
                     || inst.ProtoType is OutputRectBaseViewModel)
                    {
                        outcount++;
                    }
                }
                foreach (PLCOriginInst inst in invmodel.Insts)
                {
                    inst.Status = PLCOriginInst.STATUS_ACCEPT;
                    inst.Message = String.Empty;
                    for (int i = 1; i <= 5; i++)
                    {
                        if (inst[i].Equals("???"))
                        {
                            inst.Status = PLCOriginInst.STATUS_ERROR;
                            inst.Message = String.Format("缺少参数。");
                            break;
                        }
                    }
                    if (inst.Status == PLCOriginInst.STATUS_ERROR)
                        continue;
                    PLCOriginInst _inst = null;
                    if (inst.Type.StartsWith("LD"))
                    {
                        stkcount++;
                    }
                    switch (inst.Type)
                    {
                        case "ANDB":
                        case "ORB":
                            stkcount--;
                            break;
                        case "CTU":
                        case "CTD":
                        case "CTUD":
                            match = Regex.Match(inst[1], @"^(CV)([0-9]+)$");
                            if (!match.Success)
                            {
                                inst.Status = PLCOriginInst.STATUS_WARNING;
                                inst.Message = String.Format("变址计数器可能会重合，请谨慎使用。");
                                break;
                            }
                            if (!cvdict.ContainsKey(inst[1]))
                            {
                                cvdict.Add(inst[1], inst);
                            }
                            else
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("计数器{0:s}已被使用！", inst[1]);
                                _inst = cvdict[inst[1]];
                                if (_inst.Status == PLCOriginInst.STATUS_ACCEPT)
                                {
                                    _inst.Status = PLCOriginInst.STATUS_WARNING;
                                    _inst.Message = String.Format("计数器{0:s}被尝试在别的地方使用。", inst[1]);
                                }
                            }
                            break;
                        case "TON":
                        case "TONR":
                        case "TOF":
                            match = Regex.Match(inst[1], @"^(TV)([0-9]+)$");
                            if (!match.Success)
                            {
                                inst.Status = PLCOriginInst.STATUS_WARNING;
                                inst.Message = String.Format("变址计时器可能会重合，请谨慎使用。");
                                break;
                            }
                            if (!tvdict.ContainsKey(inst[1]))
                            {
                                tvdict.Add(inst[1], inst);
                            }
                            else
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("计时器{0:s}已被使用！", inst[1]);
                                _inst = tvdict[inst[1]];
                                if (_inst.Status == PLCOriginInst.STATUS_ACCEPT)
                                {
                                    _inst.Status = PLCOriginInst.STATUS_WARNING;
                                    _inst.Message = String.Format("计时器{0:s}被尝试在别的地方使用。", inst[1]);
                                }
                            }
                            break;
                        case "FOR":
                            if (outcount > 1)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("FOR指令不能和别的输出指令在同一个网络下使用。");
                            }
                            forcount++;
                            break;
                        case "NEXT":
                            if (stkcount > 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("LBL指令不能带条件。");
                            }
                            if (outcount > 1)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("NEXT指令不能和别的输出指令在同一个网络下使用。");
                            }
                            if (forcount == 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("未找到对应的FOR指令。");
                            }
                            else
                            {
                                forcount--;
                            }
                            break;
                        case "LBL":
                            if (stkcount > 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("LBL指令不能带条件。");
                            }
                            if (outcount > 1)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("LBL指令不能和别的输出指令在同一个网络下使用。");
                            }
                            if (!lbdict.ContainsKey(inst[1]))
                            {
                                lbdict.Add(inst[1], inst);
                            }
                            else
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("跳转标记{0:s}已被使用！", inst[1]);
                            }
                            break;
                        case "OUT":
                        case "OUTIM":
                            match = Regex.Match(inst[1], @"^([YMSTC])([0-9]+)$");
                            if (!match.Success)
                            {
                                inst.Status = PLCOriginInst.STATUS_WARNING;
                                inst.Message = String.Format("变址线圈输出可能会重合，请谨慎使用。");
                                break;
                            }
                            if (!outdict.ContainsKey(inst[1]))
                            {
                                outdict.Add(inst[1], inst);
                            }
                            else
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("{0:s}多线圈输出错误！", inst[1]);
                                _inst = outdict[inst[1]];
                                _inst.Status = PLCOriginInst.STATUS_WARNING;
                                _inst.Message = String.Format("{0:s}被尝试在别的地方线圈输入。", inst[1]);
                            }
                            break;
                        case "SET":
                        case "SETIM":
                        case "RST":
                        case "RSTIM":
                            match = Regex.Match(inst[1], @"^([YMSTC])([0-9]+)$");
                            if (!match.Success)
                            {
                                inst.Status = PLCOriginInst.STATUS_WARNING;
                                inst.Message = String.Format("变址线圈输出可能会重合，请谨慎使用。");
                                break;
                            }
                            int count = int.Parse(((PLCInstruction)inst)[2]);
                            string bname = match.Groups[1].Value;
                            int index = int.Parse(match.Groups[2].Value);
                            string suffix = match.Groups[3].Value;
                            for (int i = index; i < index + count; i++)
                            {
                                string name = String.Format("{0:s}{1:d}{2:s}",
                                    bname, i, suffix);
                                if (outdict.ContainsKey(name))
                                {
                                    inst.Status = PLCOriginInst.STATUS_ERROR;
                                    inst.Message = String.Format("多线圈输出错误！");
                                    _inst = outdict[inst[1]];
                                    _inst.Status = PLCOriginInst.STATUS_WARNING;
                                    _inst.Message = String.Format("{0:s}被尝试在别的地方线圈输入。", inst[1]);
                                    break;
                                }
                            }
                            break;
                        case "CALLM":
                            {
                                IEnumerable<FuncModel> fit = 
                                    ldvmodel.ProjectModel.Funcs.Where(
                                        (FuncModel _fmodel) => { return _fmodel.Name.Equals(inst[1]); });
                                if (fit.Count() <= 0)
                                {
                                    inst.Status = PLCOriginInst.STATUS_ERROR;
                                    inst.Message = String.Format("找不到用户函数{0:s}。", inst[1]);
                                    break;
                                }
                                FuncModel fmodel = fit.First();
                                if (!fmodel.CanCALLM())
                                {
                                    inst.Status = PLCOriginInst.STATUS_ERROR;
                                    inst.Message = String.Format("用户函数{0:s}不能被CALLM指令调用。", inst[1]);
                                    break;
                                }
                                for (int i = 0; i < fmodel.ArgCount; i++)
                                {
                                    try
                                    {
                                        switch (fmodel.GetArgType(i))
                                        {
                                            case "BIT*":
                                                ValueParser.ParseBitValue(inst[i + 2]);
                                                break;
                                            case "WORD*":
                                                ValueParser.ParseWordValue(inst[i + 2]);
                                                break;
                                            case "DWORD*":
                                                ValueParser.ParseDoubleWordValue(inst[i + 2]);
                                                break;
                                            case "FLOAT*":
                                                ValueParser.ParseFloatValue(inst[i + 2]);
                                                break;
                                        }
                                    }
                                    catch (ValueParseException)
                                    {
                                        inst.Status = PLCOriginInst.STATUS_ERROR;
                                        if (inst[i + 2].Equals(String.Empty))
                                        {
                                            inst.Message = String.Format("缺少参数。");
                                        }
                                        else
                                        {
                                            inst.Message = String.Format("第{0:d}个参数{1:s}不合法。", i + 1, inst[i + 2]);
                                        }
                                        break;
                                    }
                                }
                            }
                            break;
                        case "MBUS":
                            {
                                IEnumerable<ModbusTableModel> fit =
                                    ldvmodel.ProjectModel.MTVModel.Models.Where(
                                        (ModbusTableModel _mtmodel) =>
                                        { return _mtmodel.Name.Equals(inst[2]); });
                                if (fit.Count() <= 0)
                                {
                                    inst.Status = PLCOriginInst.STATUS_ERROR;
                                    inst.Message = String.Format("找不到MODBUS表格{0:s}。", inst[2]);
                                }
                                ModbusTableModel mtmodel = fit.First();
                                if (!mtmodel.IsVaild)
                                {
                                    inst.Status = PLCOriginInst.STATUS_ERROR;
                                    inst.Message = String.Format("MUDBUS表格{0:s}不合法。", inst[2]);
                                }
                            }
                            break;
                        case "CALL":
                        case "ATCH":
                            {
                                int id = inst.Type.Equals("CALL") ? 1 : 2;
                                IEnumerable<LadderDiagramViewModel> fit =
                                    ldvmodel.ProjectModel.SubRoutines.Where(
                                        (LadderDiagramViewModel _ldvmodel) =>
                                        {
                                            return _ldvmodel.ProgramName.Equals(inst[id]);
                                        });
                                if (fit.Count() <= 0)
                                {
                                    inst.Status = PLCOriginInst.STATUS_ERROR;
                                    inst.Message = String.Format("找不到子程序{0:s}", inst[id]);
                                }
                                if (inst.Type.Equals("ATCH"))
                                {
                                    fit.First().IsInterruptCalled = true;
                                }
                            }
                            break;
                    }
                    switch (inst.Type)
                    {
                        case "NEXT":
                        case "LBL":
                            break;
                        default:
                            if (stkcount == 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("缺少栈顶值作为条件。");
                            }
                            break;
                    }
                }
            }
            
            for (int i = invmodels.Count() - 1; i >= 0; i--)
            {
                InstructionNetworkViewModel _invmodel = invmodels[i];
                foreach (PLCOriginInst inst in _invmodel.Insts)
                {
                    if (inst.Status == PLCOriginInst.STATUS_ERROR)
                        continue;
                    switch (inst.Type)
                    {
                        case "FOR":
                            if (forcount > 0)
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("找不到对应的结束符NEXT。");
                                forcount--;
                            }
                            break;
                        case "JMP":
                            if (!lbdict.ContainsKey(inst[1]))
                            {
                                inst.Status = PLCOriginInst.STATUS_ERROR;
                                inst.Message = String.Format("找不到对应的跳转标记{0:s}", inst[1]);
                            }
                            break;
                    }
                }
            }

            List<ErrorReportElement> result = new List<ErrorReportElement>();
            foreach (InstructionNetworkViewModel invmodel in invmodels)
            {
                invmodel.UpdateCheck();
                foreach (PLCOriginInst inst in invmodel.Insts)
                {
                    if (inst.Status == PLCOriginInst.STATUS_ERROR)
                        result.Add(new ErrorReportElement(inst, invmodel.LNVModel, ldvmodel));
                }
            }
            foreach (InstructionNetworkViewModel invmodel in invmodels)
            {
                foreach (PLCOriginInst inst in invmodel.Insts)
                {
                    if (inst.Status == PLCOriginInst.STATUS_WARNING)
                        result.Add(new ErrorReportElement(inst, invmodel.LNVModel, ldvmodel));
                }
            }
            return result;
        }

        public IEnumerable<ErrorReportElement> CheckForInterrrupt()
        {
            List<ErrorReportElement> result =
                new List<ErrorReportElement>();
            if (!ldvmodel.IsInterruptCalled)
                return result;
            foreach (InstructionNetworkViewModel invmodel in invmodels)
            {
                foreach (PLCOriginInst inst in invmodel.Insts)
                {
                    if (inst.Status == PLCOriginInst.STATUS_ERROR)
                        continue;
                    switch (inst.Type)
                    {
                        case "OUT":
                        case "OUTIM":
                            inst.Status = PLCOriginInst.STATUS_ERROR;
                            inst.Message = "禁止在中断子程序中使用输出线圈。";
                            result.Add(new ErrorReportElement(inst, invmodel.LNVModel, ldvmodel));
                            break;
                        case "LDP":
                        case "LDF":
                        case "MEP":
                        case "MEF":
                            inst.Status = PLCOriginInst.STATUS_ERROR;
                            inst.Message = "禁止在中断子程序中使用边沿条件判断。";
                            result.Add(new ErrorReportElement(inst, invmodel.LNVModel, ldvmodel));
                            break;
                        case "TON":
                        case "TONR":
                        case "TOF":
                            inst.Status = PLCOriginInst.STATUS_ERROR;
                            inst.Message = "禁止在中断子程序中使用计时器。";
                            result.Add(new ErrorReportElement(inst, invmodel.LNVModel, ldvmodel));
                            break;
                        case "CTU":
                        case "CTD":
                        case "CTUD":
                            inst.Status = PLCOriginInst.STATUS_ERROR;
                            inst.Message = "禁止在中断子程序中使用计数器。";
                            result.Add(new ErrorReportElement(inst, invmodel.LNVModel, ldvmodel));
                            break;
                        case "ALTP":
                            inst.Status = PLCOriginInst.STATUS_ERROR;
                            inst.Message = "禁止在中断子程序中使用该指令。";
                            result.Add(new ErrorReportElement(inst, invmodel.LNVModel, ldvmodel));
                            break;
                    }
                }
            }
            return result;
        }

        #region Event Handler

        #region Cursor

        private const int CURSOR_SOURCE_NULL = 0x00;
        private const int CURSOR_SOURCE_THIS = 0x01;
        private const int CURSOR_SOURCE_LADDER = 0x02;
        private int cursorsource = CURSOR_SOURCE_NULL;

        private void OnLadderCursorChanged(object sender, RoutedEventArgs e)
        {
            if (cursorsource != CURSOR_SOURCE_NULL)
            {
                return;
            }
            cursorsource = CURSOR_SOURCE_LADDER;
            if (sender is SelectRect)
            {
                SelectRect sr = (SelectRect)(sender);
                if (sr != null)
                {
                    BaseViewModel bvmodel = sr.CurrentElement;
                    if (bvmodel != null)
                    {
                        double currenty = 0;
                        foreach (InstructionNetworkViewModel invmodel in invmodels)
                        {
                            if (invmodel.CatchCursor(bvmodel))
                            {
                                double cursory = 20 + Grid.GetRow(invmodel.Cursor) * 20;
                                Scroll.ScrollToVerticalOffset(Math.Max(0, currenty + cursory - Scroll.ViewportHeight / 2));
                            }
                            currenty += invmodel.ActualHeight;
                        }
                    }
                }
            }
            cursorsource = CURSOR_SOURCE_NULL;
        }

        public event RoutedEventHandler CursorChanged = delegate { };
        private void OnNetworkCursorChanged(object sender, RoutedEventArgs e)
        {
            if (cursorsource != CURSOR_SOURCE_NULL)
            {
                if (cursorsource == CURSOR_SOURCE_LADDER
                 && sender is InstructionNetworkViewModel)
                {
                    double currenty = 0;
                    foreach (InstructionNetworkViewModel invmodel in invmodels)
                    {
                        if (invmodel == sender) break;
                        currenty += invmodel.ActualHeight;
                    }
                    double cursory = 20 + Grid.GetRow(((InstructionNetworkViewModel)(sender)).Cursor) * 20;
                    Scroll.ScrollToVerticalOffset(Math.Max(0, currenty + cursory - Scroll.ViewportHeight / 2));
                }
                return;
            }
            cursorsource = CURSOR_SOURCE_THIS;
            if (sender is InstructionNetworkViewModel)
            {
                if (invmodelcursor != null && invmodelcursor != sender)
                {
                    invmodelcursor.Cursor.Visibility = Visibility.Hidden;
                }
                invmodelcursor = (InstructionNetworkViewModel)(sender);
                CursorChanged(sender, e);
            }
            cursorsource = CURSOR_SOURCE_NULL;
        }

        public event RoutedEventHandler CursorEdit = delegate { };
        private void OnNetworkCursorEdit(object sender, RoutedEventArgs e)
        {
            if (sender is InstructionNetworkViewModel)
            {
                CursorEdit(sender, e);
            }
        }

        #endregion

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up && invmodelcursor != null)
            {
                if (invmodelcursor.CursorUp())
                {
                    CursorChanged(invmodelcursor, new RoutedEventArgs());
                    return;
                }
                int cursorpos = invmodels.IndexOf(invmodelcursor);
                while (cursorpos > 0 && !invmodels[cursorpos - 1].CatchCursorBottom())
                {
                    cursorpos--;
                }
                if (cursorpos > 0)
                {
                    invmodelcursor.Cursor.Visibility = Visibility.Hidden;
                    invmodelcursor = invmodels[cursorpos - 1];
                    CursorChanged(invmodelcursor, new RoutedEventArgs());
                }
            }
            if (e.Key == Key.Down && invmodelcursor != null)
            {
                if (invmodelcursor.CursorDown())
                {
                    CursorChanged(invmodelcursor, new RoutedEventArgs());
                    return;
                }
                int cursorpos = invmodels.IndexOf(invmodelcursor);
                while (cursorpos < invmodels.Count()-1 && !invmodels[cursorpos + 1].CatchCursorTop())
                {
                    cursorpos++;
                }
                if (cursorpos < invmodels.Count()-1)
                {
                    invmodelcursor.Cursor.Visibility = Visibility.Hidden;
                    invmodelcursor = invmodels[cursorpos + 1];
                    CursorChanged(invmodelcursor, new RoutedEventArgs());
                }
            }
        }
        #endregion

    }
}
