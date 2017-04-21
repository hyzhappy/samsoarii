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

using SamSoarII.Simulation.Core.VariableModel;

/// <summary>
/// ClassName : SimuViewInputModel
/// Version : 1.0
/// Date : 2017/3/14
/// Author : morenan
/// </summary>
/// <remarks>
/// 显示【输入元件】的UI控件
/// </remarks>

namespace SamSoarII.Simulation.Shell.ViewModel
{
    /// <summary>
    /// SimuViewBaseInputModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimuViewSpecialModel : SimuViewBaseModel
    {
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="parent">仿真总模型</param>
        public SimuViewSpecialModel(SimulateModel parent) : base(parent)
        {
            // 初始化子控件
            InitializeComponent();
        }
        /// <summary>
        /// 辨别一行指令语句，设置这个控件
        /// </summary>
        /// <param name="text">指令语句，为inst arg1 arg2 ... 的格式</param>
        public override void Setup(string text)
        {
            // 空格分隔，获得指令名称和参数集合
            string[] texts = text.Split(' ');
            Inst = texts[0];
            switch (Inst)
            {
                case "INV":
                    break;
                case "MEP":
                    break;
                case "MEF":
                    break;
            }
            // 更新画面就能显示出来
            Update();
        }
        /// <summary>
        /// 更新画面，显示新的参数当前值
        /// </summary>
        public override void Update()
        {
            Line line = null;
            // 画个 -[/]- 表示【取反】
            switch (Inst)
            {
                case "INV":
                    line = new Line();
                    line.X1 = 75;
                    line.X2 = 25;
                    line.Y1 = 0;
                    line.Y2 = 100;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);
                    break;
                default:
                    break;
            }
            // 画个-[↑]-表示【上升沿】
            switch (Inst)
            {
                case "MEP":
                    line = new Line();
                    line.X1 = 50;
                    line.X2 = 70;
                    line.Y1 = 0;
                    line.Y2 = 20;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);

                    line = new Line();
                    line.X1 = 50;
                    line.X2 = 30;
                    line.Y1 = 0;
                    line.Y2 = 20;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);

                    line = new Line();
                    line.X1 = 50;
                    line.X2 = 50;
                    line.Y1 = 0;
                    line.Y2 = 100;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);
                    break;
                default:
                    break;
            }
            // 画个-[↓]-表示【下降沿】
            switch (Inst)
            {
                case "MEF":
                    line = new Line();
                    line.X1 = 50;
                    line.X2 = 50;
                    line.Y1 = 0;
                    line.Y2 = 100;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);

                    line = new Line();
                    line.X1 = 50;
                    line.X2 = 70;
                    line.Y1 = 100;
                    line.Y2 = 80;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);

                    line = new Line();
                    line.X1 = 50;
                    line.X2 = 30;
                    line.Y1 = 100;
                    line.Y2 = 80;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);
                    break;
                default:
                    break;
            }
        }

        private void FillGreen()
        {
            Rectangle rect = new Rectangle();
            rect.Width = CenterCanvas.Width;
            rect.Height = CenterCanvas.Height;
            rect.Fill = Brushes.Green;
            CenterCanvas.Children.Add(rect);
        }

        private void FillRed()
        {
            Rectangle rect = new Rectangle();
            rect.Width = CenterCanvas.Width;
            rect.Height = CenterCanvas.Height;
            rect.Fill = Brushes.Green;
            CenterCanvas.Children.Add(rect);
        }

    }
}
