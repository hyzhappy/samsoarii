using SamSoarII.AppMain.LadderCommand;
using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// ReplaceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ReplaceWindow : UserControl, INotifyPropertyChanged
    {
        #region Numbers

        private InteractionFacade parent;

        private LadderCommand.CommandManager _cmdmanager
            = new LadderCommand.CommandManager();

        private ObservableCollection<ReplaceElement> items
            = new ObservableCollection<ReplaceElement>();
        public IEnumerable<ReplaceElement> Items
        {
            get
            {
                return this.items;
            }
            set
            {
                items.Clear();
                foreach (ReplaceElement item in value)
                {
                    items.Add(item);
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Items"));
            }
        }
        
        public const int MODE_CURRENT = 0x00;
        public const int MODE_ALL = 0x01;
        public const int MODE_MULTIPLY = 0x02;
        private int mode;
        public int Mode
        {
            get { return this.mode; }
            set
            {
                this.mode = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Mode"));
                Find();
            }
        }

        private ReplaceFormat RF_Input { get; set; } = new ReplaceFormat();

        private ReplaceFormat RF_Change { get; set; } = new ReplaceFormat();
        
        #endregion

        public ReplaceWindow(InteractionFacade _parent)
        {
            InitializeComponent();
            DataContext = this;
            parent = _parent;
            Mode = MODE_CURRENT;
            TB_Input.Background = Brushes.Red;
            TB_Change.Background = Brushes.Red;
        }
        
        private void Find()
        {
            string text = TB_Input.Text;
            string[] args = text.Split(' ');

            items.Clear();
            switch (Mode)
            {
                case MODE_CURRENT:
                    ITabItem currenttab = parent.MainTabControl.CurrentTab;
                    if (currenttab is MainTabDiagramItem)
                    {
                        MainTabDiagramItem mtditem = (MainTabDiagramItem)currenttab;
                        LadderDiagramViewModel ldvmodel = mtditem.LDVM_ladder;
                        Find(ldvmodel, args);
                    }
                    if (currenttab is LadderDiagramViewModel)
                    {
                        Find((LadderDiagramViewModel)currenttab, args);
                    }
                    break;
                case MODE_ALL:
                    ProjectModel pmodel = parent.ProjectModel;
                    Find(pmodel.MainRoutine, args);
                    foreach (LadderDiagramViewModel _ldvmodel in pmodel.SubRoutines)
                    {
                        Find(_ldvmodel, args);
                    }
                    break;
            }
            PropertyChanged(this, new PropertyChangedEventArgs("Items"));
        }

        private void Find(LadderDiagramViewModel ldvmodel, string[] args)
        {
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                foreach (BaseViewModel bvmodel in lnvmodel.GetElements())
                {
                    if (bvmodel is HorizontalLineViewModel
                     || bvmodel is VerticalLineViewModel)
                        continue;
                    BaseModel bmodel = bvmodel.Model;
                    string input = bvmodel.InstructionName;
                    for (int i = 0; i < bmodel.ParaCount; i++)
                    {
                        input += " " + bmodel.GetPara(i).ValueString;
                    }
                    if (RF_Input.Match(input))
                    {
                        items.Add(new ReplaceElement(bvmodel, ldvmodel, lnvmodel));
                    }
                }
            }
        }

        private void Replace(bool showdialog = true)
        {
            int success = 0;
            int error = 0;
            string errormsg = String.Empty;

            NetworkReplaceElementsCommandGroup commandall =
                new NetworkReplaceElementsCommandGroup(
                    this, Items.ToArray());

            foreach (ReplaceElement rele in DG_List.SelectedItems)
            {
                BaseViewModel bvmodel = rele.BVModel;
                //BaseModel bmodel = bvmodel.Model;
                LadderNetworkViewModel lnvmodel = rele.LNVModel;
                LadderDiagramViewModel ldvmodel = rele.LDVModel;
                ldvmodel.IsModify = true;
                int x = bvmodel.X;
                int y = bvmodel.Y;
                
                NetworkReplaceElementsCommand command = null;
                NetworkReplaceElementsCommand_ForReplaceWindow commandrw = null;
                try
                {
                    command = RF_Change.Replace(
                        RF_Input,
                        rele.Detail, x, y,
                        ldvmodel, lnvmodel);
                    commandrw = new NetworkReplaceElementsCommand_ForReplaceWindow(
                        lnvmodel, rele, command);
                    commandall.Add(command);
                    commandall.Add(commandrw);
                    success++;
                }
                catch (ValueParseException exce2)
                {
                    error++;
                    errormsg += String.Format("在{0:s}的网络{1:d}的坐标({2:d},{3:d})处发生错误：{4:s}\r\n",
                        ldvmodel.ProgramName, lnvmodel.NetworkNumber, x, y, exce2.Message);
                }
                catch (LadderDiagramViewModel.InstructionExecption exce3)
                {
                    error++;
                    errormsg += String.Format("在{0:s}的网络{1:d}的坐标({2:d},{3:d})处发生错误：{4:s}\r\n",
                        ldvmodel.ProgramName, lnvmodel.NetworkNumber, x, y, exce3.Message);
                }
            }

            _cmdmanager.Execute(commandall);

            if (showdialog || error > 0)
            {
                ReplaceReportWindow report = new ReplaceReportWindow();
                report.TB_Subtitle.Text = String.Format("总共进行了{0:d}次替换，{1:d}次成功，{2:d}次错误。"
                    , success + error, success, error);
                report.TB_Message.Text = errormsg;
                report.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                report.ShowDialog();
            }
            //Find();
        }
        
        #region Event Handler

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void TB_Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            RF_Input.Text = TB_Input.Text;
            switch (RF_Input.Type)
            {
                case ReplaceFormat.TYPE_INVALID:
                    TB_Input.Background = Brushes.Red;
                    break;
                default:
                    TB_Input.Background = Brushes.LightGreen;
                    break;
            }
        }

        private void TB_Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (TB_Input.Background == Brushes.Red) return;
            if (e.Key != Key.Enter) return;
            Find();
            TB_Input.Background = Brushes.White;
        }

        private void TB_Change_TextChanged(object sender, TextChangedEventArgs e)
        {
            RF_Change.Text = TB_Change.Text;
            switch (RF_Change.Type)
            {
                case ReplaceFormat.TYPE_REGISTER:
                case ReplaceFormat.TYPE_LADDER:
                    if (RF_Input.Type == RF_Change.Type)
                        TB_Change.Background = Brushes.LightGreen;
                    else
                        TB_Change.Background = Brushes.Red;
                    break;
                default:
                    TB_Change.Background = Brushes.Red;
                    break;
            }
        }

        private void TB_Change_KeyDown(object sender, KeyEventArgs e)
        {
            if (TB_Input.Background != Brushes.White) return;
            if (TB_Change.Background == Brushes.Red) return;
            if (e.Key != Key.Enter) return;
            TB_Change.IsEnabled = false;
            Replace();
            TB_Change.Background = Brushes.White;
            TB_Change.IsEnabled = true;
        }
        
        private void DataGridCell_KeyDown(object sender, KeyEventArgs e)
        {
            if (TB_Input.Background != Brushes.White) return;
            if (TB_Change.Background == Brushes.Red) return;
            if (e.Key != Key.Enter) return;
            TB_Change.IsEnabled = false;
            Replace(false);
            TB_Change.Background = Brushes.White;
            TB_Change.IsEnabled = true;
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TB_Input.Background != Brushes.White) return;
            if (TB_Change.Background == Brushes.Red) return;
            TB_Change.IsEnabled = false;
            Replace(false);
            TB_Change.Background = Brushes.White;
            TB_Change.IsEnabled = true;
        }

        private void DG_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DG_List.SelectedIndex < 0) return;
            ReplaceElement fele = items[DG_List.SelectedIndex];
            BaseViewModel bvmodel = fele.BVModel;
            int x = bvmodel.X;
            int y = bvmodel.Y;
            string diagram = fele.Diagram;
            int network = int.Parse(fele.Network);
            NavigateToNetworkEventArgs _e = new NavigateToNetworkEventArgs(network, diagram, x, y);
            parent.NavigateToNetwork(_e);
        }

        private void UndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _cmdmanager.CanUndo;
        }

        private void UndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _cmdmanager.Undo();
        }

        private void RedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _cmdmanager.CanRedo;
        }

        private void RedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _cmdmanager.Redo();
        }
        
        private void OnConfigClick(object sender, RoutedEventArgs e)
        {
            G_Main.Visibility = Visibility.Hidden;
            G_Config.Visibility = Visibility.Visible;
        }
        
        private void BC_Ensure_Click(object sender, RoutedEventArgs e)
        {
            G_Main.Visibility = Visibility.Visible;
            G_Config.Visibility = Visibility.Hidden;
        }

        private void BC_Cancel_Click(object sender, RoutedEventArgs e)
        {
            G_Main.Visibility = Visibility.Visible;
            G_Config.Visibility = Visibility.Hidden;
        }

        #endregion
        
    }

    public class ReplaceFormat
    {
        public const int TYPE_INVALID = 0x00;
        public const int TYPE_LADDER = 0x01;
        public const int TYPE_REGISTER = 0x02;
        public int Type { get; private set; }

        public const int ARG_INVAILD = 0x00;
        public const int ARG_INSTRUCTION = 0x01;
        public const int ARG_REGISTER = 0x02;
        public const int ARG_ANYONE = 0x03;
        public const int ARG_ANYSUFFIX = 0x04;
        public struct ReplaceFormatArg
        {
            public int Type;
            public string Text;
            public string Base;
            public int Low;
            public int High;
            public string Offset;
            public int OLow;
            public int OHigh;
        }
        
        private ReplaceFormatArg[] args = new ReplaceFormatArg[0];
        public int ArgsCount { get { return args.Length; } }
        public ReplaceFormatArg GetArgs(int id) { return args[id]; }
        
        private static Regex VRegex = new Regex(@"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)([0-9]+)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex ARegex = new Regex(@"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)\[([0-9]+)\.\.([0-9]+)\]$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex VVRegex = new Regex(@"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)([0-9]+)(V|Z)([0-9]+)$", 
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex AVRegex = new Regex(@"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)\[([0-9]+)\.\.([0-9]+)\](V|Z)([0-9]+)$", 
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex VARegex = new Regex(@"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)([0-9]+)(V|Z)\[([0-9]+)\.\.([0-9]+)\]$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex AARegex = new Regex(@"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)\[([0-9]+)\.\.([0-9]+)\](V|Z)\[([0-9]+)\.\.([0-9]+)\]$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private string text;
        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;
                if (text.Equals(String.Empty))
                {
                    this.args = new ReplaceFormatArg[0];
                    Type = TYPE_INVALID;
                    return;
                }
                string[] sargs = text.Split(' ');
                this.args = new ReplaceFormatArg[sargs.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    args[i].Text = sargs[i];
                    args[i].Base = String.Empty;
                    args[i].Low = args[i].High = 0;
                    args[i].Offset = String.Empty;
                    args[i].OLow = args[i].OHigh = 0;
                    switch (sargs[i])
                    {
                        case "*":
                            args[i].Type = ARG_ANYONE;
                            break;
                        case ".":
                            args[i].Type = ARG_ANYSUFFIX;
                            break;
                        default:
                            args[i].Type = ARG_REGISTER;
                            Match m1 = VVRegex.Match(sargs[i]);
                            Match m2 = AVRegex.Match(sargs[i]);
                            Match m3 = VARegex.Match(sargs[i]);
                            Match m4 = AARegex.Match(sargs[i]);
                            Match m5 = VRegex.Match(sargs[i]);
                            Match m6 = ARegex.Match(sargs[i]);
                            if (m1.Success)
                            {
                                args[i].Base = m1.Groups[1].Value;
                                args[i].Low = int.Parse(m1.Groups[2].Value);
                                args[i].High = args[i].Low;
                                args[i].Offset = m1.Groups[3].Value;
                                args[i].OLow = int.Parse(m1.Groups[4].Value);
                                args[i].OHigh = args[i].OLow;
                            }
                            else if (m2.Success)
                            {
                                args[i].Base = m2.Groups[1].Value;
                                args[i].Low = int.Parse(m2.Groups[2].Value);
                                args[i].High = int.Parse(m2.Groups[3].Value);
                                args[i].Offset = m2.Groups[4].Value;
                                args[i].OLow = int.Parse(m2.Groups[5].Value);
                                args[i].OHigh = args[i].OLow;
                            }
                            else if (m3.Success)
                            {
                                args[i].Base = m3.Groups[1].Value;
                                args[i].Low = int.Parse(m3.Groups[2].Value);
                                args[i].High = args[i].Low;
                                args[i].Offset = m3.Groups[3].Value;
                                args[i].OLow = int.Parse(m3.Groups[4].Value);
                                args[i].OHigh = int.Parse(m3.Groups[5].Value);
                            }
                            else if (m4.Success)
                            {
                                args[i].Base = m4.Groups[1].Value;
                                args[i].Low = int.Parse(m4.Groups[2].Value);
                                args[i].High = int.Parse(m4.Groups[3].Value);
                                args[i].Offset = m4.Groups[4].Value;
                                args[i].OLow = int.Parse(m4.Groups[5].Value);
                                args[i].OHigh = int.Parse(m4.Groups[6].Value);
                            }
                            else if (m5.Success)
                            {
                                args[i].Base = m5.Groups[1].Value;
                                args[i].Low = int.Parse(m5.Groups[2].Value);
                                args[i].High = args[i].Low;
                            }
                            else if (m6.Success)
                            {
                                args[i].Base = m6.Groups[1].Value;
                                args[i].Low = int.Parse(m6.Groups[2].Value);
                                args[i].High = int.Parse(m6.Groups[3].Value);
                            }
                            else if (LadderInstViewModelPrototype.CheckInstructionName(sargs[i]))
                            {
                                args[i].Type = ARG_INSTRUCTION;
                            }
                            else
                            {
                                args[i].Type = ARG_INVAILD;
                            }
                            break;
                    }
                    switch (args[i].Type)
                    {
                        case ARG_INVAILD:
                            Type = TYPE_INVALID;
                            return;
                        case ARG_INSTRUCTION:
                            if (i > 0)
                            {
                                Type = TYPE_INVALID;
                                return;
                            }
                            break;
                        case ARG_REGISTER:
                            if (i == 0 && sargs.Length > 1)
                            {
                                Type = TYPE_INVALID;
                                return;
                            }
                            if (i == 0)
                            {
                                Type = TYPE_REGISTER;
                                return;
                            }
                            break;
                        case ARG_ANYONE:
                            break;
                        case ARG_ANYSUFFIX:
                            if (i < sargs.Length - 1)
                            {
                                Type = TYPE_INVALID;
                                return;
                            }
                            break;
                        default:
                            Type = TYPE_INVALID;
                            return;
                    }
                }
                Type = TYPE_LADDER;
            }
        }

        public ReplaceFormat()
        {
            Text = String.Empty;
        }

        public ReplaceFormat(string _text)
        {
            Text = _text;
        }

        public bool Match(string input)
        {
            ReplaceFormat iformat = new ReplaceFormat(input);
            if (iformat.Type != TYPE_LADDER)
                return false;
            switch (Type)
            {
                case TYPE_INVALID:
                    return false;
                case TYPE_LADDER:
                    for (int i = 0; i < ArgsCount; i++)
                    {
                        if (GetArgs(i).Type == ARG_ANYSUFFIX)
                            return true;
                        if (iformat.ArgsCount <= i)
                            return false;
                        if (!Match(GetArgs(i), iformat.GetArgs(i)))
                            return false;
                    }
                    return true;
                case TYPE_REGISTER:
                    if (iformat.ArgsCount < 2)
                    {
                        return false;
                    }
                    for (int i = 1; i < iformat.ArgsCount; i++)
                    {
                        if (!Match(GetArgs(0), iformat.GetArgs(i)))
                            return false;
                    }
                    return true;
                default:
                    return false;
            }            
        }

        private bool Match(ReplaceFormatArg arg1, ReplaceFormatArg arg2)
        {
            switch (arg1.Type)
            {
                case ARG_INVAILD:
                    return false;
                case ARG_INSTRUCTION:
                    return (arg2.Type == arg1.Type && arg2.Text.Equals(arg1.Text));
                case ARG_ANYONE:
                    return true;
                case ARG_ANYSUFFIX:
                    return true;
                case ARG_REGISTER:
                    if (!arg1.Base.Equals(arg2.Base))
                        return false;
                    if (arg2.High < arg1.Low)
                        return false;
                    if (arg2.Low > arg1.High)
                        return false;
                    if (!arg1.Offset.Equals(String.Empty))
                    {
                        if (!arg1.Offset.Equals(arg2.Offset))
                            return false;
                        if (arg2.OHigh < arg1.OLow)
                            return false;
                        if (arg2.OLow > arg1.OHigh)
                            return false;
                    }
                    return true;
                default:
                    return false;
            }
        }

        public NetworkReplaceElementsCommand Replace
        (
            ReplaceFormat prototype,
            string input, int x, int y, 
            LadderDiagramViewModel ldvmodel,
            LadderNetworkViewModel lnvmodel
        )
        {
            ReplaceFormat iformat = new ReplaceFormat(input);
            string output = String.Empty;
            for (int i = 0; i < iformat.ArgsCount; i++)
            {
                if (prototype.Type == TYPE_REGISTER)
                { 
                    if (iformat.GetArgs(i).Type == ARG_REGISTER)
                    {
                        if (!Match(prototype.GetArgs(0), iformat.GetArgs(i)))
                        {
                            output += iformat.GetArgs(i).Text + " ";
                        }
                        else
                        {
                            bool isunique = false;
                            isunique = (prototype.GetArgs(0).Low == prototype.GetArgs(0).High);
                            isunique |= (GetArgs(0).Low == GetArgs(0).High);
                            int baseid = GetArgs(0).Low;
                            if (!isunique)
                                baseid += iformat.GetArgs(i).Low - prototype.GetArgs(0).Low;
                            output += String.Format("{0:s}{1:d}",
                                    GetArgs(0).Base, baseid);
                            if (!GetArgs(0).Offset.Equals(String.Empty))
                                output += String.Format("{0:s}{1:d}",
                                    GetArgs(0).Offset, GetArgs(0).OLow);
                            output += " ";
                        }
                    }
                    else
                    {
                        output += iformat.GetArgs(i).Text + " ";
                    }
                    continue;
                }
                if (i >= ArgsCount)
                {
                    output += iformat.GetArgs(i).Text + " ";
                    continue;
                }
                switch (GetArgs(i).Type)
                {
                    case ARG_INSTRUCTION:
                        output += GetArgs(i).Text + " ";
                        break;
                    case ARG_REGISTER:
                        bool isunique = false;
                        isunique |= (i >= prototype.ArgsCount);
                        if (!isunique)
                        {
                            isunique = (prototype.GetArgs(i).Low == prototype.GetArgs(i).High);
                            isunique |= (GetArgs(i).Low == GetArgs(i).High);
                        }
                        int baseid = GetArgs(i).Low;
                        if (!isunique)
                            baseid += iformat.GetArgs(i).Low - prototype.GetArgs(i).Low;
                        output += String.Format("{0:s}{1:d}",
                                GetArgs(i).Base, baseid);
                        if (!GetArgs(i).Offset.Equals(String.Empty))
                            output += String.Format("{0:s}{1:d}",
                                GetArgs(i).Offset, GetArgs(i).OLow);
                        output += " ";
                        break;
                    default:
                        output += iformat.GetArgs(i).Text + " ";
                        break;
                }
            }
            NetworkReplaceElementsCommand command = null;
            ldvmodel.RegisterInstructionInput(
               output, x, y, lnvmodel, ref command);
            return command;
        }
    }

    public class ReplaceElement : INotifyPropertyChanged
    {
        #region Numbers
        private BaseViewModel bvmodel;
        public BaseViewModel BVModel
        {
            get
            {
                return this.bvmodel;
            }
            set
            {
                this.bvmodel = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Detail"));
            }
        }
        public string Detail
        {
            get
            {
                string result = bvmodel.InstructionName;
                for (int i = 0; i < bvmodel.Model.ParaCount; i++)
                {
                    result += " " + bvmodel.Model.GetPara(i).ValueString;
                }
                return result;
            }
        }
        private LadderDiagramViewModel ldvmodel;
        public LadderDiagramViewModel LDVModel
        {
            get
            {
                return this.ldvmodel;
            }
        }
        public string Diagram
        {
            get { return ldvmodel.ProgramName; }
        }
        private LadderNetworkViewModel lnvmodel;
        public LadderNetworkViewModel LNVModel
        {
            get
            {
                return this.lnvmodel;
            }
        }
        public string Network
        {
            get { return String.Format("{0:d}", lnvmodel.NetworkNumber); }
        }

        private bool isselected;
        public bool IsSelected
        {
            get { return this.isselected; }
            set
            {
                this.isselected = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        #endregion

        public ReplaceElement
        (
            BaseViewModel _bvmodel,
            LadderDiagramViewModel _ldvmodel,
            LadderNetworkViewModel _lnvmodel
        )
        {
            bvmodel = _bvmodel;
            ldvmodel = _ldvmodel;
            lnvmodel = _lnvmodel;
            PropertyChanged(this, new PropertyChangedEventArgs("Detail"));
            PropertyChanged(this, new PropertyChangedEventArgs("Diagram"));
            PropertyChanged(this, new PropertyChangedEventArgs("Network"));
        }

        #region Event Handler
        public virtual event PropertyChangedEventHandler PropertyChanged = delegate { };
        #endregion
    }
    
    public class NetworkReplaceElementsCommand_ForReplaceWindow : IUndoableCommand
    {
        private LadderNetworkViewModel lnvmodel;
        public  ReplaceElement Element { get; private set; }
        private BaseViewModel BVM_old;
        private BaseViewModel BVM_new;
        
        public NetworkReplaceElementsCommand_ForReplaceWindow
        (
            LadderNetworkViewModel _lnvmodel,
            ReplaceElement _element,
            BaseViewModel _BVM_old,
            BaseViewModel _BVM_new
        )
        {
            lnvmodel = _lnvmodel;
            Element = _element;
            BVM_old = _BVM_old;
            BVM_new = _BVM_new;
        }


        public NetworkReplaceElementsCommand_ForReplaceWindow
        (
            LadderNetworkViewModel _lnvmodel,
            ReplaceElement _element,
            NetworkReplaceElementsCommand _command
        )
        {
            lnvmodel = _lnvmodel;
            Element = _element;
            BVM_old = _command.PopOldElement();
            BVM_new = _command.PopNewElement();
        }

        public void Execute()
        {
            lnvmodel.ReplaceElement(BVM_new);
            Element.BVModel = BVM_new;
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            lnvmodel.ReplaceElement(BVM_old);
            Element.BVModel = BVM_old;
        }
        
    }

    public class NetworkReplaceElementsCommandGroup : IUndoableCommand
    {
        private List<IUndoableCommand> items
            = new List<IUndoableCommand>();
        private ReplaceWindow parent;
        private IEnumerable<ReplaceElement> eles_all;
        private List<ReplaceElement> eles_replaced
            = new List<ReplaceElement>();

        public NetworkReplaceElementsCommandGroup
        (
            ReplaceWindow _parent,
            IEnumerable<ReplaceElement> _eles_all
        )
        {
            parent = _parent;
            eles_all = _eles_all;
        }

        public void Add(NetworkReplaceElementsCommand command)
        {
            items.Add(command);
        }

        public void Add(NetworkReplaceElementsCommand_ForReplaceWindow command)
        {
            items.Add(command);
            eles_replaced.Add(command.Element);
        }

        public void Execute()
        {
            parent.Items = eles_all;/*
            foreach (ReplaceElement ele in eles_all)
            {
                ele.IsSelected = false;
            }
            foreach (ReplaceElement ele in eles_replaced)
            {
                ele.IsSelected = true;
            }*/
            for (int i = 0; i < items.Count(); i++)
            {
                items[i].Execute();
            }
            
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            parent.Items = eles_all;/*
            foreach (ReplaceElement ele in eles_all)
            {
                ele.IsSelected = false;
            }
            foreach (ReplaceElement ele in eles_replaced)
            {
                ele.IsSelected = true;
            }*/
            for (int i = items.Count() - 1; i >= 0; i--)
            {
                items[i].Undo();
            }
        }
    }
}
