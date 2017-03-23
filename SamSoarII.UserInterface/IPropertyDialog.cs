using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.UserInterface
{
    public interface IPropertyDialog
    {
        WindowStartupLocation WindowStartupLocation { get; set; }

        event RoutedEventHandler Commit;

        IList<string> PropertyStrings { get; }

        void ShowDialog();

        void Close();
    }
}
