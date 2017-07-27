using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.ComponentModel;
using SamSoarII.Core.Models;
using ICSharpCode.AvalonEdit;

namespace SamSoarII.Shell.Windows
{
    public class FuncBrpoTableElement : IDisposable, INotifyPropertyChanged
    {
        public FuncBrpoTableElement(FuncBrpoModel _parent)
        {
            Parent = _parent;
        }

        public void Dispose()
        {
            Parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private FuncBrpoModel parent;
        public FuncBrpoModel Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (parent == value) return;
                FuncBrpoModel _parent = parent;
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
        public string ProgramName { get { return parent.Parent.Model.Name; } }
        public string Statement { get { return parent.Parent.Model.Code.Substring(parent.Parent.IndexStart, parent.Parent.IndexEnd - parent.Parent.IndexStart + 1); } }
        public string Position
        {
            get
            {
                TextViewPosition? pos = parent.Parent.Model.View.GetPosition(parent.Parent.IndexStart);
                return pos.HasValue ? String.Format("({0},{1})", pos.Value.Line, pos.Value.Column) : String.Empty;
            }
        }

        private void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsActive":
                    PropertyChanged(this, new PropertyChangedEventArgs("ActiveInfo"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ActiveBrush"));
                    break;
            }
        }
        
        #endregion
    }
}
