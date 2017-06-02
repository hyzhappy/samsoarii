using SamSoarII.Communication;
using SamSoarII.Communication.Command;
using SamSoarII.UserInterface;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI.Monitor
{
    /// <summary>
    /// MonitorVariableTable.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorVariableTable : UserControl,INotifyPropertyChanged
    {
        private MainMonitor _parent;
        public int HashCode
        {
            get
            {
                return GetHashCode();
            }
        }
        private int _selectIndex;
        private string _tableName;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public ObservableCollection<ElementModel> Elements { get; set; } = new ObservableCollection<ElementModel>();
        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
                PropertyChanged.Invoke(this,new PropertyChangedEventArgs("TableName"));
            }
        }
        public MonitorVariableTable(string tableName,MainMonitor parent)
        {
            InitializeComponent();
            DataContext = this;
            TableName = tableName;
            if (tableName == "table_0")
            {
                ElementModel model = new ElementModel(false, BitType.BOOL);
                model.AddrType = "X";
                model.StartAddr = 0;
                Elements.Add(model);
            }
            _parent = parent;
        }
        public void AddElement(ElementModel ele)
        {
            if (ele == null)
            {
                return;
            }
            if (!Elements.ToList().Exists(x => { return x.ShowName == ele.ShowName; }))
            {
                Elements.Add(ele);
            }
        }
        public void DeleteElement(ElementModel ele)
        {
            Elements.Remove(ele);
        }
        public void DeleteAllElements()
        {
            if (Elements.Count != 0)
            {
                Elements.Clear();
            }
        }
        public XElement CreateXElmentByElements()
        {
            XElement rootNode = new XElement("Table");
            rootNode.SetAttributeValue("TableName", TableName);
            foreach (var ele in Elements)
            {
                rootNode.Add(ele.CreateXElementBySelf());
            }
            return rootNode;
        }
        public void LoadElementsByXElment(XElement rootNode)
        {
            Elements.Clear();
            foreach (var ele in rootNode.Elements("Element"))
            {
                ElementModel elementModel = new ElementModel();
                elementModel.LoadSelfByXElement(ele);
                AddElement(elementModel);
            }
        }
        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ElementModel element = (ElementModel)ElementDataGrid.SelectedItem;
            if (element != null && ElementDataGrid.CurrentCell.Column != null)
            {
                if (!_parent.IsBeingMonitored)
                {
                    ShowAddDialog(element);
                }
                else
                {
                    ShowModifyDialog(element);
                }
            }
        }
        private void ShowModifyDialog(ElementModel element)
        {
            using (ElementValueModifyDialog dialog = new ElementValueModifyDialog())
            {
                dialog.VarName = element.ShowName;
                dialog.VarType = element.ShowType;
                dialog.Value = element.CurrentValue;
                dialog.ValueModify += (sender, e) =>
                {
                    element.ShowType = dialog.VarType;
                    _parent.Manager.Handle(element, e);
                    if (!element.SetValue.Equals(String.Empty))
                        dialog.Value = element.SetValue;
                };
                dialog.ShowDialog();
            }
        }
        private void Force(ElementModel element, byte value)
        {
            if (_parent.Manager.CanLock)
            {
                _parent.Manager.Lock(element);
            }
            else
            {
                GeneralWriteCommand command = new GeneralWriteCommand(new byte[] { value }, element);
                command.RefElements_A.Add(element);
                _parent.Manager.Add(command);
            }
        }
        private void Write(ElementModel element, byte value)
        {
            if (element.IsIntrasegment)
            {
                IntrasegmentWriteCommand command = new IntrasegmentWriteCommand(new byte[] { value }, element);
                command.RefElement = element;
                _parent.Manager.Add(command);
            }
            else
            {
                GeneralWriteCommand command = new GeneralWriteCommand(new byte[] { value }, element);
                command.RefElements_A.Add(element);
                _parent.Manager.Add(command);
            }
        }
        private void Write(ElementModel element)
        {
            string value = element.SetValue;
            byte[] data;
            element.SetValue = value;
            switch (element.ShowType)
            {
                case "WORD":
                    data = ValueConverter.GetBytes(
                        (UInt16)(Int16.Parse(value)));
                    break;
                case "UWORD":
                    data = ValueConverter.GetBytes(
                        UInt16.Parse(value));
                    break;
                case "BCD":
                    data = ValueConverter.GetBytes(
                        ValueConverter.ToUINT16(
                            UInt16.Parse(value)));
                    break;
                case "DWORD":
                    data = ValueConverter.GetBytes(
                        (UInt32)(Int32.Parse(value)));
                    break;
                case "UDWORD":
                    data = ValueConverter.GetBytes(
                        UInt32.Parse(value));
                    break;
                case "FLOAT":
                    data = ValueConverter.GetBytes(
                        ValueConverter.FloatToUInt(
                            float.Parse(value)));
                    break;
                default:
                    data = new byte[0];
                    break;
            }
            if (element.IsIntrasegment)
            {
                IntrasegmentWriteCommand command = new IntrasegmentWriteCommand(data, element);
                command.RefElement = element;
                _parent.Manager.Add(command);
            }
            else
            {
                GeneralWriteCommand command = new GeneralWriteCommand(data, element);
                command.RefElements_A.Add(element);
                _parent.Manager.Add(command);
            }
        }

        private bool IsBitAddr(ElementAddressType Type)
        {
            switch (Type)
            {
                case ElementAddressType.X:
                case ElementAddressType.Y:
                case ElementAddressType.M:
                case ElementAddressType.S:
                case ElementAddressType.C:
                case ElementAddressType.T:
                    return true;
                default:
                    return false;
            }
        }
        private uint? ParseValueByDataType(string value,WordType type)
        {
            try
            {
                switch (type)
                {
                    case WordType.INT16:
                        return (uint)short.Parse(value);
                    case WordType.POS_INT16:
                        return ushort.Parse(value);
                    case WordType.INT32:
                        return (uint)int.Parse(value);
                    case WordType.POS_INT32:
                        return uint.Parse(value);
                    case WordType.BCD:
                        ushort outvalue = ValueConverter.ToUINT16(ushort.Parse(value));
                        if (outvalue > 9999)
                        {
                            throw new OverflowException();
                        }
                        else
                        {
                            return outvalue;
                        }
                    case WordType.FLOAT:
                        return ValueConverter.FloatToUInt(float.Parse(value));
                    default:
                        return null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("数值输入错误!");
                return null;
            }
        }
        private void ShowAddDialog(ElementModel element)
        {
            using (AddElementDialog dialog = new AddElementDialog())
            {
                ChangeDialogStyle(dialog, element);
                dialog.EnsureButtonClick += (sender1, e1) =>
                {
                    if (!IsElementAdded(dialog, element))
                    {
                        if (dialog.IntrasegmentType != string.Empty)
                        {
                            element.IsIntrasegment = true;
                            element.IntrasegmentType = dialog.IntrasegmentType;
                            element.IntrasegmentAddr = dialog.IntrasegmentAddr;
                        }
                        else
                        {
                            element.IsIntrasegment = false;
                            element.IntrasegmentType = string.Empty;
                            element.IntrasegmentAddr = 0;
                        }
                        element.DataType = dialog.DataType;
                        element.AddrType = dialog.AddrType;
                        element.StartAddr = dialog.StartAddr;
                        element.ShowTypes = dialog.DataTypes;
                        element.ShowPropertyChanged();
                        dialog.Close();
                    }
                    else
                    {
                        MessageBox.Show("该软元件已添加!");
                    }
                };
                dialog.ShowDialog();
            }
        }
        private bool IsElementAdded(AddElementDialog dialog, ElementModel element)
        {
            if (Elements.ToList().Exists(x => 
            {
                if (x == element)
                {
                    return false;
                }
                return x.AddrType == dialog.AddrType && x.StartAddr == dialog.StartAddr && dialog.IntrasegmentType == x.IntrasegmentType && dialog.IntrasegmentAddr == x.IntrasegmentAddr;
            }))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void ChangeDialogStyle(AddElementDialog dialog, ElementModel element)
        {
            dialog.Title = string.Format("变量修改");
            dialog.Themetextblock.Text = string.Format("变量修改");
            dialog.stackpanel.Visibility = Visibility.Hidden;
            dialog.comboBox.SelectedIndex = (int)Enum.Parse(typeof(ElementAddressType),element.AddrType);
            dialog.textBox.Text = element.StartAddr.ToString();
            dialog.DataTypeCombox.SelectedIndex = element.DataType == 0 ? 0 : element.DataType - 1;
            if (element.IsIntrasegment)
            {
                dialog.checkbox1.IsChecked = true;
                dialog.comboBox1.SelectedIndex = element.IntrasegmentType == string.Format("V") ? 0 : 1;
                dialog.textBox1.Text = element.IntrasegmentAddr.ToString();
            }
        }
        private void OnDeleteAllElementCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Elements.Count > 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbox = (ComboBox)sender;
            if (cbox.Parent is DataGridCell)
            {
                DataGridCell dgcell = (DataGridCell)(cbox.Parent);
                if (dgcell.DataContext is ElementModel)
                {
                    ElementModel emodel_old = (ElementModel)(dgcell.DataContext);
                    ElementModel emodel_new = new ElementModel(emodel_old);
                    if (e.AddedItems.Count <= 0
                     || emodel_new.ShowType.Equals(e.AddedItems[0].ToString()))
                    {
                        return;
                    }
                    emodel_new.ShowType = e.AddedItems[0].ToString();
                    _parent.Manager.Replace(emodel_old, emodel_new);
                    emodel_new = _parent.Manager.Get(emodel_new);
                    int id = Elements.IndexOf(emodel_old);
                    Elements[id] = emodel_new;
                    dgcell.DataContext = emodel_new;
                }
            }
        }
    }
}
