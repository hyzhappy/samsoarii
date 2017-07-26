using SamSoarII.Core.Helpers;
using SamSoarII.Core.Models;
using SamSoarII.PLCDevice;
using SamSoarII.Shell.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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

namespace SamSoarII.Shell.Windows
{
    /// <summary>
    /// ElementListWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ElementListWindow : UserControl, IWindow, INotifyPropertyChanged
    {
        public ElementListWindow(InteractionFacade _ifParent)
        {
            InitializeComponent();
            DataContext = this;
            ifParent = _ifParent;
            ifParent.PostIWindowEvent += OnReceiveIWindowEvent;
            rangeitems = new RangeItem[13];
            rangeitems[0] = new RangeItem(this, ValueModel.Bases.X);
            rangeitems[1] = new RangeItem(this, ValueModel.Bases.Y);
            rangeitems[2] = new RangeItem(this, ValueModel.Bases.M);
            rangeitems[3] = new RangeItem(this, ValueModel.Bases.C);
            rangeitems[4] = new RangeItem(this, ValueModel.Bases.T);
            rangeitems[5] = new RangeItem(this, ValueModel.Bases.S);
            rangeitems[6] = new RangeItem(this, ValueModel.Bases.D);
            rangeitems[7] = new RangeItem(this, ValueModel.Bases.CV);
            rangeitems[8] = new RangeItem(this, ValueModel.Bases.TV);
            rangeitems[9] = new RangeItem(this, ValueModel.Bases.AI);
            rangeitems[10] = new RangeItem(this, ValueModel.Bases.AO);
            rangeitems[11] = new RangeItem(this, ValueModel.Bases.V);
            rangeitems[12] = new RangeItem(this, ValueModel.Bases.Z);
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private InteractionFacade ifParent;
        public InteractionFacade IFParent { get { return this.ifParent; } }
        public ValueManager ValueManager { get { return ifParent.MNGValue; } }
        public Device Device { get { return ifParent.MDProj != null ? ifParent.MDProj.Device : ValueManager.MaxRange; } }

        #region Binding
        
        public bool HasUsed { get { return BT_Used?.IsChecked == true; } }
        public bool HasCommnet { get { return BT_Comment?.IsChecked == true; } }

        public IList<ValueInfo> ElementCollection
        {
            get
            {
                IEnumerable<ValueInfo> colle = null;
                if (LB_Range.SelectedItems == null || LB_Range.SelectedItems.Count == 0)
                {
                    colle = ValueManager;
                }
                else
                {
                    IEnumerable<ValueModel.Bases> types = LB_Range.SelectedItems.Cast<RangeItem>().Select(ri => ri.Type);
                    colle = ValueManager.Where(vi => types.Contains(vi.Prototype.Base));
                }
                if (HasUsed) colle = colle.Where(vi => vi.Values.Count() > 0);
                if (HasCommnet) colle = colle.Where(vi => vi.Comment.Length > 0);
                colle = colle.Where(x => { return x.Name.StartsWith(TB_Search.Text.ToUpper()); }).ToList();
                return colle.ToArray();
            }
        }
        
        private RangeItem[] rangeitems;
        public IList<RangeItem> RangeListCollection
        {
            get { return this.rangeitems; }
        }

        #endregion
        
        #endregion

        #region Event Handler

        public event IWindowEventHandler Post = delegate { };

        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {
            if (sender == ifParent && e is InteractionFacadeEventArgs)
            {
                InteractionFacadeEventArgs e1 = (InteractionFacadeEventArgs)e;
                switch (e1.Flags)
                {
                    case InteractionFacadeEventArgs.Types.DiagramModified:
                        PropertyChanged(this, new PropertyChangedEventArgs("ElementCollection"));
                        break;
                }
            }
        }

        private void LBI_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var lbi = (ListBoxItem)sender;
            lbi.IsSelected = !lbi.IsSelected;
            if (!lbi.IsSelected) LB_Range.SelectedItem = null;
            else LB_Range.SelectedItem = lbi.Content;
            PropertyChanged(this, new PropertyChangedEventArgs("ElementCollection"));
            e.Handled = true;
        }
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            PropertyChanged(this, new PropertyChangedEventArgs("ElementCollection"));
        }
        private void LB_Range_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LB_Range.SelectedItem = null;
        }
        private void LB_Range_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PropertyChanged(this, new PropertyChangedEventArgs("ElementCollection"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == BT_Clear && DG_Element.SelectedItems != null)
            {
                foreach (ValueInfo info in DG_Element.SelectedItems)
                {
                    info.Comment = null;
                    info.Alias = null;
                }
            }
            if (sender == BT_Import)
            {
                using (CSVImportDialog dialog = new CSVImportDialog())
                {
                    dialog.ImportButtonClick += (sender1, e1) =>
                    {
                        if (dialog.FileName == string.Empty)
                        {
                            LocalizedMessageBox.Show(Properties.Resources.Message_File_Requried, LocalizedMessageIcon.Warning);
                            return;
                        }
                        CSVFileHelper.ImportExcute(dialog.FileName, ValueManager, dialog.Separator.Substring(0, 1));
                        dialog.Close();
                    };
                    dialog.ShowDialog();
                }
            }
            if (sender == BT_Export)
            {
                using (CSVExportDialog dialog = new CSVExportDialog())
                {
                    dialog.ExportButtonClick += (sender1, e1) =>
                    {
                        string name = dialog.FileName;
                        string dir = dialog.Path;
                        if (!Directory.Exists(dir))
                        {
                            LocalizedMessageBox.Show(Properties.Resources.Message_Path, LocalizedMessageIcon.Warning);
                            return;
                        }
                        if (name == string.Empty)
                        {
                            LocalizedMessageBox.Show(Properties.Resources.Message_File_Name, LocalizedMessageIcon.Warning);
                            return;
                        }
                        string fullFileName = string.Format(@"{0}\{1}.csv", dir, name);
                        if (File.Exists(fullFileName))
                        {
                            LocalizedMessageBox.Show(Properties.Resources.Message_File_Exist, LocalizedMessageIcon.Warning);
                            return;
                        }
                        CSVFileHelper.ExportExcute(fullFileName, ElementCollection, dialog.Separator.Substring(0, 1));
                        dialog.Close();
                    };
                    dialog.ShowDialog();
                }
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == BT_Comment || sender == BT_Used)
                PropertyChanged(this, new PropertyChangedEventArgs("ElementCollection"));
            if (sender == BT_Detail)
                DG_Element.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender == BT_Comment || sender == BT_Used)
                PropertyChanged(this, new PropertyChangedEventArgs("ElementCollection"));
            if (sender == BT_Detail)
                DG_Element.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
        }
        
        private void NavigateMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock)
            {
                string text = ((TextBlock)sender).Text;
                ifParent.Navigate(text);
            }
        }

        #endregion
        
    }

    public class RangeItem : IDisposable
    {
        public RangeItem(ElementListWindow _parent, ValueModel.Bases _type)
        {
            parent = _parent;
            type = _type;
        }

        public void Dispose()
        {
            parent = null;
        }

        public override string ToString()
        {
            switch (type)
            {
                case ValueModel.Bases.X: return String.Format("X(Bit) - ({0} {1})", parent.Device.XRange.Start, parent.Device.XRange.End - 1);
                case ValueModel.Bases.Y: return String.Format("Y(Bit) - ({0} {1})", parent.Device.YRange.Start, parent.Device.YRange.End - 1);
                case ValueModel.Bases.S: return String.Format("S(Bit) - ({0} {1})", parent.Device.SRange.Start, parent.Device.SRange.End - 1);
                case ValueModel.Bases.M: return String.Format("M(Bit) - ({0} {1})", parent.Device.MRange.Start, parent.Device.MRange.End - 1);
                case ValueModel.Bases.C: return String.Format("C(Bit) - ({0} {1})", parent.Device.CRange.Start, parent.Device.CRange.End - 1);
                case ValueModel.Bases.T: return String.Format("T(Bit) - ({0} {1})", parent.Device.TRange.Start, parent.Device.TRange.End - 1);
                case ValueModel.Bases.D: return String.Format("D(Word) - ({0} {1})", parent.Device.DRange.Start, parent.Device.DRange.End - 1);
                case ValueModel.Bases.CV: return String.Format("CV(Word) - ({0} {1})", parent.Device.CVRange.Start, parent.Device.CVRange.End - 1);
                case ValueModel.Bases.TV: return String.Format("TV(Word) - ({0} {1})", parent.Device.TVRange.Start, parent.Device.TVRange.End - 1);
                case ValueModel.Bases.AI: return String.Format("AI(Word) - ({0} {1})", parent.Device.AIRange.Start, parent.Device.AIRange.End - 1);
                case ValueModel.Bases.AO: return String.Format("AO(Word) - ({0} {1})", parent.Device.AORange.Start, parent.Device.AORange.End - 1);
                case ValueModel.Bases.V: return String.Format("V(Word) - ({0} {1})", parent.Device.VRange.Start, parent.Device.VRange.End - 1);
                case ValueModel.Bases.Z: return String.Format("Z(Word) - ({0} {1})", parent.Device.ZRange.Start, parent.Device.ZRange.End - 1);
                default: return String.Empty;
            }
        }

        #region Number

        private ElementListWindow parent;

        private ValueModel.Bases type;
        public ValueModel.Bases Type { get { return this.type; } }

        #endregion 

    }
}
