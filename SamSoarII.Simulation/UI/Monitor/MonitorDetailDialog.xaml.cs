using SamSoarII.Simulation.Core.VariableModel;
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

namespace SamSoarII.Simulation.UI.Monitor
{
    /// <summary>
    /// MonitorDetailDialog.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorDetailDialog : Window
    {
        private class ValueTextBox : TextBox, IDisposable
        {
            private SimulateVariableUnit svunit;

            public SimulateVariableUnit SVUnit
            {
                get { return svunit; }
            }
            
            public ValueTextBox(SimulateVariableUnit _svunit) : base()
            {
                svunit = _svunit;
                Text = svunit.Value.ToString();
                IsReadOnly = true;
                svunit.ValueChanged += OnValueChanged;
            }

            public void Dispose()
            {
                if (svunit != null)
                {
                    svunit.ValueChanged -= OnValueChanged;
                }
            }
            
            private void OnValueChanged(object sender, RoutedEventArgs e)
            {
                this.Dispatcher.Invoke(() =>
                {
                    Text = svunit.Value.ToString();
                });
            }
            
        }

        private SimulateUnitSeries series;

        public SimulateUnitSeries Series
        {
            get { return this.series; }
            set
            {
                Uninstall();
                this.series = value;
                TB_Start.Text = Values.First().Name;
                TB_End.Text = Values.Last().Name;
                CB_Type.Items.Clear();
                CB_Type.Items.Add(series.DataType);
                CB_Type.SelectedIndex = 0;
                Install();
            }
        }
        public SimulateVariableUnit[] Values
        {
            get { return (SimulateVariableUnit[])(series.Value); }
        }
        private ValueTextBox[] vtb_values = new ValueTextBox[0];

        public MonitorDetailDialog(SimulateUnitSeries _series)
        {
            InitializeComponent();
            Series = _series;

            for (int i = 0; i < 8; i++)
            {
                TextBlock tb = new TextBlock();
                tb.Text = String.Format("+ {0:d}", i);
                tb.FontSize = 18;
                Grid.SetRow(tb, 0);
                Grid.SetColumn(tb, i + 1);
                G_Frame.Children.Add(tb);
            }
        }

        private void Install()
        {
            int rowcount = Values.Length >> 3;
            //int colcount = 8;

            //G_Table.RowDefinitions.Clear();
            //G_Table.Children.Clear();
            for (int i = 0; i < rowcount; i++)
            {
                RowDefinition rdef = new RowDefinition();
                rdef.Height = new GridLength(24);
                G_Table.RowDefinitions.Add(rdef);
                TextBlock tb = new TextBlock();
                tb.Text = String.Format("{0:d}", Series.Offset + (i << 3));
                tb.FontSize = 18;
                Grid.SetRow(tb, i);
                Grid.SetColumn(tb, 0);
                G_Table.Children.Add(tb);
            }

            vtb_values = new ValueTextBox[Values.Count()];
            for (int i = 0; i < Values.Count(); i++)
            {
                vtb_values[i] = new ValueTextBox(Values[i]);
                Grid.SetRow(vtb_values[i], i >> 3);
                Grid.SetColumn(vtb_values[i], (i&7)+1);
                G_Table.Children.Add(vtb_values[i]);
                vtb_values[i].GotFocus += OnVTBGotFocus;
            }
        }
        
        private void Uninstall()
        {
            for (int i = 0; i < vtb_values.Count(); i++)
            {
                vtb_values[i].GotFocus -= OnVTBGotFocus;
                vtb_values[i].Dispose();
            }
            G_Table.RowDefinitions.Clear();
            G_Table.Children.Clear();
        }

        #region Cursor

        private ValueTextBox cursor;

        private new ValueTextBox Cursor
        {
            get { return this.cursor; }
            set
            {
                if (cursor == value)
                {
                    return;
                }
                if (cursor != null)
                {
                    cursor.Background = Brushes.Transparent;
                }
                cursor = value;
                if (cursor != null)
                {
                    cursor.Background = Brushes.LightGreen;
                    TB_Current.Text = cursor.SVUnit.Name;
                }
            }
        }

        private void OnVTBGotFocus(object sender, RoutedEventArgs e)
        {
            Cursor = (ValueTextBox)sender;
        }

        private void TB_Current_TextChanged(object sender, TextChangedEventArgs e)
        {
            TB_Current.Background = Brushes.Red;
            foreach (ValueTextBox vtbox in vtb_values)
            {
                if (vtbox.SVUnit.Name.Equals(TB_Current.Text))
                {
                    Cursor = vtbox;
                    TB_Current.Background = Brushes.Transparent;
                    return;
                }
            }
        }

        #endregion

    }
}
