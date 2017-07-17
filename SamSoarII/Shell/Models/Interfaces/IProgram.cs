using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SamSoarII.Shell.Models
{
    public interface IProgram : INotifyPropertyChanged, ITabItem
    {
        string ProgramName { get; set; }
    }
}
