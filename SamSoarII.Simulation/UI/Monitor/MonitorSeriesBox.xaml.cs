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
    /// MonitorSeriesBox.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorSeriesBox : GroupBox
    {
        private class ValueTextBlock : TextBlock, IDisposable
        {
            private SimulateVariableUnit svunit;

            public SimulateVariableUnit SVUnit
            {
                get { return this.svunit; }
                set
                {
                    if (svunit != null)
                    {
                        svunit.ValueChanged -= OnValueChanged;
                    }
                    this.svunit = value;
                    if (svunit != null)
                    {
                        svunit.ValueChanged += OnValueChanged;
                        Text = svunit.Value.ToString();
                        Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Visibility = Visibility.Hidden;
                    }
                }
            }

            public void Dispose()
            {
                SVUnit = null;
            }

            private void OnValueChanged(object sender, RoutedEventArgs e)
            {
                Dispatcher.Invoke(new Utility.Delegates.Execute(() =>
                {
                    Text = svunit.Value.ToString();
                }));
            }
        }

        private ValueTextBlock[] vtb_values = new ValueTextBlock[16];

        private SimulateUnitSeries series;

        public SimulateUnitSeries Series
        {
            get { return this.series; }
            set
            {
                this.series = value;

                int count = Math.Min(Values.Count(), 16);
                int rowcount = count >> 2;
                if (Values.Count() > 16) rowcount++;
                
                G_Table.RowDefinitions.Clear();
                for (int i = 0; i < rowcount; i++)
                {
                    RowDefinition rdef = new RowDefinition();
                    rdef.Height = new GridLength(24);
                    G_Table.RowDefinitions.Add(rdef);
                }

                for (int i = 0; i < count; i++)
                {
                    vtb_values[i].SVUnit = Values[i];
                    vtb_values[i].Visibility = Visibility.Visible;
                }
                for (int i = count; i < vtb_values.Count(); i++)
                {
                    vtb_values[i].SVUnit = null;
                    vtb_values[i].Visibility = Visibility.Collapsed;
                }
                
                if (Values.Count() > 16)
                {
                    B_Detail.Visibility = Visibility.Visible;
                }
                else
                {
                    B_Detail.Visibility = Visibility.Collapsed;
                }
            }
        }

        public SimulateVariableUnit[] Values
        {
            get { return (SimulateVariableUnit[])series.Value; }
        }
        

        public MonitorSeriesBox()
        {
            InitializeComponent();

            for (int i = 0; i < 16; i++)
            {
                vtb_values[i] = new ValueTextBlock();
                Grid.SetRow(vtb_values[i], i>>2);
                Grid.SetColumn(vtb_values[i], i&3);
                G_Table.Children.Add(vtb_values[i]);
            }
            
        }
    }
}
