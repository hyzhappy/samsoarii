using SamSoarII.InstructionViewModel;
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

namespace SamSoarII.Project
{
    /// <summary>
    /// LadderDiagramViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class LadderDiagramViewModel : UserControl
    {
        public string LadderName { get; set; }
        public bool IsMainLadder { get; set; }

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

        private LadderNetworkViewModel _selectRectOwner;

        public LadderDiagramViewModel(string name)
        {
            InitializeComponent();
            LadderName = name;
            AppendNewNetwork();
        }
        
        public void AddNewNetworkBefore(LadderNetworkViewModel network)
        {
            var node = _ladderNetworks.Find(network);
            if(node != null)
            {             
                var newnetwork = new LadderNetworkViewModel(this, network.NetworkNumber);
                //LadderNetworkStackPanel.Children.Insert(network.NetworkNumber, newnetwork);
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
        #region Event handler
        //public void OnLadderDiagramMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    if(Keyboard.Modifiers == ModifierKeys.Control)
        //    { 
        //    }
        //}
        #endregion
    }
}
