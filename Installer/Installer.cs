using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;

namespace Installer
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            try
            {
                var dir = Directory.GetParent(Context.Parameters["assemblypath"]).FullName;
                if (Directory.Exists(dir)) Directory.Delete(dir, true);
            }
            catch (Exception)
            {
            }
        }
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }
    }
}
