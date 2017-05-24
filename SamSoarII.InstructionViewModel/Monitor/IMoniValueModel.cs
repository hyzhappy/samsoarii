using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.LadderInstViewModel.Monitor
{
    public interface IMoniValueModel
    {
        string Value { get; }
        event RoutedEventHandler ValueChanged;
    }
}
