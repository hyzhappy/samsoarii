﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages
{
    public abstract class NavigatePage : Page
    {
        public NavigatePage() : base() { }
        public abstract event NavigateToPageEventHandler NavigateToPage;
    }
}
