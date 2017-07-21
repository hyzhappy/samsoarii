using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamSoarII.Shell.Windows
{
    /// <summary>
    /// TextReplaceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TextReplaceWindow : UserControl, IWindow
    {
        public TextReplaceWindow(InteractionFacade _ifParent)
        {
            InitializeComponent();
            DataContext = this;
            ifParent = _ifParent;
            ifParent.PostIWindowEvent += OnReceiveIWindowEvent;
        }
        
        private InteractionFacade ifParent;
        public InteractionFacade IFParent { get { return this.ifParent; } }

        #region Event Handler

        public event IWindowEventHandler Post = delegate { };

        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {
        }
        #endregion
    }
}
