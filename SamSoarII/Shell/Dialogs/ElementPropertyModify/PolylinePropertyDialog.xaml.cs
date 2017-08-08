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
using System.ComponentModel;

using SamSoarII.Core.Models;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// PolylinePropertyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PolylinePropertyDialog : Window, IDisposable, INotifyPropertyChanged
    {
        public PolylinePropertyDialog(POLYLINEModel _core)
        {
            InitializeComponent();
            Current = null;
            Core = _core;
            DataContext = this;
        }

        public void Dispose()
        {
            Current = null;
            Core = null;
            DataContext = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private POLYLINEModel core;
        public POLYLINEModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                this.core = value;
                try
                {
                    CB_Sys.SelectedIndex = int.Parse(core.Children[0].Store.Value.ToString()) - 1;
                    CB_Ref.SelectedIndex = (int)(core.ReflictMode);
                    TB_Ref.Text = core.ReflictLocation.Text;
                }
                catch (Exception)
                {
                    CB_Sys.SelectedIndex = 0;
                    CB_Ref.SelectedIndex = 0;
                    TB_Ref.Text = "D0";
                }
                if (core is POLYLINEIModel || core is POLYLINEFModel)
                {
                    LB_Lines.Visibility = Visibility.Visible;
                    LB_Lines.IsEnabled = true;
                }
                else
                {
                    LB_Lines.Visibility = Visibility.Hidden;
                    LB_Lines.IsEnabled = false;
                    if (core is LINEIModel) Current = ((LINEIModel)core).Line;
                    if (core is LINEFModel) Current = ((LINEFModel)core).Line;
                    if (core is ARCHIModel) Current = ((ARCHIModel)core).Arch;
                    if (core is ARCHFModel) Current = ((ARCHFModel)core).Arch;
                    PropertyChanged(this, new PropertyChangedEventArgs("Collection"));
                }
            }
        }
        
        public IEnumerable<Core.Models.Polyline> Collection
        {
            get
            {
                if (core is POLYLINEIModel)
                    return ((POLYLINEIModel)core).Polylines.Cast<Core.Models.Polyline>();
                if (core is POLYLINEFModel)
                    return ((POLYLINEFModel)core).Polylines.Cast<Core.Models.Polyline>();
                return new Core.Models.Polyline[] { };
            }
        }

        private Core.Models.Polyline current;
        public Core.Models.Polyline Current
        {
            get
            {
                return this.current;
            }
            set
            {
                try
                {
                    MainModel.Core = value;
                    this.current = value;
                    MI_IA.IsEnabled = (current != null);
                    MI_IL.IsEnabled = (current != null);
                    MI_DE.IsEnabled = (current != null);
                    MI_MU.IsEnabled = (current != null && LB_Lines.SelectedIndex > 0);
                    MI_MD.IsEnabled = (current != null && LB_Lines.SelectedIndex < LB_Lines.Items.Count - 1);
                }
                catch (ValueParseException e)
                {
                    ValueFormat vformat = e.Format;
                    if (vformat != null)
                        LocalizedMessageBox.Show(String.Format("{0:s}设置错误{1:s}！({2:s})", vformat.Name, vformat.Supports, e.Message), LocalizedMessageIcon.Error);
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Current"));
            }
        }

        #endregion

        #region Event Handler

        public event RoutedEventHandler Ensure = delegate { };
        private void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MainModel.Core = null;
                core.Children[0].Text = String.Format("K{0:d}", CB_Sys.SelectedIndex + 1);
                core.Children[1].Text = "D0";
                core.Children[2].Text = String.Format("K{0:d}",
                    (core is POLYLINEIModel) ? ((POLYLINEIModel)core).Polylines.Count :
                    (core is POLYLINEFModel) ? ((POLYLINEFModel)core).Polylines.Count : 0);
                core.ReflictMode = (POLYLINEModel.ReflictModes)(CB_Ref.SelectedIndex);
                core.ReflictLocation.Text = TB_Ref.Text;
                Ensure(this, e);
            }
            catch (ValueParseException exce)
            {
                ValueFormat vformat = exce.Format;
                if (vformat != null)
                    LocalizedMessageBox.Show(String.Format("{0:s}设置错误{1:s}！({2:s})", vformat.Name, vformat.Supports, exce.Message), LocalizedMessageIcon.Error);
            }
        }

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (sender == MI_AA)
            {
                if (core is POLYLINEIModel) ((POLYLINEIModel)core).Polylines.Add(new IntArch(core));
                if (core is POLYLINEFModel) ((POLYLINEFModel)core).Polylines.Add(new FloatArch(core));
            }
            if (sender == MI_AL)
            {
                if (core is POLYLINEIModel) ((POLYLINEIModel)core).Polylines.Add(new IntLine(core));
                if (core is POLYLINEFModel) ((POLYLINEFModel)core).Polylines.Add(new FloatLine(core));
            }
            if (sender == MI_IA)
            {
                if (core is POLYLINEIModel) ((POLYLINEIModel)core).Polylines.Insert(
                    LB_Lines.SelectedIndex, new IntArch(core));
                if (core is POLYLINEFModel) ((POLYLINEFModel)core).Polylines.Insert(
                    LB_Lines.SelectedIndex, new FloatArch(core));
            }
            if (sender == MI_IL)
            {
                if (core is POLYLINEIModel) ((POLYLINEIModel)core).Polylines.Insert(
                    LB_Lines.SelectedIndex, new IntLine(core));
                if (core is POLYLINEFModel) ((POLYLINEFModel)core).Polylines.Insert(
                    LB_Lines.SelectedIndex, new FloatLine(core));
            }
            if (sender == MI_DE)
            {
                if (core is POLYLINEIModel) ((POLYLINEIModel)core).Polylines.Remove((IntPolyline)Current);
                if (core is POLYLINEFModel) ((POLYLINEFModel)core).Polylines.Remove((FloatPolyline)Current);
            }
            if (sender == MI_MU)
            {
                int id = LB_Lines.SelectedIndex;
                if (core is POLYLINEIModel)
                {
                    POLYLINEIModel corei = (POLYLINEIModel)core;
                    IntPolyline tmp = corei.Polylines[id - 1];
                    corei.Polylines[id - 1] = corei.Polylines[id];
                    corei.Polylines[id] = tmp;
                }
                if (core is POLYLINEFModel)
                {
                    POLYLINEFModel coref = (POLYLINEFModel)core;
                    FloatPolyline tmp = coref.Polylines[id - 1];
                    coref.Polylines[id - 1] = coref.Polylines[id];
                    coref.Polylines[id] = tmp;
                }
            }
            if (sender == MI_MD)
            {
                int id = LB_Lines.SelectedIndex;
                if (core is POLYLINEIModel)
                {
                    POLYLINEIModel corei = (POLYLINEIModel)core;
                    IntPolyline tmp = corei.Polylines[id + 1];
                    corei.Polylines[id + 1] = corei.Polylines[id];
                    corei.Polylines[id] = tmp;
                }
                if (core is POLYLINEFModel)
                {
                    POLYLINEFModel coref = (POLYLINEFModel)core;
                    FloatPolyline tmp = coref.Polylines[id + 1];
                    coref.Polylines[id + 1] = coref.Polylines[id];
                    coref.Polylines[id] = tmp;
                }
            }
            if (core is POLYLINEIModel) ((POLYLINEIModel)core).RefreshPolylines();
            if (core is POLYLINEFModel) ((POLYLINEFModel)core).RefreshPolylines();
            PropertyChanged(this, new PropertyChangedEventArgs("Collection"));
        }

        #endregion
        
    }
}
