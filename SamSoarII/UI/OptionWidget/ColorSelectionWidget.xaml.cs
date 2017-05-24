using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// ColorSelectionWidget.xaml 的交互逻辑
    /// </summary>
    public partial class ColorSelectionWidget : UserControl
    {
        private ColorData current;
        public ColorData Current
        {
            get
            {
                return this.current;
            }
            set
            {
                this.current = value;
                BD_Background.Background = current.Background;
                BD_Foreground.Background = current.Foreground;
                switch (current.Name)
                {
                    case "函数块背景":
                        Demo.ShowFuncBlock();
                        break;
                    case "PLC指令":
                        Demo.ShowInstruction();
                        break;
                    default:
                        Demo.ShowDiagram();
                        break;
                }
            }
        }

        public ColorSelectionWidget()
        {
            InitializeComponent();
            Initialize();
        }
        
        private void Initialize()
        {
            DemoColorManager.GetComment().Setup(
                ColorManager.GetComment());
            DemoColorManager.GetDiagramTitle().Setup(
                ColorManager.GetDiagramTitle());
            DemoColorManager.GetFuncScreen().Setup(
                ColorManager.GetFuncScreen());
            DemoColorManager.GetInst().Setup(
                ColorManager.GetInst());
            DemoColorManager.GetInstScreen().Setup(
                ColorManager.GetInstScreen());
            DemoColorManager.GetLadder().Setup(
                ColorManager.GetLadder());
            DemoColorManager.GetLadderScreen().Setup(
                ColorManager.GetLadderScreen());
            DemoColorManager.GetNetworkTitle().Setup(
                ColorManager.GetNetworkTitle());
            CB_Range.Items.Add(new ColorDataItem(
                DemoColorManager.GetComment()));
            CB_Range.Items.Add(new ColorDataItem(
                DemoColorManager.GetDiagramTitle()));
            CB_Range.Items.Add(new ColorDataItem(
                DemoColorManager.GetFuncScreen()));
            CB_Range.Items.Add(new ColorDataItem(
                DemoColorManager.GetInst()));
            CB_Range.Items.Add(new ColorDataItem(
                DemoColorManager.GetInstScreen()));
            CB_Range.Items.Add(new ColorDataItem(
                DemoColorManager.GetLadder()));
            CB_Range.Items.Add(new ColorDataItem(
                DemoColorManager.GetLadderScreen()));
            CB_Range.Items.Add(new ColorDataItem(
                DemoColorManager.GetNetworkTitle()));
            CB_Range.SelectedIndex = 0;
            //ColorDataItem cditem = (ColorDataItem)(CB_Range.SelectedItem);
            //Current = cditem.Data;
        }

        private void CB_Range_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == CB_Range)
            {
                ColorDataItem cditem = (ColorDataItem)(CB_Range.SelectedItem);
                Current = cditem.Data;
            }
        }
    }

    public class DemoColorManager
    {
        static private ColorData LadderScreen
            = new ColorData("梯形图背景");
        static private ColorData Ladder
            = new ColorData("元件");
        static private ColorData Comment
            = new ColorData("元件注释");
        static private ColorData DiagramTitle
            = new ColorData("梯形图标题");
        static private ColorData NetworkTitle
            = new ColorData("网络标题");
        static private ColorData FuncScreen
            = new ColorData("函数块背景");
        static private ColorData InstScreen
            = new ColorData("PLC指令背景");
        static private ColorData Inst
            = new ColorData("PLC指令单元格");

        static public ColorData GetLadderScreen()
        {
            return LadderScreen;
        }
        static public ColorData GetLadder()
        {
            return Ladder;
        }
        static public ColorData GetComment()
        {
            return Comment;
        }
        static public ColorData GetDiagramTitle()
        {
            return DiagramTitle;
        }
        static public ColorData GetNetworkTitle()
        {
            return NetworkTitle;
        }
        static public ColorData GetFuncScreen()
        {
            return FuncScreen;
        }
        static public ColorData GetInstScreen()
        {
            return InstScreen;
        }
        static public ColorData GetInst()
        {
            return Inst;
        }
    }

    public class ColorDataItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ColorData data;
        public ColorData Data
        {
            get { return this.data; }
            private set
            {
                this.data = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Data"));
            }
        }

        public override string ToString()
        {
            return Data.Name;
        }

        public ColorDataItem(ColorData _data)
        {
            Data = _data;
        }
    }

}
