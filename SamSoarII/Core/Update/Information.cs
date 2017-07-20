using SamSoarII.Shell.Windows.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Update
{
    public class Information
    {
        private bool ignore = false;
        public bool Ignore { get { return ignore; } set { ignore = value; } }
        private InformationUnit view;
        public InformationUnit View { get { return view; } set { view = value; } }
        public Information()
        {

        }
        public void CreateView()
        {
            view = new InformationUnit(this);
        }
    }
}
