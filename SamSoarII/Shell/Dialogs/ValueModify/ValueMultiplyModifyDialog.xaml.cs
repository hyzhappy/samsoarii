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
using System.Windows.Shapes;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// ValueMultiplyModifyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ValueMultiplyModifyDialog : Window, IDisposable
    {
        public ValueMultiplyModifyDialog(IEnumerable<object> _cores)
        {
            InitializeComponent();
            panels = new ValueModifyPanel[_cores.Count()];
            tabs = new TabItem[_cores.Count()];
            int i = 0;
            foreach (object _core in _cores)
            {
                panels[i] = new ValueModifyPanel(_core);
                tabs[i] = new TabItem();
                tabs[i].Header = panels[i].Store.Name;
                tabs[i].Content = panels[i];
                TC_Main.Items.Add(tabs[i++]);
            }
        }

        public void Dispose()
        {
            TC_Main.Items.Clear();
            for (int i = 0; i < panels.Length; i++)
            {
                tabs[i].Content = null;
                panels[i].Dispose();
                panels[i] = null;
            }
            tabs = null;
            panels = null;
        }

        public int SelectedIndex
        {
            get { return TC_Main.SelectedIndex; }
            set { TC_Main.SelectedIndex = value; }
        }

        private ValueModifyPanel[] panels;
        private TabItem[] tabs;
    }
}
