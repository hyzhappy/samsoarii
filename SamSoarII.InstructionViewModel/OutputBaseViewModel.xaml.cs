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
    /// OutputBaseViewModel.xaml 的交互逻辑
    /// </summary>
    public abstract partial class OutputBaseViewModel : BaseViewModel
    {
        public override ElementType Type
        {
            get
            {
                return ElementType.Output;
            }
        }
        public OutputBaseViewModel()
        {
            InitializeComponent();
        }
        public override bool Assert()
        {
            return NextElemnets.All(x => { return (x.Type == ElementType.Input) | (x.Type == ElementType.Special); }) & NextElemnets.Count > 0;
        }
    }
}
