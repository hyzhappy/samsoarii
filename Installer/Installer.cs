using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;

namespace Installer
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }
        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);
        }
        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
            DeleteTempFile();
        }

        protected override void OnCommitted(IDictionary savedState)
        {
            base.OnCommitted(savedState);
            CreateUnInstallBat();
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            try
            {
                var dir = GetTargetDir();
                if (Directory.Exists(dir)) Directory.Delete(dir, true);
            }
            catch (Exception)
            {
            }
        }
        private string GetTargetDir()
        {
            return Directory.GetParent(Context.Parameters["assemblypath"]).FullName;
        }

        private void CreateUnInstallBat()
        {
            try
            {
                FileStream fs = new FileStream(Path.Combine(GetTargetDir(), "unins000.bat"), FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("@echo off");
                sw.WriteLine(string.Format("start /normal %windir%\\system32\\msiexec /x {0}", Context.Parameters["productCode"].ToString()));
                sw.WriteLine("exit");
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch (Exception)
            {
            }
        }

        private void DeleteTempFile()
        {
            try
            {
                string dir = GetTargetDir();
                foreach (var file in Directory.GetFiles(dir))
                    if (file.EndsWith("tmp")) File.Delete(file);
            }
            catch (Exception)
            {
            }
        }
    }
}
