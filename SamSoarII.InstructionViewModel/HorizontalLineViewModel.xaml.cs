using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using System.ComponentModel;
using SamSoarII.PLCDevice;
using SamSoarII.ValueModel;

namespace SamSoarII.LadderInstViewModel
{
    /// <summary>
    /// HorizontalLineViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class HorizontalLineViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public override ElementType Type
        {
            get
            {
                return ElementType.HLine;
            }
        }

        private int _x;
        private int _y;
        private bool _isCommentMode;
        private bool _isMonitorMode;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public override int X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
                UpdateLeftProperty();
            }
        }

        public override int Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
                UpdateTopProperty();
            }
        }

        public override bool IsCommentMode
        {
            get
            {
                return _isCommentMode;
            }
            set
            {
                _isCommentMode = value;
                UpdateHeightProperty();
                UpdateTopProperty();
            }
        }

        public override bool IsMonitorMode
        {
            get
            {
                return _isMonitorMode;
            }

            set
            {
                _isMonitorMode = value;
            }
        }


        public override BaseModel Model
        {
            get
            {
                throw new InvalidOperationException();
            }
            protected set
            {
                throw new InvalidOperationException();
            }
        }

        public override string InstructionName { get { return "HLine"; } }

        public HorizontalLineViewModel()
        {
            InitializeComponent();
            IsCommentMode = false;
        }

        private void UpdateHeightProperty()
        {
            Height = _isCommentMode ? 500 : 300;
        }

        private void UpdateTopProperty()
        {
            if (_isCommentMode)
            {
                Canvas.SetTop(this, _y * 500);
            }
            else
            {
                Canvas.SetTop(this, _y * 300);
            }
        }

        private void UpdateLeftProperty()
        {
            Canvas.SetLeft(this, _x * 300);
        }

        public override void UpdateCommentContent()
        {
            // nothing to do
        }
        public override IPropertyDialog PreparePropertyDialog()
        {
            return null;
        }

        public override bool Assert()
        {
            return NextElemnets.All( x => { return (x.Type == ElementType.Null) | (x.Type == ElementType.Special) | (x.Type == ElementType.Input); }) & NextElemnets.Count > 0;
        }

        public override BaseViewModel Clone()
        {
            return new HorizontalLineViewModel();
        }

        public static int CatalogID { get { return 100; } }

        public override int GetCatalogID()
        {
            return CatalogID;
        }

        public override void ParseValue(IList<string> valueStrings)
        { 

        }
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {

        }
        public override IEnumerable<string> GetValueString()
        {
            return new List<string>();
        }
        public override string ToString()
        {
            return string.Format("The Mapped Instructions:(Click corresponding item! and Navigate to belonging network)");
        }

        public override IEnumerable<IValueModel> GetValueModels()
        {
            return new List<IValueModel>();
        }
    }
}
