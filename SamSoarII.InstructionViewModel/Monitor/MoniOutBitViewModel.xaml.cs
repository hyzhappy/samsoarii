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
    /// MoniOutBitViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class MoniOutBitViewModel : MoniBaseViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public MoniOutBitViewModel()
        {
            InitializeComponent();
            DataContext = this;
        }
        
        public MoniOutBitViewModel(BaseModel bmodel)
        {
            InitializeComponent();
            DataContext = this;
            Model = bmodel;
        }
        

        public override void Update()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("CenterCanva_Brush"));
            PropertyChanged(this, new PropertyChangedEventArgs("ValueTextBlock_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("CountTextBlock_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("CenterTextBlock_Text"));
        }
        
        #region UI

        static string[] BIT_0_SHOWS = { "0", "OFF", "FALSE" };
        static string[] BIT_1_SHOWS = { "1", "ON",  "TRUE" };
        
        public Brush CenterCanva_Brush
        {
            get
            {
                if (!IsRunning) return Brushes.Transparent;
                bool value = false;
                try
                {
                    if (_values[0] == null)
                        throw new FormatException("Lack of arguments.");
                    if (!BIT_0_SHOWS.Contains(_values[0].Value)
                     && !BIT_1_SHOWS.Contains(_values[0].Value))
                        throw new FormatException("value0 is not a BIT.");
                    value = BIT_1_SHOWS.Contains(_values[0].Value);
                }
                catch (FormatException)
                {
                    return Brushes.Red;
                }
                return value ? Brushes.Green : Brushes.Transparent;
            }
        }
        
        public string ValueTextBlock_Text
        {
            get
            {
                return Model.ParaCount > 0 && _values[0] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(0).ValueString,
                        IsRunning ? _values[0].Value : "???")
                    : String.Empty;
            }
        }

        public string CountTextBlock_Text
        {
            get
            {
                return Model.ParaCount > 1 && _values[1] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(1).ValueString,
                        IsRunning ? _values[1].Value : "???")
                    : String.Empty;
            }
        }

        public string CenterTextBlock_Text
        {
            get
            {
                switch (Inst)
                {
                    case "RST": return "R";
                    case "RSTIM": return "RI";
                    case "SET": return "S";
                    case "SETIM": return "SI";
                    default: return String.Empty;
                }
            }
        }

        protected override void OnValueChanged(object sender, RoutedEventArgs e)
        {
            base.OnValueChanged(sender, e);
            PropertyChanged(this, new PropertyChangedEventArgs("CenterCanva_Brush"));
            PropertyChanged(this, new PropertyChangedEventArgs("ValueTextBlock_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("CountTextBlock_Text"));

        }

        protected override void OnStart(object sender, RoutedEventArgs e)
        {
            base.OnStart(sender, e);
            PropertyChanged(this, new PropertyChangedEventArgs("CenterCanva_Brush"));
            PropertyChanged(this, new PropertyChangedEventArgs("ValueTextBlock_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("CountTextBlock_Text"));
        }

        protected override void OnAbort(object sender, RoutedEventArgs e)
        {
            base.OnAbort(sender, e);
            PropertyChanged(this, new PropertyChangedEventArgs("CenterCanva_Brush"));
            PropertyChanged(this, new PropertyChangedEventArgs("ValueTextBlock_Text"));
            PropertyChanged(this, new PropertyChangedEventArgs("CountTextBlock_Text"));
        }

        #endregion
    }
}
