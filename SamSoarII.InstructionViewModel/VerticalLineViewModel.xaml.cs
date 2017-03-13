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
                Canvas.SetLeft(this, _x * 300 + 280);
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
                Canvas.SetTop(this, _y * 300 + 100);
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
        }

        public IntPoint? GetLinkedPoint(int x, int y)
        {
            if(this._x == x && this._y == y)
            {
                return new IntPoint() { X = x, Y = y + 1};
            }
            else
            {
                if(this._x == x && this._y == y + 1)
                {
                    return new IntPoint() { X = x, Y = y };
                }
                return null;
            }
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
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

        public override void ParseValue(List<string> valueStrings)
        {
            
        }

        public override IEnumerable<string> GetValueString()
        {
            return new List<string>();
        }
    }
}
