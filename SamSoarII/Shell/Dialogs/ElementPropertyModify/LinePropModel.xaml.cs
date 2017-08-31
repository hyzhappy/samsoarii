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

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// LinePropModel.xaml 的交互逻辑
    /// </summary>
    public partial class LinePropModel : UserControl
    {
        public LinePropModel()
        {
            InitializeComponent();
            Visibility = Visibility.Hidden;
        }

        #region Number

        private Core.Models.Polyline core;
        public Core.Models.Polyline Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                if (core != null)
                {
                    core.X.Text = TBO_X.Text;
                    core.Y.Text = TBO_Y.Text;
                    core.Mode = (SamSoarII.Core.Models.Polyline.Modes)(TBO_Type.SelectedIndex);
                    core.AC.Text = TBO_AC.Text;
                    core.DC.Text = TBO_DC.Text;
                    core.V.Text = TBO_V.Text;
                    if (core is IntArch)
                    {
                        IntArch arch = (IntArch)core;
                        arch.Type = (IntArch.ArchTypes)(TBO_Arch.SelectedIndex);
                        arch.R.Text = TBO_R.Text;
                        arch.Dir = (IntArch.Directions)(TBO_Dir.SelectedIndex);
                        arch.Qua = (IntArch.Qualities)(TBO_Qua.SelectedIndex);
                        arch.CX.Text = TBO_CX.Text;
                        arch.CY.Text = TBO_CY.Text;
                    }
                    if (core is FloatArch)
                    {
                        FloatArch arch = (FloatArch)core;
                        arch.Type = (FloatArch.ArchTypes)(TBO_Arch.SelectedIndex);
                        arch.R.Text = TBO_R.Text;
                        arch.Dir = (FloatArch.Directions)(TBO_Dir.SelectedIndex);
                        arch.Qua = (FloatArch.Qualities)(TBO_Qua.SelectedIndex);
                        arch.CX.Text = TBO_CX.Text;
                        arch.CY.Text = TBO_CY.Text;
                    }
                }
                this.core = value;
                if (core != null)
                {
                    TBL_CXU.Text = TBL_CYU.Text =
                    TBL_RU.Text = TBL_XU.Text = TBL_YU.Text =
                        core.Parent.Unit == POLYLINEModel.SystemUnits.MM ? "mm" : "pls";
                    TBL_VU.Text = String.Format("{0:s}/s",
                        core.Parent.Unit == POLYLINEModel.SystemUnits.MM ? "mm" : "pls");
                    Visibility = Visibility.Visible;
                    TBO_X.Text = core.X.Text;
                    TBO_Y.Text = core.Y.Text;
                    TBO_Type.SelectedIndex = (int)(core.Mode);
                    TBO_AC.Text = core.AC.Text;
                    TBO_DC.Text = core.DC.Text;
                    TBO_V.Text = core.V.Text;
                    if (core is IntArch || core is FloatArch)
                    {
                        TBL_Arch.Visibility = Visibility.Visible;
                        TBO_Arch.Visibility = Visibility.Visible;
                        if (core is IntArch)
                        {
                            IntArch arch = (IntArch)core;
                            TBO_Arch.SelectedIndex = (int)(arch.Type);
                            TBO_R.Text = arch.R.Text;
                            TBO_Dir.SelectedIndex = (int)(arch.Dir);
                            TBO_Qua.SelectedIndex = (int)(arch.Qua);
                            TBO_CX.Text = arch.CX.Text;
                            TBO_CY.Text = arch.CY.Text;
                        }
                        if (core is FloatArch)
                        {
                            FloatArch arch = (FloatArch)core;
                            TBO_Arch.SelectedIndex = (int)(arch.Type);
                            TBO_R.Text = arch.R.Text;
                            TBO_Dir.SelectedIndex = (int)(arch.Dir);
                            TBO_Qua.SelectedIndex = (int)(arch.Qua);
                            TBO_CX.Text = arch.CX.Text;
                            TBO_CY.Text = arch.CY.Text;
                        }
                        UpdateArchType();
                    }
                    else
                    {
                        TBL_Arch.Visibility = Visibility.Hidden;
                        TBO_Arch.Visibility = Visibility.Hidden;
                        TBL_R.Visibility = Visibility.Hidden;
                        TBO_R.Visibility = Visibility.Hidden;
                        TBL_RU.Visibility = Visibility.Hidden;
                        TBL_Dir.Visibility = Visibility.Hidden;
                        TBO_Dir.Visibility = Visibility.Hidden;
                        TBL_Qua.Visibility = Visibility.Hidden;
                        TBO_Qua.Visibility = Visibility.Hidden;
                        TBL_CX.Visibility = Visibility.Hidden;
                        TBO_CX.Visibility = Visibility.Hidden;
                        TBL_CXU.Visibility = Visibility.Hidden;
                        TBL_CY.Visibility = Visibility.Hidden;
                        TBO_CY.Visibility = Visibility.Hidden;
                        TBL_CYU.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    Visibility = Visibility.Hidden;
                }
            }
        }

        #endregion

        #region Event Handler

        private void UpdateArchType()
        {
            TBL_Dir.Visibility = TBO_Dir.Visibility =
            TBL_Qua.Visibility = TBO_Qua.Visibility =
            TBL_R.Visibility = TBO_R.Visibility = TBL_RU.Visibility =
                TBO_Arch.SelectedIndex == 0 ? Visibility.Visible : Visibility.Hidden;
            TBL_CX.Visibility = TBO_CX.Visibility = TBL_CXU.Visibility =
            TBL_CY.Visibility = TBO_CY.Visibility = TBL_CYU.Visibility =
                TBO_Arch.SelectedIndex == 1 ? Visibility.Visible : Visibility.Hidden;
        }
        
        private void TBO_Arch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateArchType();
        }

        #endregion

    }
}
