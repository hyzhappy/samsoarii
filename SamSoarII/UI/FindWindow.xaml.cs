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
    /// FindWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FindWindow : UserControl, INotifyPropertyChanged
    {
        #region Numbers

        private InteractionFacade parent;
        
        private ObservableCollection<FindElement> items
            = new ObservableCollection<FindElement>();
        public IEnumerable<FindElement> Items
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

        public FindWindow
        (
            InteractionFacade _parent
        )
        {
            InitializeComponent();
            parent = _parent;
            Mode = MODE_CURRENT;
            DataContext = this;
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
                            break;
                        if (!args[i + 1].Equals("*")
                         && !args[i + 1].Equals(bvmodel.Model.GetPara(i).ValueString))
                        {
                            check = false;
                            break;
                        }
                    }
                    if (check)
                    {
                        items.Add(new FindElement(bvmodel, ldvmodel, lnvmodel));
                    }
                }
            }
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
            if (TB_Input.Background != Brushes.LightGreen) return;
            if (e.Key != Key.Enter) return;
            Find();
            TB_Input.Background = Brushes.White;
        }
        #endregion
    }

    public class FindElement : INotifyPropertyChanged
    {
        #region Numbers
        private BaseViewModel bvmodel;
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
        public string Diagram
        {
            get { return ldvmodel.ProgramName; }
        }
        private LadderNetworkViewModel lnvmodel;
        public string Network
        {
            get { return String.Format("{0:d}", lnvmodel.NetworkNumber); }
        }
        #endregion

        public FindElement
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
