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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// ElementList.xaml 的交互逻辑
    /// </summary>
    public class ValueCommentAlias : INotifyPropertyChanged
    {
        private bool _isSpecialRegister = false;
        //private string _describe = string.Empty;
        private string _base;
        private uint _offset;
        private string _comment;
        private string _alias;
        private List<TextBlock> _mappedModels;
        public bool IsSpecialRegister
        {
            get
            {
                return _isSpecialRegister;
            }
            set
            {
                _isSpecialRegister = value;
            }
        }
        //public string Describe
        //{
        //    get
        //    {
        //        return _describe;
        //    }
        //    set
        //    {
        //        _describe = value;
        //    }
        //}
        public string Base
        {
            get
            {
                return _base;
            }
            set
            {
                _base = value;
            }
        }
        public uint Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
            }
        }
        public string Name
        {
            get
            {
                return Base + Offset;
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
        public ValueCommentAlias(string Base,uint Offset ,string Comment, string Alias)
        {
            this.Base = Base;
            this.Offset = Offset;
            this.Comment = Comment;
            this.Alias = Alias;
            _mappedModels = new List<TextBlock>();
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
    public partial class ElementList : UserControl, INotifyPropertyChanged
    {
        private bool _hasUsed = false;
        private bool _hasComment = false;
        private bool _showDetails = false;
        private TextBlock _currentTextBlock;
        public string XRange
        {
            get
            {
                return string.Format("X(Bit) - ({0} {1})", PLCDeviceManager.GetPLCDeviceManager().SelectDevice.XRange.Start, PLCDeviceManager.GetPLCDeviceManager().SelectDevice.XRange.End - 1);
            }
        }
        public string YRange
        {
            get
            {
                return string.Format("Y(Bit) - ({0} {1})", PLCDeviceManager.GetPLCDeviceManager().SelectDevice.YRange.Start, PLCDeviceManager.GetPLCDeviceManager().SelectDevice.YRange.End - 1);
            }
        }
        public string MRange
        {
            get
            {
                return string.Format("M(Bit) - ({0} {1})", PLCDeviceManager.GetPLCDeviceManager().SelectDevice.MRange.Start, PLCDeviceManager.GetPLCDeviceManager().SelectDevice.MRange.End - 1);
            }
        }
        public string SRange
        {
            get
            {
                return string.Format("S(Bit) - ({0} {1})", PLCDeviceManager.GetPLCDeviceManager().SelectDevice.SRange.Start, PLCDeviceManager.GetPLCDeviceManager().SelectDevice.SRange.End - 1);
            }
        }
        public string CRange
        {
            get
            {
                return string.Format("C(Bit) - ({0} {1})", PLCDeviceManager.GetPLCDeviceManager().SelectDevice.CRange.Start, PLCDeviceManager.GetPLCDeviceManager().SelectDevice.CRange.End - 1);
            }
        }
        public string TRange
        {
            get
            {
                return string.Format("T(Bit) - ({0} {1})", PLCDeviceManager.GetPLCDeviceManager().SelectDevice.TRange.Start, PLCDeviceManager.GetPLCDeviceManager().SelectDevice.TRange.End - 1);
            }
        }
        public string DRange
        {
            get
            {
                return string.Format("D(Word) - ({0} {1})", PLCDeviceManager.GetPLCDeviceManager().SelectDevice.DRange.Start, PLCDeviceManager.GetPLCDeviceManager().SelectDevice.DRange.End - 1);
            }
        }
        public string VRange
        {
            get
            {
                return string.Format("V(Word) - ({0} {1})", PLCDeviceManager.GetPLCDeviceManager().SelectDevice.VRange.Start, PLCDeviceManager.GetPLCDeviceManager().SelectDevice.VRange.End - 1);
            }
        }
        public string ZRange
        {
            get
            {
                return string.Format("Z(Word) - ({0} {1})", PLCDeviceManager.GetPLCDeviceManager().SelectDevice.ZRange.Start, PLCDeviceManager.GetPLCDeviceManager().SelectDevice.ZRange.End - 1);
            }
        }
        public string TVRange
        {
            get
            {
                return string.Format("TV(Word) - ({0} {1})", PLCDeviceManager.GetPLCDeviceManager().SelectDevice.TVRange.Start, PLCDeviceManager.GetPLCDeviceManager().SelectDevice.TVRange.End - 1);
            }
        }
        public string CV16Range
        {
            get
            {
                return string.Format("CV(Word) - ({0} {1})", PLCDeviceManager.GetPLCDeviceManager().SelectDevice.CV16Range.Start, PLCDeviceManager.GetPLCDeviceManager().SelectDevice.CV16Range.End - 1);
            }
        }
        public string CV32Range
        {
            get
            {
                return string.Format("CV(DWord) - ({0} {1})", PLCDeviceManager.GetPLCDeviceManager().SelectDevice.CV32Range.Start, PLCDeviceManager.GetPLCDeviceManager().SelectDevice.CV32Range.End - 1);
            }
        }
        public string AIRange
        {
            get
            {
                return string.Format("AI(Word) - ({0} {1})", PLCDeviceManager.GetPLCDeviceManager().SelectDevice.AIRange.Start, PLCDeviceManager.GetPLCDeviceManager().SelectDevice.AORange.End - 1);
            }
        }
        public string AORange
        {
            get
            {
                return string.Format("AO(Word) - ({0} {1})", PLCDeviceManager.GetPLCDeviceManager().SelectDevice.AORange.Start, PLCDeviceManager.GetPLCDeviceManager().SelectDevice.AORange.End - 1);
            }
        }
        private static List<ValueCommentAlias> _elementCollection = new List<ValueCommentAlias>();
        private IEnumerable<ValueCommentAlias> _deviceType_elementCollection = new List<ValueCommentAlias>();
        public IEnumerable<ValueCommentAlias> ElementCollection
        {
            get
            {
                var tempList = _deviceType_elementCollection;
                if(_currentTextBlock != null)
                {
                    string tempstr = _currentTextBlock.Text;
                    int index = stackpanel_textblock.Children.IndexOf(_currentTextBlock);
                    switch (index)
                    {
                        case 0:case 1:case 2:case 3:case 4:case 5:case 6:case 7:case 8:
                            tempList = tempList.Where(x => { return x.Base == tempstr.Substring(0,1); }).ToList();
                            break;
                        case 9:case 12:case 13:
                            tempList = tempList.Where(x => { return x.Base == tempstr.Substring(0,2); }).ToList();
                            break;
                        case 10:
                            tempList = tempList.Where(x => { return x.Base == tempstr.Substring(0, 2) && x.Offset > 0 && x.Offset < 200; }).ToList();
                            break;
                        case 11:
                            tempList = tempList.Where(x => { return x.Base == tempstr.Substring(0, 2) && x.Offset >= 200; }).ToList();
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
        public static event NavigateToNetworkEventHandler NavigateToNetwork = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        #region InitializeElementCollection
        public static void InitializeElementCollection()
        {
            Device device = Device.MaxRangeDevice;
            for (uint i = device.XRange.Start; i < device.XRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("X"),i ,string.Empty, string.Empty));
            }
            for (uint i = device.YRange.Start; i < device.YRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("Y"), i, string.Empty, string.Empty));
            }
            for (uint i = device.SRange.Start; i < device.SRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("S"), i, string.Empty, string.Empty));
            }
            for (uint i = device.CRange.Start; i < device.CRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("C"), i, string.Empty, string.Empty));
            }
            for (uint i = device.TRange.Start; i < device.TRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("T"), i, string.Empty, string.Empty));
            }
            for (uint i = device.VRange.Start; i < device.VRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("V"), i, string.Empty, string.Empty));
            }
            for (uint i = device.ZRange.Start; i < device.ZRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("Z"), i, string.Empty, string.Empty));
            }
            for (uint i = device.AIRange.Start; i < device.AIRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("AI"), i, string.Empty, string.Empty));
            }
            for (uint i = device.AORange.Start; i < device.AORange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("AO"), i, string.Empty, string.Empty));
            }
            for (uint i = device.MRange.Start; i < device.MRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("M"), i, string.Empty, string.Empty));
            }
            for (uint i = device.DRange.Start; i < device.DRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("D"), i, string.Empty, string.Empty));
            }
            for (uint i = device.CVRange.Start; i < device.CVRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("CV"), i, string.Empty, string.Empty));
            }
            for (uint i = device.TVRange.Start; i < device.TVRange.End; i++)
            {
                _elementCollection.Add(new ValueCommentAlias(string.Format("TV"), i, string.Empty, string.Empty));
            }
        }
        #endregion
        public ElementList()
        {
            InitializeComponent();
            PLCDeviceManager.GetPLCDeviceManager().PropertyChanged += PLCDeviceType_PropertyChanged;
            DataContext = this;
            Loaded += ElementList_Loaded;
        }
        private void ElementList_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += backgroundWorker_DoWork;
            worker.RunWorkerAsync();
        }
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadElements();
        }
        private void LoadElements()
        {
            FilterCollectionByDeviceType(_elementCollection, PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
            RangePropertyChanged();
            UpdateElementCollection();
        }
        private void FilterCollectionByDeviceType(List<ValueCommentAlias> list,Device selectDevice)
        {
            _deviceType_elementCollection = list.Where(x => 
            {
                switch (x.Base)
                {
                    case "X":
                        if (!selectDevice.XRange.AssertValue(x.Offset))
                        {
                            return false;
                        }
                        break;
                    case "Y":
                        if (!selectDevice.YRange.AssertValue(x.Offset))
                        {
                            return false;
                        }
                        break;
                    case "M":
                        if (!selectDevice.MRange.AssertValue(x.Offset))
                        {
                            return false;
                        }
                        break;
                    case "S":
                        if (!selectDevice.SRange.AssertValue(x.Offset))
                        {
                            return false;
                        }
                        break;
                    case "C":
                        if (!selectDevice.CRange.AssertValue(x.Offset))
                        {
                            return false;
                        }
                        break;
                    case "T":
                        if (!selectDevice.TRange.AssertValue(x.Offset))
                        {
                            return false;
                        }
                        break;
                    case "D":
                        if (!selectDevice.DRange.AssertValue(x.Offset))
                        {
                            return false;
                        }
                        break;
                    case "V":
                        if (!selectDevice.VRange.AssertValue(x.Offset))
                        {
                            return false;
                        }
                        break;
                    case "Z":
                        if (!selectDevice.ZRange.AssertValue(x.Offset))
                        {
                            return false;
                        }
                        break;
                    case "TV":
                        if (!selectDevice.TVRange.AssertValue(x.Offset))
                        {
                            return false;
                        }
                        break;
                    case "CV":
                        if (!selectDevice.CVRange.AssertValue(x.Offset))
                        {
                            return false;
                        }
                        break;
                    case "AI":
                        if (!selectDevice.AIRange.AssertValue(x.Offset))
                        {
                            return false;
                        }
                        break;
                    case "AO":
                        if (!selectDevice.AORange.AssertValue(x.Offset))
                        {
                            return false;
                        }
                        break;
                    default:
                        return false;
                }
                return true;
            });
        }
        public void RangePropertyChanged()
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("XRange"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("YRange"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("MRange"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SRange"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CRange")); 
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TRange"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("DRange")); 
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("VRange"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ZRange"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("XRange"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TVRange"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CV16Range"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CV32Range"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("AIRange"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("AORange"));
        }
        private void PLCDeviceType_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FilterCollectionByDeviceType(_elementCollection, PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
            UpdateElementCollection();
            RangePropertyChanged();
        }
        public static void InstructionCommentManager_MappedMessageChanged(MappedMessageChangedEventArgs e)
        {
            IEnumerable<ValueCommentAlias> fit = _elementCollection.Where(x => { return x.Name == e.ValueString; });
            if (fit.Count() == 0 && e.MappedValueModel != null && e.Type != MappedMessageChangedType.Refresh)
            {
                return;
            }
            ValueCommentAlias valueCommentAlias;
            if (e.Type != MappedMessageChangedType.Clear && e.Type != MappedMessageChangedType.Refresh)
            {
                valueCommentAlias = fit.First();
            }
            else
            {
                valueCommentAlias = new ValueCommentAlias(string.Empty,0,string.Empty,string.Empty);
            }
            List<TextBlock> mappedModels;
            switch (e.Type)
            {
                case MappedMessageChangedType.Refresh:
                    ClearMappedModels();
                    RefreshMappedModels();
                    break;
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
                        var index = e.MappedValueModel.ToString().LastIndexOf("(I=");
                        if (model.Text.Contains(e.MappedValueModel.ToString().Substring(0, index)))
                        {
                            mappedModels.Remove(model);
                            break;
                        }
                    }
                    valueCommentAlias.MappedModels = mappedModels;
                    break;
                case MappedMessageChangedType.RemoveLast:
                    valueCommentAlias.MappedModels.Clear();
                    valueCommentAlias.MappedModels = new List<TextBlock>();
                    break;
                case MappedMessageChangedType.Clear:
                    ClearMappedModels();
                    break;
                default:
                    break;
            }
        }
        private static void ClearMappedModels()
        {
            foreach (var item in _elementCollection.Where(x => { return x.MappedModels.Count > 0; }))
                item.MappedModels = new List<TextBlock>();
        }
        private static void RefreshMappedModels()
        {
            foreach (var item in _elementCollection)
            {
                if (InstructionCommentManager.ValueRelatedModel.ContainsKey(item.Name))
                    RefreshMappedModelsByItem(item);
            }
        }
        private static void RefreshMappedModelsByItem(ValueCommentAlias item)
        {
            List<TextBlock> mappedModels = new List<TextBlock>();
            IEnumerable<BaseViewModel> models = InstructionCommentManager.ValueRelatedModel[item.Name].Where(x => { return x.NetWorkNum != -1; });
            if (models.Count() > 0)
            {
                TextBlock textblock = new TextBlock();
                textblock.Text = (new HorizontalLineViewModel()).ToString();
                mappedModels.Add(textblock);
                foreach (var model in models)
                {
                    textblock = new TextBlock();
                    textblock.Text = model.ToString();
                    mappedModels.Add(textblock);
                }
            }
            item.MappedModels = mappedModels;
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
                                MessageBox.Show(Properties.Resources.Message_File_Requried);
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
                                MessageBox.Show(Properties.Resources.Message_Path);
                                return;
                            }
                            if (name == string.Empty)
                            {
                                MessageBox.Show(Properties.Resources.Message_File_Name);
                                return;
                            }
                            string fullFileName = string.Format(@"{0}\{1}.csv", dir, name);
                            if (File.Exists(fullFileName))
                            {
                                MessageBox.Show(Properties.Resources.Message_File_Exist);
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
                        MessageBox.Show(Properties.Resources.Message_Alias_Exist);
                        e.Cancel = true;
                    }
                    else
                    {
                        ValueAliasManager.UpdateAlias(comment.Name, textBox.Text);
                    }
                }
            }
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
            Regex regex = new Regex("\\(R=(.*)\\)\\(N=([0-9]+)\\)\\(X=([0-9]+),Y=([0-9]+)\\)\\(I=(.*)\\)", RegexOptions.IgnoreCase);
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
