using System;
using System.Collections.Generic;
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

namespace SamSoarII.UserInterface
{
    /// <summary>
    /// ElementValueMultiplyModifyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ElementValueMultiplyModifyDialog : Window, IDisposable
    {
        public ElementValueMultiplyModifyDialog(string[] varnames, string[] vartypes)
        {
            InitializeComponent();
            Count = varnames.Length;
            PN_Values = new ElementValueModifyPanel[Count];
            for (int i = 0; i < Count; i++)
            {
                PN_Values[i] = new ElementValueModifyPanel();
                PN_Values[i].VarName = varnames[i];
                PN_Values[i].VarType = vartypes[i];
                PN_Values[i].ValueModify += OnValueModify;
                TabItem titem = new TabItem();
                titem.Header = varnames[i];
                titem.Content = PN_Values[i];
                TC_Main.Items.Add(titem);
            }
        }
        
        public int Count { get; private set; }
        private ElementValueModifyPanel[] PN_Values = null;
        public int SelectedIndex
        {
            get
            {
                return TC_Main.SelectedIndex;
            }
            set
            {
                TC_Main.SelectedIndex = value;
            }
        }

        public event ElementValueModifyEventHandler ValueModify = delegate { };
        private void OnValueModify(object sender, ElementValueModifyEventArgs e)
        {
            ValueModify(this, e);
        }

        public void Dispose()
        {
            Close();
        }
        
    }
}
