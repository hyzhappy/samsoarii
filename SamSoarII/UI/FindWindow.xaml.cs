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

        private ReplaceFormat RF_Input { get; set; } = new ReplaceFormat();

        #endregion

        public FindWindow
        (
            InteractionFacade _parent
        )
        {
            InitializeComponent();
            DataContext = this;
            parent = _parent;
            Mode = MODE_CURRENT;
            TB_Input.Background = Brushes.Red;
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
                        items.Add(new FindElement(bvmodel, ldvmodel, lnvmodel));
                    }
                }
            }
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

        private void DG_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DG_List.SelectedIndex < 0) return;
            FindElement fele = items[DG_List.SelectedIndex];
            BaseViewModel bvmodel = fele.BVModel;
            int x = bvmodel.X;
            int y = bvmodel.Y;
            string diagram = fele.Diagram;
            int network = int.Parse(fele.Network);
            NavigateToNetworkEventArgs _e = new NavigateToNetworkEventArgs(network, diagram, x, y);
            parent.NavigateToNetwork(_e);
        }

        #endregion
    }

    public class FindFormat : ReplaceFormat
    {

    }

    public class FindElement : ReplaceElement, INotifyPropertyChanged
    {
        public FindElement
        (
            BaseViewModel _bvmodel,
            LadderDiagramViewModel _ldvmodel,
            LadderNetworkViewModel _lnvmodel
        ) : base(_bvmodel, _ldvmodel, _lnvmodel)
        {
        }
    }


}
