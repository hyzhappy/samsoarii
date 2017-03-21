using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;

using SamSoarII.Simulation.Core.VariableModel;
using System.Windows;

namespace SamSoarII.Simulation.UI.Monitor
{
    public class MonitorComboBox : ComboBox
    {
        public const int TYPE_DATATYPE = 0x02;

        private int type;
        private SimulateVariableUnit svunit;
       
        public event RoutedEventHandler TextLegalChanged;

        public SimulateVariableUnit SVUnit
        {
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.svunit = value;
                    Items.Clear();
                    if (svunit is SimulateVInputUnit)
                    {
                        return;
                    }
                    if (svunit.Name[0] == 'D')
                    {
                        Items.Add("WORD");
                        Items.Add("DWORD");
                        Items.Add("FLOAT");
                        Items.Add("DOUBLE");
                        if (svunit is SimulateBitUnit)
                        {
                            throw new FormatException();
                        }
                        if (svunit is SimulateWordUnit)
                        {
                            Text = "WORD";
                        }
                        if (svunit is SimulateDWordUnit)
                        {
                            Text = "DWORD";
                        }
                        if (svunit is SimulateFloatUnit)
                        {
                            Text = "FLOAT";
                        }
                        if (svunit is SimulateDoubleUnit)
                        {
                            Text = "DOUBLE";
                        }
                    }
                    else
                    {
                        if (svunit is SimulateBitUnit)
                        {
                            Items.Add("BIT");
                            Text = "BIT";
                        }
                        if (svunit is SimulateWordUnit)
                        {
                            Items.Add("WORD");
                            Text = "WORD";
                        }
                        if (svunit is SimulateDWordUnit)
                        {
                            Items.Add("DWORD");
                            Text = "DWORD";
                        }
                        if (svunit is SimulateFloatUnit)
                        {
                            Items.Add("FLOAT");
                            Text = "FLOAT";
                        }
                        if (svunit is SimulateDoubleUnit)
                        {
                            Items.Add("DOUBLE");
                            Text = "DOUBLE";
                        }
                    }
                });
            }
        }
        
        public MonitorComboBox(int _type)
        {
            this.type = _type;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (TextLegalChanged != null)
            {
                TextLegalChanged(this, new RoutedEventArgs());
            }
        }
    }
}
