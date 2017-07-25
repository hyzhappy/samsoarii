﻿using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace SamSoarII.Shell.Models
{
    public class LadderMonitorMenu : ContextMenu, IDisposable
    {
        public LadderMonitorMenu()
        {
            miBPAdd = new MenuItem();
            miBPSetting = new MenuItem();
            miBPRemove = new MenuItem();
            miJumpTo = new MenuItem();
            miValues = new MenuItem[] { new MenuItem(), new MenuItem(), new MenuItem(), new MenuItem(), new MenuItem() };
            mdValues = new List<ValueModel>();
            idValues = new int[] { 0, 0, 0, 0, 0 };
            miBPAdd.Header = Properties.Resources.LadderNetwork_AddBreakpoint;
            miBPSetting.Header = Properties.Resources.LadderNetwork_SettingBreakpoint;
            miBPRemove.Header = Properties.Resources.LadderNetwork_RemoveBreakpoint;
            miJumpTo.Header = Properties.Resources.LadderNetwork_JumpToThis;
            miBPAdd.Click += OnMenuItemClick;
            miBPSetting.Click += OnMenuItemClick;
            miBPRemove.Click += OnMenuItemClick;
            miJumpTo.Click += OnMenuItemClick;
            for (int i = 0; i < 5; i++)
            {
                miValues[i].Click += OnMenuItemClick;
                Items.Add(miValues[i]);
            }
            Items.Add(new Separator());
            Items.Add(miBPAdd);
            Items.Add(miBPSetting);
            Items.Add(miBPRemove);
            Items.Add(miJumpTo);
        }
        
        public void Dispose()
        {
            miBPAdd.Click -= OnMenuItemClick;
            miBPSetting.Click -= OnMenuItemClick;
            miBPRemove.Click -= OnMenuItemClick;
            for (int i = 0; i < 5; i++)
                miValues[i].Click -= OnMenuItemClick;
            Items.Clear();
        }

        #region Number
        
        private LadderNetworkViewModel parent;
        public new LadderNetworkViewModel Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (parent != value)
                {
                    LadderNetworkViewModel _parent = parent;
                    this.parent = null;
                    if (_parent != null && _parent.CMMoni != null) _parent.CMMoni = null;
                    this.parent = value;
                    if (parent != null && parent.CMMoni != this) parent.CMMoni = this;
                }
                Core = parent != null ? parent.ViewParent.SelectionRect.Current : null;
            }
        }
        
        private LadderUnitModel core;
        public LadderUnitModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                this.core = value;
                miBPAdd.IsEnabled = false;
                miBPSetting.IsEnabled = false;
                miBPRemove.IsEnabled = false;
                miJumpTo.IsEnabled = false;
                mdValues.Clear();
                for (int i = 0; i < 5; i++)
                    miValues[i].Visibility = Visibility.Collapsed;
                if (core == null) return;
                miBPAdd.Visibility = parent.LadderMode == LadderModes.Simulate ? Visibility.Visible : Visibility.Collapsed;
                miBPSetting.Visibility = parent.LadderMode == LadderModes.Simulate ? Visibility.Visible : Visibility.Collapsed;
                miBPRemove.Visibility = parent.LadderMode == LadderModes.Simulate ? Visibility.Visible : Visibility.Collapsed;
                miJumpTo.Visibility = parent.LadderMode == LadderModes.Simulate ? Visibility.Visible : Visibility.Collapsed;
                miBPAdd.IsEnabled = parent.LadderMode == LadderModes.Simulate && core.Breakpoint != null && !core.Breakpoint.IsEnable;
                miBPSetting.IsEnabled = parent.LadderMode == LadderModes.Simulate && core.Breakpoint != null && core.Breakpoint.IsEnable;
                miBPRemove.IsEnabled = parent.LadderMode == LadderModes.Simulate && core.Breakpoint != null && core.Breakpoint.IsEnable;
                miJumpTo.IsEnabled = parent.LadderMode == LadderModes.Simulate && core.Breakpoint != null;
                for (int i = 0, j; i < core.Children.Count(); i++)
                {
                    for (j = 0; j < i; j++) if (core.Children[j].Text.Equals(core.Children[i].Text)) break;
                    if (core.Children[i].Store?.Parent != null && j >= i)
                    {
                        miValues[i].Visibility = Visibility.Visible;
                        miValues[i].Header = String.Format(App.CultureIsZH_CH() ? "修改{0:s}" : "Modify{0:s}", core.Children[i].Text);
                        idValues[i] = mdValues.Count();
                        mdValues.Add(core.Children[i]);
                        switch (core.LadderMode)
                        {
                            case LadderModes.Monitor: miValues[i].IsEnabled = IFParent.MNGComu.IsAlive; break;
                            case LadderModes.Simulate: miValues[i].IsEnabled = IFParent.MNGSimu.IsAlive; break;
                            default: miValues[i].IsEnabled = false; break;
                        }
                    }
                }
            }
        }
        public InteractionFacade IFParent { get { return core?.Parent?.Parent?.Parent?.Parent; } }

        private MenuItem miBPAdd;
        private MenuItem miBPSetting;
        private MenuItem miBPRemove;
        private MenuItem miJumpTo;
        private MenuItem[] miValues;
        private List<ValueModel> mdValues;
        private int[] idValues;

        #endregion

        #region Event Handler

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
                if (sender == miValues[i])
                {
                    IFParent.ShowValueModifyDialog(mdValues, idValues[i]);
                }
            if (sender == miBPAdd)
            {
                core.Breakpoint.IsEnable = true;
                core.Breakpoint.IsActive = true;
            }
            if (sender == miBPRemove)
            {
                core.Breakpoint.IsActive = false;
                core.Breakpoint.IsEnable = false;
            }
            if (sender == miJumpTo)
                IFParent.MNGSimu.JumpTo(core.Breakpoint.Address);
        }

        #endregion

    }
}
