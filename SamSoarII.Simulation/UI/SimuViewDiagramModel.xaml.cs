using SamSoarII.Extend.LadderChartModel;
using System;
using System.Collections.Generic;
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

using SamSoarII.Simulation.Core.Global;

namespace SamSoarII.Simulation.UI
{
    /// <summary>
    /// SimuViewDiagramModel.xaml 的交互逻辑
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
    
    public partial class SimuViewDiagramModel : UserControl
    {
        public string LadderName { get; set; }
        public bool IsMainLadder { get; set; }

        private SimulateModel _root;
        public SimulateModel Root
        {
            get { return this._root; }
        }
        private LinkedList<SimuViewNetworkModel> _networks;

        private SelectRect _selectRect;

        private SimuViewNetworkModel _selectStartNetwork;
        
        private SimuViewNetworkModel _selectRectOwner;

        private SortedSet<SimuViewNetworkModel> _selectAllNetworks = new SortedSet<SimuViewNetworkModel>();

        private SortedSet<SimuViewNetworkModel> _selectAllNetworkCache = new SortedSet<SimuViewNetworkModel>();

        public int NetworkCount
        {
            get
            {
                return _networks.Count();
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
                switch (_selectStatus)
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

        public SimuViewDiagramModel(SimulateModel root, string name)
        {
            InitializeComponent();
            this._root = root;
            LadderName = name;
            this.Loaded += (sender, e) =>
            {
                Focus();
                Keyboard.Focus(this);
            };
            InitAll();
            ClearAll();
        }

        private void InitAll()
        {
            _networks = new LinkedList<SimuViewNetworkModel>();
            _selectRect = new SelectRect();
            _selectStartNetwork = null;
            _selectRectOwner = null;
            _selectAllNetworks = new SortedSet<SimuViewNetworkModel>();
            _selectAllNetworkCache = new SortedSet<SimuViewNetworkModel>();
        }

        public void ClearAll()
        {
            if (_selectRectOwner != null)
                _selectRectOwner.ReleaseSelectRect();
            _networks.Clear();
            _selectStartNetwork = null;
            _selectRectOwner = null;
            _selectAllNetworks.Clear();
            _selectAllNetworkCache.Clear();
            _selectRect.X = 0;
            _selectRect.Y = 0;
        }

        public void Setup(LadderChart[] lcharts)
        {
            ClearAll();
            foreach (LadderChart lchart in lcharts)
            {
                SimuViewNetworkModel svnmodel = new SimuViewNetworkModel(this, _networks.Count() + 1);
                svnmodel.Setup(lchart);
                if (_selectRectOwner == null)
                {
                    _selectRectOwner = svnmodel;
                    svnmodel.AcquireSelectRect();
                }
                _networks.AddLast(svnmodel);
            }
            ReloadNetworksToStackPanel();
        }

        public void Update()
        {
            foreach (SimuViewNetworkModel svnmodel in _networks)
            {
                svnmodel.Update();
            }
        }

        #region Network manipulation
        public SimuViewNetworkModel GetNetworkByNumber(int number)
        {
            return _networks.ElementAt(number);
        }
       
        public void AcquireSelectRect(SimuViewNetworkModel network)
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

        public IEnumerable<SimuViewNetworkModel> GetNetworks()
        {
            return _networks;
        }

        private void ReloadNetworksToStackPanel()
        {
            LadderNetworkStackPanel.Children.Clear();
            foreach (var net in _networks)
            {
                LadderNetworkStackPanel.Children.Add(net);
            }
        }
        /// <summary>
        /// Called when initialze the ladder diagram, do not call by user, for it can not undo 
        /// </summary>
        /// <param name="network"></param>
        public void AppendNetwork(SimuViewNetworkModel network)
        {
            network.NetworkNumber = _networks.Count;
            _networks.AddLast(network);
            LadderNetworkStackPanel.Children.Add(network);
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
                        _selectRectOwner = _networks.ElementAt(_selectRectOwner.NetworkNumber - 1);
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
                        _selectRectOwner = _networks.ElementAt(_selectRectOwner.NetworkNumber + 1);
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
                if (_selectRect.X < 9)
                {
                    _selectRect.X++;
                }
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
            if (SelectionStatus == SelectStatus.MultiSelecting)
            {
                var p = Mouse.GetPosition(_selectStartNetwork.LadderCanvas);
                if (p.Y < 0 || p.X < 0 || p.X > _selectStartNetwork.LadderCanvas.ActualWidth)
                {
                    return CrossNetworkState.CrossUp;
                }
                else
                {
                    if (p.Y > _selectStartNetwork.LadderCanvas.ActualHeight)
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

            var node = _networks.Find(_selectStartNetwork);
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
            var node = _networks.Find(_selectStartNetwork);
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

        #region Event handler
        private void OnLadderDiagramKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                SelectRectLeft();
                e.Handled = true;
            }
            if (e.Key == Key.Right)
            {
                SelectRectRight();
                e.Handled = true;
            }
            if (e.Key == Key.Down)
            {
                SelectRectDown();
                e.Handled = true;
            }

            if (e.Key == Key.Up)
            {
                SelectRectUp();
                e.Handled = true;
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
            //EditCommentReact();
        }

        private void OnEditComment(object sender, RoutedEventArgs e)
        {
            //EditCommentReact();
        }

        private void OnLadderDiagramMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectionStatus = SelectStatus.Idle;
            }
        }

        private void OnLadderDiagramMouseMove(object sender, MouseEventArgs e)
        {
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

        #endregion
    }
}
