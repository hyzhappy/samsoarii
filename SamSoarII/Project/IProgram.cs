using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.Project
{
    public interface IProgram : INotifyPropertyChanged, ITabItem
    {
        string ProgramName { get; set; }
    }
}
