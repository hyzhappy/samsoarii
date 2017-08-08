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

using SamSoarII.Shell.Models;
using SamSoarII.Core.Models;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// PolylineAxisWidget.xaml 的交互逻辑
    /// </summary>
    public partial class PolylineAxisWidget : UserControl, IViewModel
    {
        public PolylineAxisWidget()
        {
            InitializeComponent();
        }

        public void Dispose()
        {

        }

        #region Core

        private PolylineAxisModel core;
        public PolylineAxisModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                PolylineAxisModel _core = core;
                this.core = null;
                if (_core != null && _core.View != null) _core.View = null;
                this.core = value;
                if (core != null && core.View != this) core.View = this;
            }
        }
        IModel IViewModel.Core
        {
            get { return Core; }
            set { Core = (PolylineAxisModel)value; }
        }

        #endregion

        #region Shell

        private PolylineSystemSettingDialog ViewParent { get { return core?.Parent?.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }



        #endregion
    }
}
