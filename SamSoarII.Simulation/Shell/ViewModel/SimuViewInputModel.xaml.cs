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
    public partial class SimuViewInputModel : SimuViewBaseModel
    {
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="parent">仿真总模型</param>
        public SimuViewInputModel(SimulateModel parent) : base(parent)
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
                // (rW, rW)
                case "LDWEQ":
                case "LDWNE":
                case "LDWGE":
                case "LDWLE":
                case "LDWG":
                case "LDWL":
                    this._args1 = _parent.GetVariableUnit(texts[1], "WORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "WORD");
                    break;
                // (rD, rD)
                case "LDDEQ":
                case "LDDNE":
                case "LDDGE":
                case "LDDLE":
                case "LDDG":
                case "LDDL":
                    this._args1 = _parent.GetVariableUnit(texts[1], "DWORD");
                    this._args2 = _parent.GetVariableUnit(texts[2], "DWORD");
                    break;
                // (rF, rF)
                case "LDFEQ":
                case "LDFNE":
                case "LDFGE":
                case "LDFLE":
                case "LDFG":
                case "LDFL":
                    this._args1 = _parent.GetVariableUnit(texts[1], "FLOAT");
                    this._args2 = _parent.GetVariableUnit(texts[2], "FLOAT");
                    break;
                default:
                    // 输入元件的唯一参数为位参数
                    this._args1 = _parent.GetVariableUnit(texts[1], "BIT");
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
            // 显示位参数的名称和值
            ValueTextBlock.Text = _args1.ToString();
            // 开始画画
            Line line = null;
            //Rectangle rect = null;
            CenterCanvas.Children.Clear();
            int i1, i2;
            float f1, f2;
            switch (Inst)
            {
                case "LDWEQ":
                    CenterTextBlock.Text = "W==";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    i1 = (int)(_args1.Value);
                    i2 = (int)(_args2.Value);
                    if (i1 == i2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDWNE":
                    CenterTextBlock.Text = "W<>";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    i1 = (int)(_args1.Value);
                    i2 = (int)(_args2.Value);
                    if (i1 != i2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDWGE":
                    CenterTextBlock.Text = "W>=";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    i1 = (int)(_args1.Value);
                    i2 = (int)(_args2.Value);
                    if (i1 >= i2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDWLE":
                    CenterTextBlock.Text = "W<=";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    i1 = (int)(_args1.Value);
                    i2 = (int)(_args2.Value);
                    if (i1 <= i2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDWG":
                    CenterTextBlock.Text = "W>";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    i1 = (int)(_args1.Value);
                    i2 = (int)(_args2.Value);
                    if (i1 > i2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDWL":
                    CenterTextBlock.Text = "W<";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    i1 = (int)(_args1.Value);
                    i2 = (int)(_args2.Value);
                    if (i1 < i2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDDEQ":
                    CenterTextBlock.Text = "D==";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    i1 = (int)(_args1.Value);
                    i2 = (int)(_args2.Value);
                    if (i1 == i2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDDNE":
                    CenterTextBlock.Text = "D<>";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    i1 = (int)(_args1.Value);
                    i2 = (int)(_args2.Value);
                    if (i1 != i2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDDGE":
                    CenterTextBlock.Text = "D>=";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    i1 = (int)(_args1.Value);
                    i2 = (int)(_args2.Value);
                    if (i1 >= i2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDDLE":
                    CenterTextBlock.Text = "D<=";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    i1 = (int)(_args1.Value);
                    i2 = (int)(_args2.Value);
                    if (i1 <= i2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDDG":
                    CenterTextBlock.Text = "D>";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    i1 = (int)(_args1.Value);
                    i2 = (int)(_args2.Value);
                    if (i1 > i2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDDL":
                    CenterTextBlock.Text = "D<";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    i1 = (int)(_args1.Value);
                    i2 = (int)(_args2.Value);
                    if (i1 < i2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDFEQ":
                    CenterTextBlock.Text = "F==";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    f1 = (float)(_args1.Value);
                    f2 = (float)(_args2.Value);
                    if (f1 == f2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDFNE":
                    CenterTextBlock.Text = "F<>";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    f1 = (float)(_args1.Value);
                    f2 = (float)(_args2.Value);
                    if (f1 != f2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDFGE":
                    CenterTextBlock.Text = "F>=";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    f1 = (float)(_args1.Value);
                    f2 = (float)(_args2.Value);
                    if (f1 >= f2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDFLE":
                    CenterTextBlock.Text = "F<=";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    f1 = (float)(_args1.Value);
                    f2 = (float)(_args2.Value);
                    if (f1 <= f2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDFG":
                    CenterTextBlock.Text = "F>";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    f1 = (float)(_args1.Value);
                    f2 = (float)(_args2.Value);
                    if (f1 > f2)
                    {
                        FillGreen();
                    }
                    break;
                case "LDFL":
                    CenterTextBlock.Text = "F<";
                    ValueTextBlock.Text = _args1.ToString();
                    Value2TextBlock.Text = _args2.ToString();
                    f1 = (float)(_args1.Value);
                    f2 = (float)(_args2.Value);
                    if (f1 < f2)
                    {
                        FillGreen();
                    }
                    break;
                default:
                    // 涂个-[绿]-表示当前值为1
                    if ((int)(_args1.Value) == 1)
                    {
                        FillGreen();
                    }
                    else
                    // 涂个-[红]-表示当前值非法
                    if ((int)(_args1.Value) != 0)
                    {
                        FillRed();
                    }
                    break;
            }
            // 画个-[/]-表示【取反】
            switch (Inst)
            {
                case "LDI":
                case "LDIIM":
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
            // 画个-[|]-表示【立即】
            switch (Inst)
            {
                case "LDIM":
                case "LDIIM":
                    line = new Line();
                    line.X1 = 50;
                    line.X2 = 50;
                    line.Y1 = 0;
                    line.Y2 = 100;
                    line.StrokeThickness = 4;
                    line.Stroke = Brushes.Black;
                    CenterCanvas.Children.Add(line);
                    break;
            }
            // 画个-[↑]-表示【上升沿】
            switch (Inst)
            {
                case "LDP":
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
                case "LDF":
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
            CenterCanvas.Children.Add(CenterTextBlock);

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
