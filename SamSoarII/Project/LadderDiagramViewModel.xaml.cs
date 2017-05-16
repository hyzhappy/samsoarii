using SamSoarII.LadderInstViewModel;
using SamSoarII.UserInterface;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
using SamSoarII.LadderInstModel;
using SamSoarII.PLCDevice;
using SamSoarII.ValueModel;
using SamSoarII.AppMain.LadderCommand;
using SamSoarII.Extend.FuncBlockModel;
using SamSoarII.AppMain.UI;

namespace SamSoarII.AppMain.Project
{
    /// <summary>
    /// LadderDiagramViewModel.xaml 的交互逻辑
    /// </summary>

    public enum SelectStatus
    {
        Idle,
        SingleSelected,
        MultiSelecting,
        MultiSelected,
    }

    public enum CrossNetworkState
    {
        CrossUp,
        CrossDown,
        NoCross
    }

    public partial class LadderDiagramViewModel : UserControl, IProgram
    {
        private ProjectModel _projectModel;
        public ProjectModel ProjectModel
        {
            get { return this._projectModel; }
        }
        
        private string _programName;
        public string ProgramName
        {
            get
            {
                return _programName;
            }
            set
            {
                _programName = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ProgramName"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TabHeader"));
            }
        }
        public class InstructionExecption : Exception
        {
            private string _message;
            public override string Message
            {
                get
                {
                    return _message;
                }
            }
            public InstructionExecption(string message)
            {
                _message = message;
            }
        }
        private LadderMode _laddermode = LadderMode.Edit;
        public LadderMode LadderMode
        {
            get { return this._laddermode; }
            set
            {
                this._laddermode = value;
                foreach (LadderNetworkViewModel lnvmodel in GetNetworks())
                {
                    lnvmodel.LadderMode = value;
                }
            }
        }

        private bool _isCommentMode;
        public bool IsCommendMode
        {
            get { return _isCommentMode; }
            set
            {
                _isCommentMode = value;
                foreach(var net in _ladderNetworks)
                {
                    net.IsCommentMode = _isCommentMode;
                }
                _selectRect.IsCommentMode = _isCommentMode;
                SelectionStatus = SelectStatus.Idle;
            }
        }

        private int WidthUnit { get { return GlobalSetting.LadderWidthUnit; } }
        private int HeightUnit { get { return _isCommentMode ? GlobalSetting.LadderCommentModeHeightUnit : GlobalSetting.LadderHeightUnit; } }
        private bool _isExpand = true;
        public bool IsExpand
        {
            get
            {
                return _isExpand;
            }
            set
            {
                _isExpand = value;
            }
        }
        public string TabHeader
        {
            get
            {
                return _programName;
            }
            set
            {
                _programName = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ProgramName"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TabHeader"));
            }
        }
        public bool IsMainLadder { get; set; }
        private Dictionary<string, List<string>> InstrutionNameAndToolTips;
        public int NetworkCount
        {
            get
            {
                return _ladderNetworks.Count;
            }
        }

        private LinkedList<LadderNetworkViewModel> _ladderNetworks = new LinkedList<LadderNetworkViewModel>();
        public LinkedList<LadderNetworkViewModel> LadderNetworks
        {
            get
            {
                return _ladderNetworks;
            }
            set
            {
                _ladderNetworks = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LadderNetworks"));
            }
        }
        private SelectRect _selectRect = new SelectRect();

        private LadderNetworkViewModel _selectStartNetwork;
        public LadderNetworkViewModel SelectStartNetwork
        {
            get
            {
                return _selectStartNetwork;
            }
        }
        private SortedSet<LadderNetworkViewModel> _selectAllNetworks = new SortedSet<LadderNetworkViewModel>();
        public SortedSet<LadderNetworkViewModel> SelectAllNetworks
        {
            get
            {
                return _selectAllNetworks;
            }
        }
        private SortedSet<LadderNetworkViewModel> _selectAllNetworkCache = new SortedSet<LadderNetworkViewModel>();
        public SortedSet<LadderNetworkViewModel> SelectAllNetworkCache
        {
            get
            {
                return _selectAllNetworkCache;
            }
        }
        public SelectRect SelectionRect
        {
            get
            {
                return _selectRect;
            }
            private set
            {
                _selectRect = value;
            }
        }

        private LadderNetworkViewModel _selectRectOwner = null;
        public LadderNetworkViewModel SelectRectOwner
        {
            get
            {
                return _selectRectOwner;
            }
        }
        private SelectStatus _selectStatus = SelectStatus.Idle;

        public SelectStatus SelectionStatus
        {
            get
            {
                return _selectStatus;
            }
            set
            {
                _selectStatus = value;
                switch(_selectStatus)
                {
                    case SelectStatus.Idle:
                        EnterIdleState();
                        break;
                    case SelectStatus.SingleSelected:
                        EnterSingleSelectedState();
                        break;
                    case SelectStatus.MultiSelecting:
                        EnterMultiSelectingState();
                        break;
                    case SelectStatus.MultiSelected:
                        EnterMultiSelectedState();
                        break;
                    default:
                        break;
                }
            }
        }

        public CrossNetworkState CrossNetState
        {
            get; set; 
        }

        #region About CommandManager
        private LadderCommand.CommandManager _commandManager = new LadderCommand.CommandManager();
        
        public bool IsModify
        {
            get { return _commandManager.IsModify; }
            set { _commandManager.IsModify = value; }
        }

        public void CommandExecute(IUndoableCommand command)
        {
            _commandManager.Execute(command);
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string LadderComment
        {
            get
            {
                return LadderCommentTextBlock.Text;
            }
            set
            {
                LadderCommentTextBlock.Text = value;
            }
        }

        private double _actualWidth;
        private double _actualHeight;

        double ITabItem.ActualWidth
        {
            get
            {
                return this._actualWidth;
            }

            set
            {
                this._actualWidth = value;
            }
        }

        double ITabItem.ActualHeight
        {
            get
            {
                return this._actualHeight;
            }

            set
            {
                this._actualHeight = value;
            }
        }
        private bool _canScrollToolTip = false;
        private InstructionDiagramViewModel idvmodel;
        public InstructionDiagramViewModel IDVModel
        {
            get { return this.idvmodel; }
            set
            {
                if (idvmodel != null)
                {
                    idvmodel.CursorChanged -= OnInstructionCursorChanged;
                    idvmodel.CursorEdit -= OnInstructionCursorEdit;
                }
                this.idvmodel = value;
                idvmodel.CursorChanged += OnInstructionCursorChanged;
                idvmodel.CursorEdit += OnInstructionCursorEdit;
                idvmodel.Setup(this);
            }
        }

        public LadderDiagramViewModel(string name, ProjectModel _parent)
        {
            InitializeComponent();
            InitializeInstructionNameAndToolTips();
            _projectModel = _parent;
            ProgramName = name;
            LadderCommentTextBlock.DataContext = this;
            this.Loaded += (sender, e) =>
            {
                Focus();
                Keyboard.Focus(this);
                ladderExpander.IsExpand = IsExpand;
            };
            IDVModel = new InstructionDiagramViewModel();
            AppendNetwork(new LadderNetworkViewModel(this, 0));
            ladderExpander.MouseEnter += OnMouseEnter;
            ladderExpander.MouseLeave += OnMouseLeave;
            ladderExpander.line.Visibility = Visibility.Hidden;
            ladderExpander.line1.Visibility = Visibility.Hidden;
            ladderExpander.expandButton.IsExpandChanged += ExpandButton_IsExpandChanged;
            ThumbnailButton.ToolTipOpening += ThumbnailButton_ToolTipOpening;
            ThumbnailButton.ToolTipClosing += ThumbnailButton_ToolTipClosing;
        }
        private void InitializeInstructionNameAndToolTips()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            //Console.WriteLine(assembly.GetManifestResourceNames());
            Stream stream = assembly.GetManifestResourceStream("SamSoarII.AppMain.Resources.InstructionPopup.xml");
            Dictionary<string, List<string>> tempDic = new Dictionary<string, List<string>>();
            XDocument xDoc = XDocument.Load(stream);
            XElement rootNode = xDoc.Root;
            List<XElement> nodes = rootNode.Elements().ToList();
            foreach (var node in nodes)
            {
                if (node.Name != "HLine" && node.Name != "VLine")
                {
                    List <string> tempList = new List<string>();
                    tempList.Add(node.Attribute("Describe").Value);
                    tempList.Add(node.Attribute("Text_1").Value);
                    tempList.Add(node.Attribute("Text_2").Value);
                    tempList.Add(node.Attribute("Text_3").Value);
                    tempList.Add(node.Attribute("Text_4").Value);
                    tempList.Add(node.Attribute("Text_5").Value);
                    tempList.Add(node.Attribute("Detail").Value);
                    tempDic.Add(node.Name.ToString(), tempList);
                }
            }
            InstrutionNameAndToolTips = tempDic;
        }
        #region Network manipulation
        public LadderNetworkViewModel GetNetworkByNumber(int number)
        {
            return _ladderNetworks.ElementAt(number);
        }

        /// <summary>
        /// 初始化，删除所有的网络，不要在用户交互中使用，仅仅在初始化的时候调用以初始化
        /// </summary>
        public void InitNetworks()
        {
            LadderNetworkStackPanel.Children.Clear();
            foreach (var network in _ladderNetworks)
            {
                network.PropertyChanged -= Network_PropertyChanged;
            }
            _ladderNetworks.Clear();
            IDVModel.Setup(this);
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LadderNetworks"));
        }

        public void AcquireSelectRect(LadderNetworkViewModel network)
        {
            if (_selectRectOwner == null)
            {
                _selectRectOwner = network;
            }
            else
            {
                if (_selectRectOwner != network)
                {
                    _selectRectOwner.ReleaseSelectRect();
                    _selectRectOwner = network;
                }
            }
            SelectionStatus = SelectStatus.SingleSelected;
        }

        public IEnumerable<LadderNetworkViewModel> GetNetworks()
        {
            return _ladderNetworks;
        }

        private void ReloadNetworksToStackPanel()
        {
            LadderNetworkStackPanel.Children.Clear();
            foreach (var net in _ladderNetworks)
            {
                LadderNetworkStackPanel.Children.Add(net);
            }
        }
        /// <summary>
        /// Called when initialze the ladder diagram, do not call by user, for it can not undo 
        /// </summary>
        /// <param name="network"></param>
        public void AppendNetwork(LadderNetworkViewModel network)
        {
            network.NetworkNumber = _ladderNetworks.Count;
            _ladderNetworks.AddLast(network);
            LadderNetworkStackPanel.Children.Add(network);
            network.PropertyChanged += Network_PropertyChanged;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LadderNetworks"));
            IDVModel.Setup(this);
        }

        private void Network_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "NetworkBrief")
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LadderNetworks"));
            }
        }

        #region Network manipulation，undoable command form method
        public void ReplaceSingleElement(LadderNetworkViewModel network, BaseViewModel element)
        {
            var elements = new List<BaseViewModel>();
            elements.Add(element);
            var oldelements = new List<BaseViewModel>();
            var oldele = _selectRectOwner.SearchElement(element.X, element.Y);
            if (oldele != null)
            {
                oldelements.Add(oldele);
            }
            if (element.Type == ElementType.Output)
            {
                for (int i = Math.Max(SelectionRect.X,1); i < GlobalSetting.LadderXCapacity - 1; i++)
                {
                    if (_selectRectOwner.SearchElement(i,SelectionRect.Y) == null)
                    {
                        elements.Add(new HorizontalLineViewModel() {X = i,Y = SelectionRect.Y });
                    }
                }
            }
            var command = new LadderCommand.NetworkReplaceElementsCommand(_selectRectOwner, elements, oldelements);
            _commandManager.Execute(command);
        }

        public void AddNewNetworkBefore(LadderNetworkViewModel network)
        {
            _projectModel.IFacade.CreateNetwork(
                this, network.NetworkNumber);
        }

        public void AddNewNetworkAfter(LadderNetworkViewModel network)
        {
            _projectModel.IFacade.CreateNetwork(
                this, network.NetworkNumber + 1);
        }

        public void AppendNewNetwork()
        {
            _projectModel.IFacade.CreateNetwork(
                this, _ladderNetworks.Count());
        }

        public void IFAddNetwork(LadderNetworkViewModel network)
        {
            var command = new LadderCommand.LadderDiagramReplaceNetworksCommand(this, network, network.NetworkNumber);
            _commandManager.Execute(command);
            network.PropertyChanged += Network_PropertyChanged;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LadderNetworks"));
        }
        
        public void RemoveSingleNetworkCommand(LadderNetworkViewModel network)
        {
            _projectModel.IFacade.RemoveNetwork(network);
        }

        public void IFRemoveNetwork(LadderNetworkViewModel network)
        {
            var command = new LadderCommand.LadderDiagramRemoveNetworksCommand(this, new List<LadderNetworkViewModel>() { network }, network.NetworkNumber);
            _commandManager.Execute(command);
            network.PropertyChanged -= Network_PropertyChanged;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LadderNetworks"));
            IDVModel.Setup(this);
        }
        
        /// <summary>
        /// 放置一个新的元素在选择框内
        /// </summary>
        /// <param name="catalogId"></param>
        public void ReplaceSingleElement(int catalogId)
        {
            if (_selectRectOwner != null)
            {
                var viewmodel = LadderInstViewModelPrototype.Clone(catalogId);
                viewmodel.X = (viewmodel.Type == LadderInstModel.ElementType.Output) ? GlobalSetting.LadderXCapacity - 1 : _selectRect.X;
                viewmodel.Y = _selectRect.Y;
                if (viewmodel.X == GlobalSetting.LadderXCapacity - 1 && viewmodel.Type != ElementType.Output)
                {
                    return;
                }
                else
                {
                    ReplaceSingleElement(_selectRectOwner, viewmodel);
                }
            }
        }

        public void ReplaceSingleVerticalLine(LadderNetworkViewModel network, VerticalLineViewModel vline)
        {
            var vlines = new List<VerticalLineViewModel>();
            var oldvline = _selectRectOwner.SearchVerticalLine(vline.X, vline.Y);
            if (oldvline == null)
            {
                vlines.Add(vline);
                var command = new LadderCommand.NetworkReplaceElementsCommand(network, new List<BaseViewModel>(), vlines, new List<BaseViewModel>(), new List<VerticalLineViewModel>());
                _commandManager.Execute(command);
            }
        }

        public void RemoveSingleVerticalLine(LadderNetworkViewModel network, VerticalLineViewModel vline)
        {
            var vlines = new List<VerticalLineViewModel>() { vline };
            var command = new LadderCommand.NetworkRemoveElementsCommand(network, new List<BaseViewModel>(), vlines);
            _commandManager.Execute(command);
        }

        public void NetworkRemoveRow(LadderNetworkViewModel network, int rowNumber)
        {
            var command = new LadderCommand.NetworkRemoveRowCommand(network, rowNumber);
            _commandManager.Execute(command);
            SelectionStatus = SelectStatus.Idle;
        }

        public void NetworkRemoveRows(LadderNetworkViewModel network,int startRowNumber,int count)
        {
            NetworkRemoveRowCommand command;
            for (int i = 0; i < count; i++)
            {
                command = new LadderCommand.NetworkRemoveRowCommand(network, startRowNumber + i);
                _commandManager.Execute(command);
            }
            SelectionStatus = SelectStatus.Idle;
        }

        public void NetworkAddRow(LadderNetworkViewModel network, int rowNumber)
        {
            var command = new LadderCommand.NetworkAddRowCommand(network, rowNumber);
            _commandManager.Execute(command);
        }

        #endregion

        #region Network manipulation，no command, invoked by command form method

        public void AddNetwork(LadderNetworkViewModel net, int index)
        {
            if(_ladderNetworks.Count > 0)
            {
                if (index == 0)
                {
                    LinkedListNode<LadderNetworkViewModel> node = new LinkedListNode<LadderNetworkViewModel>(net);
                    _ladderNetworks.AddFirst(node);
                    net.NetworkNumber = 0;
                    // 下面的网络序号加一
                    node = node.Next;
                    while (node != null)
                    {
                        node.Value.NetworkNumber++;
                        node = node.Next;
                    }
                }
                else
                {
                    if (index > _ladderNetworks.Count)
                    {
                        index = _ladderNetworks.Count;
                    }
                    var node = _ladderNetworks.First;
                    // 向下移动index - 1次
                    for (int i = 1; i < index; i++)
                    {
                        node = node.Next;
                    }
                    _ladderNetworks.AddAfter(node, net);
                    // 下面的网络序号加一
                    node = node.Next;
                    int n = index;
                    while (node != null)
                    {
                        node.Value.NetworkNumber = n;
                        n++;
                        node = node.Next;
                    }
                }
            }
            else
            {
                net.NetworkNumber = 0;
                _ladderNetworks.AddFirst(net);
            }
            ReloadNetworksToStackPanel();
        }

        public void AddNetwork(IEnumerable<LadderNetworkViewModel> nets, int index)
        {
            if(_ladderNetworks.Count > 0)
            {
                if (index == 0)
                {
                    foreach (var net in nets.Reverse())
                    {
                        LinkedListNode<LadderNetworkViewModel> newnode = new LinkedListNode<LadderNetworkViewModel>(net);
                        _ladderNetworks.AddFirst(newnode);
                        newnode.Value.PropertyChanged += Network_PropertyChanged;
                    }
                    var node = _ladderNetworks.First;
                    int n = 0;
                    while (node != null)
                    {
                        node.Value.NetworkNumber = n;
                        n++;
                        node = node.Next;
                    }
                }
                else
                {
                    if (index > _ladderNetworks.Count)
                    {
                        index = _ladderNetworks.Count;
                    }
                    var node = _ladderNetworks.First;
                    // 向下移动index - 1次
                    for (int i = 1; i < index; i++)
                    {
                        node = node.Next;
                    }
                    int n = index;
                    foreach (var net in nets)
                    {
                        net.NetworkNumber = n;
                        _ladderNetworks.AddAfter(node, net);
                        net.PropertyChanged += Network_PropertyChanged;
                        n++;
                        node = node.Next;
                    }
                    node = node.Next;
                    while (node != null)
                    {
                        node.Value.NetworkNumber = n;
                        n++;
                        node = node.Next;
                    }
                }
            }
            else
            {
                int n = 0;
                foreach(var net in nets)
                {
                    net.NetworkNumber = n;
                    _ladderNetworks.AddLast(net);
                    net.PropertyChanged += Network_PropertyChanged;
                    n++;
                }
            }
            ReloadNetworksToStackPanel();
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LadderNetworks"));
        }
        public void RemoveNetwork(LadderNetworkViewModel network)
        {
            if (_ladderNetworks.Count > 1)
            {
                var node = _ladderNetworks.Find(network);
                if (node != null)
                {
                    var temp = node;
                    node = node.Next;
                    while (node != null)
                    {
                        node.Value.NetworkNumber--;
                        node = node.Next;
                    }
                    _ladderNetworks.Remove(network);
                    network.PropertyChanged -= Network_PropertyChanged;
                    LadderNetworkStackPanel.Children.Remove(network);
                }
            }
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LadderNetworks"));
        }

        #endregion

        
        #endregion

        #region Comment area manipulation
        private void EditCommentReact()
        {
            LadderDiagramCommentEditDialog dialog = new LadderDiagramCommentEditDialog();
            dialog.LadderComment = LadderComment;
            dialog.LadderName = ProgramName;
            dialog.EnsureButtonClick += (sender, e) =>
            {
                this.LadderComment = dialog.LadderComment;
                dialog.Close();
            };
            dialog.ShowDialog();
        }
        #endregion

        #region Compile relative

        public string GenerateDeclarationCode(string functionName)
        {
            return string.Format("void {0}();\r\n", functionName);
        }

        public string GenerateCode(string functionName)
        {
            string logicCode = string.Empty;
            foreach (var net in _ladderNetworks.Where(x => { return !x.IsMasked; }))
            {
                logicCode += net.GenerateCode();
            }
            string code = string.Format("void {0}()\r\n{{\r\n{1}\r\n}}\r\n", functionName, logicCode);
            return code;
        }
        #endregion

        #region Selection Rectangle Relative
        public void NavigateToNetworkByNum(int num)
        {
            VScrollToRect(num, _selectRect.Y);
            HScrollToRect(_selectRect.X);
        }
        public void VScrollToRect(int networkNumber, int row)
        {
            double scale = GlobalSetting.LadderScaleY;
            double offset = scale * (MainBorder.ActualHeight + 20) / 3.6;
            foreach (var item in GetNetworks().Where(x => { return x.NetworkNumber < networkNumber; }))
            {
                offset += scale * (item.ActualHeight + 20) / 3.035;
            }
            offset += scale * (_selectRect.ActualHeight * row + 20) / 2.89;
            offset = Math.Max(0, offset);
            MainScrollViewer.ScrollToVerticalOffset(offset);
        }
        public void HScrollToRect(int XIndex)
        {
            double scale = GlobalSetting.LadderScaleX;
            double offset = 0;
            offset += scale * GlobalSetting.LadderWidthUnit * (XIndex + 1) / 2.89;
            offset -= scale * MainScrollViewer.ViewportWidth / 2.5;
            offset = Math.Max(0, offset);
            MainScrollViewer.ScrollToHorizontalOffset(offset);
        }
        private void SelectRectUp()
        {
            if (_selectRectOwner != null)
            {
                if (_selectRect.Y > 0)
                {
                    _selectRect.Y--;
                }
                else
                {
                    if (!_selectRectOwner.IsFirstNetwork())
                    {
                        _selectRectOwner.ReleaseSelectRect();
                        _selectRectOwner = _ladderNetworks.ElementAt(_selectRectOwner.NetworkNumber - 1);
                        _selectRect.Y = _selectRectOwner.RowCount - 1;
                        _selectRectOwner.AcquireSelectRect();
                    }
                }
                VScrollToRect(_selectRect.NetworkParent.NetworkNumber, _selectRect.Y);
            }
        }

        private void SelectRectDown()
        {
            if (_selectRectOwner != null)
            {
                if (_selectRect.Y + 1 < _selectRectOwner.RowCount)
                {
                    _selectRect.Y++;
                }
                else
                {
                    if (!_selectRectOwner.IsLastNetwork())
                    {
                        _selectRectOwner.ReleaseSelectRect();
                        _selectRectOwner = _ladderNetworks.ElementAt(_selectRectOwner.NetworkNumber + 1);
                        _selectRect.Y = 0;
                        _selectRectOwner.AcquireSelectRect();
                    }

                }
                VScrollToRect(_selectRect.NetworkParent.NetworkNumber, _selectRect.Y);
            }
        }

        private void SelectRectLeft()
        {
            if (_selectRectOwner != null)
            {
                if (_selectRect.X > 0)
                {
                    _selectRect.X--;
                    HScrollToRect(_selectRect.X);
                }
            }
        }

        private void SelectRectRight()
        {
            if (_selectRectOwner != null)
            {
                if (_selectRect.X < GlobalSetting.LadderXCapacity - 1)
                {
                    _selectRect.X++;
                    HScrollToRect(_selectRect.X);
                }
            }
        }

        private void SelectRectLeftWithLine()
        {
            SelectRectLeft();
            if (_selectRectOwner != null)
            {
                var model = _selectRectOwner.SearchElement(_selectRect.X, _selectRect.Y);
                if (model != null)
                {
                    if (model.Type == LadderInstModel.ElementType.HLine)
                    {
                        var elements = new List<BaseViewModel>();
                        elements.Add(model);
                        var command = new LadderCommand.NetworkRemoveElementsCommand(_selectRectOwner, elements);
                        _commandManager.Execute(command);
                    }
                }
                else
                {
                    ReplaceSingleElement(_selectRectOwner, new HorizontalLineViewModel() { X = _selectRect.X, Y = _selectRect.Y });
                }
            }
        }

        private void SelectRectRightWithLine()
        {
            int x = _selectRect.X;
            int y = _selectRect.Y;
            SelectRectRight();  
            if (_selectRectOwner != null)
            {
                var model = _selectRectOwner.SearchElement(x, y);
                if (model != null)
                {
                    if (model.Type == LadderInstModel.ElementType.HLine)
                    {
                        var elements = new List<BaseViewModel>();
                        elements.Add(model);
                        var command = new LadderCommand.NetworkRemoveElementsCommand(_selectRectOwner, elements);
                        _commandManager.Execute(command);
                    }
                }
                else
                {
                    ReplaceSingleElement(_selectRectOwner, new HorizontalLineViewModel() { X = x, Y = y });
                }
            }
        }

        private void SelectRectUpWithLine()
        {
            int x = _selectRect.X - 1;
            int y = _selectRect.Y - 1;
            if (_selectRectOwner != null)
            {
                if (y >= 0)
                {
                    SelectRectUp();
                    if(x >= 0)
                    {
                        var vline = _selectRectOwner.SearchVerticalLine(x, y);
                        if (vline != null)
                        {
                            RemoveSingleVerticalLine(_selectRectOwner, vline);
                        }
                        else
                        {
                            ReplaceSingleVerticalLine(_selectRectOwner, new VerticalLineViewModel() { X = x, Y = y });                     
                        }
                    }
                }
            }

        }

        private void SelectRectDownWithLine()
        {
            int x = _selectRect.X - 1;
            int y = _selectRect.Y;
            if (_selectRectOwner != null)
            {
                if (y + 1 == _selectRectOwner.RowCount)
                {
                    NetworkAddRow(_selectRectOwner, _selectRectOwner.RowCount);
                }
                if (x >= 0)
                {
                    var vline = _selectRectOwner.SearchVerticalLine(x, y);
                    if (vline != null)
                    {
                        RemoveSingleVerticalLine(_selectRectOwner, vline);
                    }
                    else
                    {
                        ReplaceSingleVerticalLine(_selectRectOwner, new VerticalLineViewModel() { X = x, Y = y });
                    }
                }
                SelectRectDown();
            }
        }

        #endregion

        #region Selection Area Relative

        /// <summary>
        /// 获取鼠标跨网络状态
        /// </summary>
        /// <returns></returns>
        private CrossNetworkState GetSelectionCrossNetworkState()
        { 
            if(SelectionStatus == SelectStatus.MultiSelecting)
            {
                var p = Mouse.GetPosition(_selectStartNetwork.LadderCanvas);
                if (p.Y < 0 || p.X < 0 || p.X > _selectStartNetwork.LadderCanvas.ActualWidth)
                {
                    return CrossNetworkState.CrossUp;
                }
                else
                {
                    if(p.Y > _selectStartNetwork.LadderCanvas.ActualHeight)
                    {
                        return CrossNetworkState.CrossDown;
                    }
                    else
                    {
                        return CrossNetworkState.NoCross;
                    }
                }
            }
            return CrossNetworkState.NoCross;
        }

        /// <summary>
        /// 收集从起始网络(_selectStartNetwork)开始，鼠标向上掠过的网络，加入到_selectAllNetworks中（不包括_selectStartNetwork）
        /// </summary>
        private void CollectSelectAllNetworkUp()
        {
            _selectAllNetworks.Clear();
           
            var node = _ladderNetworks.Find(_selectStartNetwork);
            node = node.Previous;
            while (node != null)
            {
                if (Mouse.GetPosition(node.Value.LadderCanvas).Y > node.Value.LadderCanvas.ActualHeight)
                {
                    break;
                }
                else
                {
                    _selectAllNetworks.Add(node.Value);
                    _selectAllNetworkCache.Add(node.Value);
                    node = node.Previous;
                }
            }
            
        }
        private void CollectSelectAllNetworkUpByCount(int count)
        {
            _selectAllNetworks.Clear();
            var node = _ladderNetworks.Find(_selectStartNetwork);
            node = node.Previous;
            while (node != null && count-- > 0)
            {
                _selectAllNetworks.Add(node.Value);
                _selectAllNetworkCache.Add(node.Value);
                node = node.Previous;
            }
        }
        /// <summary>
        /// 收集从起始网络(_selectStartNetwork)开始，鼠标向下掠过的网络，加入到_selectAllNetworks中（不包括_selectStartNetwork）
        /// </summary>
        private void CollectSelectAllNetworkDown()
        {
            _selectAllNetworks.Clear();
            var node = _ladderNetworks.Find(_selectStartNetwork);
            node = node.Next;
            while (node != null)
            {
                if (Mouse.GetPosition(node.Value.LadderCanvas).Y < 0)
                {
                    break;                    
                }
                else
                {
                    _selectAllNetworks.Add(node.Value);
                    _selectAllNetworkCache.Add(node.Value);
                    node = node.Next;
                }
            }
        }
        private void CollectSelectAllNetworkDownByCount(int count)
        {
            _selectAllNetworks.Clear();
            var node = _ladderNetworks.Find(_selectStartNetwork);
            node = node.Next;
            while (node != null && count-- > 0)
            {
                _selectAllNetworks.Add(node.Value);
                _selectAllNetworkCache.Add(node.Value);
                node = node.Next;
            }
        }
        #endregion

        #region Instruction relative
        private void ShowInstructionInputDialog(string initialString)
        {
            IEnumerable<string> subdiagramNames = _projectModel.SubRoutines.Select(
                (LadderDiagramViewModel ldvmodel) => { return ldvmodel.ProgramName; });
            IEnumerable<string[]> functionMessages = _projectModel.Funcs.Where(
                (FuncModel fmodel) => { return fmodel.CanCALLM(); }
            ).Select(
                (FuncModel fmodel) => { return fmodel.GetMessageList(); }
            );
            IEnumerable<string> modbusNames = _projectModel.MTVModel.Models.Select(
                (ModbusTableModel mtmodel) => { return mtmodel.Name; });
            InstructionInputDialog dialog = new InstructionInputDialog(
                initialString, 
                InstrutionNameAndToolTips,
                subdiagramNames,
                functionMessages,
                modbusNames);

            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dialog.EnsureButtonClick += (sender, e) =>
            {
                try
                {
                    NetworkReplaceElementsCommand command = null;
                    RegisterInstructionInput(
                        dialog.InstructionInput.Trim(),
                        _selectRect.X, 
                        _selectRect.Y, 
                        _selectRectOwner,
                        ref command);
                    _commandManager.Execute(command);
                    dialog.Close();
                }
                catch (ValueParseException exce2)
                {
                    MessageBox.Show(string.Format(exce2.Message));
                }
                catch (InstructionExecption exce3)
                {
                    MessageBox.Show(string.Format(exce3.Message));
                }
            };
            dialog.ShowDialog();
        }

        public void RegisterInstructionInput(
            string input, int x, int y, LadderNetworkViewModel lnvmodel,
            ref NetworkReplaceElementsCommand command)
        {
            LadderDiagramViewModel selectedSubdiagram = null;
            FuncModel selectedFunction = null;
            ModbusTableModel selectedModbus = null;
            List<string> InstructionInput = null;
            BaseViewModel viewmodel;
            
            InstructionInput = input.Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < InstructionInput.Count(); i++)
            {
                if (InstructionInput[0].Equals("CALLM") && i == 1)
                    continue;
                InstructionInput[i] = InstructionInput[i].ToUpper();
            }
            if (InstructionInput.Count == 0)
            {
                throw new InstructionExecption(string.Format("输入为空."));
            }
            else
            {
                if (!LadderInstViewModelPrototype.CheckInstructionName(InstructionInput[0]))
                {
                    throw new InstructionExecption(string.Format("输入的指令不存在！"));
                }
            }
            switch (InstructionInput[0])
            {
                case "CALL":
                    if (InstructionInput.Count() < 2)
                    {
                        throw new InstructionExecption("必须输入子程序名称。");
                    }
                    try
                    {
                        selectedSubdiagram = _projectModel.SubRoutines.Where(
                            (LadderDiagramViewModel ldvmodel) => { return ldvmodel.ProgramName.Equals(InstructionInput[1]); }).First();
                    }
                    catch (InvalidOperationException)
                    {
                        throw new InstructionExecption(String.Format("找不到子程序{0:s}", InstructionInput[1]));
                    }
                    break;
                case "ATCH":
                    if (InstructionInput.Count() < 3)
                    {
                        throw new InstructionExecption("必须输入子程序名称。");
                    }
                    try
                    {
                        selectedSubdiagram = _projectModel.SubRoutines.Where(
                            (LadderDiagramViewModel ldvmodel) => { return ldvmodel.ProgramName.Equals(InstructionInput[2]); }).First();
                    }
                    catch (InvalidOperationException)
                    {
                        throw new InstructionExecption(String.Format("找不到子程序{0:s}", InstructionInput[2]));
                    }
                    break;
                case "CALLM":
                    if (InstructionInput.Count() < 2)
                    {
                        throw new FormatException("必须输入函数名称。");
                    }
                    try
                    {
                        selectedFunction = _projectModel.Funcs.Where(
                            (FuncModel fmodel) => { return fmodel.Name.Equals(InstructionInput[1]); }).First();
                    }
                    catch (InvalidOperationException)
                    {
                        throw new InstructionExecption(String.Format("找不到C函数{0:s}", InstructionInput[1]));
                    }
                    break;
                case "MBUS":
                    if (InstructionInput.Count() < 3)
                    {
                        throw new InstructionExecption("必须输入MODBUS表格名称。");
                    }
                    try
                    {
                        selectedModbus = _projectModel.MTVModel.Models.Where(
                            (ModbusTableModel mtmodel) => { return mtmodel.Name.Equals(InstructionInput[2]); }).First();
                    }
                    catch (InvalidOperationException)
                    {
                        throw new InstructionExecption(String.Format("找不到MODBUS表格{0:s}", InstructionInput[2]));
                    }
                    break;
            }
            List<string> valueStrings = new List<string>();
            if (selectedFunction != null)
            {
                ArgumentValue[] _values = new ArgumentValue[selectedFunction.ArgCount];
                if (InstructionInput.Count() - 2 != selectedFunction.ArgCount)
                {
                    throw new FormatException("输入的参数数量与函数不相符！");
                }
                for (int i = 0; i < selectedFunction.ArgCount; i++)
                {
                    string _name = InstructionInput[i + 2];
                    string _argname = selectedFunction.GetArgName(i);
                    string _argtype = selectedFunction.GetArgType(i);
                    _values[i] = ArgumentValue.Create(_argname, _argtype, _name,
                        PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
                    if (ValueCommentManager.ContainValue(_name))
                    {
                        _values[i].Comment = ValueCommentManager.GetComment(_name);
                    }
                }
                viewmodel = LadderInstViewModelPrototype.Clone("CALLM");
                ((CALLMViewModel)(viewmodel)).AcceptNewValues(selectedFunction.Name, _values);
            }
            else
            {
                viewmodel = LadderInstViewModelPrototype.Clone(InstructionInput[0]);
                for (int i = 1; i < InstructionInput.Count(); i++)
                {
                    string valueString = InstructionInput[i];
                    valueStrings.Add(valueString);
                    if (ValueCommentManager.ContainValue(valueString))
                    {
                        valueStrings.Add(ValueCommentManager.GetComment(valueString));
                    }
                    else
                    {
                        valueStrings.Add(string.Empty);
                    }
                }
                if (valueStrings.Count == viewmodel.GetValueString().Count() * 2)
                {
                    viewmodel.AcceptNewValues(valueStrings, PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
                }
                else if (selectedFunction == null)
                {
                    throw new InstructionExecption("输入的参数数量与指令不相符！");
                }
            }
            IEnumerable<BaseViewModel> eles_old = null;
            List<BaseViewModel> eles_new = null;
            if (viewmodel.Type == LadderInstModel.ElementType.Output)
            {
                eles_old = lnvmodel.GetElements().Where(ele => ele.Y == y && ele.X >= x);
                eles_new = new List<BaseViewModel>();
                for (int i = x; i < GlobalSetting.LadderXCapacity - 1; i++)
                {
                    eles_new.Add(new HorizontalLineViewModel() { X = i, Y = y });
                }
                viewmodel.X = GlobalSetting.LadderXCapacity - 1;
                viewmodel.Y = y;
                if (valueStrings.Count == viewmodel.GetValueString().Count() * 2)
                {
                    viewmodel.AcceptNewValues(valueStrings, PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
                }
                eles_new.Add(viewmodel);
                _selectRect.X = GlobalSetting.LadderXCapacity - 1;
                command = new LadderCommand.NetworkReplaceElementsCommand(lnvmodel, eles_new, eles_old);
                //_commandManager.Execute(command);
            }
            else
            {
                eles_old = lnvmodel.GetElements().Where(ele => ele.Y == y && ele.X == x);
                eles_new = new List<BaseViewModel>();
                viewmodel.X = x;
                viewmodel.Y = y;
                if (valueStrings.Count == viewmodel.GetValueString().Count() * 2)
                {
                    viewmodel.AcceptNewValues(valueStrings, PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
                }
                eles_new.Add(viewmodel);
                command = new LadderCommand.NetworkReplaceElementsCommand(lnvmodel, eles_new, eles_old);
                //_commandManager.Execute(command);
                if (_selectRect.X < GlobalSetting.LadderXCapacity - 1)
                {
                    _selectRect.X++;
                }
            }
        }

        #endregion

        #region Event handler
        
        public bool IsPressingCtrl
        {
            get; private set;
        }
        private void OnLadderDiagramKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                IsPressingCtrl = false;
            }
            if (_selectStatus == SelectStatus.MultiSelecting)
            {
                SelectionStatus = SelectStatus.MultiSelected;
            }
        }
        private void OnLadderDiagramKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                IsPressingCtrl = true;
            }
            if(e.Key == Key.Left)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    SelectRectLeftWithLine();
                }
                else if((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    SelectionAreaChanged(e.Key);
                }
                else
                {
                    SelectRectLeft();
                }
                e.Handled = true;
            }
            if(e.Key == Key.Right)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    SelectRectRightWithLine();
                }
                else if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    SelectionAreaChanged(e.Key);
                }
                else
                {
                    SelectRectRight();
                }
                e.Handled = true;
            }
            if(e.Key == Key.Down)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    SelectRectDownWithLine();
                }
                else if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    SelectionAreaChanged(e.Key);
                }
                else
                {
                    SelectRectDown();
                }
                e.Handled = true;
            }

            if(e.Key == Key.Up)
            {         
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    SelectRectUpWithLine();            
                }
                else if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    SelectionAreaChanged(e.Key);
                }
                else
                {
                    SelectRectUp();
                }
                e.Handled = true;
            }      
            if(e.Key >= Key.A && e.Key <= Key.Z)
            {
                if((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.None)
                {
                    char c;
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {

                        c = (char)((int)e.Key + 21);
                    }
                    else
                    {
                        c = (char)((int)e.Key + 53);
                    }
                    string s = new string(c, 1);
                    ShowInstructionInputDialog(s);
                }

            }
            if(e.Key == Key.Enter)
            {
                if(_selectRectOwner != null)
                {
                    var viewmodel = _selectRectOwner.SearchElement(_selectRect.X, _selectRect.Y);
                    if(viewmodel != null && viewmodel.Type != LadderInstModel.ElementType.HLine)
                    {
                        viewmodel.BeginShowPropertyDialog();
                    }
                    else
                    {
                        ShowInstructionInputDialog(string.Empty);
                    }
                }
                e.Handled = true;
            }
            if(e.Key == Key.Delete)
            {
                if(SelectionStatus == SelectStatus.SingleSelected)
                {
                    var model = _selectRectOwner.SearchElement(_selectRect.X, _selectRect.Y);
                    if (model != null)
                    {
                        var elements = new List<BaseViewModel>();
                        elements.Add(model);
                        var command = new LadderCommand.NetworkRemoveElementsCommand(_selectRectOwner, elements);
                        _commandManager.Execute(command);
                    }
                }
                else
                {
                    // 多选删除
                    if(SelectionStatus == SelectStatus.MultiSelected)
                    {
                        if(CrossNetState == CrossNetworkState.NoCross)
                        {
                            // 图元多选
                            _commandManager.Execute(new LadderCommand.NetworkRemoveElementsCommand(_selectStartNetwork, _selectStartNetwork.GetSelectedElements(), _selectStartNetwork.GetSelectedVerticalLines()));
                        }
                        else
                        {
                            // 网络多选
                            if(_selectStartNetwork != null)
                            {
                                _selectAllNetworks.Add(_selectStartNetwork);
                            }
                            int index = _selectAllNetworks.ElementAt(0).NetworkNumber;
                            if(_selectAllNetworks.Count == _ladderNetworks.Count)
                            {
                                //全选, 补回一个网络
                                _commandManager.Execute(new LadderCommand.LadderDiagramReplaceNetworksCommand(this, new LadderNetworkViewModel(this, 0), _selectAllNetworks, index));
                            }
                            else
                            {
                                _commandManager.Execute(new LadderCommand.LadderDiagramRemoveNetworksCommand(this, _selectAllNetworks, index));
                            }
                           
                        }

                    }
                }
            }
        }
        private void SelectionAreaChanged(Key key)
        {
            if (_selectStatus == SelectStatus.SingleSelected)
            {
                if (_selectRectOwner != null)
                {
                    _selectStartNetwork = _selectRectOwner;
                }
                if (SingleSelectionAreaCanChange(key))
                {
                    ChangeSingleSelectionArea(key);
                }
            }
            else if(_selectStatus == SelectStatus.MultiSelected)
            {
                _selectStatus = SelectStatus.MultiSelecting;
                if (MutiSelectionAreaCanChange(key))
                {
                    ChangeMutiSelectionArea(key);
                }
            }
            else if(_selectStatus == SelectStatus.MultiSelecting)
            {
                if (MutiSelectionAreaCanChange(key))
                {
                    ChangeMutiSelectionArea(key);
                }
            }
        }
        private bool MutiSelectionAreaCanChange(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    return _selectStartNetwork.SelectAreaSecondX > 0;
                case Key.Right:
                    return _selectStartNetwork.SelectAreaSecondX < GlobalSetting.LadderXCapacity - 1;
                case Key.Up:
                    if (CrossNetState == CrossNetworkState.NoCross)
                    {
                        return _selectStartNetwork.NetworkNumber > 0 || _selectStartNetwork.SelectAreaSecondY > 0;
                    }
                    else if (CrossNetState == CrossNetworkState.CrossDown)
                    {
                        return true;
                    }
                    else
                    {
                        if (_selectAllNetworks.ToList().Exists(x => { return x.NetworkNumber == 0; }))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                case Key.Down:
                    if (CrossNetState == CrossNetworkState.NoCross)
                    {
                        return _selectStartNetwork.NetworkNumber < _ladderNetworks.Count - 1 || _selectStartNetwork.SelectAreaSecondY < _selectStartNetwork.RowCount - 1;
                    }
                    else if (CrossNetState == CrossNetworkState.CrossUp)
                    {
                        return true;
                    }
                    else
                    {
                        if (_selectAllNetworks.ToList().Exists(x => { return x.NetworkNumber == _ladderNetworks.Count - 1; }))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                default:
                    return false;
            }
        }
        private bool SingleSelectionAreaCanChange(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    return SelectionRect.X > 0;
                case Key.Right:
                    return SelectionRect.X < GlobalSetting.LadderXCapacity - 1;
                case Key.Up:
                    return SelectStartNetwork.NetworkNumber > 0 || SelectionRect.Y > 0;
                case Key.Down:
                    return SelectStartNetwork.NetworkNumber < _ladderNetworks.Count - 1 || SelectionRect.Y < SelectStartNetwork.RowCount - 1;
                default:
                    return false;
            }
        }
        private void ChangeMutiSelectionArea(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    if (CrossNetState == CrossNetworkState.NoCross)
                    {
                        if (_selectStartNetwork.SelectAreaSecondX > 0)
                        {
                            _selectStartNetwork.SelectAreaSecondX--;
                            HScrollToRect(_selectStartNetwork.SelectAreaSecondX);
                            if (_selectStartNetwork.SelectAreaSecondX == _selectStartNetwork.SelectAreaFirstX && _selectStartNetwork.SelectArea.Height == GlobalSetting.LadderHeightUnit)
                            {
                                SelectionRect.X = _selectStartNetwork.SelectAreaFirstX;
                                SelectionRect.Y = _selectStartNetwork.SelectAreaFirstY;
                                _selectStartNetwork.AcquireSelectRect();
                            }
                        }
                    }
                    break;
                case Key.Right:
                    if (CrossNetState == CrossNetworkState.NoCross)
                    {
                        if (_selectStartNetwork.SelectAreaSecondX < GlobalSetting.LadderXCapacity - 1)
                        {
                            _selectStartNetwork.SelectAreaSecondX++;
                            HScrollToRect(_selectStartNetwork.SelectAreaSecondX);
                            if (_selectStartNetwork.SelectAreaSecondX == _selectStartNetwork.SelectAreaFirstX && _selectStartNetwork.SelectArea.Height == GlobalSetting.LadderHeightUnit)
                            {
                                SelectionRect.X = _selectStartNetwork.SelectAreaFirstX;
                                SelectionRect.Y = _selectStartNetwork.SelectAreaFirstY;
                                _selectStartNetwork.AcquireSelectRect();
                            }
                        }
                    }
                    break;
                case Key.Up:
                    if (CrossNetState == CrossNetworkState.CrossUp)
                    {
                        CollectSelectAllNetworkUpByCount(_selectAllNetworks.Count + 1);
                        foreach (var net in _selectAllNetworkCache)
                        {
                            net.IsSelectAreaMode = false;
                            net.IsSelectAllMode = false;
                        }
                        foreach (var net in _selectAllNetworks)
                        {
                            net.IsSelectAllMode = true;
                        }
                        VScrollToRect(_selectAllNetworks.First().NetworkNumber,0);
                    }
                    else if (CrossNetState == CrossNetworkState.CrossDown)
                    {
                        CollectSelectAllNetworkDownByCount(_selectAllNetworks.Count - 1);
                        foreach (var net in _selectAllNetworkCache)
                        {
                            net.IsSelectAreaMode = false;
                            net.IsSelectAllMode = false;
                        }
                        if (_selectAllNetworks.Count == 0)
                        {
                            CrossNetState = CrossNetworkState.NoCross;
                            _selectStartNetwork.IsSelectAllMode = false;
                            VScrollToRect(_selectStartNetwork.NetworkNumber, _selectStartNetwork.RowCount - 1);
                        }
                        else
                        {
                            foreach (var net in _selectAllNetworks)
                            {
                                net.IsSelectAllMode = true;
                            }
                            VScrollToRect(_selectAllNetworks.Last().NetworkNumber, _selectAllNetworks.Last().RowCount - 1);
                        }
                    }
                    else
                    {
                        if (_selectStartNetwork.SelectAreaSecondY == 0)
                        {
                            CrossNetState = CrossNetworkState.CrossUp;
                            CollectSelectAllNetworkUpByCount(_selectAllNetworks.Count + 1);
                            _selectStartNetwork.IsSelectAllMode = true;
                            foreach (var net in _selectAllNetworkCache)
                            {
                                net.IsSelectAreaMode = false;
                                net.IsSelectAllMode = false;
                            }
                            foreach (var net in _selectAllNetworks)
                            {
                                net.IsSelectAllMode = true;
                            }
                            VScrollToRect(_selectAllNetworks.First().NetworkNumber, 0);
                        }
                        else
                        {
                            _selectStartNetwork.SelectAreaSecondY--;
                            VScrollToRect(_selectStartNetwork.NetworkNumber, _selectStartNetwork.SelectAreaSecondY);
                            if (_selectStartNetwork.SelectArea.Height == GlobalSetting.LadderHeightUnit && _selectStartNetwork.SelectArea.Width == GlobalSetting.LadderWidthUnit)
                            {
                                _selectRect.X = _selectStartNetwork.SelectAreaFirstX;
                                _selectRect.Y = _selectStartNetwork.SelectAreaFirstY;
                                _selectStartNetwork.AcquireSelectRect();
                            }
                        }
                    }
                    break;
                case Key.Down:
                    if (CrossNetState == CrossNetworkState.CrossDown)
                    {
                        CollectSelectAllNetworkDownByCount(_selectAllNetworks.Count + 1);
                        foreach (var net in _selectAllNetworkCache)
                        {
                            net.IsSelectAreaMode = false;
                            net.IsSelectAllMode = false;
                        }
                        foreach (var net in _selectAllNetworks)
                        {
                            net.IsSelectAllMode = true;
                        }
                        VScrollToRect(_selectAllNetworks.Last().NetworkNumber, _selectAllNetworks.Last().RowCount - 1);
                    }
                    else if (CrossNetState == CrossNetworkState.CrossUp)
                    {
                        CollectSelectAllNetworkUpByCount(_selectAllNetworks.Count - 1);
                        foreach (var net in _selectAllNetworkCache)
                        {
                            net.IsSelectAreaMode = false;
                            net.IsSelectAllMode = false;
                        }
                        if (_selectAllNetworks.Count == 0)
                        {
                            CrossNetState = CrossNetworkState.NoCross;
                            _selectStartNetwork.IsSelectAllMode = false;
                            VScrollToRect(_selectStartNetwork.NetworkNumber, 0);
                        }
                        else
                        {
                            foreach (var net in _selectAllNetworks)
                            {
                                net.IsSelectAllMode = true;
                            }
                            VScrollToRect(_selectAllNetworks.First().NetworkNumber, 0);
                        }
                    }
                    else
                    {
                        if (_selectStartNetwork.SelectAreaSecondY == _selectStartNetwork.RowCount - 1)
                        {
                            CrossNetState = CrossNetworkState.CrossDown;
                            CollectSelectAllNetworkDownByCount(_selectAllNetworks.Count + 1);
                            _selectStartNetwork.IsSelectAllMode = true;
                            foreach (var net in _selectAllNetworkCache)
                            {
                                net.IsSelectAreaMode = false;
                                net.IsSelectAllMode = false;
                            }
                            foreach (var net in _selectAllNetworks)
                            {
                                net.IsSelectAllMode = true;
                            }
                            VScrollToRect(_selectAllNetworks.Last().NetworkNumber, _selectAllNetworks.Last().RowCount - 1);
                        }
                        else
                        {
                            _selectStartNetwork.SelectAreaSecondY++;
                            VScrollToRect(_selectStartNetwork.NetworkNumber, _selectStartNetwork.SelectAreaSecondY);
                            if (_selectStartNetwork.SelectArea.Height == GlobalSetting.LadderHeightUnit && _selectStartNetwork.SelectArea.Width == GlobalSetting.LadderWidthUnit)
                            {
                                _selectRect.X = _selectStartNetwork.SelectAreaFirstX;
                                _selectRect.Y = _selectStartNetwork.SelectAreaFirstY;
                                _selectStartNetwork.AcquireSelectRect();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        private void ChangeSingleSelectionArea(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    _selectStartNetwork.SelectAreaOriginX = _selectRect.X;
                    _selectStartNetwork.SelectAreaOriginY = _selectRect.Y;
                    _selectStartNetwork.SelectAreaFirstX = _selectRect.X;
                    _selectStartNetwork.SelectAreaFirstY = _selectRect.Y;
                    _selectStartNetwork.SelectAreaSecondX = _selectRect.X - 1;
                    _selectStartNetwork.SelectAreaSecondY = _selectRect.Y;
                    HScrollToRect(_selectStartNetwork.SelectAreaSecondX);
                    CrossNetState = CrossNetworkState.NoCross;
                    SelectionStatus = SelectStatus.MultiSelecting;
                    break;
                case Key.Right:
                    _selectStartNetwork.SelectAreaOriginX = _selectRect.X;
                    _selectStartNetwork.SelectAreaOriginY = _selectRect.Y;
                    _selectStartNetwork.SelectAreaFirstX = _selectRect.X;
                    _selectStartNetwork.SelectAreaFirstY = _selectRect.Y;
                    _selectStartNetwork.SelectAreaSecondX = _selectRect.X + 1;
                    _selectStartNetwork.SelectAreaSecondY = _selectRect.Y;
                    HScrollToRect(_selectStartNetwork.SelectAreaSecondX);
                    CrossNetState = CrossNetworkState.NoCross;
                    SelectionStatus = SelectStatus.MultiSelecting;
                    break;
                case Key.Up:
                    if (_selectRect.Y == 0)
                    {
                        CrossNetState = CrossNetworkState.CrossUp;
                        CollectSelectAllNetworkUpByCount(_selectAllNetworks.Count + 1);
                        _selectStartNetwork.IsSelectAllMode = true;
                        foreach (var net in _selectAllNetworkCache)
                        {
                            net.IsSelectAreaMode = false;
                            net.IsSelectAllMode = false;
                        }
                        foreach (var net in _selectAllNetworks)
                        {
                            net.IsSelectAllMode = true;
                        }
                        VScrollToRect(_selectAllNetworks.First().NetworkNumber,0);
                    }
                    else
                    {
                        _selectStartNetwork.SelectAreaOriginX = _selectRect.X;
                        _selectStartNetwork.SelectAreaOriginY = _selectRect.Y;
                        _selectStartNetwork.SelectAreaFirstX = _selectRect.X;
                        _selectStartNetwork.SelectAreaFirstY = _selectRect.Y;
                        _selectStartNetwork.SelectAreaSecondX = _selectRect.X;
                        _selectStartNetwork.SelectAreaSecondY = _selectRect.Y - 1;
                        CrossNetState = CrossNetworkState.NoCross;
                        VScrollToRect(_selectStartNetwork.NetworkNumber, _selectStartNetwork.SelectAreaSecondY);
                    }
                    SelectionStatus = SelectStatus.MultiSelecting;
                    break;
                case Key.Down:
                    if (_selectRect.Y == _selectStartNetwork.RowCount - 1)
                    {
                        CrossNetState = CrossNetworkState.CrossDown;
                        CollectSelectAllNetworkDownByCount(_selectAllNetworks.Count + 1);
                        _selectStartNetwork.IsSelectAllMode = true;
                        foreach (var net in _selectAllNetworkCache)
                        {
                            net.IsSelectAreaMode = false;
                            net.IsSelectAllMode = false;
                        }
                        foreach (var net in _selectAllNetworks)
                        {
                            net.IsSelectAllMode = true;
                        }
                        VScrollToRect(_selectAllNetworks.Last().NetworkNumber, _selectAllNetworks.Last().RowCount - 1);
                    }
                    else
                    {
                        _selectStartNetwork.SelectAreaOriginX = _selectRect.X;
                        _selectStartNetwork.SelectAreaOriginY = _selectRect.Y;
                        _selectStartNetwork.SelectAreaFirstX = _selectRect.X;
                        _selectStartNetwork.SelectAreaFirstY = _selectRect.Y;
                        _selectStartNetwork.SelectAreaSecondX = _selectRect.X;
                        _selectStartNetwork.SelectAreaSecondY = _selectRect.Y + 1;
                        CrossNetState = CrossNetworkState.NoCross;
                        VScrollToRect(_selectStartNetwork.NetworkNumber, _selectStartNetwork.SelectAreaSecondY);
                    }
                    SelectionStatus = SelectStatus.MultiSelecting;
                    break;
                default:
                    break;
            }
        }
        private void OnLadderDiagramMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                int scaleX = e.Delta / 100;
                int scaleY = scaleX;
                GlobalSetting.LadderScaleX += scaleX * 0.01;
                GlobalSetting.LadderScaleY += scaleY * 0.01;
                // 不继续冒泡传递事件
                e.Handled = true;
            }
        }
        private void OnCommentAreaMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditCommentReact();
        }
        private void OnEditComment(object sender, RoutedEventArgs e)
        {
            EditCommentReact();
        }
        private void OnLadderDiagramMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this);
            if(e.LeftButton == MouseButtonState.Pressed)
            {           
                SelectionStatus = SelectStatus.Idle;
            }
        }
        private void OnLadderDiagramMouseMove(object sender, MouseEventArgs e)
        {
            Point _p = e.GetPosition(this);
            if (_p.X > _actualWidth - 30
             || _p.Y > _actualHeight - 50)
            {
                return;
            }
            if(_selectStatus == SelectStatus.SingleSelected)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if(_selectRectOwner != null)
                    {
                        var p = e.GetPosition(_selectRectOwner.LadderCanvas);
                        var pp = IntPoint.GetIntpointByDouble(p.X, p.Y, WidthUnit, HeightUnit);
                        if(p.Y < 0 || (pp.X != _selectRect.X) || (pp.Y != _selectRect.Y))
                        {
                            _selectStartNetwork = _selectRectOwner;
                            _selectStartNetwork.SelectAreaOriginX = _selectRect.X;
                            _selectStartNetwork.SelectAreaOriginY = _selectRect.Y;
                            _selectStartNetwork.SelectAreaFirstX = _selectRect.X;
                            _selectStartNetwork.SelectAreaFirstY = _selectRect.Y;
                            _selectStartNetwork.SelectAreaSecondX = _selectRect.X;
                            _selectStartNetwork.SelectAreaSecondY = _selectRect.Y;
                            SelectionStatus = SelectStatus.MultiSelecting;
                        } 
                    }
                }
            }
            if (_selectStatus == SelectStatus.MultiSelecting)
            {
                if(e.LeftButton == MouseButtonState.Pressed)
                {
                    CrossNetState = GetSelectionCrossNetworkState();
                    switch (CrossNetState)
                    {
                        case CrossNetworkState.NoCross:
                            foreach (var net in _selectAllNetworkCache)
                            {
                                net.IsSelectAreaMode = false;
                                net.IsSelectAllMode = false;
                            }
                            var p = e.GetPosition(_selectStartNetwork.LadderCanvas);
                            var pp = IntPoint.GetIntpointByDouble(p.X, p.Y, WidthUnit, HeightUnit);
                            _selectStartNetwork.IsSelectAllMode = false;
                            if (pp.X == _selectStartNetwork.SelectAreaOriginX && pp.Y == _selectStartNetwork.SelectAreaOriginY)
                            {
                                SelectionRect.X = pp.X;
                                SelectionRect.Y = pp.Y;
                                _selectStartNetwork.AcquireSelectRect();
                            }
                            else
                            {
                                _selectStartNetwork.SelectAreaFirstX = _selectStartNetwork.SelectAreaOriginX;
                                _selectStartNetwork.SelectAreaFirstY = _selectStartNetwork.SelectAreaOriginY;
                                _selectStartNetwork.SelectAreaSecondX = pp.X;
                                _selectStartNetwork.SelectAreaSecondY = pp.Y;
                            }
                            break;
                        case CrossNetworkState.CrossUp:
                            CollectSelectAllNetworkUp();
                            _selectStartNetwork.IsSelectAllMode = true;
                            foreach (var net in _selectAllNetworkCache)
                            {
                                net.IsSelectAreaMode = false;
                                net.IsSelectAllMode = false;
                            }
                            foreach (var net in _selectAllNetworks)
                            {
                                net.IsSelectAllMode = true;
                            }
                            break;
                        case CrossNetworkState.CrossDown:
                            CollectSelectAllNetworkDown();
                            _selectStartNetwork.IsSelectAllMode = true;
                            foreach (var net in _selectAllNetworkCache)
                            {
                                net.IsSelectAreaMode = false;
                                net.IsSelectAllMode = false;
                            }
                            foreach (var net in _selectAllNetworks)
                            {
                                net.IsSelectAllMode = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void OnLadderDiagramMouseUp(object sender, MouseButtonEventArgs e)
        {
            // 如果处于选择模式则关闭
            if (_selectStatus == SelectStatus.MultiSelecting)
            {
                SelectionStatus = SelectStatus.MultiSelected;
            }
        }

        private void OnLadderDiagramLostFocus(object sender, RoutedEventArgs e)
        {
            SelectionStatus = SelectStatus.Idle;
        }

        private void OnInstructionCursorChanged(object sender, RoutedEventArgs e)
        {
            if (sender is InstructionNetworkViewModel)
            {
                InstructionNetworkViewModel invmodel = (InstructionNetworkViewModel)(sender);
                BaseViewModel viewmodel = invmodel.CurrentViewModel;
                LadderNetworkViewModel lnvmodel = invmodel.LadderNetwork;
                _projectModel.IFacade.NavigateToNetwork(
                    new NavigateToNetworkEventArgs(
                        lnvmodel.NetworkNumber, ProgramName,
                        viewmodel.X, viewmodel.Y));
            }
        }

        private void OnInstructionCursorEdit(object sender, RoutedEventArgs e)
        {
            if (sender is InstructionNetworkViewModel)
            {
                ShowInstructionInputDialog("");
            }
        }
        
        #endregion

        #region Command execute

        /// <summary>
        /// 执行梯形图复制剪切
        /// </summary>
        /// <param name="copy">False = 剪切，True = 复制</param>
        private void CutCopyExecute(bool copy)
        {
            if (SelectionStatus == SelectStatus.SingleSelected)
            {
                // 单元素复制
                List<BaseViewModel> listele = new List<BaseViewModel>();
                var viewmodel = _selectRectOwner.SearchElement(_selectRect.X, _selectRect.Y);
                if (viewmodel != null)
                {
                    listele.Add(viewmodel);
                    if(!copy)
                    {
                        var command = new LadderCommand.NetworkRemoveElementsCommand(_selectRectOwner, listele, new List<VerticalLineViewModel>());
                        _commandManager.Execute(command);
                    }
                    var xEle = ProjectHelper.CreateXElementByLadderElementsAndVertialLines(listele, new List<VerticalLineViewModel>(), _selectRect.X, _selectRect.Y, 1, 1);
                    Clipboard.SetData("LadderContent", xEle.ToString());
                }
            }
            else
            {
                if (SelectionStatus == SelectStatus.MultiSelected)
                {
                    // 多网络复制
                    if (CrossNetState == CrossNetworkState.CrossDown || CrossNetState == CrossNetworkState.CrossUp)
                    {
                        XElement xEle = new XElement("Networks");
                        if(_selectStartNetwork != null)
                        {
                            _selectAllNetworks.Add(_selectStartNetwork);
                        }
                        var removednets = new List<LadderNetworkViewModel>();
                        foreach (var net in _selectAllNetworks)
                        {
                            if(!copy)
                            {
                                removednets.Add(net);
                            }
                            xEle.Add(ProjectHelper.CreateXElemnetByLadderNetwork(net));
                        }
                        if(!copy)
                        {
                            int index = removednets[0].NetworkNumber;
                            if(removednets.Count == _ladderNetworks.Count)
                            {
                                //全选，补回一个空网络
                                _commandManager.Execute(new LadderCommand.LadderDiagramReplaceNetworksCommand(this, new LadderNetworkViewModel(this, 0), removednets, index));
                            }
                            else
                            {
                                _commandManager.Execute(new LadderCommand.LadderDiagramRemoveNetworksCommand(this, removednets, index));
                            }

                        }
                        Clipboard.SetData("LadderContent", xEle.ToString());
                        SelectionStatus = SelectStatus.Idle;
                    }
                    else
                    {
                        // 单网络多图元复制
                        int xBegin = Math.Min(_selectStartNetwork.SelectAreaFirstX, _selectStartNetwork.SelectAreaSecondX);
                        int yBegin = Math.Min(_selectStartNetwork.SelectAreaFirstY, _selectStartNetwork.SelectAreaSecondY);
                        int xEnd = Math.Max(_selectStartNetwork.SelectAreaFirstX, _selectStartNetwork.SelectAreaSecondX);
                        int yEnd = Math.Max(_selectStartNetwork.SelectAreaFirstY, _selectStartNetwork.SelectAreaSecondY);
                        XElement xEle = ProjectHelper.CreateXElementByLadderElementsAndVertialLines(_selectStartNetwork.GetSelectedElements(),
                                                                                                    _selectStartNetwork.GetSelectedVerticalLines(),
                                                                                                    xBegin, yBegin, xEnd - xBegin + 1, yEnd - yBegin + 1);
                        if(!copy)
                        {
                            var command = new LadderCommand.NetworkRemoveElementsCommand(_selectStartNetwork, _selectStartNetwork.GetSelectedElements(), _selectStartNetwork.GetSelectedVerticalLines());
                            _commandManager.Execute(command);
                        }
                        Clipboard.SetData("LadderContent", xEle.ToString());
                        SelectionStatus = SelectStatus.Idle;
                    }

                }
            }
        }

        private void PasteNetworksExecute(XElement xEle)
        {
            if (_selectRectOwner != null)
            {
                // TODO 命令化
                var replacednets = new SortedSet<LadderNetworkViewModel>();
                var removednets = new SortedSet<LadderNetworkViewModel>();            
                removednets.Add(_selectRectOwner);
                foreach (XElement netEle in xEle.Elements("Network"))
                {
                    var net = ProjectHelper.CreateLadderNetworkByXElement(netEle, this);
                    replacednets.Add(net);
                }
                int index = removednets.First().NetworkNumber;
                var command = new LadderCommand.LadderDiagramReplaceNetworksCommand(this, replacednets, removednets, index);
                _commandManager.Execute(command);
            }
            else
            {
                if (_selectStartNetwork != null)
                {
                    // TODO 命令化
                    var replacednets = new SortedSet<LadderNetworkViewModel>();
                    var removednets = new SortedSet<LadderNetworkViewModel>();
                    foreach (XElement netEle in xEle.Elements("Network"))
                    {
                        var net = ProjectHelper.CreateLadderNetworkByXElement(netEle, this);
                        replacednets.Add(net);
                    }
                    removednets.Add(_selectStartNetwork);
                    foreach (var net in _selectAllNetworks)
                    {
                        removednets.Add(net);
                    }
                    int index = removednets.First().NetworkNumber;
                    var command = new LadderCommand.LadderDiagramReplaceNetworksCommand(this, replacednets, removednets, index);
                    _commandManager.Execute(command);
                }
            }
        }

        private void PasteElementsExecute(XElement xEle)
        {
            //获取复制区域的大小
            int xBegin = int.Parse(xEle.Attribute("XBegin").Value);
            int yBegin = int.Parse(xEle.Attribute("YBegin").Value);
            int width = int.Parse(xEle.Attribute("Width").Value);
            int height = int.Parse(xEle.Attribute("Height").Value);
            int xStart = 0;
            int yStart = 0;
            var elements = ProjectHelper.CreateLadderElementsByXElement(xEle);
            var vlines = ProjectHelper.CreateLadderVertialLineByXElement(xEle);
            IEnumerable<BaseViewModel> oldelements = new List<BaseViewModel>();
            IEnumerable<VerticalLineViewModel> oldvlines = new List<VerticalLineViewModel>();
            bool containOutput = elements.Any(x => x.Type == LadderInstModel.ElementType.Output);
            var targetNetwork = _selectRectOwner;
            if (_selectRectOwner != null)
            {
                //单选
                if(containOutput)
                {
                    xStart = GlobalSetting.LadderXCapacity - width;
                }
                else
                {
                    xStart = _selectRect.X;
                }
                yStart = _selectRect.Y;
                //若超出网络高度，重新设定高度
                if(yStart + height > _selectRectOwner.RowCount)
                {
                    // 可撤销的命令
                    var command1 = new LadderCommand.NetworkRowCountChangeCommand(_selectRectOwner, yStart + height);
                    _commandManager.Execute(command1);
                }
                oldelements = _selectRectOwner.GetElements().Where(ele => ele.X >= xStart && ele.Y >= yStart && ele.X <= xStart + width -1 && ele.Y <= yStart + height - 1);
                oldvlines = _selectRectOwner.GetVerticalLines().Where(ele => ele.X >= xStart && ele.Y >= yStart && ele.X < xStart + width - 1 && ele.Y < yStart + height - 1);
            }
            else
            {
                if(_selectStartNetwork != null)
                {
                    //多选
                    targetNetwork = _selectStartNetwork;
                    if (containOutput)
                    {
                        xStart = GlobalSetting.LadderXCapacity - width;
                    }
                    else
                    {
                        xStart = Math.Min(_selectStartNetwork.SelectAreaFirstX, _selectStartNetwork.SelectAreaSecondX);
                    }         
                    yStart = Math.Min(_selectStartNetwork.SelectAreaFirstY, _selectStartNetwork.SelectAreaSecondY);
                    if (yStart + height > _selectStartNetwork.RowCount)
                    {
                        //可撤销的命令
                        var command1 = new LadderCommand.NetworkRowCountChangeCommand(_selectStartNetwork, yStart + height);
                        _commandManager.Execute(command1);
                    }
                    oldelements = _selectStartNetwork.GetSelectedElements().Union(_selectStartNetwork.GetElements().Where(ele => ele.X >= xStart && ele.Y >= yStart && ele.X <= xStart + width - 1 && ele.Y <= yStart + height - 1));
                    oldvlines = _selectStartNetwork.GetSelectedVerticalLines().Union(_selectStartNetwork.GetVerticalLines().Where(ele => ele.X >= xStart && ele.Y >= yStart && ele.X < xStart + width - 1 && ele.Y < yStart + height - 1));
                }
            }
            foreach (var element in elements)
            {
                element.X = xStart + element.X - xBegin;
                element.Y = yStart + element.Y - yBegin;
            }
            foreach (var vline in vlines)
            {
                vline.X = xStart + vline.X - xBegin;
                vline.Y = yStart + vline.Y - yBegin;
            }
            LadderCommand.NetworkReplaceElementsCommand command = new LadderCommand.NetworkReplaceElementsCommand(targetNetwork, elements, vlines, oldelements, oldvlines);
            _commandManager.Execute(command);
        }

        private void OnCutCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            CutCopyExecute(false);
        }

        private void OnCopyCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            CutCopyExecute(true);
        }

        private void OnPasteCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            string str;
            str = Clipboard.GetData("LadderContent") as string;
            if(str != null)
            {
                //多网络粘贴
                XElement xEle = XElement.Parse(str);
                if(xEle.Name == "LadderContent")
                {
                    PasteElementsExecute(xEle);
                }
                else
                {
                    if(xEle.Name == "Networks")
                    {
                        PasteNetworksExecute(xEle);
                    }
                }
            }
        }

        private void OnUndoCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            _commandManager.Undo();
        }

        private void OnRedoCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            _commandManager.Redo();
        }

        private void OnSelectAllCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            SelectionStatus = SelectStatus.MultiSelected;
            foreach (var net in _ladderNetworks)
            {
                _selectAllNetworkCache.Add(net);
                _selectAllNetworks.Add(net);
                net.IsSelectAllMode = true;
            }
        }

        //public event ExecutedRoutedEventHandler FindCommandExecute = delegate { };
        
        private void OnFindCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            //FindCommandExecute(sender, e);   
            ProjectModel.IFacade.MainWindow.LACFind.Show();
        }

        //public event ExecutedRoutedEventHandler ReplaceCommandExecute = delegate { };

        private void OnReplaceCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            //ReplaceCommandExecute(sender, e);
            ProjectModel.IFacade.MainWindow.LACReplace.Show();
        }
        #endregion

        #region Command can execute

        private void CutCopyCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if(SelectionStatus == SelectStatus.SingleSelected)
            {
                int x = _selectRect.X;
                int y = _selectRect.Y;
                if(_selectRectOwner.SearchElement(x, y) != null || _selectRectOwner.SearchVerticalLine(x, y) != null)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            else
            {
                e.CanExecute = SelectionStatus == SelectStatus.MultiSelected;
            }
        }

        private void PasteCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if(Clipboard.ContainsData("LadderContent") && (SelectionStatus == SelectStatus.MultiSelected || SelectionStatus == SelectStatus.SingleSelected))
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void UndoCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _commandManager.CanUndo;
        }

        private void RedoCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _commandManager.CanRedo;
        }

        private void FindCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ReplaceCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region Selection state transfers
        private void EnterIdleState()
        {
            if (_selectStartNetwork != null)
            {
                _selectStartNetwork.IsSelectAreaMode = false;
                _selectStartNetwork.IsSelectAllMode = false;
                _selectStartNetwork = null;
            }
            if (_selectRectOwner != null)
            {
                _selectRectOwner.ReleaseSelectRect();
                _selectRectOwner = null;
            }
            _selectAllNetworks.Clear();
            foreach (var net in _selectAllNetworkCache)
            {
                net.IsSelectAreaMode = false;
                net.IsSelectAllMode = false;
            }
            _selectAllNetworkCache.Clear();
            CrossNetState = CrossNetworkState.NoCross;
        }

        private void EnterSingleSelectedState()
        {
            if (_selectStartNetwork != null)
            {
                _selectStartNetwork.IsSelectAreaMode = false;
                _selectStartNetwork.IsSelectAllMode = false;
                _selectStartNetwork = null;
            }
            _selectAllNetworks.Clear();
            foreach (var net in _selectAllNetworkCache)
            {
                net.IsSelectAreaMode = false;
                net.IsSelectAllMode = false;
            }
            _selectAllNetworkCache.Clear();
            CrossNetState = CrossNetworkState.NoCross;
        }

        private void EnterMultiSelectingState()
        {
            if (_selectStartNetwork != null)
            {
                _selectStartNetwork.IsSelectAreaMode = true;
            }
            if (_selectRectOwner != null)
            {
                _selectRectOwner.ReleaseSelectRect();
                _selectRectOwner = null;
            }
        }

        private void EnterMultiSelectedState()
        {
            if (_selectRectOwner != null)
            {
                _selectRectOwner.ReleaseSelectRect();
                _selectRectOwner = null;
            }
        }

        #endregion
	    private bool loadedbefore = false;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!loadedbefore)
            {
                Focus();
                Keyboard.Focus(this);
                loadedbefore = true;
            }
        }
        #region ladder expand module
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Color color = new Color();
            color.A = 255;
            color.R = 60;
            color.G = 58;
            color.B = 58;
            SolidColorBrush brush = new SolidColorBrush(color);
            Rect.Fill = brush;
            Rect.Opacity = 0.08;
            ladderExpander.Background = brush;
            ladderExpander.Opacity = 0.2;
        }
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.Transparent);
            Rect.Fill = brush;
            Rect.Opacity = 1;
            ladderExpander.Background = brush;
            ladderExpander.Opacity = 1;
        }
        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ladderExpander.IsExpand = !ladderExpander.IsExpand;
        }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_canScrollToolTip)
            {
                ScrollViewer scroll = (ScrollViewer)((ToolTip)ThumbnailButton.ToolTip).Content;
                scroll.ScrollToVerticalOffset(scroll.VerticalOffset + e.Delta / 20);
            }
        }
        private void ThumbnailButton_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            _canScrollToolTip = false;
        }
        private void ThumbnailButton_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            _canScrollToolTip = true;
        }
        private void ExpandButton_IsExpandChanged(object sender, RoutedEventArgs e)
        {
            ExpandLadder(ladderExpander.IsExpand);
        }
        private ToolTip GenerateToolTipByLadder()
        {
            ToolTip tooltip = new ToolTip();
            ScrollViewer scroll = new ScrollViewer();
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            StackPanel stackpanel = new StackPanel();
            scroll.MaxHeight = 385;
            stackpanel.Background = new SolidColorBrush(Colors.White);
            stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            ScaleTransform transform = new ScaleTransform(GlobalSetting.LadderOriginScaleX / 1.7, GlobalSetting.LadderOriginScaleY / 1.7);
            foreach (LadderNetworkViewModel net in _ladderNetworks)
            {
                stackpanel.Children.Add(net);
            }
            stackpanel.LayoutTransform = transform;
            scroll.Content = stackpanel;
            tooltip.Content = scroll;
            return tooltip;
        }
        private void RemoveToolTipByLadder(ToolTip tooltip)
        {
            if (tooltip != null)
            {
                StackPanel stackpanel = (StackPanel)((ScrollViewer)tooltip.Content).Content;
                stackpanel.LayoutTransform = null;
                stackpanel.Children.Clear();
            }
        }
        private void ExpandLadder(bool isExpand)
        {
            if (isExpand)
            {
                RemoveToolTipByLadder((ToolTip)ThumbnailButton.ToolTip);
                ThumbnailButton.ToolTip = null;
                ReloadNetworksToStackPanel();
            }
            else
            {
                LadderNetworkStackPanel.Children.Clear();
                ThumbnailButton.ToolTip = GenerateToolTipByLadder();
            }
        }
        #endregion
        private LadderNetworkViewModel dragItem;
        private LadderNetworkViewModel currentItem;
        private void OnDragOver(object sender, DragEventArgs e)
        {

        }
        private void OnDragEnter(object sender, DragEventArgs e)
        {

        }
        private void OnDragLeave(object sender, DragEventArgs e)
        {

        }
        private void OnDrop(object sender, DragEventArgs e)
        {
            var t = e.Source;
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var network = GetNetworkByMouse();
                if (network != null && !network.ladderExpander.IsExpand)
                {
                    if (currentItem == null)
                    {
                        currentItem = network;
                    }
                    else
                    {
                        if (currentItem != network)
                        {
                            dragItem = currentItem;
                            DragDrop.DoDragDrop(this, dragItem, DragDropEffects.Move);
                        }
                    }
                }
            }
        }
        private LadderNetworkViewModel GetNetworkByMouse()
        {
            foreach (var network in LadderNetworks)
            {
                if (network.IsMouseOver)
                {
                    return network;
                }
            }
            return null;
        }
    }
}
