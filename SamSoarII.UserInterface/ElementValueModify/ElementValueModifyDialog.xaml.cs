using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SamSoarII.UserInterface
{
    /// <summary>
    /// ElementValueModifyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ElementValueModifyDialog : Window, IDisposable
    {
        public ElementValueModifyDialog()
        {
            InitializeComponent();
            KeyDown += ElementValueModifyDialog_KeyDown;
            PN_Main.ValueModify += OnValueModify;
            PN_Main.Closing += OnPanelClosing;
        }
        
        public string VarName
        {
            get { return PN_Main.VarName; }
            set { PN_Main.VarName = value; }
        }
        public string VarType
        {
            get { return PN_Main.VarType; }
            set { PN_Main.VarType = value; }
        }
        public string Value
        {
            get
            {
                return PN_Main.TB_Value.Text;
            }
            set
            {
                PN_Main.TB_Value.Text = value;
            }
        }

        public event ElementValueModifyEventHandler ValueModify = delegate { };
        private void OnValueModify(object sender, ElementValueModifyEventArgs e)
        {
            ValueModify(this, e);
        }
        
        private void OnPanelClosing(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ElementValueModifyDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
        
        public void Dispose()
        {
            Close();
        }
    }
}
