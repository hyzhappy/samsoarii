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
using System.Windows.Shapes;

using SamSoarII.Core.Models;
using SamSoarII.Shell.Models;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// PolylineSystemSettingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PolylineSystemSettingDialog : Window, IViewModel
    {
        public PolylineSystemSettingDialog(PolylineSystemModel _core)
        {
            InitializeComponent();
            Core = _core;
        }

        public void Dispose()
        {
            Core = null;
        }

        #region Core

        private PolylineSystemModel core;
        public PolylineSystemModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                PolylineSystemModel _core = core;
                this.core = null;
                if (_core != null && _core.View != null) _core.View = null;
                this.core = value;
                if (core != null && core.View != this) core.View = this;
            }
        }
        IModel IViewModel.Core
        {
            get { return Core; }
            set { Core = (PolylineSystemModel)value; }
        }

        #endregion

        #region Shell

        public ProjectViewModel ViewParent { get { return core.Parent.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        #endregion
    }
}
