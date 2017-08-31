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
            core = null;
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
                try
                {
                    this.core = null;
                    if (_core != null)
                    {
                        if (_core.Parent.IsEnabled)
                        {
                            _core.PLS.Text = TB_PLS.Text;
                            _core.DIR.Text = TB_DIR.Text;
                            _core.WEI.Text = TB_WEI.Text;
                            _core.LIM.Text = TB_LIM.Text;
                            _core.CLM.Text = TB_CLM.Text;
                            _core.ITV.Text = TB_ITV.Text;
                        }
                        if (_core.View != null) _core.View = null;
                    }
                    this.core = value;
                    if (core != null)
                    {
                        TB_PLS.Text = core.PLS.Text;
                        TB_DIR.Text = core.DIR.Text;
                        TB_WEI.Text = core.WEI.Text;
                        TB_LIM.Text = core.LIM.Text;
                        TB_CLM.Text = core.CLM.Text;
                        TB_ITV.Text = core.ITV.Text;
                        if (core.View != this) core.View = this;
                    }
                }
                catch (Exception e)
                {
                    this.core = _core;
                    throw e;
                }
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
