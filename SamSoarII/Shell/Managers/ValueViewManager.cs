using SamSoarII.Shell.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SamSoarII.Core.Models;

namespace SamSoarII.Shell.Managers
{
    public class ValueViewManager : IViewModel
    {
        public ValueViewManager(ValueManager _core)
        {
            Core = _core;
        }

        public void Dispose()
        {
            Core = null;
        }

        #region Core

        private ValueManager core;
        public ValueManager Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core != null)
                {
                    core.View = null;
                }
                this.core = value;
                if (core != null)
                {
                    if (core.View != this) core.View = this;
                }
            }
        }
        IModel IViewModel.Core
        {
            get { return Core; }
            set { Core = (ValueManager)value; }
        }

        #endregion
        
        public ProjectViewModel ViewParent { get { return core?.Parent.VMDProj; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

    }
}
