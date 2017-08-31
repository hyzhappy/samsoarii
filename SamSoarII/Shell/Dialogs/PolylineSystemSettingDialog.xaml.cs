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
        public PolylineSystemSettingDialog(ProjectModel _project)
        {
            InitializeComponent();
            project = _project;
            polylines = new PolylineSystemModel[project.Polylines.Count];
            for (int i = 0; i < polylines.Length; i++)
                polylines[i] = project.Polylines[i].Clone();
            CB_Sys.SelectedIndex = 0;
            Core = polylines[0];
        }

        public void Dispose()
        {
            core = null;
            WG_X.Dispose();
            WG_Y.Dispose();
            project = null;
        }

        #region Core

        private ProjectModel project;
        public ProjectModel Project { get { return this.project; } }

        private PolylineSystemModel[] polylines;

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
                try
                {
                    this.core = null;
                    if (_core != null)
                    {
                        _core.IsEnabled = CK_Sys.IsChecked == true;
                        if (_core.IsEnabled)
                        {
                            _core.Unit = (PolylineSystemModel.SystemUnits)(CB_Unit.SelectedIndex);
                            _core.Overflow = (PolylineSystemModel.OverflowHandles)(CB_OF.SelectedIndex);
                            _core.IsHMIEnabled = CK_HMI.IsChecked == true;
                            if (_core.IsHMIEnabled)
                                _core.HMI.Text = TB_HMI.Text;
                        }
                        WG_X.Core = null;
                        WG_Y.Core = null;
                        if (_core.View != null) _core.View = null;
                    }
                    this.core = value;
                    if (core != null)
                    {
                        CK_Sys.IsChecked = core.IsEnabled;
                        CB_Unit.SelectedIndex = (int)(core.Unit);
                        CB_OF.SelectedIndex = (int)(core.Overflow);
                        CK_HMI.IsChecked = core.IsHMIEnabled;
                        TB_HMI.Text = core.HMI.Text;
                        WG_X.Core = core.X;
                        WG_Y.Core = core.Y;
                        UpdateEnable();
                        UpdateUnit();
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
            set { Core = (PolylineSystemModel)value; }
        }

        #endregion

        #region Shell

        public ProjectViewModel ViewParent { get { return core.Parent.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        #endregion

        public void Save()
        {
            Core = null;
            for (int i = 0; i < polylines.Length; i++)
            {
                project.Polylines[i].Load(polylines[i]);
                project.InvokeModify(project.Polylines[i]);
            }
        }

        #region Event Handler

        private void UpdateEnable()
        {
            CB_Unit.IsEnabled = CB_OF.IsEnabled =
            WG_X.IsEnabled = WG_Y.IsEnabled =
            CK_HMI.IsEnabled = CK_Sys.IsChecked == true;
            TB_HMI.IsEnabled = CK_HMI.IsEnabled && (CK_HMI.IsChecked == true);
        }

        private void UpdateUnit()
        {
            WG_X.TBU_WEI.Text = WG_Y.TBU_WEI.Text = "mm/pls";
            WG_X.TBU_LIM.Text = WG_X.TBU_CLM.Text = WG_X.TBU_ITV.Text =
            WG_Y.TBU_LIM.Text = WG_Y.TBU_CLM.Text = WG_Y.TBU_ITV.Text = CB_Unit.SelectedIndex == 0 ? "mm" : "pls";
        }
        
        private void CB_Sys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int oldindex = Core != null ? Core.ID - 1 : -1;
            try
            {
                Core = polylines[CB_Sys.SelectedIndex];
                UpdateEnable();
            }
            catch (ValueParseException exce)
            {
                LocalizedMessageBox.Show(string.Format(exce.Message), LocalizedMessageIcon.Error);
                CB_Sys.SelectedIndex = oldindex;
            }
        }
        
        private void CK_Sys_Checked(object sender, RoutedEventArgs e)
        {
            UpdateEnable();
        }

        private void CK_Sys_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateEnable();
        }

        private void CK_HMI_Checked(object sender, RoutedEventArgs e)
        {
            UpdateEnable();
        }

        private void CK_HMI_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateEnable();
        }

        private void CB_Unit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUnit();
        }

        public event RoutedEventHandler Ensure = delegate { };
        private void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            Ensure(this, new RoutedEventArgs());
        }

        public event RoutedEventHandler Help = delegate { };
        private void OnHelpButtonClick(object sender, RoutedEventArgs e)
        {
            Help(this, new RoutedEventArgs());
        }
        #endregion
        
    }
}
