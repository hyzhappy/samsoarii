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
                if (core == null) return;
                CB_Unit.SelectedIndex = (int)(core.Unit);
                CB_Unit.IsEnabled = false;
                try
                {
                    CB_Sys.SelectedIndex = int.Parse(core.Children[0].Store.Value.ToString()) - 1;
                    CB_Ref.SelectedIndex = (int)(core.RefMode);
                    TB_Ref.Text = core.RefAddr.Base != ValueModel.Bases.D ? "D0" : core.RefAddr.Text;
                }
                catch (Exception e)
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
                UpdateReflict();
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
                        LocalizedMessageBox.Show(String.Format("{0:s}设置错误({1:s})！({2:s})", vformat.Name, vformat.Supports, e.Message), LocalizedMessageIcon.Error);
                    LB_Lines.SelectedItem = MainModel.Core;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Current"));
                UpdateReflict();
            }
        }

        #endregion

        #region Event Handler

        private void UpdateReflict()
        {
            TB_Ref.IsEnabled = (CB_Ref.SelectedIndex != 0);
            if (CB_Ref.SelectedIndex == 1)
            {
                try
                {
                    int length = 0;
                    int offset = 0;
                    if (core is POLYLINEIModel)
                    {
                        POLYLINEIModel polyi = (POLYLINEIModel)core;
                        for (int i = 0; i < polyi.Polylines.Count; i++)
                        {
                            if (i == LB_Lines.SelectedIndex)
                                offset = length;
                            length += polyi.Polylines[i].Count;
                        }
                    }
                    if (core is POLYLINEFModel)
                    {
                        POLYLINEFModel polyf = (POLYLINEFModel)core;
                        for (int i = 0; i < polyf.Polylines.Count; i++)
                        {
                            if (i == LB_Lines.SelectedIndex)
                                offset = length;
                            length += polyf.Polylines[i].Count;
                        }
                    }
                    if (core is LINEIModel)
                    {
                        LINEIModel linei = (LINEIModel)core;
                        length = linei.Line.Count;
                    }
                    if (core is LINEFModel)
                    {
                        LINEFModel linef = (LINEFModel)core;
                        length = linef.Line.Count;
                    }
                    if (core is ARCHIModel)
                    {
                        ARCHIModel archi = (ARCHIModel)core;
                        length = archi.Arch.Count;
                    }
                    if (core is ARCHFModel)
                    {
                        ARCHFModel archf = (ARCHFModel)core;
                        length = archf.Arch.Count;
                    }
                    ValueModel.Analyzer_DWord.Text = TB_Ref.Text;
                    if (ValueModel.Analyzer_DWord.Intra != ValueModel.Bases.NULL)
                    {
                        MainModel.TBL_X.Text = String.Format("x轴终点坐标({0:s}{1:d}{2:s}{3:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset,
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Intra)],
                            ValueModel.Analyzer_DWord.IntraOffset);
                        MainModel.TBL_Y.Text = String.Format("y轴终点坐标({0:s}{1:d}{2:s}{3:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 2,
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Intra)],
                            ValueModel.Analyzer_DWord.IntraOffset);
                        MainModel.TBL_Type.Text = String.Format("坐标类型({0:s}{1:d}{2:s}{3:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 4,
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Intra)],
                            ValueModel.Analyzer_DWord.IntraOffset);
                        MainModel.TBL_AC.Text = String.Format("加速时间({0:s}{1:d}{2:s}{3:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 7,
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Intra)],
                            ValueModel.Analyzer_DWord.IntraOffset);
                        MainModel.TBL_DC.Text = String.Format("减速时间({0:s}{1:d}{2:s}{3:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 8,
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Intra)],
                            ValueModel.Analyzer_DWord.IntraOffset);
                        MainModel.TBL_V.Text = String.Format("速度({0:s}{1:d}{2:s}{3:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 5,
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Intra)],
                            ValueModel.Analyzer_DWord.IntraOffset);
                        MainModel.TBL_Type.Text = String.Format("圆弧类({0:s}{1:d}{2:s}{3:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 4,
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Intra)],
                            ValueModel.Analyzer_DWord.IntraOffset);
                        MainModel.TBL_R.Text = String.Format("圆弧半径({0:s}{1:d}{2:s}{3:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 5,
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Intra)],
                            ValueModel.Analyzer_DWord.IntraOffset);
                        MainModel.TBL_Dir.Text = String.Format("顺/逆时针({0:s}{1:d}{2:s}{3:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 4,
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Intra)],
                            ValueModel.Analyzer_DWord.IntraOffset);
                        MainModel.TBL_Qua.Text = String.Format("优/劣弧({0:s}{1:d}{2:s}{3:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 4,
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Intra)],
                            ValueModel.Analyzer_DWord.IntraOffset);
                        MainModel.TBL_CX.Text = String.Format("X轴中心点({0:s}{1:d}{2:s}{3:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 5,
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Intra)],
                            ValueModel.Analyzer_DWord.IntraOffset);
                        MainModel.TBL_CY.Text = String.Format("Y轴中心点({0:s}{1:d}{2:s}{3:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 7,
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Intra)],
                            ValueModel.Analyzer_DWord.IntraOffset);
                    }
                    else
                    {
                        MainModel.TBL_X.Text = String.Format("x轴终点坐标({0:s}{1:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset);
                        MainModel.TBL_Y.Text = String.Format("y轴终点坐标({0:s}{1:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 2);
                        MainModel.TBL_Type.Text = String.Format("坐标类型({0:s}{1:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 4);
                        MainModel.TBL_AC.Text = String.Format("加速时间({0:s}{1:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 7);
                        MainModel.TBL_DC.Text = String.Format("减速时间({0:s}{1:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 8);
                        MainModel.TBL_V.Text = String.Format("速度({0:s}{1:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 5);
                        MainModel.TBL_Type.Text = String.Format("圆弧类({0:s}{1:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 4);
                        MainModel.TBL_R.Text = String.Format("圆弧半径({0:s}{1:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 5);
                        MainModel.TBL_Dir.Text = String.Format("顺/逆时针({0:s}{1:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 4);
                        MainModel.TBL_Qua.Text = String.Format("优/劣弧({0:s}{1:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 4);
                        MainModel.TBL_CX.Text = String.Format("X轴中心点({0:s}{1:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 5);
                        MainModel.TBL_CY.Text = String.Format("Y轴中心点({0:s}{1:d})：",
                            ValueModel.NameOfBases[(int)(ValueModel.Analyzer_DWord.Base)],
                            ValueModel.Analyzer_DWord.Offset + offset + 7);
                    }
                }
                catch (ValueParseException)
                {
                }
            }
            else
            {
                MainModel.TBL_X.Text = String.Format("x轴终点坐标：");
                MainModel.TBL_Y.Text = String.Format("y轴终点坐标：");
                MainModel.TBL_Type.Text = String.Format("坐标类型：");
                MainModel.TBL_AC.Text = String.Format("加速时间：");
                MainModel.TBL_DC.Text = String.Format("减速时间：");
                MainModel.TBL_V.Text = String.Format("速度：");
                MainModel.TBL_Type.Text = String.Format("圆弧类：");
                MainModel.TBL_R.Text = String.Format("圆弧半径：");
                MainModel.TBL_Dir.Text = String.Format("顺/逆时针：");
                MainModel.TBL_Qua.Text = String.Format("优/劣弧：");
                MainModel.TBL_CX.Text = String.Format("X轴中心点：");
                MainModel.TBL_CY.Text = String.Format("Y轴中心点：");
            }
        }

        private void CB_Ref_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateReflict();
        }

        private void TB_Ref_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateReflict();
        }

        public event RoutedEventHandler Ensure = delegate { };
        private void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MainModel.Core = null;
                core.Children[0].Text = String.Format("K{0:d}", CB_Sys.SelectedIndex + 1);
                core.RefMode = (POLYLINEModel.ReflictModes)(CB_Ref.SelectedIndex);
                core.RefAddr.Text = TB_Ref.Text;
                Ensure(this, e);
            }
            catch (ValueParseException exce)
            {
                ValueFormat vformat = exce.Format;
                if (vformat != null)
                    LocalizedMessageBox.Show(String.Format("{0:s}设置错误({1:s})！({2:s})", vformat.Name, vformat.Supports, exce.Message), LocalizedMessageIcon.Error);
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
