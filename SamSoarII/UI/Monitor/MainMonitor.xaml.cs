using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstModel;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI.Monitor
{
    /// <summary>
    /// MainMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class MainMonitor : UserControl,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ProjectModel _projectmodel;
        public LadderMode LadderMode
        {
            get
            {
                return _projectmodel.LadderMode;
            }
        }

        public IMonitorManager Manager { get; set; }
        
        public ObservableCollection<MonitorVariableTable> tables { get; set; } 
            = new ObservableCollection<MonitorVariableTable>();
        public MonitorVariableTable SelectTable = null;
        
        //private bool _isBeingMonitored = false;
        public bool IsBeingMonitored
        {
            get
            {
                return Manager != null ? Manager.IsRunning : false;
            }
        }
        public bool IsEnableModify
        {
            get
            {
                return true;
                //return !IsBeingMonitored;
            }
        }
        public bool IsEnableStart
        {
            get
            {
                return _projectmodel != null
                    && LadderMode != LadderMode.Edit
                    && !IsBeingMonitored;
            }
        }
        public bool IsEnableStop
        {
            get
            {
                return IsBeingMonitored;
            }
        }
        
        private string oldTableName;
        public MonitorVariableTable CurrentTable
        {
            get
            {
                return SelectTable;
            }
            set
            {
                SelectTable = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentTable"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentTableName"));
            }
        }
        public string CurrentTableName
        {
            get
            {
                return CurrentTable.TableName;
            }
            set
            {
                CurrentTable.TableName = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentTableName"));
            }
        }

        public MainMonitor(ProjectModel projectModel)
        {
            InitializeComponent();
            MonitorVariableTable table = new MonitorVariableTable(string.Format("table_{0}", tables.Count), this);
            InitializeTableCommand(table);
            tables.Add(table);
            Tables.SelectedIndex = 0;
            CurrentTable = tables[0];
            DataContext = this;
            _projectmodel = projectModel;
        }
        private void InitializeTableCommand(MonitorVariableTable table)
        {
            table.AddElementCommand.Executed += OnAddElementCommandExecute;
            table.AddElementCommand.CanExecute += OnAddElementCommandCanExecute;
            table.QuickAddElementCommand.Executed += OnQuickAddElementsCommandExecute;
            table.QuickAddElementCommand.CanExecute += OnQuickAddElementsCommandCanExecute;
            table.DeleteElementCommand.Executed += OnDeleteElementsCommandExecute;
            table.DeleteElementCommand.CanExecute += OnDeleteElementsCommandCanExecute;
            table.DeleteAllElementCommand.Executed += OnDeleteAllElementCommandExecute;
        }
        private void RemoveTableCommand(MonitorVariableTable table)
        {
            table.AddElementCommand.Executed -= OnAddElementCommandExecute;
            table.AddElementCommand.CanExecute -= OnAddElementCommandCanExecute;
            table.QuickAddElementCommand.Executed -= OnQuickAddElementsCommandExecute;
            table.QuickAddElementCommand.CanExecute -= OnQuickAddElementsCommandCanExecute;
            table.DeleteElementCommand.Executed -= OnDeleteElementsCommandExecute;
            table.DeleteElementCommand.CanExecute -= OnDeleteElementsCommandCanExecute;
            table.DeleteAllElementCommand.Executed -= OnDeleteAllElementCommandExecute;
        }
        private void AddTableClick(object sender, RoutedEventArgs e)
        {
            int cnt = tables.Count;
            while (tables.ToList().Exists(x => { return x.TableName == string.Format("table_{0}", cnt); }))
            {
                cnt++;
            }
            MonitorVariableTable table = new MonitorVariableTable(string.Format("table_{0}", cnt), this);
            InitializeTableCommand(table);
            tables.Add(table);
            Tables.SelectedIndex = tables.Count - 1;
            CurrentTable = tables[tables.Count - 1];
        }
        private void DeleteTableClick(object sender, RoutedEventArgs e)
        {
            if (tables.Count > 1 && CurrentTable != null && tables.Contains(CurrentTable))
            {
                RemoveTableCommand(CurrentTable);
                tables.Remove(CurrentTable);
                Tables.SelectedIndex = 0;
                CurrentTable = tables[0];
            }
        }
        private void DeleteAllTables(object sender, RoutedEventArgs e)
        {
            int cnt = tables.Count;
            for (int i = 0; i < cnt - 1; i++)
            {
                RemoveTableCommand(tables.ElementAt(1));
                tables.RemoveAt(1);
            }
            Tables.SelectedIndex = 0;
            CurrentTable = tables[0];
        }
        private void OnSelected(object sender, RoutedEventArgs e)
        {
            CurrentTable = (sender as ListBoxItem).Content as MonitorVariableTable;
            PropertyChanged.Invoke(this,new PropertyChangedEventArgs("CurrentTableName"));
        }
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            oldTableName = textbox.Text;
        }
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (textbox.Text == string.Empty)
            {
                textbox.Text = oldTableName;
                return;
            }
            foreach (var table in tables.Where(x => { return x != CurrentTable; }))
            {
                if (textbox.Text.Equals(table.TableName,StringComparison.CurrentCultureIgnoreCase))
                {
                    textbox.Text = oldTableName;
                    MessageBox.Show("表格名称已存在!");
                    return;
                }
            }
        }
        private void ShowTable(MonitorVariableTable table)
        {
            if (CurrentTable != table)
            {
                CurrentTable = table;
            }
        }
        private void ShowTable(int index)
        {
            if (index < tables.Count)
            {
                CurrentTable = tables[index];
            }
            else
            {
                CurrentTable = tables[0];
            }
        }

        private bool AssertValueModel(IValueModel model)
        {
            if (model is NullBitValue || model is NullWordValue || model is NullFloatValue
                || model is NullDoubleWordValue || model is HDoubleWordValue || model is KDoubleWordValue
                || model is KFloatValue || model is HWordValue || model is KWordValue
                || model is StringValue || model is ArgumentValue)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private int GetDataType(LadderValueType type)
        {
            switch (type)
            {
                case LadderValueType.Bool:
                    return 0;
                case LadderValueType.DoubleWord:
                    return 3;
                case LadderValueType.Word:
                    return 1;
                case LadderValueType.Float:
                    return 6;
                default:
                    return -1;
            }
        }
        private void AddElement()
        {
            using (AddElementDialog dialog = new AddElementDialog())
            {
                dialog.EnsureButtonClick += (sender1, e1) => 
                {
                    for (int i = 0; i < dialog.AddNums; i++)
                    {
                        ElementModel ele = new ElementModel(dialog.IntrasegmentType != string.Empty,dialog.DataType);
                        ele.AddrType = dialog.AddrType;
                        ele.StartAddr = (uint)(dialog.StartAddr + i);
                        ele.IntrasegmentType = dialog.IntrasegmentType;
                        ele.IntrasegmentAddr = dialog.IntrasegmentAddr;
                        Manager.Add(ele);
                        ele = Manager.Get(ele);
                        CurrentTable.AddElement(ele);
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
                    MessageBoxResult result = MessageBox.Show("已添加软元件将会删除，是否继续?",string.Empty,MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        tables.Clear();
                        if (!(bool)dialog.checkbox.IsChecked)
                        {
                            AddElementByAllRoutine();
                        }
                        else
                        {
                            if (dialog.combox1.SelectedIndex == 0)
                            {
                                AddElementByAllRoutine();
                            }
                            else
                            {
                                if ((bool)dialog.checkbox1.IsChecked && dialog.combox.SelectedIndex == 1)
                                {
                                    int startIndex = int.Parse(dialog.textbox1.Text);
                                    int endIndex = int.Parse(dialog.textbox2.Text);
                                    if (startIndex > endIndex)
                                    {
                                        MessageBox.Show(string.Format("网络范围输入错误!"));
                                        return;
                                    }
                                    else
                                    {
                                        AddElementByCurrentRoutine(false, startIndex, endIndex);
                                    }
                                }
                                else
                                {
                                    AddElementByCurrentRoutine(true,0,0);
                                }
                            }
                        }
                        ShowTable(0);
                        dialog.Close();
                        Manager.Initialize();
                    }
                };
                dialog.ShowDialog();
            }
        }
        private void AddElementByCurrentRoutine(bool isAll,int startindex,int endindex)
        {
            if (isAll)
            {
                AddElementByRoutine(_projectmodel.IFacade.CurrentLadder, new MonitorVariableTable(string.Format("{0}", _projectmodel.IFacade.CurrentLadder.ProgramName), this), 0, _projectmodel.IFacade.CurrentLadder.LadderNetworks.Count - 1);
            }
            else
            {
                AddElementByRoutine(_projectmodel.IFacade.CurrentLadder, new MonitorVariableTable(string.Format("{0}", _projectmodel.IFacade.CurrentLadder.ProgramName), this), startindex, endindex);
            }
        }
        private void AddElementByAllRoutine()
        {
            AddElementByRoutine(_projectmodel.MainRoutine,new MonitorVariableTable(string.Format("{0}", _projectmodel.MainRoutine.ProgramName),this),0, _projectmodel.MainRoutine.LadderNetworks.Count - 1);
            foreach (var routine in _projectmodel.SubRoutines)
            {
                AddElementByRoutine(routine, new MonitorVariableTable(string.Format("{0}", routine.ProgramName), this), 0, routine.LadderNetworks.Count - 1);
            }
        }
        private void AddElementByRoutine(LadderDiagramViewModel ldmodel,MonitorVariableTable table,int startindex,int endindex)
        {
            foreach (var network in ldmodel.LadderNetworks.Where(x => { return x.NetworkNumber >= startindex && x.NetworkNumber <= endindex; }))
            {
                AddElementByNetWork(network, table);
            }
            tables.Add(table);
        }
        private void AddElementByNetWork(LadderNetworkViewModel network, MonitorVariableTable table)
        {
            foreach (var ele in network.LadderElements.Values.Where(x => { return x.Type != ElementType.HLine; }))
            {
                foreach (var model in ele.GetValueModels().Where(x => { return !x.IsVariable; }))
                {
                    if (AssertValueModel(model))
                    {
                        ElementModel elementmodel = new ElementModel();
                        elementmodel.AddrType = model.Base;
                        elementmodel.StartAddr = model.Index;
                        elementmodel.DataType = GetDataType(model.Type);
                        if (model.Offset != WordValue.Null)
                        {
                            elementmodel.IsIntrasegment = true;
                            elementmodel.IntrasegmentType = model.Offset.Base;
                            elementmodel.IntrasegmentAddr = model.Offset.Index;
                        }
                        else
                        {
                            elementmodel.IsIntrasegment = false;
                        }
                        elementmodel.SetShowTypes();
                        table.AddElement(elementmodel);
                    }
                }
            }
        }
        private void DeleteElement()
        {
            var temp = new List<ElementModel>(CurrentTable.ElementDataGrid.SelectedItems.OfType<ElementModel>());
            Manager.Remove(temp);
            foreach (ElementModel item in temp)
            {
                CurrentTable.DeleteElement(item);
            }
        }
        private void DeleteAllElements()
        {
            Manager.Remove(CurrentTable.Elements);
            CurrentTable.DeleteAllElements();
        }
        public XElement CreateXElementByTables()
        {
            XElement rootNode = new XElement("Tables");
            foreach (var table in tables)
            {
                rootNode.Add(table.CreateXElmentByElements());
            }
            return rootNode;
        }
        public void LoadTablesByXElement(XElement rootNode)
        {
            tables.Clear();
            foreach (var node in rootNode.Elements("Table"))
            {
                MonitorVariableTable table = new MonitorVariableTable(node.Attribute("TableName").Value,this);
                table.LoadElementsByXElment(node);
                tables.Add(table);
                InitializeTableCommand(table);
            }
        }

        public void Start()
        {
            Manager.Start();
            //IsBeingMonitored = true;
        }
        public void Stop()
        {
            Manager.Abort();
            //IsBeingMonitored = false;
        }

        #region Command Bindings

        #region Can Execute
        private void OnAddElementCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsEnableModify;
            e.CanExecute = (e.CanExecute && CurrentTable != null);
        }
        private void OnQuickAddElementsCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsEnableModify;
        }
        private void OnDeleteElementsCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsEnableModify;
            e.CanExecute = (e.CanExecute && CurrentTable != null);
            e.CanExecute = (e.CanExecute && CurrentTable.ElementDataGrid.SelectedItems.Count > 0);
        }
        private void OnDeleteAllElementCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsEnableModify;
            e.CanExecute = (e.CanExecute && CurrentTable != null);
            e.CanExecute = (e.CanExecute && CurrentTable.Elements.Count > 0);
        }
        private void OnStartCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsEnableStart;
        }
        private void OnStopCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsEnableStop;
        }
        #endregion

        #region Execute
        private void OnAddElementCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            AddElement();
        }
        private void OnQuickAddElementsCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            QuickAddElement();
        }
        private void OnDeleteElementsCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteElement();
        }
        private void OnDeleteAllElementCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteAllElements();
        }
        private void OnStartCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Start();
        }
        private void OnStopCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Stop();
            ResetShowValue();
        }
        private void ResetShowValue()
        {
            foreach (var table in tables)
            {
                foreach (var ele in table.Elements)
                {
                    ele.CurrentValue = string.Format("????");
                }
            }
        }
        #endregion

        #endregion
    }
}
