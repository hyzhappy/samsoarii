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

/// <summary>
/// ClassName : SimuViewInputModel
/// Version : 1.0
/// Date : 2017/3/14
/// Author : morenan
/// </summary>
/// <remarks>
/// 显示【位输出元件】的UI控件
/// </remarks>

namespace SamSoarII.Simulation.Shell.ViewModel
{
    /// <summary>
    /// SimuViewBaseOutBitModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimuViewOutBitModel : SimuViewBaseModel
    {
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="parent">仿真总模型</param>
        public SimuViewOutBitModel(SimulateModel parent) : base(parent)
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
            // 位输出元件的第一个参数为位参数
            this._args1 = _parent.GetVariableUnit(texts[1], "BIT");
            // 如果存在第二个参数，表示连续操作的位的数量
            if (texts.Length > 2)
            {
                this._args2 = _parent.GetConstantUnit(texts[2], "WORD");
            }
            // 更新画面就能显示出来
            Update();
        }
        /// <summary>
        /// 更新画面
        /// </summary>
        public override void Update()
        {
            // 显示位参数的名称和值
            ValueTextBlock.Text = _args1.ToString();
            // 显示连续操作的总数
            if (_args2 != null)
            {
                CountTextBlock.Text = _args2.ToString();
            }
            // 开始画画
            //Line line = null;
            Rectangle rect = null;
            CenterCanvas.Children.Clear();
            // 涂个-[绿]-表示当前值为1
            if ((int)(_args1.Value) == 1)
            {
                rect = new Rectangle();
                rect.Width = CenterCanvas.Width;
                rect.Height = CenterCanvas.Height;
                rect.Fill = Brushes.Green;
                CenterCanvas.Children.Add(rect);
            }
            else
            // 涂个-[红]-表示当前值非法
            if ((int)(_args1.Value) != 0)
            {
                rect = new Rectangle();
                rect.Width = CenterCanvas.Width;
                rect.Height = CenterCanvas.Height;
                rect.Fill = Brushes.Red;
                CenterCanvas.Children.Add(rect);
            }

            switch (Inst)
            {
                case "RST": case "RSTIM":
                    CenterTextBlock.Text = "R";
                    break;
                default:
                    break;
            }

            switch (Inst)
            {
                case "SET": case "SETIM":
                    CenterTextBlock.Text = "S";
                    break;
                default:
                    break;
            }

            CenterCanvas.Children.Add(CenterTextBlock);

        }
    }
}
