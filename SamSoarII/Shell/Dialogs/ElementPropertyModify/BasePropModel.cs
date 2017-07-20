using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SamSoarII.Shell.Dialogs
{
    public abstract class BasePropModel : UserControl, INotifyPropertyChanged, IDisposable
    {
        public BasePropModel() { }

        public BasePropModel(LadderUnitModel _core)
        {
            core = _core;
            valuestrings = new string[5] { "", "", "", "", "" };
            commentstrings = new string[5] { "", "", "", "", "" };
            for (int i = 0; i < core.Children.Count; i++)
            {
                ValueModel value = core.Children[i];
                valuestrings[i] = value.Text.Equals("???") ? "" : value.Text;
                commentstrings[i] = value.ValueManager[value].Comment;
            }
        }

        public virtual void Dispose()
        {
            core = null;
            valuestrings = null;
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

        private string[] valuestrings;
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
                //PropertyChanged(this, new PropertyChangedEventArgs(String.Format("ValueString{0:d}", id + 1)));
                string comment = null;
                try
                {
                    comment = core.ValueManager[str].Comment;
                }
                catch (ValueParseException)
                {
                    comment = "";
                }
                SetCommentString(id, comment);
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
