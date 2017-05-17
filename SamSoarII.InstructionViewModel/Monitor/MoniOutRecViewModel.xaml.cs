using SamSoarII.LadderInstModel;
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

        public MoniOutRecViewModel(BaseModel bmodel)
        {
            InitializeComponent();
            DataContext = this;
            Model = bmodel;
        }
        
        public override void Update()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("TopTextBlock_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBlock1_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBlock2_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBlock3_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBlock4_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBlock5_Text"));
        }
        
        #region UI

        public string TopTextBlock_Text
        {
            get
            {
                return Inst;
            }
        }

        public string MiddleTextBlock1_Text
        {
            get
            {
                return Model.ParaCount > 0 && _values[0] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(0).ValueString,
                        _values[0].Value)
                    : String.Empty;
            }
        }
        
        public string MiddleTextBlock2_Text
        {
            get
            {
                return Model.ParaCount > 1 && _values[1] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(1).ValueString,
                        _values[1].Value)
                    : String.Empty;
            }
        }

        public string MiddleTextBlock3_Text
        {
            get
            {
                return Model.ParaCount > 2 && _values[2] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(2).ValueString,
                        _values[2].Value)
                    : String.Empty;
            }
        }

        public string MiddleTextBlock4_Text
        {
            get
            {
                return Model.ParaCount > 3 && _values[3] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(3).ValueString,
                        _values[3].Value)
                    : String.Empty;
            }
        }
        
        public string MiddleTextBlock5_Text
        {
            get
            {
                return Model.ParaCount > 4 && _values[4] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(4).ValueString,
                        _values[4].Value)
                    : String.Empty;
            }
        }

        protected override void OnValueChanged(object sender, RoutedEventArgs e)
        {
            base.OnValueChanged(sender, e);
            for (int i = 0; i < Model.ParaCount; i++)
            {
                if (sender == _values[i])
                {
                    switch (i)
                    {
                        case 0:
                            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBlock1_Text"));
                            break;
                        case 1:
                            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBlock2_Text"));
                            break;
                        case 2:
                            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBlock3_Text"));
                            break;
                        case 3:
                            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBlock4_Text"));
                            break;
                        case 4:
                            PropertyChanged(this, new PropertyChangedEventArgs("MiddleTextBlock5_Text"));
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
