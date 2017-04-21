using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
using SamSoarII.PLCDevice;
using SamSoarII.UserInterface;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// ElementList.xaml 的交互逻辑
    /// </summary>
    public class ValueCommentAlias : INotifyPropertyChanged
    {
        private string _name;
        private string _comment;
        private string _alias;
        private List<TextBlock> _mappedModels;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }
        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Comment"));
            }
        }
        public string Alias
        {
            get
            {
                return _alias;
            }
            set
            {
                _alias = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Alias"));
            }
        }
        public List<TextBlock> MappedModels
        {
            get
            {
                return _mappedModels;
            }
            set
            {
                _mappedModels = value;
                PropertyChanged.Invoke(this,new PropertyChangedEventArgs("MappedModels"));
            }
        }
        private bool _hasComment = false;
        private bool _hasAlias = false;
        public bool HasComment { get { return _hasComment; } set { _hasComment = value; } }
        public bool HasAlias { get { return _hasAlias; } set { _hasAlias = value; } }
        public bool HasUsed { get; set; }
        public ValueCommentAlias(string Name, string Comment, string Alias)
        {
            this.Name = Name;
            this.Comment = Comment;
            this.Alias = Alias;
            _mappedModels = new List<TextBlock>();
            TextBlock textblock = new TextBlock();
            textblock.Text = (new VerticalLineViewModel()).ToString();
            _mappedModels.Add(textblock);
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
    public partial class ElementList : Window, INotifyPropertyChanged
    {
        private bool _hasUsed = false;
        private bool _hasComment = false;
        private bool _showDetails = false;
        private TextBlock _currentTextBlock;
        private static List<ValueCommentAlias> _elementCollection = new List<ValueCommentAlias>();
        public IEnumerable<ValueCommentAlias> ElementCollection
        {
            get
            {
                var tempList = new List<ValueCommentAlias>(_elementCollection);
                if(_currentTextBlock != null)
                {
                    string tempstr = _currentTextBlock.Text;
                    int index = stackpanel_textblock.Children.IndexOf(_currentTextBlock);
                    switch (index)
                    {
                        case 0:case 1:case 2:case 3:case 6:case 7:case 8:
                            tempList = tempList.Where(x => { return x.Name.StartsWith(tempstr.Substring(0, 1)); }).ToList();
                            break;
                        case 9:case 12:case 13:
                            tempList = tempList.Where(x => { return x.Name.StartsWith(tempstr.Substring(0, 2)); }).ToList();
                            break;
                        case 4:case 5:
                            tempList = tempList.Where(x => { return x.Name.StartsWith(tempstr.Substring(0, 1)) && !x.Name.Contains("V"); }).ToList();
                            break;
                        case 10:
                            tempList = tempList.Where(x => { return x.Name.StartsWith(tempstr.Substring(0, 2)) && CheckValue(x.Name); }).ToList();
                            break;
                        case 11:
                            tempList = tempList.Where(x => { return x.Name.StartsWith(tempstr.Substring(0, 2)) && !CheckValue(x.Name); }).ToList();
                            break;
                        default:
                            break;
                    }
                }
                if (_hasUsed)
                {
                    tempList = tempList.Where(x => { return InstructionCommentManager.ContainValueString(x.Name); }).ToList();
                }
                if (_hasComment)
                {
                    tempList = tempList.Where(x => { return x.HasComment; }).ToList();
                }
                tempList = tempList.Where(x => { return x.Name.StartsWith(SearchTextBox.Text.ToUpper()); }).ToList();
                return tempList;
            }
        }
        private bool CheckValue(string valueString)
        {
            int value = int.Parse(valueString.Substring(2));
            return value >= 0 && value < 200;//此处用Device类的CV范围代替
        }
        public static event NavigateToNetworkEventHandler NavigateToNetwork = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        #region InitializeElementCollection
        public static void InitializeElementCollection()
        {
            Device device = new FGs16MRDevice();//此处应用用户选择的设备类型替代
            for (uint i = device.XRange.Start; i < device.XRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("X{0}", i), string.Empty, string.Empty));
            }
            for (uint i = device.YRange.Start; i < device.YRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("Y{0}", i), string.Empty, string.Empty));
            }
            for (uint i = device.MRange.Start; i < device.MRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("M{0}", i), string.Empty, string.Empty));
            }
            for (uint i = device.SRange.Start; i < device.SRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("S{0}", i), string.Empty, string.Empty));
            }
            for (uint i = device.CRange.Start; i < device.CRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("C{0}", i), string.Empty, string.Empty));
            }
            for (uint i = device.TRange.Start; i < device.TRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("T{0}", i), string.Empty, string.Empty));
            }
            for (uint i = device.DRange.Start; i < device.DRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("D{0}", i), string.Empty, string.Empty));
            }
            for (uint i = device.CVRange.Start; i < device.CVRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("CV{0}", i), string.Empty, string.Empty));
            }
            for (uint i = device.TVRange.Start; i < device.TVRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("TV{0}", i), string.Empty, string.Empty));
            }
            for (uint i = device.VRange.Start; i < device.VRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("V{0}", i), string.Empty, string.Empty));
            }
            for (uint i = device.ZRange.Start; i < device.ZRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("Z{0}", i), string.Empty, string.Empty));
            }
            for (uint i = device.AIRange.Start; i < device.AIRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("AI{0}", i), string.Empty, string.Empty));
            }
            for (uint i = device.AORange.Start; i < device.AORange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("AO{0}", i), string.Empty, string.Empty));
            }
        }
        #endregion
        public ElementList()
        {
            InitializeComponent();
            DataContext = this;
        }
        public static void InstructionCommentManager_MappedMessageChanged(MappedMessageChangedEventArgs e)
        {
            IEnumerable<ValueCommentAlias> fit = _elementCollection.Where(x => { return x.Name == e.ValueString; });
            if (fit.Count() == 0)
            {
                return;
            }
            var valueCommentAlias = fit.First();
            List<TextBlock> mappedModels;
            switch (e.Type)
            {
                case MappedMessageChangedType.Add:
                    if (!valueCommentAlias.MappedModels.Exists(x => { return x.Text == e.MappedValueModel.ToString(); }))
                    {
                        mappedModels = new List<TextBlock>(valueCommentAlias.MappedModels);
                        TextBlock textblock = new TextBlock();
                        textblock.Text = e.MappedValueModel.ToString();
                        mappedModels.Add(textblock);
                        valueCommentAlias.MappedModels = mappedModels;
                    }
                    break;
                case MappedMessageChangedType.AddFirst:
                    valueCommentAlias.MappedModels.Clear();
                    mappedModels = new List<TextBlock>(valueCommentAlias.MappedModels);
                    TextBlock textblock1 = new TextBlock();
                    textblock1.Text = (new HorizontalLineViewModel()).ToString();
                    mappedModels.Add(textblock1);
                    TextBlock textblock2 = new TextBlock();
                    textblock2.Text = e.MappedValueModel.ToString();
                    mappedModels.Add(textblock2);
                    valueCommentAlias.MappedModels = mappedModels;
                    break;
                case MappedMessageChangedType.Remove:
                    mappedModels = new List<TextBlock>(valueCommentAlias.MappedModels);
                    foreach (var model in mappedModels)
                    {
                        if (model.Text == e.MappedValueModel.ToString())
                        {
                            mappedModels.Remove(model);
                            break;
                        }
                    }
                    valueCommentAlias.MappedModels = mappedModels;
                    break;
                case MappedMessageChangedType.RemoveLast:
                    valueCommentAlias.MappedModels.Clear();
                    mappedModels = new List<TextBlock>(valueCommentAlias.MappedModels);
                    TextBlock textblock3 = new TextBlock();
                    textblock3.Text = (new VerticalLineViewModel()).ToString();
                    mappedModels.Add(textblock3);
                    valueCommentAlias.MappedModels = mappedModels;
                    break;
                default:
                    break;
            }
        }

        public static void ValueAliasManager_ValueAliasChanged(ValueAliasChangedEventArgs e)
        {
            if (e.Type == ValueChangedType.Clear)
            {
                foreach (var item in _elementCollection.Where(x => { return x.HasAlias; }))
                {
                    item.Alias = string.Empty;
                    item.HasAlias = false;
                }
            }
            else
            {
                foreach (var item in _elementCollection.Where(x => { return x.Name == e.ValueString; }))
                {
                    if (e.Type == ValueChangedType.Add)
                    {
                        item.HasAlias = true;
                    }
                    else if (e.Type == ValueChangedType.Remove)
                    {
                        item.HasAlias = false;
                    }
                    item.Alias = e.Alias;
                }
            }
        }

        public static void ValueCommentManager_ValueCommentChanged(ValueCommentChangedEventArgs e)
        {
            if (e.Type == ValueChangedType.Clear)
            {
                foreach (var item in _elementCollection.Where(x => { return x.HasComment; }))
                {
                    item.Comment = string.Empty;
                    item.HasComment = false;
                }
            }
            else
            {
                foreach (var item in _elementCollection.Where(x => { return x.Name == e.ValueString; }))
                {
                    if (e.Type == ValueChangedType.Add)
                    {
                        item.HasComment = true;
                    }
                    else if (e.Type == ValueChangedType.Remove)
                    {
                        item.HasComment = false;
                    }
                    item.Comment = e.Comment;
                }
            }
        }
        private void UpdateElementCollection()
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ElementCollection"));
        }
        private void OnTextBlockMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock textblock = sender as TextBlock;
            if (_currentTextBlock == null)
            {
                _currentTextBlock = textblock;
                SetTextBlockBackground(_currentTextBlock);
            }
            else if (_currentTextBlock == textblock)
            {
                ResetTextBlockBackground(_currentTextBlock);
                _currentTextBlock = null;
            }
            else
            {
                ResetTextBlockBackground(_currentTextBlock);
                _currentTextBlock = textblock;
                SetTextBlockBackground(_currentTextBlock);
            }
            UpdateElementCollection();
        }
        #region set,reset
        private void ResetTextBlockBackground(TextBlock textblock)
        {
            Color color = new Color();
            color.A = 255;
            color.R = 249;
            color.G = 249;
            color.B = 243;
            textblock.Background = new SolidColorBrush(color);
            color.A = 255;
            color.R = 0;
            color.G = 0;
            color.B = 0;
            textblock.Foreground = new SolidColorBrush(color);
        }
        private void SetTextBlockBackground(TextBlock textblock)
        {
            Color color = new Color();
            color.A = 255;
            color.R = 26;
            color.G = 134;
            color.B = 243;
            textblock.Background = new SolidColorBrush(color);
            color.A = 255;
            color.R = 255;
            color.G = 255;
            color.B = 255;
            textblock.Foreground = new SolidColorBrush(color);
        }
        private void SetButtonBackground(Button button)
        {
            Color color = new Color();
            color.A = 255;
            color.R = 211;
            color.G = 211;
            color.B = 211;
            button.Background = new SolidColorBrush(color);
        }
        private void ResetButtonBackground(Button button)
        {
            Color color = new Color();
            color.A = 255;
            color.R = 255;
            color.G = 255;
            color.B = 191;
            button.Background = new SolidColorBrush(color);
        }
        #endregion
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Name)
            {
                case "button1":
                    _hasUsed = !_hasUsed;
                    if (_hasUsed)
                    {
                        SetButtonBackground(button);
                    }
                    else
                    {
                        ResetButtonBackground(button);
                    }
                    UpdateElementCollection();
                    break;
                case "button2":
                    _hasComment = !_hasComment;
                    if (_hasComment)
                    {
                        SetButtonBackground(button);
                    }
                    else
                    {
                        ResetButtonBackground(button);
                    }
                    UpdateElementCollection();
                    break;
                case "button3":
                    CSVImportDialog dialogImport;
                    using (dialogImport = new CSVImportDialog())
                    {
                        dialogImport.ImportButtonClick += (sender1, e1) =>
                        {
                            if (dialogImport.FileName == string.Empty)
                            {
                                MessageBox.Show("请选择文件");
                                return;
                            }
                            CSVFileHelper.ImportExcute(dialogImport.FileName, _elementCollection, dialogImport.Separator.Substring(0, 1));
                            dialogImport.Close();
                        };
                        dialogImport.ShowDialog();
                    }
                    break;
                case "button4":
                    CSVExportDialog dialogExport;
                    using (dialogExport = new CSVExportDialog())
                    {
                        dialogExport.ExportButtonClick += (sender1, e1) =>
                        {
                            string name = dialogExport.FileName;
                            string dir = dialogExport.Path;
                            if (!Directory.Exists(dir))
                            {
                                MessageBox.Show("指定路径不存在");
                                return;
                            }
                            if (name == string.Empty)
                            {
                                MessageBox.Show("文件名不能为空");
                                return;
                            }
                            string fullFileName = string.Format(@"{0}\{1}.csv", dir, name);
                            if (File.Exists(fullFileName))
                            {
                                MessageBox.Show("指定路径已存在同名文件");
                                return;
                            }
                            CSVFileHelper.ExportExcute(fullFileName,ElementCollection,dialogExport.Separator.Substring(0,1));
                            dialogExport.Close();
                        };
                        dialogExport.ShowDialog();
                    }
                    break;
                default:
                    break;
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ValueCommentAlias comment = e.Row.DataContext as ValueCommentAlias;
            TextBox textBox = e.EditingElement as TextBox;
            if (e.Column == elementComment)
            {
                if (comment != null && textBox != null)
                {
                    ValueCommentManager.UpdateComment(comment.Name,textBox.Text);
                }
            }
            else if(e.Column == elementAlias)
            {
                if (comment != null && textBox != null)
                {
                    if (textBox.Text != string.Empty && !ValueAliasManager.CheckAlias(comment.Name,textBox.Text))
                    {
                        MessageBox.Show("alias is already exist");
                        e.Cancel = true;
                    }
                    else
                    {
                        ValueAliasManager.UpdateAlias(comment.Name, textBox.Text);
                    }
                }
            }
        }
        public void OnClosing(object sender, CancelEventArgs e)
        {
            Window window = sender as Window;
            e.Cancel = true;
            window.Hide();
        }
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateElementCollection();
        }
        private void OnClearButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedItems = ElementDataGrid.SelectedItems;
            foreach (var item in selectedItems)
            {
                var tempitem = item as ValueCommentAlias;
                if (tempitem.Comment != string.Empty)
                {
                    tempitem.Comment = string.Empty;
                    tempitem.HasComment = false;
                    ValueCommentManager.UpdateComment(tempitem.Name, tempitem.Comment);
                }
                if (tempitem.Alias != string.Empty)
                {
                    tempitem.Alias = string.Empty;
                    tempitem.HasAlias = false;
                    ValueAliasManager.UpdateAlias(tempitem.Name, tempitem.Alias);
                }
            }
        }
        private void NavigateMouseLeftButtonClick(object sender, MouseButtonEventArgs e)
        {
            TextBlock textblock = sender as TextBlock;
            string message = textblock.Text;
            Regex regex = new Regex(".+RoutineName:(.+)NetworkNumber:([0-9]+)\\s+X:([0-9]+)\\s+Y:([0-9]+)", RegexOptions.IgnoreCase);
            Match match = regex.Match(message);
            if (match.Success)
            {
                string refLadderName = match.Groups[1].Value.TrimEnd();
                int netWorkNum = int.Parse(match.Groups[2].Value);
                int x = int.Parse(match.Groups[3].Value);
                int y = int.Parse(match.Groups[4].Value);
                NavigateToNetwork.Invoke(new NavigateToNetworkEventArgs(netWorkNum,refLadderName,x,y));
            }
        }
        private void OnDetailShowClick(object sender, RoutedEventArgs e)
        {
            _showDetails = !_showDetails;
            if (_showDetails)
            {
                SetButtonBackground(sender as Button);
                ElementDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
            }
            else
            {
                ResetButtonBackground(sender as Button);
                ElementDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            }
        }
    }
}
