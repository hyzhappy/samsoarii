using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

using SamSoarII.Core.Models;
using SamSoarII.Shell.Windows;
using SamSoarII.Global;

namespace SamSoarII.Shell.Models
{
    public class SelectAreaCore : IModel
    {
        public SelectAreaCore(LadderDiagramModel _parent)
        {
            parent = _parent;
        }

        public void Dispose()
        {
            parent = null;
        }
        
        public void Save(XElement xele)
        {
            throw new NotImplementedException();
        }

        public void Load(XElement xele)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number
        
        private LadderDiagramModel parent;
        public LadderDiagramModel Parent { get { return this.parent; } }
        IModel IModel.Parent { get { return Parent; } }

        public enum Status { NotSelected, SelectRange, SelectCross }
        private Status state;
        public Status State { get { return this.state; } }
        private Direction movedir;

        private int netorigin;
        public int NetOrigin { get { return this.netorigin; } }
        private int netstart;
        public int NetStart { get { return this.netstart; } }
        private int netend;
        public int NetEnd { get { return this.netend; } }
        
        private int xorigin;
        public int XOrigin { get { return this.xorigin; } }
        private int yorigin;
        public int YOrigin { get { return this.yorigin; } }
        private int xstart;
        public int XStart { get { return this.xstart; } } 
        private int xend;
        public int XEnd { get { return this.xend; } }
        private int ystart;
        public int YStart { get { return this.ystart; } }
        private int yend;
        public int YEnd { get { return this.yend; } }
        
        public IEnumerable<LadderNetworkModel> SelectNetworks
        {
            get
            {
                for (int i = netstart; i <= netend; i++)
                {
                    LadderNetworkModel net = parent.Children[i];
                    if (!net.IsMasked) yield return net;
                }
            }
        }

        public IEnumerable<LadderUnitModel> SelectUnits
        {
            get
            {
                for (int i = netstart; i <= netend; i++)
                {
                    LadderNetworkModel net = parent.Children[i];
                    if (net.IsMasked) continue;
                    if (state == Status.SelectCross)
                    {
                        foreach (LadderUnitModel unit in net.Children)
                            yield return unit;
                        foreach (LadderUnitModel vline in net.VLines)
                            yield return vline;
                    }
                    else
                    {
                        foreach (LadderUnitModel unit in net.Children.SelectRange(xstart, xend, ystart, yend))
                            yield return unit;
                        foreach (LadderUnitModel vline in net.VLines.SelectRange(xstart, xend, ystart, yend))
                            yield return vline;

                    }
                }
            }
        }

        #endregion

        #region View

        private SelectArea view;
        public SelectArea View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                SelectArea _view = view;
                this.view = null;
                if (_view != null && _view.Core != null) _view.Core = null;
                this.view = value;
                if (view != null && view.Core != this) view.Core = this;
            }
        }
        IViewModel IModel.View
        {
            get { return View; }
            set { View = (SelectArea)value; }
        }

        public ProjectTreeViewItem PTVItem
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion

        #region Select Changed

        #region Base

        internal void Select(int _netstart, int _netend, Direction _movedir = Direction.Down)
        {
            state = Status.SelectCross;
            netorigin = _netstart;
            netstart = Math.Min(_netstart, _netend);
            netend = Math.Max(_netstart, _netend);
            movedir = netstart < netend ? (netorigin == netstart ? Direction.Down : Direction.Up) : _movedir;
            view?.Update();
        }

        internal void Select(int _netid, int _xstart, int _ystart, int _xend, int _yend)
        {
            state = Status.SelectRange;
            netorigin = netstart = netend = _netid;
            xorigin = _xstart;
            yorigin = _ystart;
            xstart = Math.Min(_xstart, _xend);
            xend = Math.Max(_xstart, _xend);
            ystart = Math.Min(_ystart, _yend);
            yend = Math.Max(_ystart, _yend);
            view?.Update();
        }

        internal void Release()
        {
            if (state == Status.NotSelected) return;
            state = Status.NotSelected;
            view?.Update();
        }

        #endregion

        #region Move to ...
        
        public void Move(LadderUnitModel unit)
        {
            if (unit.Parent.ID == netorigin)
                Select(netorigin, xorigin, yorigin, unit.X, unit.Y);
            else
                Select(netorigin, unit.Parent.ID);
        }

        public void Move(LadderNetworkPositionModel pos)
        {
            if (pos.Network.ID == netorigin)
                Select(netorigin, xorigin, yorigin, pos.X, pos.Y);
            else
                Select(netorigin, pos.Network.ID);
        }

        public void Move(LadderNetworkModel net)
        {
            Select(netorigin, net.ID);
        }

        #endregion

        #region Direction Move

        public bool CanMoveLeft()
        {
            switch (state)
            {
                case Status.SelectRange:
                    if (xorigin == xstart)
                        return xorigin > 0 || xend > xstart;
                    else
                        return xstart > 0;
                default:
                    return false;
            }
        }

        public bool CanMoveRight()
        {
            switch (state)
            {
                case Status.SelectRange:
                    if (xorigin == xend)
                        return xorigin < GlobalSetting.LadderXCapacity - 1 || xstart < xend;
                    else
                        return xend < GlobalSetting.LadderXCapacity - 1;
                default:
                    return false;
            }
        }

        public bool CanMoveUp()
        {
            switch (state)
            {
                case Status.SelectRange:
                    return true;
                case Status.SelectCross:
                    if (netorigin == netstart)
                        return true;
                    else
                        return netstart > 0;
                default:
                    return false;
            }
        }

        public bool CanMoveDown()
        {
            switch (state)
            {
                case Status.SelectRange:
                    return true;
                case Status.SelectCross:
                    if (netorigin == netend)
                        return true;
                    else
                        return netend < parent.NetworkCount - 1;
                default:
                    return false;
            }
        }

        public void MoveLeft()
        {
            switch (state)
            {
                case Status.SelectRange:
                    if (xorigin == xstart)
                    {
                        if (--xend < xstart)
                        {
                            xstart--; xend++;
                        }
                    }
                    else
                    {
                        xstart--;
                    }
                    View?.Update();
                    break;
            }
        }

        public void MoveRight()
        {
            switch (state)
            {
                case Status.SelectRange:
                    if (xorigin == xend)
                    {
                        if (++xstart > xend)
                        {
                            xstart--; xend++;
                        }
                    }
                    else
                    {
                        xend++;
                    }
                    View?.Update();
                    break;
            }
        }

        public void MoveUp()
        {
            switch (state)
            {
                case Status.SelectRange:
                    if (yorigin == ystart)
                    {
                        if (--yend < ystart)
                        {
                            ystart--; yend++;
                        }
                    }
                    else
                    {
                        ystart--;
                    }
                    if (ystart < 0)
                    {
                        Select(netorigin, netorigin, Direction.Up);
                    }
                    View?.Update();
                    break;
                case Status.SelectCross:
                    if (netorigin == netstart && movedir != Direction.Up) netend--; else netstart--;
                    if (netend < netstart) Select(netorigin, xorigin, yorigin,
                        xorigin == xstart ? xend : xstart, parent.Children[netorigin].RowCount - 1);
                    View?.Update();
                    break;
            }
        }

        public void MoveDown()
        {
            switch (state)
            {
                case Status.SelectRange:
                    if (yorigin == yend)
                    {
                        if (++ystart > yend)
                        {
                            ystart--; yend++;
                        }
                    }
                    else
                    {
                        yend++;
                    }
                    if (yend >= parent.Children[netorigin].RowCount)
                    {
                        Select(netorigin, netorigin, Direction.Down);
                    }
                    View?.Update();
                    break;
                case Status.SelectCross:
                    if (netorigin == netend && movedir != Direction.Down) netstart++; else netend++;
                    if (netend < netstart) Select(netorigin, xorigin, yorigin,
                         xorigin == xstart ? xend : xstart, 0);
                    View?.Update();
                    break;
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// SelectArea.xaml 的交互逻辑
    /// </summary>
    public partial class SelectArea : UserControl, IViewModel
    {
        public SelectArea(LadderDiagramModel _parent)
        {
            InitializeComponent();
            Core = new SelectAreaCore(_parent);
            Canvas.SetZIndex(this, -1);
        }

        public void Dispose()
        {
            Core = null;
        }

        #region Core

        private SelectAreaCore core;
        public SelectAreaCore Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                SelectAreaCore _core = core;
                this.core = null;
                if (_core != null && _core.View != null) _core.View = null;
                this.core = value;
                if (core != null && core.View != this) core.View = this;
            }
        }
        IModel IViewModel.Core
        {
            get { return Core; }
            set { Core = (SelectAreaCore)value; }
        }

        public LadderDiagramViewModel ViewParent { get { return core?.Parent?.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        public LadderDiagramModel CoreParent { get { return core?.Parent; } }
        public LadderNetworkModel NetOrigin { get { return CoreParent.Children[core.NetOrigin]; } }
        public LadderNetworkModel NetStart { get { return CoreParent.Children[core.NetStart]; } }
        public LadderNetworkModel NetEnd { get { return CoreParent.Children[core.NetEnd]; } }
        
        #endregion

        #region Shell
        
        private bool iscommentmode;
        public bool IsCommentMode
        {
            get
            {
                return this.iscommentmode;
            }
            set
            {
                this.iscommentmode = value;
                Update();
            }
        }

        public void Update()
        {
            int unitwidth = GlobalSetting.LadderWidthUnit;
            int unitheight = IsCommentMode ? GlobalSetting.LadderCommentModeHeightUnit : GlobalSetting.LadderHeightUnit;
            switch (core.State)
            {
                case SelectAreaCore.Status.NotSelected:
                    Visibility = Visibility.Hidden;
                    break;
                case SelectAreaCore.Status.SelectRange:
                    Visibility = Visibility.Visible;
                    Canvas.SetLeft(this, core.XStart * unitwidth);
                    Canvas.SetTop(this, NetStart.UnitBaseTop + core.YStart * unitheight);
                    Width = (core.XEnd - core.XStart + 1) * unitwidth;
                    Height = (core.YEnd - core.YStart + 1) * unitheight;
                    break;
                case SelectAreaCore.Status.SelectCross:
                    Visibility = Visibility.Visible;
                    Canvas.SetLeft(this, 0);
                    Canvas.SetTop(this, NetStart.CanvasTop);
                    Width = unitwidth * GlobalSetting.LadderXCapacity;
                    Height = NetEnd.CanvasTop + NetEnd.ViewHeight - NetStart.CanvasTop;
                    break;
            }
        }

        #endregion
    }
}
