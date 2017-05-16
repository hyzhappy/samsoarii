﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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


namespace SamSoarII.LadderInstViewModel.Monitor
{
    /// <summary>
    /// MoniInputViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class MoniInputViewModel : MoniBaseViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public MoniInputViewModel()
        {
            InitializeComponent();
            DataContext = this;
        }

        public MoniInputViewModel(string text)
        {
            InitializeComponent();
            DataContext = this;
            Setup(text);
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
            for (int i = 1; i < texts.Length; i++)
            {
                _labels[i - 1] = texts[i];
            }
            Update();
        }
        
        /// <summary>
        /// 更新画面
        /// </summary>
        public override void Update()
        {
            // 显示位参数的名称和值
            ValueTextBlock.Text = _labels[0];
            // 开始画画
            Line line = null;
            //Rectangle rect = null;
            CenterCanvas.Children.Clear();
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
            PropertyChanged(this, new PropertyChangedEventArgs("CenterCanva_Brush"));
            PropertyChanged(this, new PropertyChangedEventArgs("ValueTextBox_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("ValueTextBox2_Text"));
        }

        public override void SetValue(int id, string value)
        {
            base.SetValue(id, value);
            PropertyChanged(this, new PropertyChangedEventArgs("CenterCanva_Brush"));
            PropertyChanged(this, new PropertyChangedEventArgs("ValueTextBox_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("ValueTextBox2_Text"));
        }

        #region UI 

        static string[] BIT_0_SHOWS = { "0", "OFF", "FALSE"};
        static string[] BIT_1_SHOWS = { "1", "ON", "TRUE" };
        
        public Brush CenterCanva_Brush
        {
            get
            {
                bool value = false;
                Int32 w1 = 0, w2 = 0;
                Int64 d1 = 0, d2 = 0;
                double f1 = 0, f2 = 0;
                try
                {
                    switch (Inst)
                    {
                        case "LD":
                        case "LDI":
                        case "LDIM":
                        case "LDIIM":
                        case "LDP":
                        case "LDF":
                            if (!BIT_0_SHOWS.Contains(_values[0])
                             && !BIT_1_SHOWS.Contains(_values[0]))
                                throw new FormatException("value0 is not a BIT.");
                            break;
                    }
                    switch (Inst)
                    {
                        case "LD":
                        case "LDIM":
                            value = BIT_1_SHOWS.Contains(_values[0]);
                            break;
                        case "LDI":
                        case "LDIIM":
                            value = BIT_0_SHOWS.Contains(_values[0]);
                            break;
                        case "LDWEQ":
                        case "LDWNE":
                        case "LDWLE":
                        case "LDWGE":
                        case "LDWL":
                        case "LDWG":
                            w1 = Int32.Parse(_values[0]);
                            w2 = Int32.Parse(_values[1]);
                            break;
                        case "LDDEQ":
                        case "LDDNE":
                        case "LDDLE":
                        case "LDDGE":
                        case "LDDL":
                        case "LDDG":
                            d1 = Int64.Parse(_values[0]);
                            d2 = Int64.Parse(_values[1]);
                            break;
                        case "LDFEQ":
                        case "LDFNE":
                        case "LDFLE":
                        case "LDFGE":
                        case "LDFL":
                        case "LDFG":
                            f1 = double.Parse(_values[0]);
                            f2 = double.Parse(_values[1]);
                            break;
                    }
                    switch (Inst)
                    {
                        case "LDWEQ": value = (w1 == w2); break;
                        case "LDWNE": value = (w1 != w2); break;
                        case "LDWLE": value = (w1 <= w2); break;
                        case "LDWGE": value = (w1 >= w2); break;
                        case "LDWL": value = (w1 < w2); break;
                        case "LDWG": value = (w1 > w2); break;
                        case "LDDEQ": value = (d1 == d2); break;
                        case "LDDNE": value = (d1 != d2); break;
                        case "LDDLE": value = (d1 <= d2); break;
                        case "LDDGE": value = (d1 >= d2); break;
                        case "LDDL": value = (d1 < d2); break;
                        case "LDDG": value = (d1 > d2); break;
                        case "LDFEQ": value = (f1 == f2); break;
                        case "LDFNE": value = (f1 != f2); break;
                        case "LDFLE": value = (f1 <= f2); break;
                        case "LDFGE": value = (f1 >= f2); break;
                        case "LDFL": value = (f1 < f2); break;
                        case "LDFG": value = (f1 > f2); break;
                    }
                }
                catch (FormatException)
                {
                    return Brushes.Red;
                }
                return value ? Brushes.Green : Brushes.Transparent;
            }
        }

        public string ValueTextBox_Text
        {
            get
            {
                return _labels[0].Length > 0
                    ? String.Format("{0:s} = {1:s}", _labels[0], _values[0])
                    : String.Empty;
            }
        }
        
        public string ValueTextBox2_Text
        {
            get
            {
                return _labels[1].Length > 0
                    ? String.Format("{0:s} = {1:s}", _labels[0], _values[0])
                    : String.Empty;
            }
        }

        #endregion

    }
}
