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
using SamSoarII.Utility;
using SamSoarII.UserInterface;
using SamSoarII.PLCDevice;

namespace SamSoarII.LadderInstViewModel
{
    /// <summary>
    /// VerticalLineViewModel.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class VerticalLineViewModel : BaseViewModel
    {
        public override ElementType Type
        {
            get
            {
                return ElementType.VLine;
            }
        }

        private int _x;
        private int _y;
        private bool _isCommentMode;
        private bool _isMonitorMode;
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
        public int CountLevel { get; set; }
        public override BaseModel Model
        {
            get
            {
                throw new NotImplementedException();
            }
            protected set
            {
                throw new NotImplementedException();
            }
        }
        public override string InstructionName { get { return "VLine"; } }

        public VerticalLineViewModel()
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
                Canvas.SetTop(this, _y * 500 + 100);
            }
            else
            {
                Canvas.SetTop(this, _y * 300 + 100);
            }
        }

        private void UpdateLeftProperty()
        {
            Canvas.SetLeft(this, _x * 300 + 280);
        }

        public override IPropertyDialog PreparePropertyDialog()
        {
            return null;
        }

        public override BaseViewModel Clone()
        {
            return new VerticalLineViewModel();
        }

        public static int CatalogID { get { return 101; } }

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

        public override void UpdateCommentContent()
        {
            // nothing to do
        }
        public override string ToString()
        {
            return string.Format("Has no Mapped Instructions");
        }
    }
}
