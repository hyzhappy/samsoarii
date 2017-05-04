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
        }

        public const int MODE_CURRENT = 0x00;
        public const int MODE_ALL = 0x01;
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

        #endregion

        public ReplaceWindow(InteractionFacade _parent)
        {
            InitializeComponent();
            DataContext = this;
            parent = _parent;
            Mode = MODE_CURRENT;
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
                    if (!(currenttab is MainTabDiagramItem))
                        break;
                    MainTabDiagramItem mtditem = (MainTabDiagramItem)currenttab;
                    LadderDiagramViewModel ldvmodel = (LadderDiagramViewModel)(mtditem.LAP_Ladder.Children.First().Content);
                    Find(ldvmodel, args);
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
            bool check = false;
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                foreach (BaseViewModel bvmodel in lnvmodel.GetElements())
                {
                    if (bvmodel is HorizontalLineViewModel
                     || bvmodel is VerticalLineViewModel)
                        continue;
                    if (args.Length > 0 && !args[0].Equals("*")
                     && !args[0].Equals(bvmodel.InstructionName))
                        continue;
                    check = true;
                    for (int i = 0; i < bvmodel.Model.ParaCount; i++)
                    {
                        if (args.Length <= i + 1)
                        {
                            if (!args.Last().Equals("."))
                            {
                                check = false;
                            }
                            break;
                        }
                        if (!args[i + 1].Equals("*")
                         && !args[i + 1].Equals(".")
                         && !args[i + 1].Equals(bvmodel.Model.GetPara(i).ValueString))
                        {
                            check = false;
                            break;
                        }
                    }
                    if (args.Length - 1 > bvmodel.Model.ParaCount
                     && !args[bvmodel.Model.ParaCount].Equals("."))
                    {
                        check = false;
                    }
                    if (check)
                    {
                        items.Add(new ReplaceElement(bvmodel, ldvmodel, lnvmodel));
                    }
                }
            }
        }

        private void Replace()
        {
            int success = 0;
            int error = 0;
            string errormsg = String.Empty;
            NetworkReplaceElementsCommandGroup commandall = new NetworkReplaceElementsCommandGroup();

            foreach (ReplaceElement rele in Items)
            {
                BaseViewModel bvmodel = rele.BVModel;
                //BaseModel bmodel = bvmodel.Model;
                LadderNetworkViewModel lnvmodel = rele.LNVModel;
                LadderDiagramViewModel ldvmodel = rele.LDVModel;
                int x = bvmodel.X;
                int y = bvmodel.Y;

                string text_old = rele.Detail;
                string text_new = TB_Change.Text;
                string[] args_old = text_old.Split(' ');
                string[] args_new = text_new.Split(' ');
                string text_fin = String.Empty;
                for (int i = 0; i < args_old.Length; i++)
                {
                    if (i >= args_new.Length 
                     || args_new[i].Equals("*"))
                    {
                        text_fin += args_old[i] + " ";
                    }
                    else
                    {
                        text_fin += args_new[i] + " ";
                    }
                }
                NetworkReplaceElementsCommand command = null;
                try
                {
                    ldvmodel.RegisterInstructionInput(
                        text_fin, x, y, lnvmodel, ref command);
                    commandall += command;
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
         
            ReplaceReportWindow report = new ReplaceReportWindow();
            report.TB_Subtitle.Text = String.Format("总共进行了{0:d}次替换，{1:d}次成功，{2:d}次错误。"
                ,success + error, success, error);
            report.TB_Message.Text = errormsg;
            report.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            report.ShowDialog();

            Find();
        }

        #region Event Handler

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void TB_Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = TB_Input.Text;
            string[] args = text.Split(' ');
            bool check = false;

            TB_Input.Background = Brushes.LightGreen;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("*"))
                    continue;
                if (args[i].Equals("."))
                    continue;
                if (i == 0)
                {
                    check = true;
                }
                else
                {
                    check = ValueParser.CheckValueString(args[i], new Regex[]
                    {
                        ValueParser.VarRegex,
                        ValueParser.VerifyIntKHValueRegex,
                        ValueParser.VerifyFloatKValueRegex
                    });
                }
                if (!check)
                {
                    TB_Input.Background = Brushes.Red;
                    break;
                }
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
            string text = TB_Change.Text;
            string[] args = text.Split(' ');
            bool check = false;

            TB_Change.Background = Brushes.LightGreen;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("*"))
                    continue;
                if (i == 0)
                {
                    check = true;
                }
                else
                {
                    check = ValueParser.CheckValueString(args[i], new Regex[]
                    {
                        ValueParser.VarRegex,
                        ValueParser.VerifyIntKHValueRegex,
                        ValueParser.VerifyFloatKValueRegex
                    });
                }
                if (!check)
                {
                    TB_Change.Background = Brushes.Red;
                    break;
                }
            }
        }

        private void TB_Change_KeyDown(object sender, KeyEventArgs e)
        {
            if (TB_Input.Background != Brushes.White) return;
            if (TB_Change.Background == Brushes.Red) return;
            if (e.Key != Key.Enter) return;
            Replace();
            TB_Change.Background = Brushes.White;
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
            e.Handled = _cmdmanager.CanUndo;
        }

        private void UndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _cmdmanager.Undo();
        }
        
        private void RedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = _cmdmanager.CanRedo;
        }

        private void RedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _cmdmanager.Redo();
        }

        #endregion

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
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        #endregion
    }
}
