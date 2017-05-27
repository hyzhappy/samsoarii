using SamSoarII.PLCDevice;
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// ElementInitializeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ElementInitializeWindow : UserControl, INotifyPropertyChanged
    {
        private bool isTypeChanged = false;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ObservableCollection<IElementInitializeModel> Elements { get; set; } = new ObservableCollection<IElementInitializeModel>();
        public ElementInitializeWindow()
        {
            InitializeComponent();
            DataContext = this;
            CloseButton.Click += CloseButton_Click;
            KeyDown += ElementInitializeWindow_KeyDown;
            AddButton.Click += AddButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            DeleteAllButton.Click += DeleteAllButton_Click;
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var temp = new List<IElementInitializeModel>(ElementDataGrid.SelectedItems.OfType<IElementInitializeModel>());
            foreach (IElementInitializeModel item in temp)
            {
                Elements.Remove(item);
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ElementAddressType Type = (ElementAddressType)Enum.ToObject(typeof(ElementAddressType), EleTypeCombox.SelectedIndex);
            Device device = PLCDeviceManager.GetPLCDeviceManager().SelectDevice;
            if (EleTypeCombox.SelectedIndex == 0 || EleTypeCombox.SelectedIndex == 1 || EleTypeCombox.SelectedIndex == 13 || EleTypeCombox.SelectedIndex == 14)
            {
                MessageBox.Show("该类寄存器不可写入!");
            }
            else if(!ElementAddressHelper.AssertAddrRange(Type,uint.Parse(textBox.Text) + uint.Parse(LengthTextbox.Text), device))
            {
                MessageBox.Show("超过最大长度范围,请重新输入!");
            }
            else
            {
                if (ElementAddressHelper.IsBitAddr(Type))
                {
                    for (uint i = 0; i < uint.Parse(LengthTextbox.Text); i++)
                    {
                        AddElement(GenerateElementModel(true, Type.ToString(), uint.Parse(textBox.Text) + i, 0));
                    }
                }
                else
                {
                    for (uint i = 0; i < uint.Parse(LengthTextbox.Text); i++)
                    {
                        AddElement(GenerateElementModel(false, Type.ToString(), uint.Parse(textBox.Text) + i, DataTypeCombox.SelectedIndex + 1));
                    }
                }
            }
        }
        private void AddElement(IElementInitializeModel element)
        {
            if (!Elements.ToList().Exists(x => { return x.Base == element.Base && x.Offset == element.Offset; }))
            {
                Elements.Add(element);
            }
        }
        private IElementInitializeModel GenerateElementModel(bool isBit,string Base,uint Offset, int DataType)
        {
            IElementInitializeModel model;
            if (isBit)
            {
                model = new BitElementModel();
                model.ShowValue = string.Format("OFF");
            }
            else
            {
                model = new WordElementModel();
                model.ShowValue = 0.ToString();
            }
            model.Base = Base;
            model.Offset = Offset;
            model.Value = 0;
            model.DataType = DataType;
            return model;
        }
        private void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            Elements.Clear();
        }
        private void ElementInitializeWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else
            {
                if (e.Key == Key.Escape)
                {
                    CloseButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            //Close();
        }

        public string[] DataTypes
        {
            get
            {
                switch (EleTypeCombox.SelectedIndex)
                {
                    case 0:case 1:case 2:
                    case 3:case 4:case 5:
                        return Enum.GetNames(typeof(BitType));
                    case 6:case 7:case 8:
                    case 9:case 10:case 11:case 12:
                        return Enum.GetNames(typeof(WordType));
                    default:
                        return null;
                }
            }
        }
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (isTypeChanged)
            {
                isTypeChanged = false;
                DataTypeCombox.SelectedIndex = 0;
            }
        }
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("DataTypes"));
            if (textBox != null)
            {
                textBox.Text = string.Empty;
                isTypeChanged = true;
                textBox.Text = 0.ToString();
            }
        }
        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            IElementInitializeModel model = e.Row.DataContext as IElementInitializeModel;
            if (e.EditingElement is TextBox)
            {
                TextBox textBox = e.EditingElement as TextBox;
                if (model is BitElementModel)
                {
                    if (model.ShowValue.Trim().ToUpper() != "ON" && model.ShowValue.Trim().ToUpper() != "OFF")
                    {
                        e.Cancel = true;
                        MessageBox.Show("输入值非法!");
                    }
                    else
                    {
                        if (model.ShowValue.Trim().ToUpper() == "ON")
                        {
                            model.Value = 1;
                        }
                        else
                        {
                            model.Value = 0;
                        }
                    }
                }
                else
                {
                    try
                    {
                        uint tempvalue = ValueConverter.ParseShowValue(textBox.Text,(WordType)Enum.ToObject(typeof(WordType),model.DataType));
                        model.Value = tempvalue;
                    }
                    catch (Exception ex)
                    {
                        e.Cancel = true;
                        MessageBox.Show("输入值非法!");
                    }
                }
            }
            else
            {
                ComboBox combox = e.EditingElement as ComboBox;
                if (model is WordElementModel && model.SelectIndex != combox.SelectedIndex)
                {
                    WordType oldType = (WordType)Enum.ToObject(typeof(WordType), model.SelectIndex + 1);
                    WordType newType = (WordType)Enum.ToObject(typeof(WordType), combox.SelectedIndex + 1);
                    try
                    {
                        model.ShowValue = ValueConverter.ChangeShowValue(oldType, newType, model.Value);
                    }
                    catch (Exception ex)
                    {
                        e.Cancel = true;
                        MessageBox.Show("类型之间无法转换!");
                    }
                }
            }
        }
        public XElement CreatXElementByElements()
        {
            XElement rootNode = new XElement("EleInitialize");
            foreach (var ele in Elements)
            {
                rootNode.Add(ele.CreateXElementByModel());
            }
            return rootNode;
        }
        public void LoadElementsByXElement(XElement rootNode)
        {
            foreach (var node in rootNode.Elements("EleModel"))
            {
                string type = node.Attribute("Type").Value;
                IElementInitializeModel model;
                if (type == string.Format("Bit"))
                {
                    model = new BitElementModel();
                }
                else
                {
                    model = new WordElementModel();
                }
                model.LoadByXElement(node);
                Elements.Add(model);
            }
        }
    }
}
