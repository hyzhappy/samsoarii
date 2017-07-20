using SamSoarII.Shell.Windows.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Update
{
    public class Information:IDisposable
    {
        private bool ignore = false;
        public bool Ignore
        {
            get { return ignore; }
            set
            {
                if (ignore ^ value == true)
                    parent.ChildrensChanged();
                ignore = value;
            }
        }
        private UpdateManager parent;
        public UpdateManager Parent { get { return parent; } }
        private InformationUnit view;
        public InformationUnit View { get { return view; } set { view = value; } }
        public Information(UpdateManager _parent)
        {
            parent = _parent;
        }

        public void CreateView()
        {
            view = new InformationUnit(this);
        }

        public void Dispose()
        {
            view.Dispose();
            parent = null;
            view = null;
        }
    }
}
