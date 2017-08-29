using SamSoarII.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

using SamSoarII.Core.Models;

namespace SamSoarII.Shell.Models
{
    public class LadderEditMenu : ContextMenu, IDisposable
    {
        public LadderEditMenu()
        {
            miCut = new MenuItem();
            miCopy = new MenuItem();
            miPaste = new MenuItem();
            miDelete = new MenuItem();
            miRowInsert = new MenuItem();
            miRowIBefore = new MenuItem();
            miRowIAfter = new MenuItem();
            miRowIEnd = new MenuItem();
            miRowDelete = new MenuItem();
            miNetInsert = new MenuItem();
            miNetIBefore = new MenuItem();
            miNetIAfter = new MenuItem();
            miNetIEnd = new MenuItem();
            miNetRemove = new MenuItem();
            miNetCut = new MenuItem();
            miNetCopy = new MenuItem();
            miNetShield = new MenuItem();
            ExpandOrCollapsed = new MenuItem();
            Expand = new MenuItem();
            Collapsed = new MenuItem();
            ExpandAll = new MenuItem();
            CollapsedAll = new MenuItem();
            miZoomIn = new MenuItem();
            miZoomOut = new MenuItem();
            miNetMerge = new MenuItem();
            miNetSplit = new MenuItem();
            miRowInsert.Items.Add(miRowIBefore);
            miRowInsert.Items.Add(miRowIAfter);
            miRowInsert.Items.Add(miRowIEnd);
            miNetInsert.Items.Add(miNetIBefore);
            miNetInsert.Items.Add(miNetIAfter);
            miNetInsert.Items.Add(miNetIEnd);
            ExpandOrCollapsed.Items.Add(Expand);
            ExpandOrCollapsed.Items.Add(Collapsed);
            ExpandOrCollapsed.Items.Add(ExpandAll);
            ExpandOrCollapsed.Items.Add(CollapsedAll);
            ExpandOrCollapsed.Header = Properties.Resources.Expand_Collapsed;
            Expand.Header = Properties.Resources.Expand;
            Collapsed.Header = Properties.Resources.Collapsed;
            ExpandAll.Header = Properties.Resources.Expand_All;
            CollapsedAll.Header = Properties.Resources.Collapsed_All;
            miCut.Header = Properties.Resources.LadderNetwork_Element_Cut;
            miCopy.Header = Properties.Resources.LadderNetwork_Element_Copy;
            miPaste.Header = Properties.Resources.LadderNetwork_Element_Paste;
            miDelete.Header = Properties.Resources.LadderNetwork_Element_Delete;
            miRowInsert.Header = Properties.Resources.MainWindow_Row_Insert;
            miRowIBefore.Header = Properties.Resources.LadderNetwork_Insert_before;
            miRowIAfter.Header = Properties.Resources.LadderNetwork_Insert_After;
            miRowIEnd.Header = Properties.Resources.LadderNetwork_Insert_End;
            miRowDelete.Header = Properties.Resources.MainWindow_Row_Delete;
            miNetInsert.Header = Properties.Resources.LadderNetwork_Network_Insert;
            miNetIBefore.Header = Properties.Resources.LadderNetwork_Insert_before;
            miNetIAfter.Header = Properties.Resources.LadderNetwork_Insert_After;
            miNetIEnd.Header = Properties.Resources.LadderNetwork_Insert_End;
            miNetRemove.Header = Properties.Resources.LadderNetwork_Network_Delete;
            miNetCut.Header = Properties.Resources.LadderNetwork_Network_Cut;
            miNetCopy.Header = Properties.Resources.LadderNetwork_Network_Copy;
            miNetShield.Header = Properties.Resources.LadderNetwork_Shield_Network;
            miZoomIn.Header = Properties.Resources.MainWindow_Zoom_In;
            miZoomOut.Header = Properties.Resources.MainWindow_Zoom_Out;
            miNetMerge.Header = Properties.Resources.LadderNetwork_Network_Merge;
            miNetSplit.Header = Properties.Resources.LadderNetwork_Network_Split;
            miCut.Command = ApplicationCommands.Cut;
            miCopy.Command = ApplicationCommands.Copy;
            miPaste.Command = ApplicationCommands.Paste;
            miDelete.Click += OnMenuItemClicked;
            miRowIBefore.Click += OnMenuItemClicked;
            miRowIAfter.Click += OnMenuItemClicked;
            miRowIEnd.Click += OnMenuItemClicked;
            miRowDelete.Click += OnMenuItemClicked;
            miNetIBefore.Click += OnMenuItemClicked;
            miNetIAfter.Click += OnMenuItemClicked;
            miNetIEnd.Click += OnMenuItemClicked;
            miNetRemove.Click += OnMenuItemClicked;
            miNetCut.Click += OnMenuItemClicked;
            miNetCopy.Click += OnMenuItemClicked;
            miNetShield.Click += OnMenuItemClicked;
            miNetMerge.Click += OnMenuItemClicked;
            miNetSplit.Click += OnMenuItemClicked;
            Expand.Click += OnMenuItemClicked;
            Collapsed.Click += OnMenuItemClicked;
            ExpandAll.Click += OnMenuItemClicked;
            CollapsedAll.Click += OnMenuItemClicked;
            miZoomIn.Command = GlobalCommand.ZoomInCommand;
            miZoomOut.Command = GlobalCommand.ZoomOutCommand;

            Items.Add(miCut);
            Items.Add(miCopy);
            Items.Add(miPaste);
            Items.Add(miDelete);
            Items.Add(miRowInsert);
            Items.Add(miRowDelete);
            Items.Add(new Separator());
            Items.Add(miNetInsert);
            Items.Add(miNetRemove);
            Items.Add(miNetCut);
            Items.Add(miNetCopy);
            Items.Add(miNetMerge);
            Items.Add(miNetSplit);
            Items.Add(new Separator());
            Items.Add(ExpandOrCollapsed);
            Items.Add(miNetShield);
            Items.Add(new Separator());
            Items.Add(miZoomIn);
            Items.Add(miZoomOut);
        }

        public void Dispose()
        {
            Items.Clear();
            miCut.Command = null;
            miCopy.Command = null;
            miPaste.Command = null;
            miDelete.Click -= OnMenuItemClicked;
            miRowIBefore.Click -= OnMenuItemClicked;
            miRowIAfter.Click -= OnMenuItemClicked;
            miRowIEnd.Click -= OnMenuItemClicked;
            miRowDelete.Click -= OnMenuItemClicked;
            miNetIBefore.Click -= OnMenuItemClicked;
            miNetIAfter.Click -= OnMenuItemClicked;
            miNetIEnd.Click -= OnMenuItemClicked;
            miNetCut.Click -= OnMenuItemClicked;
            miNetCopy.Click -= OnMenuItemClicked;
            miNetShield.Click -= OnMenuItemClicked;
            miNetMerge.Click -= OnMenuItemClicked;
            miNetSplit.Click -= OnMenuItemClicked;
            Expand.Click -= OnMenuItemClicked;
            Collapsed.Click -= OnMenuItemClicked;
            ExpandAll.Click -= OnMenuItemClicked;
            CollapsedAll.Click -= OnMenuItemClicked;
            miZoomIn.Command = null;
            miZoomOut.Command = null;
        }

        #region Number
        
        private LadderNetworkModel core;
        public LadderNetworkModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                LadderNetworkModel _core = core;
                this.core = null;
                if (_core != null)
                    _core.PropertyChanged -= OnCorePropertyChanged;
                this.core = value;
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
                    OnCorePropertyChanged(core, new PropertyChangedEventArgs("IsMasked"));
                }
            }
        }
        
        private MenuItem miCut;
        private MenuItem miCopy;
        private MenuItem miPaste;
        private MenuItem miDelete;
        private MenuItem miRowInsert;
        private MenuItem miRowIBefore;
        private MenuItem miRowIAfter;
        private MenuItem miRowIEnd;
        private MenuItem miRowDelete;
        private MenuItem miNetInsert;
        private MenuItem miNetIBefore;
        private MenuItem miNetIAfter;
        private MenuItem miNetIEnd;
        private MenuItem miNetRemove;
        private MenuItem miNetCut;
        private MenuItem miNetCopy;
        private MenuItem miNetShield;
        private MenuItem miZoomIn;
        private MenuItem miZoomOut;
        private MenuItem ExpandOrCollapsed;
        private MenuItem Expand;
        private MenuItem Collapsed;
        private MenuItem ExpandAll;
        private MenuItem CollapsedAll;
        private MenuItem miNetMerge;
        private MenuItem miNetSplit;

        #endregion

        #region Event Handler

        public event LadderEditEventHandler Post = delegate { };
         
        private void OnMenuItemClicked(object sender, RoutedEventArgs e)
        {
            if (sender == miDelete) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.Delete));
            if (sender == miRowIBefore) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.RowInsertBefore));
            if (sender == miRowIAfter) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.RowInsertAfter));
            if (sender == miRowIEnd) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.RowInsertEnd));
            if (sender == miRowDelete) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.RowDelete));
            if (sender == miNetIBefore) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.NetInsertBefore));
            if (sender == miNetIAfter) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.NetInsertAfter));
            if (sender == miNetIEnd) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.NetInsertEnd));
            if (sender == miNetRemove) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.NetDelete));
            if (sender == miNetCut) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.NetCut));
            if (sender == miNetCopy) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.NetCopy));
            if (sender == miNetShield) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.NetShield));
            if (sender == miNetMerge) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.NetMerge));
            if (sender == miNetSplit) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.NetSplit));
            if (sender == Expand) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.NetExpand));
            if (sender == ExpandAll) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.NetExpandAll));
            if (sender == Collapsed) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.NetCollapsed));
            if (sender == CollapsedAll) Post(this, new LadderEditEventArgs(LadderEditEventArgs.Types.NetCollapsedAll));
        }
        
        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsMasked":
                    miNetShield.IsChecked = core.IsMasked;
                    miCut.IsEnabled = !core.IsMasked;
                    miCopy.IsEnabled = !core.IsMasked;
                    miPaste.IsEnabled = !core.IsMasked;
                    miDelete.IsEnabled = !core.IsMasked;
                    miRowInsert.IsEnabled = !core.IsMasked;
                    miRowIBefore.IsEnabled = !core.IsMasked;
                    miRowIAfter.IsEnabled = !core.IsMasked;
                    miRowIEnd.IsEnabled = !core.IsMasked;
                    miRowDelete.IsEnabled = !core.IsMasked;
                    miNetRemove.IsEnabled = !core.IsMasked;
                    miNetCut.IsEnabled = !core.IsMasked;
                    miNetCopy.IsEnabled = !core.IsMasked;
                    miNetMerge.IsEnabled = !core.IsMasked;
                    miNetSplit.IsEnabled = !core.IsMasked;
                    break;
            }
        }


        #endregion
    }

    public class LadderEditEventArgs
    {
        public enum Types { Delete,
            RowInsertBefore, RowInsertAfter, RowInsertEnd, RowDelete,
            NetInsertBefore, NetInsertAfter, NetInsertEnd, NetDelete, NetCut, NetCopy, NetShield,
            NetExpand, NetExpandAll, NetCollapsed, NetCollapsedAll,
            NetMerge, NetSplit}
        private Types type;
        public Types Type { get { return this.type; } }

        public LadderEditEventArgs(Types _type)
        {
            type = _type;
        }
    }

    public delegate void LadderEditEventHandler(object sender, LadderEditEventArgs e);
}
