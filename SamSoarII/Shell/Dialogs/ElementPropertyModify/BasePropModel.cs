using SamSoarII.Core.Models;
using SamSoarII.Threads;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SamSoarII.Shell.Dialogs
{
    public abstract class BasePropModel : UserControl, INotifyPropertyChanged, IDisposable
    {
        public BasePropModel() { }

        public BasePropModel(LadderUnitModel _core)
        {
            core = _core;
            selectedindex = -1;
            valuestrings = new string[5] { "", "", "", "", "" };
            commentstrings = new string[5] { "", "", "", "", "" };
            valuesmodifieds = new bool[5] { false, false, false, false, false };
            for (int i = 0; i < core.Children.Count; i++)
            {
                ValueModel value = core.Children[i];
                valuestrings[i] = value.Text.Equals("???") ? "" : value.Text;
                commentstrings[i] = value.ValueManager[value].Comment;
            }
            mngUpdate = new UpdateValueChangedManager(this);
            mngUpdate.Start();
        }

        public virtual void Dispose()
        {
            mngUpdate.Abort();
            core = null;
            valuestrings = null;
            valuesmodifieds = null;
            commentstrings = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Core

        private LadderUnitModel core;
        public LadderUnitModel Core
        {
            get { return this.core; }
        }

        #endregion

        #region Shell

        #region Thread
        
        private class UpdateValueChangedManager : TimerThreadManager, IDisposable
        {
            public UpdateValueChangedManager(BasePropModel _core) : base(false, true)
            {
                core = _core;
                TimeSpan = 200;
            }

            public void Dispose()
            {
                core = null;
            }
            
            private BasePropModel core;
            public BasePropModel Core { get { return this.core; } }

            protected override void Handle()
            {
                if (core == null) return;
                try
                {
                    for (int i = 0; i < core.Count; i++)
                    {
                        if (core.valuesmodifieds[i])
                        {
                            core.valuesmodifieds[i] = false;
                            string comment = null;
                            try
                            {
                                comment = core.Core.ValueManager[core.valuestrings[i]].Comment;
                            }
                            catch (ValueParseException)
                            {
                                comment = "";
                            }
                            core.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)(delegate ()
                            {
                                try
                                {
                                    core.SetCommentString(i, comment);
                                }
                                catch (Exception)
                                {
                                }
                            }));
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private UpdateValueChangedManager mngUpdate;

        #endregion

        private string[] valuestrings;
        private bool[] valuesmodifieds;
        private string[] commentstrings;
        
        public string InstructionName { get { return core.InstName; } }
        public virtual int Count { get { return core.Children.Count; } set { } }
        public string GetValueString(int id) { return id >= 0 && id < Count ? valuestrings[id] : null;  }
        public string GetCommentString(int id) { return id >= 0 && id < Count ? commentstrings[id] : null; }
        protected void SetValueString(int id, string str)
        {
            if (id >= 0 && id < Count)
            {
                valuestrings[id] = str;
                valuesmodifieds[id] = true;
            }
        }
        protected void SetCommentString(int id, string str)
        {
            if (id < 0 || id >= Count) return;
            for (int i = 0; i < Count; i++)
            {
                if (valuestrings[i].Equals(valuestrings[id]) && !commentstrings[id].Equals(str))
                {
                    commentstrings[id] = str;
                    //PropertyChanged(this, new PropertyChangedEventArgs(String.Format("CommentString{0:d}", id + 1)));
                    if (id == selectedindex)
                        PropertyChanged(this, new PropertyChangedEventArgs("SelectedComment"));
                }
            }
        }
        
        protected int selectedindex;
        public virtual int SelectedIndex
        {
            get
            {
                return this.selectedindex;
            }
            set
            {
                this.selectedindex = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedIndex"));
                //PropertyChanged(this, new PropertyChangedEventArgs("SelectedComment"));
            }
        }
        public string SelectedComment
        {
            get { return GetCommentString(selectedindex); }
            set { SetCommentString(selectedindex, value); }
        }

        #endregion


    }
}
