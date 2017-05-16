using System;
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
    /// MoniOutRecViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class MoniOutRecViewModel : MoniBaseViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public MoniOutRecViewModel()
        {
            InitializeComponent();
            DataContext = this;
        }

        public MoniOutRecViewModel(string text)
        {
            InitializeComponent();
            DataContext = this;
            Setup(text);
        }

        public override void Setup(string text)
        {
            string[] texts = text.Split(' ');
            Inst = texts[0];
            for (int i = 1; i < texts.Length; i++)
            {
                _labels[i - 1] = texts[i];
            }
            Update();
        }

        public override void Update()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("TopTextBox_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBox1_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBox2_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBox3_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBox4_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBox5_Text"));
        }

        public override void SetValue(int id, string value)
        {
            base.SetValue(id, value);
            //PropertyChanged(this, new PropertyChangedEventArgs("TopTextBox_Text"));
            switch (id)
            {
                case 0: 
                    PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBox1_Text"));
                    break;
                case 1:
                    PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBox2_Text"));
                    break;
                case 2:
                    PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBox3_Text"));
                    break;
                case 3:
                    PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBox4_Text"));
                    break;
                case 4:
                    PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBox5_Text"));
                    break;
            }
        }

        #region UI

        public string TopTextBox_Text
        {
            get
            {
                return Inst;
            }
        }

        public string MiddleTextBox1_Text
        {
            get
            {
                return _labels[0].Length > 0
                    ? String.Format("{0:s} = {1:s}", _labels[0], _values[0])
                    : String.Empty;
            }
        }
        
        public string MiddleTextBox2_Text
        {
            get
            {
                return _labels[1].Length > 0
                    ? String.Format("{0:s} = {1:s}", _labels[1], _values[1])
                    : String.Empty;
            }
        }

        public string MiddleTextBox3_Text
        {
            get
            {
                return _labels[2].Length > 0
                    ? String.Format("{0:s} = {1:s}", _labels[2], _values[2])
                    : String.Empty;
            }
        }

        public string MiddleTextBox4_Text
        {
            get
            {
                return _labels[3].Length > 0
                    ? String.Format("{0:s} = {1:s}", _labels[3], _values[3])
                    : String.Empty;
            }
        }
        
        public string MiddleTextBox5_Text
        {
            get
            {
                return _labels[4].Length > 0
                    ? String.Format("{0:s} = {1:s}", _labels[4], _values[4])
                    : String.Empty;
            }
        }

        #endregion
    }
}
