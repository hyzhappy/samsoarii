using SamSoarII.Shell.Models;
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
using SamSoarII.Core.Models;
using SamSoarII.Core.Update;

namespace SamSoarII.Shell.Windows.Update
{
    /// <summary>
    /// InformationUnit.xaml 的交互逻辑
    /// </summary>
    public partial class InformationUnit : UserControl,IViewModel
    {
        public InformationUnit()
        {
            InitializeComponent();
        }
        public InformationUnit(Information _core)
        {
            InitializeComponent();
            core = _core;
        }
        private Information core;
        public Information Core
        {
            get { return core; }
            set { core = value; }
        }
        IModel IViewModel.Core
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }

        public IViewModel ViewParent
        {
            get
            {
                return null;
            }
        }

        public void Dispose()
        {
            core = null;
        }
    }
}
