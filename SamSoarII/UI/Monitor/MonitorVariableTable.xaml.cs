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
        private bool _isModify = true;
        public bool IsModify
        {
            get
            {
                if (_isModify)
                {
                    return true;
                }
                else
                {
                    foreach (var ele in Elements)
                    {
                        if (ele.IsModify)
                        {
                            _isModify = true;
                            return true;
                        }
                    }
                    return false;
                }
            }
            set
            {
                _isModify = value;
                if (!_isModify)
                {
                    foreach (var ele in Elements)
                    {
                        ele.IsModify = false;
                    }
                }
            }
        }
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
            TableName = tableName;
            if (tableName == "table_0")
            {
                ElementModel model = new ElementModel(false, BitType.BOOL);
                model.AddrType = "X";
                model.StartAddr = 0;
                Elements.Add(model);
            }
            _parent = parent;
            DataContext = this;
        }
        public void AddElement(ElementModel ele)
        {
            if (!Elements.ToList().Exists(x => { return x.ShowName == ele.ShowName; }))
            {
                Elements.Add(ele);
                IsModify = true;
            }
            else
            {
                MessageBox.Show(string.Format("{0}元件已添加!",ele.ShowName));
            }
        }
        public void DeleteElement(ElementModel ele)
        {
            Elements.Remove(ele);
            IsModify = true;
        }
        public void DeleteAllElements()
        {
            if (Elements.Count != 0)
            {
                Elements.Clear();
                IsModify = true;
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
                Elements.Add(elementModel);
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
                ((TextBlock)dialog.FindName("ElementNameTextBlock")).Text = element.ShowName;
                if (element.AddrType == "X" || element.AddrType == "Y")
                {
                    InitializeForceDialog(dialog, element);
                }
                else if(IsBitAddr((ElementAddressType)Enum.Parse(typeof(ElementAddressType),element.AddrType)))
                {
                    InitializeBitDialog(dialog, element);
                }
                else
                {
                    InitializeWordDialog(dialog, element);
                }
                dialog.ShowDialog();
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
        private void InitializeForceDialog(ElementValueModifyDialog dialog, ElementModel element)
        {
            ((Grid)dialog.FindName("Content1")).Visibility = Visibility.Visible;
            dialog.ForceButtonClick += (sender1, e1) =>
            {
                byte value;
                if (sender1 == dialog.FindName("ForceON") || sender1 == dialog.FindName("ForceOFF"))
                {
                    if (sender1 == dialog.FindName("ForceON")) value = 0x01;
                    else value = 0x00;
                    GeneralWriteCommand command = new GeneralWriteCommand(new byte[] { value },element);
                    command.RefElements_A.Add(element);
                    _parent.dataHandle.WriteCommands.Enqueue(command);
                }
                else if (sender1 == dialog.FindName("UndoForce"))
                {
                    ForceCancelCommand command = new ForceCancelCommand(false,element);
                    _parent.dataHandle.WriteCommands.Enqueue(command);
                }
                else
                {
                    ForceCancelCommand command = new ForceCancelCommand(true,element);
                    _parent.dataHandle.WriteCommands.Enqueue(command);
                }
            };
        }
        private void InitializeBitDialog(ElementValueModifyDialog dialog, ElementModel element)
        {
            ((StackPanel)dialog.FindName("Content2")).Visibility = Visibility.Visible;
            dialog.BitButtonClick += (sender1, e1) =>
            {
                byte bitvalue;
                if (sender1 == dialog.FindName("WriteON")) bitvalue = 0x01;
                else bitvalue = 0x00;
                if (element.IsIntrasegment)
                {
                    IntrasegmentWriteCommand command = new IntrasegmentWriteCommand(new byte[] { bitvalue },element);
                    command.RefElement = element;
                    _parent.dataHandle.WriteCommands.Enqueue(command);
                }
                else
                {
                    GeneralWriteCommand command = new GeneralWriteCommand(new byte[] { bitvalue },element);
                    command.RefElements_A.Add(element);
                    _parent.dataHandle.WriteCommands.Enqueue(command);
                }
            };
        }
        private void InitializeWordDialog(ElementValueModifyDialog dialog, ElementModel element)
        {
            ((StackPanel)dialog.FindName("Content3")).Visibility = Visibility.Visible;
            WordType type = (WordType)Enum.ToObject(typeof(WordType), element.DataType);
            ((TextBlock)dialog.FindName("DataTypeTextBlock")).Text = type.ToString();
            //((ComboBox)dialog.FindName("DataTypeCombox")).SelectedIndex = element.DataType - 1;
            dialog.WordButtonClick += (sender1, e1) =>
            {
                var obj = ParseValueByDataType(((TextBox)dialog.FindName("ValueTextBox")).Text, type);
                if (obj != null)
                {
                    uint value = (uint)obj;
                    byte[] data;
                    if (type == WordType.BCD || type == WordType.INT16 || type == WordType.POS_INT16)
                    {
                        data = ValueConverter.GetBytes((ushort)value);
                    }
                    else
                    {
                        data = ValueConverter.GetBytes(value);
                    }
                    if (element.IsIntrasegment)
                    {
                        IntrasegmentWriteCommand command = new IntrasegmentWriteCommand(data,element);
                        command.RefElement = element;
                        _parent.dataHandle.WriteCommands.Enqueue(command);
                    }
                    else
                    {
                        GeneralWriteCommand command = new GeneralWriteCommand(data,element);
                        command.RefElements_A.Add(element);
                        _parent.dataHandle.WriteCommands.Enqueue(command);
                    }
                }
            };
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
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            ComboBox combox = sender as ComboBox;
            _selectIndex = combox.SelectedIndex;
        }
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (_parent.IsBeingMonitored)
            {
                MessageBox.Show("监视时无法改变数据类型!");
                ((ComboBox)sender).SelectedIndex = _selectIndex;
            }
        }
        public List<ICommunicationCommand> GenerateReadCommands()
        {
            List<ICommunicationCommand> commands = new List<ICommunicationCommand>();
            Queue<string> tempQueue_Base = new Queue<string>();
            foreach (var ele in Elements)
            {
                if (!tempQueue_Base.Contains(ele.AddrType))
                {
                    tempQueue_Base.Enqueue(ele.AddrType);
                }
            }
            string str;
            uint cnt;
            GeneralReadCommand command_base = new GeneralReadCommand();//非变址命令最多可以包含两种基地址
            while (tempQueue_Base.Count > 0)
            {
                str = tempQueue_Base.Dequeue();
                List<ElementModel> elements = Elements.Where(x => { return x.AddrType == str && !x.IsIntrasegment; }).OrderBy(x => { return x.StartAddr; }).ToList();
                if (elements.Count > 0)
                {
                    cnt = elements.First().StartAddr;
                    foreach (var ele in elements)
                    {
                        if (ele.StartAddr != cnt)
                        {
                            cnt = ele.StartAddr;
                            if (command_base.RefElements_B.Count == 0)
                            {
                                command_base.RefElements_B.Add(ele);
                            }
                            else
                            {
                                command_base.InitializeCommandByElement();
                                commands.Add(command_base);
                                command_base = new GeneralReadCommand();
                                command_base.RefElements_A.Add(ele);
                            }
                        }
                        else
                        {
                            if (command_base.RefElements_A.Count < CommunicationDataDefine.MAX_ELEM_NUM && command_base.RefElements_B.Count == 0)
                            {
                                if (command_base.RefElements_A.Count > 0 && command_base.RefElements_A.Last().AddrType != ele.AddrType)
                                {
                                    command_base.RefElements_B.Add(ele);
                                }
                                else
                                {
                                    if (ele.AddrType == string.Format("CV") && ele.StartAddr == 200)
                                    {
                                        command_base.RefElements_B.Add(ele);
                                    }
                                    else
                                    {
                                        command_base.RefElements_A.Add(ele);
                                    }
                                }
                            }
                            else
                            {
                                if (command_base.RefElements_B.Count < CommunicationDataDefine.MAX_ELEM_NUM)
                                {
                                    if (command_base.RefElements_B.Count > 0 && command_base.RefElements_B.Last().AddrType != ele.AddrType)
                                    {
                                        command_base.InitializeCommandByElement();
                                        commands.Add(command_base);
                                        command_base = new GeneralReadCommand();
                                        command_base.RefElements_A.Add(ele);
                                    }
                                    else
                                    {
                                        if (ele.AddrType == string.Format("CV") && ele.StartAddr == 200)
                                        {
                                            command_base.InitializeCommandByElement();
                                            commands.Add(command_base);
                                            command_base = new GeneralReadCommand();
                                            command_base.RefElements_A.Add(ele);
                                        }
                                        else
                                        {
                                            command_base.RefElements_B.Add(ele);
                                        }
                                    }
                                }
                                else
                                {
                                    command_base.InitializeCommandByElement();
                                    commands.Add(command_base);
                                    command_base = new GeneralReadCommand();
                                    command_base.RefElements_A.Add(ele);
                                }
                            }
                        }
                        cnt++;
                    }
                }
                elements = Elements.Where(x => { return x.AddrType == str && x.IsIntrasegment; }).ToList();
                if (elements.Count > 0)
                {
                    IntrasegmentReadCommand command_intra = new IntrasegmentReadCommand();//变址命令基地址类型必须一致,
                    Tuple<string, uint> tuple;
                    Queue<Tuple<string,uint>> tempQuene_Intra = new Queue<Tuple<string, uint>>();
                    foreach (var ele in elements)
                    {
                        if (!tempQuene_Intra.ToList().Exists(x => { return x.Item1 == ele.IntrasegmentType && x.Item2 == ele.IntrasegmentAddr; }))
                        {
                            tempQuene_Intra.Enqueue(new Tuple<string, uint>(ele.IntrasegmentType,ele.IntrasegmentAddr));
                        }
                    }
                    while (tempQuene_Intra.Count > 0)
                    {
                        tuple = tempQuene_Intra.Dequeue();
                        var templist = elements.Where(x => { return x.IntrasegmentType == tuple.Item1 && x.IntrasegmentAddr == tuple.Item2; }).OrderBy(x => { return x.StartAddr; });
                        cnt = templist.First().StartAddr;
                        foreach (var ele in templist)
                        {
                            if (ele.StartAddr != cnt)
                            {
                                cnt = ele.StartAddr;
                                command_intra.InitializeCommandByElement();
                                commands.Add(command_intra);
                                command_intra = new IntrasegmentReadCommand();
                                command_intra.RefElements.Add(ele);
                            }
                            else
                            {
                                if (command_intra.RefElements.Count < CommunicationDataDefine.MAX_ELEM_NUM)
                                {
                                    if (ele.AddrType == string.Format("CV") && ele.StartAddr == 200)
                                    {
                                        command_intra.InitializeCommandByElement();
                                        commands.Add(command_intra);
                                        command_intra = new IntrasegmentReadCommand();
                                        command_intra.RefElements.Add(ele);
                                    }
                                    else
                                    {
                                        command_intra.RefElements.Add(ele);
                                    }
                                }
                                else
                                {
                                    command_intra.InitializeCommandByElement();
                                    commands.Add(command_intra);
                                    command_intra = new IntrasegmentReadCommand();
                                    command_intra.RefElements.Add(ele);
                                }
                            }
                            cnt++;
                        }
                    }
                    command_intra.InitializeCommandByElement();
                    commands.Add(command_intra);//添加剩余命令
                }
            }
            command_base.InitializeCommandByElement();
            commands.Add(command_base);//添加剩余命令
            return commands;
        }
        private void OnDeleteAllElementCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Elements.Count > 0;
        }
    }
}
