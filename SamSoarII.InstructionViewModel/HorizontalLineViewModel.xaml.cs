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
using SamSoarII.InstructionModel;
using SamSoarII.UserInterface;
namespace SamSoarII.InstructionViewModel
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
