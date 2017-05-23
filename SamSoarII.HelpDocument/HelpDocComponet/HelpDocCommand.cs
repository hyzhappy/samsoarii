using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamSoarII.HelpDocument.HelpDocComponet
{
    public static class HelpDocCommand
    {
        public static RoutedUICommand HideOrShowCommand { get; set; }
        public static RoutedUICommand HistoryBack { get; set; }
        public static RoutedUICommand HistoryAhead { get; set; }
        public static RoutedUICommand DeletePage { get; set; }
        public static RoutedUICommand DeleteAllPage { get; set; }
        public static RoutedUICommand CollectPage { get; set; }
        public static RoutedUICommand OpenPage { get; set; }
        public static RoutedUICommand ClosePage { get; set; }
        public static RoutedUICommand ScrollToLeft { get; set; }
        public static RoutedUICommand ScrollToRight { get; set; }
        public static RoutedUICommand PrintPage { get; set; }
        static HelpDocCommand()
        {
            HideOrShowCommand = new RoutedUICommand();
            HistoryBack = new RoutedUICommand();
            HistoryAhead = new RoutedUICommand();
            DeletePage = new RoutedUICommand();
            DeleteAllPage = new RoutedUICommand();
            CollectPage = new RoutedUICommand();
            OpenPage = new RoutedUICommand();
            ClosePage = new RoutedUICommand();
            ScrollToLeft = new RoutedUICommand();
            ScrollToRight = new RoutedUICommand();
            PrintPage = new RoutedUICommand();
        }
    }
}
