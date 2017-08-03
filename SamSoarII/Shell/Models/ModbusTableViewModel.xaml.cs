using SamSoarII.Core.Models;
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
using SamSoarII.Shell.Dialogs;
using SamSoarII.Shell.Windows;
using System.Collections.Specialized;

namespace SamSoarII.Shell.Models
{
    /// <summary>
    /// ModbusTableViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class ModbusTableViewModel : BaseTabItem, IViewModel
    {
        public ModbusTableViewModel(ModbusTableModel _core, MainTabControl _tabcontrol) : base(_tabcontrol)
        {
            InitializeComponent();
            DataContext = this;
            Core = _core;
            UpdateButtonEnable();
        }

        public override void Dispose()
        {
            base.Dispose();
            Core = null;
        }

        #region Core

        private ModbusTableModel core;
        public ModbusTableModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                ModbusTableModel _core = core;
                this.core = null;
                if (_core != null)
                {
                    _core.PropertyChanged -= OnCorePropertyChanged;
                    _core.ChildrenChanged -= OnCoreChildrenChanged;
                    if (_core.View != null) _core.View = null;
                }
                this.core = value;
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
                    core.ChildrenChanged += OnCoreChildrenChanged;
                    if (core.View != this) core.View = this;
                }
            }
        }
        
        IModel IViewModel.Core
        {
            get { return core; }
            set { Core = (ModbusTableModel)value; }
        }

        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }
        
        private void OnCoreChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvokePropertyChanged("ListItems");
            if (e.NewItems != null) Current = (ModbusModel)(e.NewItems[0]);
            UpdateButtonEnable();
        }

        #endregion

        #region Shell

        public ProjectViewModel ViewParent { get { return core?.Parent.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        #region Binding

        public override string TabHeader { get { return Properties.Resources.Modbus_Table; } }

        public IList<ModbusModel> ListItems
        {
            get { return Core?.Children; }
        }

        private ModbusModel current = null;
        public ModbusModel Current
        {
            get
            {
                return this.current;
            }
            set
            {
                if (current == value) return;
                if (current != null)
                    current.ChildrenChanged -= OnModelChildrenChanged;
                this.current = value;
                if (current != null)
                    current.ChildrenChanged += OnModelChildrenChanged;
                UpdateButtonEnable();
                InvokePropertyChanged("GridItems");
                Invoke(TabAction.ACTIVE);
            }
        }

        public IList<ModbusItem> GridItems
        {
            get
            {
                return Current != null ? Current.Children : new ModbusItem[] { };
            }
        }
        
        public ModbusItem CurrentItem
        {
            get
            {
                return DG_Table.SelectedItem != null ? (ModbusItem)(DG_Table.SelectedItem) : null;
            }
            set
            {
                DG_Table.SelectedItem = value;
            }
        }

        #endregion

        #region Dialog

        private ModbusDialog dialog;
        public const int DIALOG_CREATE = 0x00;
        public const int DIALOG_RENAME = 0x01;
        private int dialogtype;
        private int DialogType
        {
            get { return this.dialogtype; }
            set
            {
                this.dialogtype = value;
                switch (value)
                {
                    case DIALOG_CREATE:
                        dialog.Title = Properties.Resources.Modbus_Create_New_Table;
                        break;
                    case DIALOG_RENAME:
                        dialog.Title = Properties.Resources.PTV_Rename;
                        break;
                }
            }
        }

        public void InitializeDialog(int dialogType)
        {
            if (dialog != null) return;
            if (DialogType == DIALOG_RENAME && Current == null) return;
            dialog = new ModbusDialog();
            dialog.B_Ensure.Click += OnDialogEnsureClick;
            dialog.B_Cancel.Click += OnDialogCancelClick;
            dialog.Closed += OnDialogClosed;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            switch (dialogType)
            {
                case DIALOG_CREATE:
                    DialogType = DIALOG_CREATE;
                    dialog.TB_Name.Text = String.Empty;
                    dialog.TB_Comment.Text = String.Empty;
                    break;
                case DIALOG_RENAME:
                    DialogType = DIALOG_RENAME;
                    dialog.TB_Name.Text = Current.Name;
                    dialog.TB_Comment.Text = Current.Comment;
                    break;
            }
            dialog.ShowDialog();
        }

        #endregion
        
        #endregion

        #region Event Handler

        #region Tools
        
        private void B_AddModel_Click(object sender, RoutedEventArgs e)
        {
            InitializeDialog(DIALOG_CREATE);
        }

        private void B_RenameModel_Click(object sender, RoutedEventArgs e)
        {
            InitializeDialog(DIALOG_RENAME);
        }

        private void B_RemoveModel_Click(object sender, RoutedEventArgs e)
        {
            if (Current == null) return;
            Core.Children.Remove(Current); 
        }

        private void B_ModelUp_Click(object sender, RoutedEventArgs e)
        {
            if (Current == null) return;
            int id = LB_Tables.SelectedIndex;
            Core.ChildrenSwap(id, id - 1);
            LB_Tables.SelectedIndex = id - 1;
        }

        private void B_ModelDown_Click(object sender, RoutedEventArgs e)
        {
            if (Current == null) return;
            int id = LB_Tables.SelectedIndex;
            Core.ChildrenSwap(id, id + 1);
            LB_Tables.SelectedIndex = id + 1;
        }

        private void B_ModelTop_Click(object sender, RoutedEventArgs e)
        {
            if (Current == null) return;
            LB_Tables.Items.MoveCurrentToFirst();
            int id = LB_Tables.SelectedIndex;
            Core.ChildrenSwap(id, 0);
            LB_Tables.SelectedIndex = 0;
        }

        private void B_ModelBottom_Click(object sender, RoutedEventArgs e)
        {
            if (Current == null) return;
            int id = LB_Tables.SelectedIndex;
            Core.ChildrenSwap(id, ListItems.Count() - 1);
            LB_Tables.SelectedIndex = ListItems.Count() - 1;
        }

        private void B_Insert_Click(object sender, RoutedEventArgs e)
        {
            if (Current == null) return;
            if (CurrentItem != null)
            {
                int id = Current.Children.IndexOf(CurrentItem);
                Current.Children.Insert(id, new ModbusItem(Current));
            }
            else
            {
                Current.Children.Add(new ModbusItem(Current));
            }
        }

        private void B_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (Current == null || CurrentItem == null) return;
            Current.Children.Remove(CurrentItem);
        }

        private void B_Up_Click(object sender, RoutedEventArgs e)
        {
            if (Current == null || CurrentItem == null) return;
            int id = DG_Table.SelectedIndex;
            Current.ChildrenSwap(id, id - 1);
            LB_Tables.SelectedIndex = id - 1;
        }

        private void B_Down_Click(object sender, RoutedEventArgs e)
        {
            if (Current == null || CurrentItem == null) return;
            int id = DG_Table.SelectedIndex;
            Current.ChildrenSwap(id, id + 1);
            LB_Tables.SelectedIndex = id + 1;
        }

        private void B_Top_Click(object sender, RoutedEventArgs e)
        {
            if (Current == null || CurrentItem == null) return;
            int id = DG_Table.SelectedIndex;
            Current.ChildrenSwap(id, 0);
            LB_Tables.SelectedIndex = 0;
        }

        private void B_Bottom_Click(object sender, RoutedEventArgs e)
        {
            if (Current == null || CurrentItem == null) return;
            int id = DG_Table.SelectedIndex;
            Current.ChildrenSwap(id, GridItems.Count() - 1);
            LB_Tables.SelectedIndex = GridItems.Count() - 1;
        }
        
        #endregion

        #region Dialog

        private void OnDialogEnsureClick(object sender, RoutedEventArgs e)
        {
            string name = dialog.TB_Name.Text;
            string comment = dialog.TB_Comment.Text;
            if (name.Equals(String.Empty))
            {
                LocalizedMessageBox.Show(Properties.Resources.Message_Name_Needed, LocalizedMessageIcon.Information);
                return;
            }
            IEnumerable<ModbusModel> fit = ListItems.Where(
                (ModbusModel model) => { return model.Name.Equals(name); });
            switch (DialogType)
            {
                case DIALOG_CREATE:
                    if (fit.Count() > 0)
                        LocalizedMessageBox.Show(Properties.Resources.Message_Table_Exist, LocalizedMessageIcon.Warning);
                    else
                    {
                        //AddModel(model, currentindex);
                        ModbusModel modbus = new ModbusModel(Core, name);
                        modbus.Comment = comment;
                        if (Current != null)
                        {
                            int id = Core.Children.IndexOf(Current);
                            Core.Children.Insert(id, modbus);
                        }
                        else
                        {
                            Core.Children.Add(modbus);
                        }
                        dialog.Close();
                    }
                    break;
                case DIALOG_RENAME:
                    if (fit.Count() > 0 && fit.First() != Current)
                        LocalizedMessageBox.Show(Properties.Resources.Message_Table_Exist, LocalizedMessageIcon.Warning);
                    else
                    {
                        Current.Name = name;
                        Current.Comment = comment;
                        dialog.Close();
                    }
                    break;
            }
        }

        private void OnDialogCancelClick(object sender, RoutedEventArgs e)
        {
            dialog.Close();
        }

        private void OnDialogClosed(object sender, EventArgs e)
        {
            dialog.B_Ensure.Click -= OnDialogEnsureClick;
            dialog.B_Cancel.Click -= OnDialogCancelClick;
            dialog.Closed -= OnDialogClosed;
            dialog = null;
        }

        #endregion
        
        private void OnModelChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvokePropertyChanged("GridItems");
            UpdateButtonEnable();
        }

        private void DG_Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Invoke(TabAction.ACTIVE);
            UpdateButtonEnable();
        }

        private void UpdateButtonEnable()
        {
            B_Insert.IsEnabled = (Current != null);
            B_Remove.IsEnabled = (Current != null && CurrentItem != null);
            B_Up.IsEnabled = (Current != null && CurrentItem != null && DG_Table.SelectedIndex > 0);
            B_Down.IsEnabled = (Current != null && CurrentItem != null && DG_Table.SelectedIndex < GridItems.Count() - 1);
            B_Top.IsEnabled = (Current != null && CurrentItem != null && DG_Table.SelectedIndex > 0);
            B_Bottom.IsEnabled = (Current != null && CurrentItem != null && DG_Table.SelectedIndex < GridItems.Count() - 1);
            B_AddModel.IsEnabled = true;
            B_RemoveModel.IsEnabled = (Current != null);
            B_RenameModel.IsEnabled = (Current != null);
            B_ModelUp.IsEnabled = (Current != null && LB_Tables.SelectedIndex > 0);
            B_ModelDown.IsEnabled = (Current != null && LB_Tables.SelectedIndex < ListItems?.Count() - 1);
            B_ModelTop.IsEnabled = (Current != null && LB_Tables.SelectedIndex > 0);
            B_ModelDown.IsEnabled = (Current != null && LB_Tables.SelectedIndex < ListItems?.Count() - 1);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox tb = (TextBox)sender;
                if (tb.Parent is DataGridCell)
                {
                    DataGridCell dgc = (DataGridCell)(tb.Parent);
                }
            }
        }
        
        #region DataGrid Update

        private void DataGridRow_Selected(object sender, RoutedEventArgs e)
        {
            DataGridRow dgrow = (DataGridRow)sender;
            Update(dgrow, true);
        }

        private void DataGridRow_Unselected(object sender, RoutedEventArgs e)
        {
            DataGridRow dgrow = (DataGridRow)sender;
            Update(dgrow, false);
        }

        private void DataGridRow_Loaded(object sender, RoutedEventArgs e)
        {
            DataGridRow dgrow = (DataGridRow)sender;
            Update(dgrow);
        }

        private void DataGridCell_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DataGridCell dgcell = (DataGridCell)sender;
            ModbusItem mtable = (ModbusItem)(dgcell.DataContext);
            DataGridColumn dgcolumn = dgcell.Column;
            string text = String.Empty;
            if (dgcell.Content is TextBox)
            {
                text = ((TextBox)(dgcell.Content)).Text;
            }
            if (dgcell.Content is TextBlock)
            {
                text = ((TextBlock)(dgcell.Content)).Text;
            }
            if (dgcell.Content is ComboBox)
            {
                text = ((ComboBox)(dgcell.Content)).Text;
            }
            if (dgcolumn.Header.ToString() == Properties.Resources.Modbus_Slave_Station_Number)
                mtable.SlaveID = text;
            else if (dgcolumn.Header.ToString() == Properties.Resources.Modbus_Function_Code)
                mtable.HandleCode = text;
            else if (dgcolumn.Header.ToString() == Properties.Resources.Modbus_Slave_Register)
                mtable.SlaveRegister = text;
            else if (dgcolumn.Header.ToString() == Properties.Resources.Modbus_Slave_Length)
                mtable.SlaveCount = text;
            else if (dgcolumn.Header.ToString() == Properties.Resources.Modbus_Master_Register)
                mtable.MasteRegister = text;
            Update(dgcell);
        }

        private void Update(DataGridRow dgrow, bool? isselected = null)
        {
            ModbusItem mtable = (ModbusItem)(dgrow.DataContext);
            foreach (DataGridColumn dgcol in DG_Table.Columns)
            {
                FrameworkElement fele = dgcol.GetCellContent(dgrow);
                if (fele.Parent is DataGridCell)
                {
                    DataGridCell dgcell = (DataGridCell)(fele.Parent);
                    Update(dgcell, isselected);
                }
            }
        }

        private void Update(DataGridCell dgcell, bool? isselected = null)
        {
            ModbusItem mtable = (ModbusItem)(dgcell.DataContext);
            DataGridColumn dgcolumn = dgcell.Column;
            bool isvalid = false;
            if (dgcolumn.Header.ToString() == Properties.Resources.Modbus_Slave_Station_Number)
                isvalid = mtable.SlaveID_IsValid;
            else if (dgcolumn.Header.ToString() == Properties.Resources.Modbus_Function_Code)
                isvalid = mtable.HandleCode_IsValid;
            else if (dgcolumn.Header.ToString() == Properties.Resources.Modbus_Slave_Register)
                isvalid = mtable.SlaveRegister_IsValid;
            else if (dgcolumn.Header.ToString() == Properties.Resources.Modbus_Slave_Length)
                isvalid = mtable.SlaveCount_IsValid;
            else if (dgcolumn.Header.ToString() == Properties.Resources.Modbus_Master_Register)
                isvalid = mtable.MasterRegister_IsValid;
            if (isselected == null)
                isselected = dgcell.IsSelected;
            if (isselected == true && isvalid)
            {
                dgcell.Background = Brushes.Blue;
                dgcell.Foreground = Brushes.White;
                dgcell.FontWeight = FontWeights.Heavy;
            }
            else if (isselected == true && !isvalid)
            {
                dgcell.Background = Brushes.Blue;
                dgcell.Foreground = Brushes.OrangeRed;
                dgcell.FontWeight = FontWeights.Heavy;
            }
            else if (isselected == false && isvalid)
            {
                dgcell.Background = Brushes.White;
                dgcell.Foreground = Brushes.Black;
                dgcell.FontWeight = FontWeights.Light;
            }
            else if (isselected == false && !isvalid)
            {
                dgcell.Background = Brushes.Red;
                dgcell.Foreground = Brushes.Black;
                dgcell.FontWeight = FontWeights.Light;
            }
        }

        #endregion

        #endregion
        
    }
}
