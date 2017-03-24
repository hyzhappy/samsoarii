using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SamSoarII.AppMain.Project;
namespace SamSoarII.AppMain.UI
{

    /// <summary>
    /// VariableListControl.xaml 的交互逻辑
    /// </summary>
    public partial class VariableListControl : UserControl, INotifyPropertyChanged, ITabItem
    {
        public IEnumerable<IVariableValue> VariableCollection
        {
            get
            {
                int filterIndex = FilterCombobox.SelectedIndex - 1;
                var temp = VariableManager.VariableCollection.Where(x => (x.VarName.StartsWith(SearchTextBox.Text, StringComparison.CurrentCultureIgnoreCase)));
                if (filterIndex >= 0)
                {
                    LadderValueType type = (LadderValueType)filterIndex;
                    return temp.Where(y => y.Type == type);
                }
                return temp;
            }
        }

        public string TabHeader
        {
            get
            {
                return "元件变量表";
            }
            set
            {

            }
        }

        public VariableListControl()
        {
            InitializeComponent();
            this.DataContext = this;
            ValueCommentManager.ValueCommentChanged += (e) =>
            {
                UpdateVariableCollection();
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region Event handler

        private void OnAddVariable(object sender, RoutedEventArgs e)
        {
            EditVariableDialog dialog = new EditVariableDialog();
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dialog.EnsureButtonClick += (sender1, e1) =>
            {
                try
                {
                    dialog.Commit();
                    UpdateVariableCollection();
                    dialog.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };
            dialog.ShowDialog();
        }

        private void OnDeleteRows(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (IVariableValue row in VariableGrid.SelectedItems)
            {
                VariableManager.RemoveVariable(row);
                if (ValueCommentManager.CheckValueString(row.ValueString))
                {
                    ValueCommentManager.DeleteValueString(row.ValueString);
                }
            }
            UpdateVariableCollection();
        }

        private void DelectRowsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (VariableGrid?.SelectedItems != null)
            {
                e.CanExecute = VariableGrid.SelectedItems.Count > 0;
            }
            else
            {
                e.CanExecute = false;
            }
        }



        private void OnRowMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            var variable = row.Item as IVariableValue;
            EditVariableDialog dialog = new EditVariableDialog(variable);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dialog.EnsureButtonClick += (sender1, e1) =>
            {
                try
                {
                    dialog.Commit();
                    UpdateVariableCollection();
                    dialog.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };
            dialog.ShowDialog();
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateVariableCollection();
        }

        private void OnFilterTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateVariableCollection();
        }
        #endregion



        public void UpdateVariableCollection()
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("VariableCollection"));
        }
    }
}
