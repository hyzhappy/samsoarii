using SamSoarII.Core.Models;
using SamSoarII.Shell.Models;
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
using System.Collections.Specialized;
using System.ComponentModel;
using SamSoarII.Global;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Utility;

namespace SamSoarII.Shell.Windows
{
    /// <summary>
    /// MonitorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorWindow : UserControl, IWindow, IViewModel, INotifyPropertyChanged
    {
        public MonitorWindow(InteractionFacade _ifParent)
        {
            InitializeComponent();
            ifParent = _ifParent;
            ifParent.PostIWindowEvent += OnReceiveIWindowEvent;
        }
        
        public void Dispose()
        {
            ifParent = null;
            Core = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Core

        private InteractionFacade ifParent;
        public InteractionFacade IFParent { get { return this.ifParent; } }
        public ValueManager ValueManager { get { return ifParent.MNGValue; } }

        private MonitorModel core;
        public MonitorModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                MonitorModel _core = core;
                this.core = value;
                if (_core != null)
                {
                    _core.ChildrenChanged -= OnCoreChildrenChanged;
                    if (_core.View != null) _core.View = null;
                }
                if (core != null)
                {
                    core.ChildrenChanged += OnCoreChildrenChanged;
                    if (core.View != this) core.View = this;
                }
            }
        }
        public IList<MonitorTable> Children { get { return core.Children; } }
        private void OnCoreChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged(this, new PropertyChangedEventArgs("Children"));
        }
        IModel IViewModel.Core { get { return this.Core; } set { this.Core = (MonitorModel)value; } }
        public bool IsBeingMonitored
        {
            get
            {
                if (ifParent.VMDProj == null) return false;
                switch (ifParent.VMDProj.LadderMode)
                {
                    case LadderModes.Edit: return false;
                    case LadderModes.Simulate: return ifParent.CanStop;
                    default: return false;
                }
            }
        }


        private MonitorTable selectedtable;
        public MonitorTable SelectedTable
        {
            get
            {
                return this.selectedtable;
            }
            set
            {
                if (selectedtable == value) return;
                MonitorTable _selectedtable = selectedtable;
                this.selectedtable = value;
                if (_selectedtable != null)
                {
                    textbox.Text = "";
                    _selectedtable.ChildrenChanged -= OnTableChildrenChanged;
                    if (_selectedtable.View != null) _selectedtable.View = null;
                }
                if (selectedtable != null)
                {
                    textbox.Text = selectedtable.Name;
                    _selectedtable.ChildrenChanged += OnTableChildrenChanged;
                    if (_selectedtable.View != this) _selectedtable.View = this;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("HasSelectedTable"));
                PropertyChanged(this, new PropertyChangedEventArgs("TableElements"));
            }
        }
        public bool HasSelectedTable
        {
            get { return SelectedTable != null; }
        }
        public IList<MonitorElement> TableElements { get { return selectedtable != null ? selectedtable.Children : new MonitorElement[] { }; } }
        private void OnTableChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged(this, new PropertyChangedEventArgs("TableElements"));
        }

        #endregion

        #region Shell

        public ProjectViewModel ViewParent { get { return Core.Parent.View; } }
        IViewModel IViewModel.ViewParent { get { return this.ViewParent; } }

        #endregion

        #region Elements

        private void AddElement(MonitorTable table)
        {
            using (AddElementDialog dialog = new AddElementDialog())
            {
                dialog.EnsureButtonClick += (sender1, e1) =>
                {
                    for (int i = 0; i < dialog.AddNums; i++)
                    {
                        MonitorElement element = new MonitorElement(table, 
                            dialog.DataType, dialog.AddrType, (int)(dialog.StartAddr + i), dialog.IntrasegmentType, (int)(dialog.IntrasegmentAddr));
                        table.Children.Add(element);
                    }
                    dialog.Close();
                };
                dialog.ShowDialog();
            }
        }

        private void QuickAddElement()
        {
            using (QuickAddElementDialog dialog = new QuickAddElementDialog())
            {
                dialog.EnsureButtonClick += (sender1, e1) =>
                {
                    LocalizedMessageResult result = LocalizedMessageBox.Show(Properties.Resources.Message_Tooltip, LocalizedMessageButton.OKCancel);
                    if (result == LocalizedMessageResult.Yes)
                    {
                        Children.Clear();
                        ProjectModel project = ifParent.MDProj;
                        Dictionary<string, MonitorTable> dict = new Dictionary<string, MonitorTable>();
                        foreach (LadderDiagramModel diagram in project.Diagrams)
                        {
                            MonitorTable table = new MonitorTable(core, diagram.Name);
                            Children.Add(table);
                            dict.Add(table.Name, table);
                        }
                        foreach (ValueInfo vinfo in ValueManager)
                        {
                            foreach (MonitorTable table in Children)
                                table.IsVisited = false;
                            foreach (ValueModel value in vinfo.Values)
                            {
                                LadderDiagramModel diagram = value.Parent.Parent.Parent;
                                MonitorTable table = dict[diagram.Name];
                                if (!table.IsVisited)
                                {
                                    table.Children.Add(new MonitorElement(table, value.Store));
                                    table.IsVisited = true;
                                }
                            }
                        }
                    }
                };
                dialog.ShowDialog();
            }
        }
        
        public void DeleteElement(IEnumerable<MonitorElement> elements)
        {
            foreach (MonitorElement element in elements.ToArray())
                TableElements.Remove(element);
        }

        public void ShowAddDialog(MonitorElement element)
        {
            using (AddElementDialog dialog = new AddElementDialog())
            {
                ChangeDialogStyle(dialog, element);
                dialog.EnsureButtonClick += (sender1, e1) =>
                {
                    if (!IsElementAdded(dialog, element))
                    {
                        MonitorElement newelement = new MonitorElement(SelectedTable,
                               dialog.DataType, dialog.AddrType, (int)(dialog.StartAddr), dialog.IntrasegmentType, (int)(dialog.IntrasegmentAddr));
                        int id = TableElements.IndexOf(element);
                        TableElements[id] = newelement;
                    }
                    else
                    {
                        LocalizedMessageBox.Show(Properties.Resources.Message_Element_Has_Added, LocalizedMessageIcon.Information);
                    }
                };
                dialog.ShowDialog();
            }
        }
        
        private void ChangeDialogStyle(AddElementDialog dialog, MonitorElement element)
        {
            dialog.Title = string.Format(Properties.Resources.Variable_Modification);
            dialog.Themetextblock.Text = string.Format(Properties.Resources.Variable_Modification);
            dialog.stackpanel.Visibility = Visibility.Hidden;
            dialog.comboBox.SelectedIndex = (int)Enum.Parse(typeof(ElementAddressType), element.AddrType);
            dialog.textBox.Text = element.StartAddr.ToString();
            dialog.DataTypeCombox.SelectedIndex = element.DataType == 0 ? 0 : element.DataType - 1;
            if (element.IsIntrasegment)
            {
                dialog.checkbox1.IsChecked = true;
                dialog.comboBox1.SelectedIndex = element.IntrasegmentType == string.Format("V") ? 0 : 1;
                dialog.textBox1.Text = element.IntrasegmentAddr.ToString();
            }
        }
        
        private bool IsElementAdded(AddElementDialog dialog, MonitorElement element)
        {
            return TableElements.ToList().Exists(x =>
            {
                if (x == element)
                {
                    return false;
                }
                return x.AddrType == dialog.AddrType && x.StartAddr == dialog.StartAddr && dialog.IntrasegmentType == x.IntrasegmentType && dialog.IntrasegmentAddr == x.IntrasegmentAddr;
            });
        }

        public void ShowModifyDialog(MonitorElement element)
        {
            ifParent.ShowValueModifyDialog(new object[] { element });
        }
        
        #endregion

        #region Event Handler

        public event IWindowEventHandler Post = delegate { };

        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {
            
        }

        #region Commands
        
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ifParent == null)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = ifParent.CanExecute(e);
            if (e.Command == MonitorCommand.DeleteElementsCommand)
                e.CanExecute &= ElementDataGrid.SelectedItems != null;
            if (e.Command == MonitorCommand.DeleteAllElementCommand)
                e.CanExecute &= TableElements.Count() > 0;
            if (e.Command == MonitorCommand.AddElementCommand)
                e.CanExecute &= SelectedTable != null;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        { 
            switch (ifParent.VMDProj.LadderMode)
            {
                case LadderModes.Simulate:
                    if (e.Command == MonitorCommand.StartCommand)
                        ifParent.MNGSimu.Start();
                    if (e.Command == MonitorCommand.StopCommand)
                        ifParent.MNGSimu.Abort();
                    break;
            }
            if (e.Command == MonitorCommand.AddElementCommand)
                AddElement(SelectedTable);
            if (e.Command == MonitorCommand.QuickAddElementsCommand)
                QuickAddElement();
            if (e.Command == MonitorCommand.DeleteElementsCommand)
                DeleteElement(ElementDataGrid.SelectedItems.Cast<MonitorElement>());
            if (e.Command == MonitorCommand.DeleteAllElementCommand)
                TableElements.Clear();
        }
        
        #endregion

        #region ContextMenu

        private void AddTableClick(object sender, RoutedEventArgs e)
        {
            string name = "New";
            int retry = 0;
            while (Children.Where(t => t.Name.Equals(name)).Count() > 0)
                name = String.Format("New_{0:d}", ++retry);
            Children.Add(new MonitorTable(core, name));
        }

        private void DeleteTableClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTable != null)
                Children.Remove(SelectedTable);
        }

        private void DeleteAllTables(object sender, RoutedEventArgs e)
        {
            Children.Clear();
        }

        #endregion

        #region Controls

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void textbox_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textbox.Text.Length == 0)
            {
                textbox.Text = SelectedTable.Name;
                return;
            }
            foreach (var table in Children.Where(x => { return x != SelectedTable; }))
            {
                if (textbox.Text.Equals(table.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    LocalizedMessageBox.Show(Properties.Resources.Message_Table_Name_Exist, LocalizedMessageIcon.Warning);
                    return;
                }
            }
            SelectedTable.Name = textbox.Text;
        }

        private void ElementDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MonitorElement element = (MonitorElement)ElementDataGrid.SelectedItem;
            if (element != null && ElementDataGrid.CurrentCell.Column != null)
            {
                if (!IsBeingMonitored)
                {
                    ShowAddDialog(element);
                }
                else
                {
                    ShowModifyDialog(element);
                }
            }
        }
        
        #endregion

        #endregion
        
    }
}
