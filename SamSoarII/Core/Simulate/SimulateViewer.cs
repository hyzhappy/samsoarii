using SamSoarII.Core.Models;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Threads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Windows;
using System.ComponentModel;
using SamSoarII.Utility;

namespace SamSoarII.Core.Simulate
{
    public class SimulateViewer : TimerThreadManager
    {
        public SimulateViewer(SimulateManager _parent) : base(false, true)
        {
            parent = _parent;
            DllModel.SimulateException += OnSimulateException;
            parent.Aborted += OnSimulateAborted;
            TimeSpan = 100;
            cursor = new BreakpointCursor(this);
        }
        
        #region Number

        private SimulateManager parent;
        public SimulateManager Parent { get { return this.parent; } }
        public ValueManager ValueManager { get { return parent.IFParent.MNGValue; } }
        public SimulateDllModel DllModel { get { return parent.DllModel; } }
        
        private bool isenable;
        public bool IsEnable
        {
            get
            {
                return this.isenable;
            }
            set
            {
                if (isenable == value) return;
                this.isenable = value;
                if (isenable)
                    ValueManager.PostValueStoreEvent += OnReceiveValueStoreEvent;
                else
                    ValueManager.PostValueStoreEvent -= OnReceiveValueStoreEvent;
            }
        }

        #endregion

        #region Thread
        
        protected override void Handle()
        {
            foreach (ValueInfo vinfo in ValueManager)
                foreach (ValueStore vstore in vinfo.Stores)
                {
                    if (vstore.Parent == null || vstore.VisualRefNum == 0) continue;
                    if (vstore.IsWordBit)
                    {
                        int value = DllModel.GetValue_Word(vstore.Parent.Name);
                        vstore.Value = ((value >> vstore.Flag) & 1);
                    }
                    else if (vstore.IsBitWord || vstore.IsBitDoubleWord)
                    {
                        int[] values = DllModel.GetValue_Bit(vstore.Parent.Name, vstore.Flag);
                        int value = 0;
                        for (int i = vstore.Flag - 1; i >= 0; i--)
                            value = (value << 1) + (values[i] & 1);
                        vstore.Value = vstore.IsBitWord ? (short)value : (int)value;
                    }
                    else
                    {
                        switch (vstore.Type)
                        {
                            case ValueModel.Types.BOOL: vstore.Value = DllModel.GetValue_Bit(vstore.Name); break;
                            case ValueModel.Types.WORD: vstore.Value = (short)DllModel.GetValue_Word(vstore.Name); break;
                            case ValueModel.Types.UWORD:
                            case ValueModel.Types.BCD:
                            case ValueModel.Types.HEX: vstore.Value = (short)DllModel.GetValue_Word(vstore.Name); break;
                            case ValueModel.Types.DWORD:
                            case ValueModel.Types.UDWORD:
                            case ValueModel.Types.DHEX: vstore.Value = (int)DllModel.GetValue_DWord(vstore.Name); break;
                            case ValueModel.Types.FLOAT: vstore.Value = (float)DllModel.GetValue_Float(vstore.Name); break;
                        }
                    }
                }
            // 若检查到暂停状态的改变则进行处理
            if (SimulateDllModel.GetBPPause() > 0 && cursor.Address < 0)
            {
                cursor.Address = SimulateDllModel.GetBPAddr();
                if (cursor.Current != null)
                    BreakpointPaused(this, new BreakpointPauseEventArgs(cursor));
                else if (SimulateDllModel.GetCallCount() <= 256)
                    SimulateDllModel.SetBPPause(0);
                if (SimulateDllModel.GetCallCount() > 256)
                    OnSimulateException(new StackOverflowException("子程序 & 用户函数嵌套调用超过了上限（256）。"), new RoutedEventArgs());
            }
        }

        #endregion

        #region Breakpoint Control

        private BreakpointCursor cursor;
        public BreakpointCursor Cursor { get { return this.cursor; } }
        public bool IsBPPause { get { return cursor.Address >= 0; } }
        
        public void Resume()
        {
            BreakpointResumed(this, new BreakpointPauseEventArgs(cursor));
            cursor.Address = -1;
            SimulateDllModel.SetBPPause(0);
        }
        
        public void MoveStep()
        {
            BreakpointResumed(this, new BreakpointPauseEventArgs(cursor));
            cursor.Address = -1;
            SimulateDllModel.MoveStep();
        }

        public void CallStep()
        {
            BreakpointResumed(this, new BreakpointPauseEventArgs(cursor));
            cursor.Address = -1;
            SimulateDllModel.CallStep();
        }

        public void JumpTo(int bpaddr)
        {
            BreakpointResumed(this, new BreakpointPauseEventArgs(cursor));
            cursor.Address = -1;
            SimulateDllModel.JumpTo(bpaddr);
        }

        public void JumpOut()
        {
            BreakpointResumed(this, new BreakpointPauseEventArgs(cursor));
            cursor.Address = -1;
            SimulateDllModel.JumpOut();
        }

        #endregion

        #region Event Handler
        
        private void OnReceiveValueStoreEvent(object sender, ValueStoreWriteEventArgs e)
        {
            ValueStore store = (ValueStore)sender;
            if (e.ToLock)
            {   
                DllModel.Lock(store.Parent.Name, store.IsBitWord || store.IsBitDoubleWord ? store.Flag : 1);
                store.IsLocked = true;
            }
            if (e.Unlock)
            {
                DllModel.Unlock(store.Parent.Name, store.IsBitWord || store.IsBitDoubleWord ? store.Flag : 1);
                store.IsLocked = false;
            }
            if (e.UnlockAll)
            {
                foreach (ValueInfo vinfo in ValueManager)
                    foreach (ValueStore vstore in vinfo.Stores)
                        if (vstore.IsLocked) vstore.Unlock();
            }
            if (e.IsWrite)
            {
                if (store.IsWordBit)
                {
                    int value = DllModel.GetValue_Word(store.Parent.Name);
                    if (e.ToValue.ToString().Equals("ON"))
                        value |=  (1 << store.Flag);
                    else
                        value &= ~(1 << store.Flag);
                    DllModel.SetValue_Word(store.Parent.Name, value);
                }
                else if (store.IsBitWord || store.IsBitDoubleWord)
                {
                    int[] bits = DllModel.GetValue_Bit(store.Parent.Name, store.Flag);
                    int value = int.Parse(e.ToValue.ToString());
                    for (int i = 0; i < bits.Length; i++)
                    {
                        bits[i] = value & 1;
                        value >>= 1;
                    }
                    DllModel.SetValue_Bit(store.Parent.Name, store.Flag, bits);
                }
                else
                {
                    switch (store.Type)
                    {
                        case ValueModel.Types.BOOL:
                            DllModel.SetValue_Bit(store.Name, e.ToValue.ToString().Equals("ON") ? 1 : 0);
                            break;
                        case ValueModel.Types.WORD:
                        case ValueModel.Types.UWORD:
                            DllModel.SetValue_Word(store.Name, Int32.Parse(e.ToValue.ToString())); break;
                        case ValueModel.Types.BCD:
                            DllModel.SetValue_Word(store.Name, (Int32)(ValueConverter.ToBCD(UInt16.Parse(e.ToValue.ToString())))); break;
                        case ValueModel.Types.DWORD:
                        case ValueModel.Types.UDWORD:
                            DllModel.SetValue_DWord(store.Name, Int64.Parse(e.ToValue.ToString())); break;
                        case ValueModel.Types.FLOAT:
                            DllModel.SetValue_Float(store.Name, double.Parse(e.ToValue.ToString())); break;
                    }
                }
            }
        }

        private void OnStorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ValueStore vstore = (ValueStore)sender;
            switch (e.PropertyName)
            {
                case "IsLocked":
                    if (vstore.IsLocked) DllModel.Lock(vstore.Name);
                    if (!vstore.IsLocked) DllModel.Unlock(vstore.Name);
                    break;
            }
        }

        public event BreakpointPauseEventHandler BreakpointPaused = delegate { };
        public event BreakpointPauseEventHandler BreakpointResumed = delegate { };

        private void OnSimulateAborted(object sender, RoutedEventArgs e)
        {
            Cursor.Address = -1;
        }

        private void OnSimulateException(object sender, RoutedEventArgs e)
        {
            if (!IsEnable || !DllModel.IsAlive)
            {
                DllModel.Abort();
                return;
            }
            System.Windows.Application.Current.Dispatcher.Invoke(new Utility.Delegates.Execute(() =>
            {
                Exception exc = (Exception)sender;
                SimulateExceptionDialog dialog = new SimulateExceptionDialog();
                dialog.TB_Message.Text = exc.Message;
                //bool iscritical = (sender is StackOverflowException || sender is AccessViolationException);
                bool iscritical = true;
                dialog.B_Continue.IsEnabled = !iscritical;
                dialog.B_Pause.IsEnabled = !iscritical;
                dialog.B_Continue.Click += (_sender, _e) =>
                {
                    DllModel.Start();
                    dialog.Close();
                };
                dialog.B_Pause.Click += (_sender, _e) =>
                {
                    DllModel.Pause();
                    dialog.Close();
                };
                dialog.B_Abort.Click += (_sender, _e) =>
                {
                    DllModel.Abort();
                    dialog.Close();
                };
                dialog.ShowDialog();
            }));
        }

        #endregion

    }
}
