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
        private class InstructionExecption : Exception
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

        private SelectRect _selectRect = new SelectRect();

        private LadderNetworkViewModel _selectStartNetwork;

        private SortedSet<LadderNetworkViewModel> _selectAllNetworks = new SortedSet<LadderNetworkViewModel>();

        private SortedSet<LadderNetworkViewModel> _selectAllNetworkCache = new SortedSet<LadderNetworkViewModel>();

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

        private LadderCommand.CommandManager _commandManager = new LadderCommand.CommandManager();

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

        public LadderDiagramViewModel(string name)
        {
            InitializeComponent();
            InitializeInstructionNameAndToolTips();
            ProgramName = name;
            LadderCommentTextBlock.DataContext = this;
            this.Loaded += (sender, e) =>
            {
                Focus();
                Keyboard.Focus(this);
            };
            IDVModel = new InstructionDiagramViewModel();
            AppendNetwork(new LadderNetworkViewModel(this, 0));
        }
        public void NavigateToNetworkByNum(int num)
        {
            double scale = GlobalSetting.LadderScaleX;
            double offset = scale * (MainBorder.ActualHeight + 20) / 3;
            foreach (var item in GetNetworks().Where(x => { return x.NetworkNumber < num; }))
            {
                offset += scale * (item.ActualHeight + 20) / 3;
            }
            MainScrollViewer.ScrollToVerticalOffset(offset);
        }
        private void InitializeInstructionNameAndToolTips()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine(assembly.GetManifestResourceNames());
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
                    tempDic.Add(node.Name.ToString(), tempList);
                }
            }
            InstrutionNameAndToolTips = tempDic;
        }
        #region JMP,LBL,FOR,NEXT instructions check
        public bool CheckProgramControlInstructions()
        {
             List<BaseViewModel> eles = GetProgramControlViewModels();
            List<BaseViewModel> eles_for = eles.Where(x => { return x.GetType() == typeof(FORViewModel); }).ToList();
            List<BaseViewModel> eles_next = eles.Where(x => { return x.GetType() == typeof(NEXTViewModel); }).ToList();
            List<BaseViewModel> eles_jmp = eles.Where(x => { return x.GetType() == typeof(JMPViewModel); }).ToList();
            List<BaseViewModel> eles_lbl = eles.Where(x => { return x.GetType() == typeof(LBLViewModel); }).ToList();
            if (eles_for.Count != eles_next.Count || eles_jmp.Count != eles_lbl.Count)
            {
                return false;
            }
            else
            {
                foreach (var ele_jmp in eles_jmp)
                {
                    string lblindex = (ele_jmp.Model as JMPModel).LBLIndex.ToString();
                    if (!eles_lbl.Exists(x => { return (x.Model as LBLModel).LBLIndex.ToString() == lblindex; }))
                        {
                        return false;
                    }
                }
                return true;
            }
        }
        private List<BaseViewModel> GetProgramControlViewModels()
            {
            List<BaseViewModel> eles = new List<BaseViewModel>();
            foreach (var network in GetNetworks())
            {
                foreach (var model in network.GetElements().Where(x => { return x.GetType() == typeof(FORViewModel) || x.GetType() == typeof(NEXTViewModel) || x.GetType() == typeof(JMPViewModel) || x.GetType() == typeof(LBLViewModel); }))
                {
                    eles.Add(model);
                }
            }
            return eles;
        }
        #endregion
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
            _ladderNetworks.Clear();
            IDVModel.Setup(this);
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
            IDVModel.Setup(this);
        }

        #region Network manipulation，undoable command form method
        public void RemoveSingleNetworkCommand(LadderNetworkViewModel network)
        {
            var command = new LadderCommand.LadderDiagramRemoveNetworksCommand(this, new List<LadderNetworkViewModel>() { network }, network.NetworkNumber);
            _commandManager.Execute(command);
            IDVModel.Setup(this);
        }
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
            var command = new LadderCommand.NetworkReplaceElementsCommand(_selectRectOwner, elements, oldelements);
            _commandManager.Execute(command);
            IDVModel.Setup(this);
        }
        public void AddNewNetworkBefore(LadderNetworkViewModel network)
        {
            var newnetwork = new LadderNetworkViewModel(this, network.NetworkNumber);
            var command = new LadderCommand.LadderDiagramReplaceNetworksCommand(this, newnetwork, newnetwork.NetworkNumber);
            _commandManager.Execute(command);
            IDVModel.Setup(this);
        }
        public void AddNewNetworkAfter(LadderNetworkViewModel network)
        {
            var newnetwork = new LadderNetworkViewModel(this, network.NetworkNumber + 1);
            var command = new LadderCommand.LadderDiagramReplaceNetworksCommand(this, newnetwork, newnetwork.NetworkNumber);
            _commandManager.Execute(command);
            IDVModel.Setup(this);
        }
        public void AppendNewNetwork()
        {
            var network = new LadderNetworkViewModel(this, _ladderNetworks.Count);
            var command = new LadderCommand.LadderDiagramReplaceNetworksCommand(this, network, network.NetworkNumber);
            _commandManager.Execute(command);
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
                ReplaceSingleElement(_selectRectOwner, viewmodel);
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
                    n++;
                }
            }
     
            ReloadNetworksToStackPanel();
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
                    LadderNetworkStackPanel.Children.Remove(network);
                }
            }
        }

        #endregion

        
        #endregion

        #region Comment area manipulation
        private void EditCommentReact()
        {
            LadderDiagramCommentEditDialog dialog = new LadderDiagramCommentEditDialog();
            dialog.LadderComment = this.LadderComment;
            dialog.LadderName = this.ProgramName;
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
            foreach (var net in _ladderNetworks)
            {
                logicCode += net.GenerateCode();
            }
            string code = string.Format("void {0}()\r\n{{\r\n{1}\r\n}}\r\n", functionName, logicCode);
            return code;
        }
        #endregion

        #region Selection Rectangle Relative

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
            }
        }

        private void SelectRectLeft()
        {
            if (_selectRectOwner != null)
            {
                if (_selectRect.X > 0)
                {
                    _selectRect.X--;
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

        #endregion

        #region Instruction relative
        private void ShowInstructionInputDialog(string initialString)
        {
            InstructionInputDialog dialog = new InstructionInputDialog(initialString, InstrutionNameAndToolTips);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dialog.EnsureButtonClick += (sender, e) =>
            {
                BaseViewModel viewmodel;
                try
                {
                    List<string> InstructionInput = dialog.InstructionInput.ToUpper().Trim().Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (InstructionInput.Count == 0)
                    {
                        throw new InstructionExecption(string.Format("the input is empty"));
                    }
                    else
                    {
                        if (!LadderInstViewModelPrototype.CheckInstructionName(InstructionInput[0]))
                        {
                            throw new InstructionExecption(string.Format("the instructionName is not exist!"));
                        }
                    }
                    viewmodel = LadderInstViewModelPrototype.Clone(InstructionInput[0]);
                    InstructionInput.RemoveAt(0);
                    List<string> valueStrings = new List<string>();
                    foreach (var valueString in InstructionInput)
                    {
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
                        viewmodel.AcceptNewValues(valueStrings,PLCDeviceManager.SelectDevice);
                    }
                    if (viewmodel.Type == LadderInstModel.ElementType.Output)
                    {
                        int x = _selectRect.X;
                        int y = _selectRect.Y;
                        var oldelements = _selectRectOwner.GetElements().Where(ele => ele.Y == y && ele.X >= x);
                        var elements = new List<BaseViewModel>();
                        for (int i = x; i < GlobalSetting.LadderXCapacity - 1; i++)
                        {
                            elements.Add(new HorizontalLineViewModel() { X = i, Y = y });
                        }
                        viewmodel.X = GlobalSetting.LadderXCapacity - 1;
                        viewmodel.Y = _selectRect.Y;
                        if (valueStrings.Count == viewmodel.GetValueString().Count() * 2)
                        {
                            viewmodel.AcceptNewValues(valueStrings, PLCDeviceManager.SelectDevice);
                        }
                        elements.Add(viewmodel);
                        _selectRect.X = GlobalSetting.LadderXCapacity - 1;
                        var command = new LadderCommand.NetworkReplaceElementsCommand(_selectRectOwner, elements, oldelements);
                        _commandManager.Execute(command);
                    }
                    else
                    {
                        viewmodel.X = _selectRect.X;
                        viewmodel.Y = _selectRect.Y;
                        if (valueStrings.Count == viewmodel.GetValueString().Count() * 2)
                        {
                            viewmodel.AcceptNewValues(valueStrings, PLCDeviceManager.SelectDevice);
                        }
                        ReplaceSingleElement(_selectRectOwner, viewmodel);
                        if (_selectRect.X < GlobalSetting.LadderXCapacity - 1)
                        {
                            _selectRect.X++;
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(string.Format(exception.Message));
                    return;
                }
                dialog.Close();
            };
            dialog.ShowDialog();
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
            if(_selectStatus == SelectStatus.SingleSelected)
            {
                if (IsPressingCtrl)
                {
                    var p = e.GetPosition(_selectRectOwner.LadderCanvas);
                    var pp = IntPoint.GetIntpointByDouble(p.X, p.Y, WidthUnit, HeightUnit);
                    if ((pp.X == _selectRect.X - 1) && (pp.Y == _selectRect.Y))
                    {
                        SelectRectLeftWithLine();
                    }
                    if ((pp.X == _selectRect.X + 1) && (pp.Y == _selectRect.Y))
                    {
                        SelectRectRightWithLine();
                    }
                    if (pp.X == _selectRect.X && (pp.Y == _selectRect.Y - 1))
                    {
                        SelectRectUpWithLine();
                    }
                    if (pp.X == _selectRect.X && (pp.Y == _selectRect.Y + 1))
                    {
                        SelectRectDownWithLine();
                    }
                }
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if(_selectRectOwner != null)
                    {
                        var p = e.GetPosition(_selectRectOwner.LadderCanvas);
                        var pp = IntPoint.GetIntpointByDouble(p.X, p.Y, WidthUnit, HeightUnit);
                        if((pp.X != _selectRect.X) || (pp.Y != _selectRect.Y))
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
                            _selectStartNetwork.SelectAreaFirstX = _selectStartNetwork.SelectAreaOriginX;
                            _selectStartNetwork.SelectAreaFirstY = _selectStartNetwork.SelectAreaOriginY;
                            _selectStartNetwork.SelectAreaSecondX = pp.X;
                            _selectStartNetwork.SelectAreaSecondY = pp.Y;
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
                LadderNetworkViewModel lvnmodel = invmodel.LadderNetwork;
                if (lvnmodel.GetElements().Contains(viewmodel))
                {
                    if (_selectRect.NetworkParent != null)
                        _selectRect.NetworkParent.ReleaseSelectRect();
                    _selectRect.X = viewmodel.X;
                    _selectRect.Y = viewmodel.Y;
                    lvnmodel.AcquireSelectRect();
                }
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
            IEnumerable<BaseViewModel> oldelements = null;
            IEnumerable<VerticalLineViewModel> oldvlines = null;
            bool containOutput = elements.Any(x => x.Type == LadderInstModel.ElementType.Output);
            var targetNetwork = _selectRectOwner;
            if (_selectRectOwner != null)
            {
                //单选
                if(containOutput)
                {
                    xStart = 10 - width;
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
                oldvlines = _selectRectOwner.GetVerticalLines().Where(ele => ele.X >= xStart && ele.Y >= yStart && ele.X <= xStart + width - 1 && ele.Y <= yStart + height - 1);
            }
            else
            {
                if(_selectStartNetwork != null)
                {
                    //多选
                    targetNetwork = _selectStartNetwork;
                    if (containOutput)
                    {
                        xStart = 10 - width;
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
                    oldvlines = _selectStartNetwork.GetSelectedVerticalLines().Union(_selectStartNetwork.GetVerticalLines().Where(ele => ele.X >= xStart && ele.Y >= yStart && ele.X <= xStart + width - 1 && ele.Y <= yStart + height - 1));           
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


    }
}
