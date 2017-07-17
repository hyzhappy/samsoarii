using SamSoarII.Core.Generate;
using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Global;

namespace SamSoarII.Shell.Windows
{
    public class ErrorReportElement_FB : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public const int STATUS_WARNING = 0x01;
        public const int STATUS_ERROR = 0x02;
        public int Status { get; private set; }

        public Brush BrushFill
        {
            get
            {
                switch (Status)
                {
                    case PLCOriginInst.STATUS_WARNING:
                        return Brushes.Yellow;
                    case PLCOriginInst.STATUS_ERROR:
                        return Brushes.Red;
                    default:
                        return Brushes.Transparent;
                }
            }
        }

        public string Message { get; private set; }

        private FuncBlockModel funcblock;
        public FuncBlockModel FuncBlock
        {
            get { return this.funcblock; }
        }
        public string Program
        {
            get { return funcblock.Name; }
        }

        public int Line { get; private set; }
        public int Column { get; private set; }
        public string Point
        {
            get { return String.Format("({0:d}, {1:d})", Line, Column); }
        }

        public ErrorReportElement_FB
        (
            int _status,
            string _message,
            FuncBlockModel _funcblock,
            int _line,
            int _column
        )
        {
            Status = _status;
            Message = _message;
            funcblock = _funcblock;
            Line = _line;
            Column = _column;
            PropertyChanged(this, new PropertyChangedEventArgs("Message"));
            PropertyChanged(this, new PropertyChangedEventArgs("Program"));
            PropertyChanged(this, new PropertyChangedEventArgs("Point"));
        }
    }
}
