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
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.PLCDevice;
using SamSoarII.Utility;

namespace SamSoarII.LadderInstViewModel
{
    /// <summary>
    /// SpecialBaseViewModel.xaml 的交互逻辑
    /// </summary>
    public abstract partial class SpecialBaseViewModel : BaseViewModel
    {
        public override ElementType Type
        {
            get
            {
                return ElementType.Special; 
            }
        }
        private int _x;
        private int _y;
        private bool _isCommentMode;
        private IntPoint intPos = new IntPoint();
        public override IntPoint IntPos
        {
            get => intPos;
            set => intPos = value;
        }
        public override int X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
                intPos.X = value;
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
                intPos.Y = value;
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
                UpdateTopProperty();
                UpdateHeightProperty();
            }
        }
        

        public SpecialBaseViewModel()
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
                if (BPRect != null)
                    Canvas.SetTop(BPRect, _y * 500);
            }
            else
            {
                Canvas.SetTop(this, _y * 300);
                if (BPRect != null)
                    Canvas.SetTop(BPRect, _y * 300);
            }
        }

        private void UpdateLeftProperty()
        {
            Canvas.SetLeft(this, _x * 300);
        }
        public override bool Assert()
        {
            return NextElements.All(x => { return x.Type == ElementType.Special || x.Type == ElementType.Input; }) & NextElements.Count > 0;
        }

        public override void ParseValue(IList<string> valueStrings)
        {
            
        }

        public override IEnumerable<string> GetValueString()
        {
            return new List<string>();
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {

        }
        public override void UpdateCommentContent()
        {
            // nothing to do
        }

        #region Monitor

        public override bool IsMonitorMode { get; set; }

        #endregion
    }
}
