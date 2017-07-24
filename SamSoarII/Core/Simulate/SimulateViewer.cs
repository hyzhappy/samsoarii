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
        public SimulateViewer(SimulateManager _parent) : base(false)
        {
            parent = _parent;
            TimeSpan = 100;
            stores = new ObservableCollection<ValueStore>();
            stores.CollectionChanged += OnStoresChanged;
            cursor = new BreakpointCursor(this);
        }
        
        #region Number

        private SimulateManager parent;
        public SimulateManager Parent { get { return this.parent; } }
        public ValueManager ValueManager { get { return parent.IFParent.MNGValue; } }
        public SimulateDllModel DllModel { get { return parent.DllModel; } }

        private ObservableCollection<ValueStore> stores;
        public IList<ValueStore> Stores { get { return this.stores; } }
        private void OnStoresChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (ValueStore vstore in e.NewItems)
                {
                    if (vstore.IsLocked) DllModel.Lock(vstore.Name);
                    if (!vstore.IsLocked) DllModel.Unlock(vstore.Name);
                    vstore.PropertyChanged += OnStorePropertyChanged;
                    vstore.Post += OnReceiveValueStoreEvent;
                }
            if (e.OldItems != null)
                foreach (ValueStore vstore in e.OldItems)
                {
                    DllModel.Unlock(vstore.Name);
                    vstore.PropertyChanged -= OnStorePropertyChanged;
                    vstore.Post -= OnReceiveValueStoreEvent;
                }

        }
        
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
                stores.Clear();
                if (isenable)
                {
                    foreach (ValueInfo vinfo in ValueManager)
                    {
                        vinfo.StoresChanged += OnValueInfoStoresChanged;
                        foreach (ValueStore vstore in vinfo.Stores)
                            stores.Add(vstore);
                    }
                }
                else
                {
                    foreach (ValueInfo vinfo in ValueManager)
                    {
                        vinfo.StoresChanged -= OnValueInfoStoresChanged;
                    }
                }
            }
        }
        private void OnValueInfoStoresChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (ValueStore vstore in e.NewItems)
                    stores.Add(vstore);
            if (e.OldItems != null)
                foreach (ValueStore vstore in e.OldItems)
                    stores.Remove(vstore);
        }

        #endregion

        #region Thread

        private int _pause_old;
        private int _pause_new;

        protected override void Handle()
        {
            foreach (ValueStore vstore in stores)
            {
                switch (vstore.Type)
                {
                    case ValueModel.Types.BOOL: vstore.Value = DllModel.GetValue_Bit(vstore.Name); break;
                    case ValueModel.Types.WORD: vstore.Value = (Int16)DllModel.GetValue_Word(vstore.Name); break;
                    case ValueModel.Types.UWORD: vstore.Value = (UInt16)DllModel.GetValue_Word(vstore.Name); break;
                    case ValueModel.Types.DWORD: vstore.Value = (Int32)DllModel.GetValue_DWord(vstore.Name); break;
                    case ValueModel.Types.UDWORD: vstore.Value = (UInt32)DllModel.GetValue_DWord(vstore.Name); break;
                    case ValueModel.Types.FLOAT: vstore.Value = (float)DllModel.GetValue_Float(vstore.Name); break;
                    case ValueModel.Types.BCD: vstore.Value = (Int16)DllModel.GetValue_Word(vstore.Name); break;
                }
            }
            // 若检查到暂停状态的改变则进行处理
            if (SimulateDllModel.GetBPPause() > 0 && cursor.Address < 0)
            {
                cursor.Address = SimulateDllModel.GetBPAddr();
                BreakpointPaused(this, new BreakpointPauseEventArgs(cursor));
                if (SimulateDllModel.GetCallCount() > 256)
                    OnSimulateException(new StackOverflowException("子程序 & 用户函数嵌套调用超过了上限（256）。"), new RoutedEventArgs());
            }
            _pause_old = _pause_new;
        }

        #endregion

        #region Breakpoint Control

        private BreakpointCursor cursor;
        public BreakpointCursor Cursor { get { return this.cursor; } }
        public bool IsBPPause { get { return cursor.Address >= 0; } }
        
        public void Resume()
        {
            SimulateDllModel.SetBPPause(0);
            BreakpointResumed(this, new BreakpointPauseEventArgs(cursor));
            cursor.Address = -1;
        }
        
        public void MoveStep()
        {
            SimulateDllModel.MoveStep();
            BreakpointResumed(this, new BreakpointPauseEventArgs(cursor));
            cursor.Address = -1;
        }

        public void CallStep()
        {
            SimulateDllModel.CallStep();
            BreakpointResumed(this, new BreakpointPauseEventArgs(cursor));
            cursor.Address = -1;
        }

        public void JumpTo(int bpaddr)
        {
            SimulateDllModel.JumpTo(bpaddr);
            BreakpointResumed(this, new BreakpointPauseEventArgs(cursor));
            cursor.Address = -1;
        }

        public void JumpOut()
        {
            SimulateDllModel.JumpOut();
            BreakpointResumed(this, new BreakpointPauseEventArgs(cursor));
            cursor.Address = -1;
        }

        #endregion

        #region Event Handler
        
        private void OnReceiveValueStoreEvent(object sender, ValueStoreWriteEventArgs e)
        {
            ValueStore store = (ValueStore)sender;
            if (e.ToLock)
            {
                DllModel.Lock(store.Name);
                store.IsLocked = true;
            }
            if (e.Unlock)
            {
                DllModel.Unlock(store.Name);
                store.IsLocked = false;
            }
            if (e.UnlockAll)
            {
                foreach (ValueStore vstore in stores)
                    if (vstore.IsLocked) vstore.Unlock();
            }
            if (e.IsWrite)
            {
                switch (store.Type)
                {
                    case ValueModel.Types.BOOL:
                        DllModel.SetValue_Bit(store.Name, e.ToValue.ToString().Equals("ON") ? 1 : 0); break;
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

        private void OnSimulateException(object sender, RoutedEventArgs e)
        {
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
