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
namespace SamSoarII.LadderInstViewModel
{
    /// <summary>
    /// HorizontalLineViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class HorizontalLineViewModel : BaseViewModel
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

        public override int X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
                Canvas.SetLeft(this, X * 300);
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
                Canvas.SetTop(this, Y * 300);
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

        public override string InstructionName { get { return "HLine"; } }

        public HorizontalLineViewModel()
        {
            InitializeComponent();
        }

        public override void ShowPropertyDialog(ElementPropertyDialog dialog)
        {
            
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

        public override void ParseValue(List<string> valueStrings)
        { 

        }

        public override IEnumerable<string> GetValueString()
        {
            return new List<string>();
        }
    }
}
