using SamSoarII.AppMain.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Xml.Linq;
using System.ComponentModel;
using SamSoarII.ValueModel;
using SamSoarII.PLCDevice;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;

namespace SamSoarII.AppMain.Project
{
    public enum MOVE_DIRECTION
    {
        UP, DOWN
    }
    /// <summary>
    /// ModbusTableViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class ModbusTableViewModel : UserControl, ITabItem, INotifyPropertyChanged
    {
        #region ITabItem Interfaces
        public string TabHeader
        {
            get
            {
                return "Modbus表格";
            }

            set
            {
            }
        }

        private double actualwidth;
        double ITabItem.ActualWidth
        {
            get
            {
                return this.actualwidth;
            }

            set
            {
                this.actualwidth = value;
            }
        }

        private double actualheight;
        double ITabItem.ActualHeight
        {
            get
            {
                return this.actualheight;
            }

            set
            {
                this.actualheight = value;
            }
        }
        
        #endregion

        /// <summary>
        /// 初始化构造函数 
        /// </summary>
        public ModbusTableViewModel(ProjectModel _parent)
        {
            InitializeComponent();
            //InitializeDialog();
            parent = _parent;
            Current = null;
            DataContext = this;
            ModelChanged += OnModelChanged;
        }

        #region Numbers
        
        private ProjectModel parent;

        #region Models & Tables

        private List<ModbusTableModel> models = new List<ModbusTableModel>();
        
        public IEnumerable<ModbusTableModel> Models
        {
            get { return this.models; }
        }
        
        public IEnumerable<ModbusTable> Tables
        {
            get
            {
                if (Current == null) return new List<ModbusTable>();
                return Current.Tables;
            }
        }
        
        #endregion

        #region Cursor

        private ModbusTableModel currentmodel = null;

        private int currentindex = 0;
        
        public ModbusTableModel Current
        {
            get { return this.currentmodel; }
            set
            {
                currentmodel = null;
                currentindex = -1;
                B_RemoveModel.IsEnabled = false;
                B_ModelUp.IsEnabled = false;
                B_ModelDown.IsEnabled = false;
                B_Insert.IsEnabled = false;
                B_Remove.IsEnabled = false;
                B_Up.IsEnabled = false;
                B_Down.IsEnabled = false;
                for (int i = 0; i < models.Count(); i++)
                {
                    ModbusTableModel model = models[i];
                    if (model == value)
                    {
                        LB_Tables.SelectedIndex = i;
                        currentmodel = model;
                        currentindex = i;
                        B_RemoveModel.IsEnabled = true;
                        B_ModelUp.IsEnabled = (i > 0);
                        B_ModelDown.IsEnabled = (i < models.Count() - 1);
                        B_Insert.IsEnabled = true;
                        if (currentmodel.Current >= 0)
                        {
                            B_Remove.IsEnabled = true;
                            B_Up.IsEnabled = (currentmodel.Current > 0);
                            B_Down.IsEnabled = (currentmodel.Current < currentmodel.Tables.Count() - 1);
                        }
                    }
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Tables"));
            }
        }

        public string CurrentName
        {
            get
            {
                if (Current == null) return String.Empty;
                return Current.Name;
            }
            set
            {
                foreach (object obj in LB_Tables.Items)
                {
                    if (obj.ToString().Equals(value))
                    {
                        int id = LB_Tables.Items.IndexOf(obj);
                        LB_Tables.SelectedIndex = id;
                        break;
                    }
                }
            }
        }
        
        #endregion

        #region Dialog
        private ModbusTableDialog dialog;
        private const int DIALOG_CREATE = 0x00;
        private const int DIALOG_RENAME = 0x01;
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
                        dialog.Title = "新建表格";
                        break;
                    case DIALOG_RENAME:
                        dialog.Title = "表格重命名";
                        break;
                }
            }
        }
        
        private void InitializeDialog()
        {
            dialog = new ModbusTableDialog();
            dialog.B_Ensure.Click += OnDialogEnsureClick;
            dialog.B_Cancel.Click += OnDialogCancelClick;
            dialog.Closed += OnDialogClosed;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        #endregion

        #region Device
         
        public Device PLCDevice
        {
            get { return parent.CurrentDevice; }
        }

        #endregion

        #endregion

        #region Modification

        private void _ArguemntsTranslate(ref ModbusTableModel model, ref int index)
        {
            if (model == null && index >= 0 && index < models.Count)
            {
                model = models[index];
            }
            else if (model != null && models.Contains(model))
            {
                index = models.IndexOf(model);
            }
            else if (index < 0 && currentmodel != null)
            {
                model = currentmodel;
                index = currentindex;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void AddModel(ModbusTableModel model = null, int index = -1)
        {
            if (model == null)
            {
                if (dialog == null)
                {
                    InitializeDialog();
                    DialogType = DIALOG_CREATE;
                    dialog.TB_Name.Text = String.Empty;
                    dialog.TB_Comment.Text = String.Empty;
                    dialog.ShowDialog();
                }
                return;
            }
            if (index < 0 && Current != null)
            {
                index = currentindex + 1;
            }
            if (index < 0)
            {
                models.Add(model);
            }
            else
            {
                models.Insert(index, model);
            }
            ModelChanged(this, new RoutedEventArgs());
            Current = model;
        }

        public void RemoveModel(ModbusTableModel model = null, int index = -1)
        {
            _ArguemntsTranslate(ref model, ref index);
            models.Remove(model);
            if (model == Current)
            {
                if (currentindex < models.Count)
                {
                    Current = models[currentindex];
                }
                else if (models.Count > 0)
                {
                    Current = models[models.Count - 1];
                }
                else
                {
                    Current = null;
                }
            }
            else
            {
                Current = model;
            }
            ModelChanged(this, new RoutedEventArgs());
        }

        public void RenameModel(ModbusTableModel model = null, int index = -1)
        {
            _ArguemntsTranslate(ref model, ref index);
            Current = model;
            if (dialog == null)
            {
                InitializeDialog();
                DialogType = DIALOG_RENAME;
                dialog.TB_Name.Text = Current.Name;
                dialog.TB_Comment.Text = Current.Comment;
                dialog.ShowDialog();
            }
        }

        public void MoveModel(ModbusTableModel model = null, int index = -1, MOVE_DIRECTION direct = MOVE_DIRECTION.UP)
        {
            _ArguemntsTranslate(ref model, ref index);
            switch (direct)
            {
                case MOVE_DIRECTION.UP:
                    if (index > 0)
                    {
                        RemoveModel(model);
                        AddModel(model, index - 1);
                    }
                    break;
                case MOVE_DIRECTION.DOWN:
                    if (index < Models.Count() - 1)
                    {
                        RemoveModel(model);
                        AddModel(model, index + 1);
                    }
                    break;
            }
        }
            
        public void AddTable(ModbusTable table = null, int index = -1)
        {
            if (Current != null)
            {
                Current.Add(table, index);
                Current = Current;
            }
        }

        public void RemoveTable(ModbusTable table = null, int index = -1)
        {
            if (Current != null)
            {
                Current.Remove(table, index);
                Current = Current;
            }
        }
        
        public void MoveTable(ModbusTable table = null, int index = -1, MOVE_DIRECTION direct = MOVE_DIRECTION.UP)
        {
            if (Current != null)
            {
                int _currentindex = Current.Current;
                Current.Move(table, index, direct);
                switch (direct)
                {
                    case MOVE_DIRECTION.UP:
                        if (--_currentindex >= 0)
                        {
                            DG_Table.SelectedIndex = _currentindex;
                        }
                        break;
                    case MOVE_DIRECTION.DOWN:
                        if (++_currentindex < Tables.Count())
                        {
                            DG_Table.SelectedIndex = _currentindex;
                        }
                        break;
                }
                Current = Current;
            }
        }

        #endregion

        #region Save & Load XML

        public void Load(XElement xele)
        {
            models.Clear();
            IEnumerable<XElement> xeles_model = xele.Elements("ModbusTableModel");
            foreach (XElement xele_model in xeles_model)
            {
                ModbusTableModel model = new ModbusTableModel();
                model.Load(xele_model);
                models.Add(model);
            }
            Current = models.FirstOrDefault();
            ModelChanged(this, new RoutedEventArgs());
            
        }

        public void Save(XElement xele)
        {
            foreach (ModbusTableModel model in models)
            {
                XElement xele_model = new XElement("ModbusTableModel");
                model.Save(xele_model);
                xele.Add(xele_model);
            }
        }

        #endregion

        #region Event Handler

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Tools
        public event RoutedEventHandler ModelChanged = delegate { };

        public event RoutedEventHandler Close = delegate { };
        
        private void B_Insert_Click(object sender, RoutedEventArgs e)
        {
            AddTable();
        }

        private void B_Remove_Click(object sender, RoutedEventArgs e)
        {
            RemoveTable();
        }

        private void B_Up_Click(object sender, RoutedEventArgs e)
        {
            MoveTable(null, -1, MOVE_DIRECTION.UP);
        }

        private void B_Down_Click(object sender, RoutedEventArgs e)
        {
            MoveTable(null, -1, MOVE_DIRECTION.DOWN);
        }

        private void B_Close_Click(object sender, RoutedEventArgs e)
        {
            Close(this, e);
        }
        
        private void B_AddModel_Click(object sender, RoutedEventArgs e)
        {
            AddModel();
        }

        private void B_RemoveModel_Click(object sender, RoutedEventArgs e)
        {
            RemoveModel();
        }
        
        private void B_ModelUp_Click(object sender, RoutedEventArgs e)
        {
            MoveModel(currentmodel, currentindex, MOVE_DIRECTION.UP);
        }

        private void B_ModelDown_Click(object sender, RoutedEventArgs e)
        {
            MoveModel(currentmodel, currentindex, MOVE_DIRECTION.DOWN);
        }
        #endregion

        #region Dialog

        private void OnDialogEnsureClick(object sender, RoutedEventArgs e)
        {
            string name = dialog.TB_Name.Text;
            string comment = dialog.TB_Comment.Text;
            if (name.Equals(String.Empty))
            {
                MessageBox.Show("表格名称不能为空！");
                return;
            }
            IEnumerable<ModbusTableModel> fit = models.Where(
                (ModbusTableModel model) => { return model.Name.Equals(name); });
            switch (DialogType)
            {
                case DIALOG_CREATE:
                    if (fit.Count() > 0)
                    {
                        MessageBox.Show(String.Format("已存在表格{0:s}！", name));
                    }
                    else
                    {
                        ModbusTableModel model = new ModbusTableModel();
                        model.Name = name;
                        model.Comment = comment;
                        AddModel(model, currentindex);
                        dialog.Close();
                    }
                    break;
                case DIALOG_RENAME:
                    if (fit.Count() > 0 && fit.First() != Current)
                    {
                        MessageBox.Show(String.Format("已存在表格{0:s}！", name));
                    }
                    else
                    {
                        Current.Name = name;
                        Current.Comment = comment;
                        ModelChanged(this, e);
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

        private void OnModelChanged(object sender, RoutedEventArgs e)
        {
            LB_Tables.Items.Clear();
            foreach (ModbusTableModel model in Models)
            {
                LB_Tables.Items.Add(model.Name);
            }
        }

        private void LB_Tables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LB_Tables.SelectedItem != null)
            { 
                string name = LB_Tables.SelectedItem.ToString();
                IEnumerable<ModbusTableModel> fit = Models.Where(
                    (ModbusTableModel model) => { return model.Name.Equals(name); });
                if (fit.Count() > 0)
                {
                    Current = fit.First();
                    return;
                }
            }
            Current = null;
        }
        
        private void DG_Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (Current != null)
            {
                Current.Current = DG_Table.SelectedIndex;
                Current = Current;
            }
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
            ModbusTable mtable = (ModbusTable)(dgcell.DataContext);
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
            switch (dgcolumn.Header.ToString())
            {
                case "从站号":
                    mtable.SlaveID = text; break;
                case "功能码":
                    mtable.HandleCode = text; break;
                case "从站寄存器":
                    mtable.SlaveRegister = text; break;
                case "从站长度":
                    mtable.SlaveCount = text; break;
                case "主站寄存器":
                    mtable.MasteRegister = text; break;
            }
            Update(dgcell);
        }
        
        private void Update(DataGridRow dgrow, bool? isselected = null)
        {
            ModbusTable mtable = (ModbusTable)(dgrow.DataContext);
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
            ModbusTable mtable = (ModbusTable)(dgcell.DataContext);
            DataGridColumn dgcolumn = dgcell.Column;
            bool isvalid = false;
            switch (dgcolumn.Header.ToString())
            {
                case "从站号":
                    isvalid = mtable.SlaveID_IsValid; break;
                case "功能码":
                    isvalid = mtable.HandleCode_IsValid; break;
                case "从站寄存器":
                    isvalid = mtable.SlaveRegister_IsValid; break;
                case "从站长度":
                    isvalid = mtable.SlaveCount_IsValid; break;
                case "主站寄存器":
                    isvalid = mtable.MasterRegister_IsValid; break;
            }
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

    public class ModbusTableComboBoxItems
    {
        static private string[] selectedhandlecodes = {
            "0x01（读位）", "0x02（读位）", "0x03（读字）",  "0x04（读字）",
            "0x05（写位）", "0x06（写字）", "0x0F（写多位）","0x10（写多字）" };
        
        public IEnumerable<string> SelectedHandleCodes()
        {
            return selectedhandlecodes;
        }
    }
    
    public class ModbusTable : INotifyPropertyChanged
    {
        #region Private Numbers

        private string slaveid = String.Empty;
        private string handlecode = String.Empty;
        private string slaveregister = String.Empty;
        private string slavecount = String.Empty;
        private string masteregister = String.Empty;
        
        #endregion

        #region Numbers Interface

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public string SlaveID
        {
            get { return this.slaveid; }
            set
            {
                this.slaveid = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SlaveID"));
            }
        }

        public string HandleCode
        {
            get { return this.handlecode; }
            set
            {
                this.handlecode = value;
                PropertyChanged(this, new PropertyChangedEventArgs("HandleCode"));
            }
        }
        
        public string SlaveRegister
        {
            get { return this.slaveregister; }
            set
            {
                this.slaveregister = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SlaveRegister"));
            }
        }

        public string SlaveCount
        {
            get { return this.slavecount; }
            set
            {
                this.slavecount = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SlaveCount"));
            }
        }

        public string MasteRegister
        {
            get { return this.masteregister; }
            set
            {
                this.masteregister = value;
                PropertyChanged(this, new PropertyChangedEventArgs("MasteRegister"));
            }
        }

        #endregion

        #region Check

        public bool SlaveID_IsValid
        {
            get
            {
                try
                {
                    int slaveid = int.Parse(SlaveID);
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
        }
 
        public bool HandleCode_IsValid
        {
            get
            {
                return HandleCode.Length > 0;
            }
        }
        
        public bool SlaveRegister_IsValid
        {
            get
            {
                try
                {
                    int slaveregister = int.Parse(SlaveRegister);
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
        }

        public bool SlaveCount_IsValid
        {
            get
            {
                try
                {
                    int slavecount = int.Parse(SlaveCount);
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
        }

        public bool MasterRegister_IsValid
        {
            get
            {
                try
                {
                    bool check1 = ValueParser.CheckValueString(MasteRegister, new Regex[] {
                            ValueParser.VerifyWordRegex1});
                    bool check2 = ValueParser.CheckValueString(MasteRegister, new Regex[] {
                            ValueParser.VerifyBitRegex1});
                    switch (HandleCode)
                    {
                        case "0x01（读位）":
                        case "0x02（读位）":
                        case "0x05（写位）":
                        case "0x0F（写多位）":
                            if (!check2)
                            {
                                throw new ValueParseException("需要输入位寄存器！");
                            }
                            ValueParser.ParseBitValue(MasteRegister, PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
                            break;
                        case "0x03（读字）":
                        case "0x04（读字）":
                        case "0x06（写字）":
                        case "0x10（写多字）":
                            if (!check1)
                            {
                                throw new ValueParseException("需要输入单字寄存器！");
                            }
                            ValueParser.ParseWordValue(MasteRegister, PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
                            break;
                        default:
                            return false;
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsValid
        {
            get
            {
                return SlaveID_IsValid
                    && HandleCode_IsValid
                    && SlaveRegister_IsValid
                    && SlaveCount_IsValid
                    && MasterRegister_IsValid;
            }
        }

        #endregion

    }

    public class ModbusTableModel
    {
        public ModbusTableModel()
        {
        }

        public bool IsVaild
        {
            get
            {
                foreach (ModbusTable mtable in Tables)
                {
                    if (mtable.SlaveID.Equals(String.Empty))
                        return false;
                    if (mtable.HandleCode.Equals(String.Empty))
                        return false;
                    if (mtable.SlaveCount.Equals(String.Empty))
                        return false;
                    if (mtable.SlaveRegister.Equals(String.Empty))
                        return false;
                    if (mtable.MasteRegister.Equals(String.Empty))
                        return false;
                }
                return true;
            }
        }

        #region Numbers
        
        #region Name

        private string name;

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        #endregion

        #region Comment

        private string comment;

        public string Comment
        {
            get { return this.comment; }
            set { this.comment = value; }
        }

        #endregion

        #region Tables

        private readonly object _TablesLock = new object();

        private ObservableCollection<ModbusTable> tables = new ObservableCollection<ModbusTable>();
        
        public IEnumerable<ModbusTable> Tables
        {
            get { return this.tables; }
        }

        #endregion

        #region Cursor

        private ModbusTable currenttable;

        private int currentindex = -1;

        public int Current
        {
            get { return this.currentindex; }
            set
            {
                try
                {
                    currenttable = tables[value];
                    currentindex = value;
                }
                catch (ArgumentOutOfRangeException)
                {
                    currenttable = null;
                    currentindex = -1;
                }
            }
        }

        public ModbusTable CurrentTable
        {
            get { return this.currenttable; }
        }

        #endregion

        #endregion
        
        #region Manipulation

        private void _ArguemntsTranslate(ref ModbusTable table, ref int index)
        {
            if (table == null && index >= 0 && index < tables.Count)
            {
                table = tables[index];
            }
            else if (table != null && tables.Contains(table))
            {
                index = tables.IndexOf(table);
            }
            else if (index < 0 && currenttable != null)
            {
                table = currenttable;
                index = currentindex;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void Add(ModbusTable table = null, int index = -1)
        {
            if (index < 0)
            {
                index = Current;
            }
            if (table == null)
            {
                table = new ModbusTable();
            }
            if (index < 0)
            {
                tables.Add(table);
                index = tables.Count - 1;
            }
            else
            {
                tables.Insert(index, table);
            }
            Current = index;
        }

        public void Remove(ModbusTable table = null, int index = -1)
        {
            _ArguemntsTranslate(ref table, ref index);
            tables.Remove(table);
            Current = Math.Max(0, Math.Min(Current, tables.Count - 1));
        }

        public void Move(ModbusTable table = null, int index = -1, MOVE_DIRECTION direct = MOVE_DIRECTION.UP)
        {
            _ArguemntsTranslate(ref table, ref index);
            switch (direct)
            {
                case MOVE_DIRECTION.UP:
                    if (index > 0)
                    {
                        tables.Remove(table);
                        tables.Insert(index - 1, table);
                        Current = index - 1;
                    }
                    break;
                case MOVE_DIRECTION.DOWN:
                    if (index < tables.Count-1)
                    {
                        tables.Remove(table);
                        tables.Insert(index + 1, table);
                        Current = index + 1;
                    }
                    break;
            }
        }

        #endregion

        #region Save & Load XML

        public void Load(XElement xele)
        {
            Name = xele.Attribute("Name").Value;
            Comment = xele.Attribute("Comment").Value;

            tables.Clear();
            IEnumerable<XElement> xeles_table = xele.Elements("ModbusTable");
            foreach (XElement xele_table in xeles_table)
            {
                ModbusTable table = new ModbusTable();
                table.SlaveID = xele_table.Attribute("SlaveID").Value;
                table.HandleCode = xele_table.Attribute("HandleCode").Value;
                table.SlaveRegister = xele_table.Attribute("SlaveRegister").Value;
                table.SlaveCount = xele_table.Attribute("SlaveCount").Value;
                table.MasteRegister = xele_table.Attribute("MasteRegister").Value;
                tables.Add(table);
            }
        }

        public void Save(XElement xele)
        {
            xele.SetAttributeValue("Name", Name);
            xele.SetAttributeValue("Comment", Comment);

            foreach (ModbusTable table in tables)
            {
                XElement xele_table = new XElement("ModbusTable");
                xele_table.SetAttributeValue("SlaveID", table.SlaveID);
                xele_table.SetAttributeValue("HandleCode", table.HandleCode);
                xele_table.SetAttributeValue("SlaveRegister", table.SlaveRegister);
                xele_table.SetAttributeValue("SlaveCount", table.SlaveCount);
                xele_table.SetAttributeValue("MasteRegister", table.MasteRegister);
                xele.Add(xele_table);
            }
        }
        
        #endregion
    }


}
