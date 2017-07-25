using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SamSoarII.Core.Models;
using System.Drawing;

namespace SamSoarII.Shell.Windows
{
    public class LadderBrpoTableElement : IDisposable, INotifyPropertyChanged
    {
        #region Resources

        static private string[] selectedconditions
            = { "无", "0", "1", "上升沿", "下降沿" };

        public IEnumerable<string> SelectedConditions()
        {
            return selectedconditions;
        }

        #endregion
        
        public LadderBrpoTableElement()
        {

        }

        public LadderBrpoTableElement(LadderBrpoModel _parent)
        {
            Parent = _parent;
        }

        public void Dispose()
        {
            Parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private LadderBrpoModel parent;
        public LadderBrpoModel Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (parent == value) return;
                LadderBrpoModel _parent = parent;
                this.parent = null;
                if (_parent != null)
                {
                    _parent.PropertyChanged -= OnParentPropertyChanged;
                    if (_parent.Element != null) _parent.Element = null;
                }
                this.parent = value;
                if (parent != null)
                {
                    parent.PropertyChanged += OnParentPropertyChanged;
                    if (parent.Element != this) parent.Element = this;
                }
            }
        }
        public string ActiveInfo { get { return parent.IsActive ? "启用" : "禁用"; } }
        public Brush ActiveBrush { get { return parent.IsActive ? Brushes.Red : Brushes.Gray; } }
        public string ProgramName { get { return parent.Parent.Parent.Parent.Name; } }
        public string Instruction { get { return parent.Parent.ToInstString(); } }
        public int SelectedIndex
        {
            get { return parent.ConditionIndex; }
            set { parent.ConditionIndex = value; }
        }
        public string SkipCount
        {
            get { return parent.SkipCount.ToString(); }
            set { try { parent.SkipCount = Math.Max(1, int.Parse(value)); } catch (Exception) { parent.SkipCount = 1; } }
        }

        private void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsActive":
                    PropertyChanged(this, new PropertyChangedEventArgs("ActiveInfo"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ActiveBrush"));
                    break;
                case "ConditionIndex":
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedIndex"));
                    break;
                case "SkipCount":
                    PropertyChanged(this, new PropertyChangedEventArgs("SkipCount"));
                    break;
            }
        }

        #endregion
    }
}
