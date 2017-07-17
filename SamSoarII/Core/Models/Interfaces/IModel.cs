using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public interface IModel : IDisposable, INotifyPropertyChanged
    {
        IModel Parent { get; }
        IViewModel View { get; set; }
        ProjectTreeViewItem PTVItem { get; set; }
        void Save(XElement xele);
        void Load(XElement xele);
    }
}
