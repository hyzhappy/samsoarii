using SamSoarII.InstructionViewModel;
using SamSoarII.UserInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SamSoarII.AppMain.Project
{
    /// <summary>
    /// LadderDiagramViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class LadderDiagramViewModel : UserControl
    {
        public string LadderName { get; set; }
        public bool IsMainLadder { get; set; }

        public int NetworkCount
        {
            get
            {
                return _ladderNetworks.Count;
            }
        }

        private LinkedList<LadderNetworkViewModel> _ladderNetworks = new LinkedList<LadderNetworkViewModel>();

        private SelectRect _selectRect = new SelectRect();
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

        public LadderDiagramViewModel(string name)
        {
            InitializeComponent();
            LadderName = name;
            LadderCommentTextBlock.DataContext = this;
            this.Loaded += (sender, e) =>
            {
                Focus();
                Keyboard.Focus(this);
            };
            AppendNewNetwork();
        }

        public LadderNetworkViewModel GetNetworkByNumber(int number)
        {
            return _ladderNetworks.ElementAt(number);
        }

        public void AddNewNetworkBefore(LadderNetworkViewModel network)
        {
            var node = _ladderNetworks.Find(network);
            if(node != null)
            {             
                var newnetwork = new LadderNetworkViewModel(this, network.NetworkNumber);
                _ladderNetworks.AddBefore(node, newnetwork);
                while(node != null)
                {
                    node.Value.NetworkNumber++;
                    node = node.Next;                   
                }
                ReloadNetworksToStackPanel();         
            }
        }

        public void AddNewNetworkAfter(LadderNetworkViewModel network)
        {
            var node = _ladderNetworks.Find(network);
            if (node != null)
            {
                var newnetwork = new LadderNetworkViewModel(this, network.NetworkNumber);
                _ladderNetworks.AddAfter(node, newnetwork);
                node = node.Next;
                while (node != null)
                {
                    node.Value.NetworkNumber++;
                    node = node.Next;
                }
                ReloadNetworksToStackPanel();
            }
        }

        public void DeleteNetwork(LadderNetworkViewModel network)
        {
            if(_ladderNetworks.Count > 1)
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

        private void ReloadNetworksToStackPanel()
        {
            LadderNetworkStackPanel.Children.Clear();
            foreach(var net in _ladderNetworks)
            {
                LadderNetworkStackPanel.Children.Add(net);
            }
        }

        public void AppendNewNetwork()
        {
            var network = new LadderNetworkViewModel(this, _ladderNetworks.Count);
            LadderNetworkStackPanel.Children.Add(network);
            _ladderNetworks.AddLast(network);
        }

        public void AppendNetwork(LadderNetworkViewModel network)
        {
            network.NetworkNumber = _ladderNetworks.Count;
            _ladderNetworks.AddLast(network);
            LadderNetworkStackPanel.Children.Add(network);
        }

        public void RemoveAllNetworks()
        {
            LadderNetworkStackPanel.Children.Clear();
            _ladderNetworks.Clear();
        }




        public void AcquireSelectRect(LadderNetworkViewModel network)
        {
            if(_selectRectOwner == null)
            {
                _selectRectOwner = network;
            }
            else
            {
                if(_selectRectOwner != network)
                {
                    _selectRectOwner.ReleaseSelectRect();
                    _selectRectOwner = network;
                }
            }
        }

        public void SetTransForm(Transform transform)
        {
            LayoutTransform = transform;
        }

        public void ReplaceElement(int catalogId)
        {
            if(_selectRectOwner != null)
            {
                var viewmodel = InstructionViewModelPrototype.Clone(catalogId);
                viewmodel.X = _selectRect.X;
                viewmodel.Y = _selectRect.Y;
                _selectRectOwner.ReplaceElement(viewmodel);
            }
        }

        public IEnumerable<LadderNetworkViewModel> GetNetworks()
        {
            return _ladderNetworks;
        }

      

        private void EditCommentReact()
        {
            LadderDiagramCommentEditDialog dialog = new LadderDiagramCommentEditDialog();
            dialog.LadderComment = this.LadderComment;
            dialog.LadderName = this.LadderName;
            dialog.EnsureButtonClick += (sender, e) =>
            {
                this.LadderComment = dialog.LadderComment;
                dialog.Close();
            };
            dialog.ShowDialog();
        }


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
                if (_selectRect.X < 9)
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
                    if (model.Type == InstructionModel.ElementType.HLine)
                    {
                        _selectRectOwner.RemoveElement(model.X, model.Y);
                    }
                }
                else
                {
                    _selectRectOwner.ReplaceElement(new HorizontalLineViewModel() { X = _selectRect.X, Y = _selectRect.Y });
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
                    if (model.Type == InstructionModel.ElementType.HLine)
                    {
                        _selectRectOwner.RemoveElement(x, y);
                    }
                }
                else
                {
                    _selectRectOwner.ReplaceElement(new HorizontalLineViewModel() { X = x, Y = y });
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
                            _selectRectOwner.RemoveVerticalLine(x, y);
                        }
                        else
                        {
                            _selectRectOwner.AddVerticalLine(new VerticalLineViewModel() { X = x, Y = y });
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
                    _selectRectOwner.AppendNewRow();
                }
                if (x >= 0)
                {
                    var vline = _selectRectOwner.SearchVerticalLine(x, y);
                    if (vline != null)
                    {
                        _selectRectOwner.RemoveVerticalLine(x, y);
                    }
                    else
                    {
                        _selectRectOwner.AddVerticalLine(new VerticalLineViewModel() { X = x, Y = y });
                    }
                }
                SelectRectDown();
            }
        }

        #endregion

        #region Instruction relative
        private void ShowInstructionInputDialog(string initialString)
        {
            InstructionInputDialog dialog = new InstructionInputDialog(initialString);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dialog.EnsureButtonClick += (sender, e) =>
            {
                BaseViewModel viewmodel;
                try
                {
                    viewmodel = InstructionViewModelPrototype.Clone(dialog.InstructionInput.ToUpper());
                    if(viewmodel.Type == InstructionModel.ElementType.Output)
                    {
                        int x = _selectRect.X;
                        int y = _selectRect.Y;
                        for(int i = x; i < 9; i++)
                        {
                            _selectRectOwner.ReplaceElement(new HorizontalLineViewModel() { X = i, Y = y });
                        }
                        viewmodel.X = 9;
                        viewmodel.Y = _selectRect.Y;
                        _selectRectOwner.ReplaceElement(viewmodel);
                        _selectRect.X = viewmodel.X;
                    }
                    else
                    {
                        viewmodel.X = _selectRect.X;
                        viewmodel.Y = _selectRect.Y;
                        _selectRectOwner.ReplaceElement(viewmodel);
                        if(_selectRect.X < 9)
                        {
                            _selectRect.X++;
                        }
                    }
                }
                catch(Exception exception)
                {
                    
                }
                
                dialog.Close();
            };
            dialog.ShowDialog();
        }



        #endregion

        #region Event handler
        private void OnLadderDiagramKeyDown(object sender, KeyEventArgs e)
        {
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
                    if(viewmodel != null && viewmodel.Type != InstructionModel.ElementType.HLine)
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
                if(_selectRectOwner != null)
                {
                    _selectRectOwner.RemoveElement(_selectRect.X, _selectRect.Y);
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
        }
        #endregion


    }
}
