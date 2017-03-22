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

using SamSoarII.Simulation.Core.Event;
using SamSoarII.Simulation.Core.VariableModel;

namespace SamSoarII.Simulation.UI.Monitor
{
    /// <summary>
    /// MonitorTable.xaml 的交互逻辑
    /// </summary> 
    public partial class MonitorTable : UserControl
    {
        private class RowElement
        {
            private MonitorTable parent;
            private SimulateVariableUnit svunit;
            private RowDefinition currentRowDefinition;
            private int id;
            private bool settingup;

            public MonitorTextBox TextBox_Name;
            public MonitorComboBox ComboBox_Type;
            public MonitorTextBox TextBox_Var;
            public MonitorTextBox TextBox_Value;
            public MonitorBitButton Button_Value;
            public MonitorLockButton Button_Lock;
            public MonitorCloseButton Button_Close;
            
            public RowElement(MonitorTable _parent, int _id, SimulateVariableUnit svunit = null)
            {
                settingup = false;
                parent = _parent;
                id = _id;
                Install();
                if (svunit != null)
                {
                    Setup(svunit);
                }
            }

            public void Setup(SimulateVariableUnit _svunit)
            {
                settingup = true;
                svunit = _svunit;
                TextBox_Name.SVUnit = svunit;
                ComboBox_Type.SVUnit = svunit;
                TextBox_Var.SVUnit = svunit;
                TextBox_Value.SVUnit = svunit;
                Button_Value.SVUnit = svunit;
                if (svunit is SimulateBitUnit)
                {
                    if (parent.MainGrid.Children.Contains(TextBox_Value))
                    {
                        parent.MainGrid.Children.Remove(TextBox_Value);
                    }
                    if (!parent.MainGrid.Children.Contains(Button_Value))
                    {
                        parent.MainGrid.Children.Add(Button_Value);
                    }
                }
                else
                {
                    if (parent.MainGrid.Children.Contains(Button_Value))
                    {
                        parent.MainGrid.Children.Remove(Button_Value);
                    }
                    if (!parent.MainGrid.Children.Contains(TextBox_Value))
                    {
                        parent.MainGrid.Children.Add(TextBox_Value);
                    }
                }
                settingup = false;
            }

            public void Update()
            {
                settingup = true;
                if (!Button_Lock.IsLocked)
                {
                    TextBox_Value.SetText();
                    Button_Value.SetText();
                }
                settingup = false;
            }

            public void Install()
            {
                TextBox_Name = new MonitorTextBox(MonitorTextBox.TYPE_NAME);
                ComboBox_Type = new MonitorComboBox(MonitorComboBox.TYPE_DATATYPE);
                TextBox_Var = new MonitorTextBox(MonitorTextBox.TYPE_VAR);
                TextBox_Value = new MonitorTextBox(MonitorTextBox.TYPE_VALUE);
                Button_Value = new MonitorBitButton();
                Button_Close = new MonitorCloseButton();
                Button_Lock = new MonitorLockButton();

                TextBox_Name.ContextMenu = parent.RightClickMenu;
                ComboBox_Type.ContextMenu = parent.RightClickMenu;
                TextBox_Var.ContextMenu = parent.RightClickMenu;
                TextBox_Value.ContextMenu = parent.RightClickMenu;
                TextBox_Value.IsReadOnly = true;
                Button_Value.ContextMenu = parent.RightClickMenu;
                Button_Value.IsReadOnly = true;
               // Button_Value.Opacity = 0.0;

                TextBox_Name.TextLegalChanged += OnTextLegalChanged;
                ComboBox_Type.TextLegalChanged += OnTextLegalChanged;
                TextBox_Var.TextLegalChanged += OnTextLegalChanged;
                TextBox_Value.TextLegalChanged += OnTextLegalChanged;
                Button_Value.TextLegalChanged += OnTextLegalChanged;
                Button_Lock.MouseUp += OnLockButtonClicked;
                Button_Close.MouseUp += OnCloseButtonClicked;
                VariableUnitChanged += parent.OnVariableUnitChanged;
                VariableUnitClosed += parent.OnVariableUnitClosed;
                VariableUnitLocked += parent.OnVariableUnitLocked;
                VariableUnitUnlocked += parent.OnVariableUnitUnlocked;

                Grid.SetRow(TextBox_Name, id);
                Grid.SetRow(ComboBox_Type, id);
                Grid.SetRow(TextBox_Var, id);
                Grid.SetRow(TextBox_Value, id);
                Grid.SetRow(Button_Value, id);
                Grid.SetRow(Button_Lock, id);
                Grid.SetRow(Button_Close, id);
                Grid.SetColumn(TextBox_Name, 0);
                Grid.SetColumn(ComboBox_Type, 1);
                Grid.SetColumn(TextBox_Var, 2);
                Grid.SetColumn(TextBox_Value, 3);
                Grid.SetColumn(Button_Value, 3);
                Grid.SetColumn(Button_Lock, 4);
                Grid.SetColumn(Button_Close, 5);
                currentRowDefinition = new RowDefinition();
                currentRowDefinition.Height = new GridLength(24);
                parent.MainGrid.RowDefinitions.Add(currentRowDefinition);
                parent.MainGrid.Children.Add(TextBox_Name);
                parent.MainGrid.Children.Add(ComboBox_Type);
                parent.MainGrid.Children.Add(TextBox_Var);
                parent.MainGrid.Children.Add(TextBox_Value);
                //parent.MainGrid.Children.Add(Button_Value);
                parent.MainGrid.Children.Add(Button_Lock);
                parent.MainGrid.Children.Add(Button_Close);
            }
            
            public void Uninstall()
            {
                VariableUnitChanged -= parent.OnVariableUnitChanged;

                parent.MainGrid.Children.Remove(TextBox_Name);
                parent.MainGrid.Children.Remove(ComboBox_Type);
                parent.MainGrid.Children.Remove(TextBox_Var);
                parent.MainGrid.Children.Remove(TextBox_Value);
                parent.MainGrid.Children.Remove(Button_Value);
                parent.MainGrid.Children.Remove(Button_Lock);
                parent.MainGrid.Children.Remove(Button_Close);
                parent.MainGrid.RowDefinitions.Remove(currentRowDefinition);
            }

            #region Event Handler
            public event VariableUnitChangeEventHandler VariableUnitChanged;
            
            private void OnTextLegalChanged(object sender, RoutedEventArgs e)
            {
                if (settingup)
                {
                    return;
                }
                if (sender == Button_Value && Button_Lock.IsLocked)
                {
                    switch (Button_Value.Status)
                    {
                        case MonitorBitButton.STATUS_ON:
                            svunit.Value = 1;
                            break;
                        case MonitorBitButton.STATUS_OFF:
                            svunit.Value = 0;
                            break;
                        case MonitorBitButton.STATUS_ERROR:
                            break;
                    }
                    return;
                }
                if (sender == TextBox_Value && Button_Lock.IsLocked)
                {
                    if (svunit is SimulateDoubleUnit)
                    {
                        svunit.Value = double.Parse(TextBox_Value.Text);
                    }
                    else
                    if (svunit is SimulateFloatUnit)
                    {
                        svunit.Value = float.Parse(TextBox_Value.Text);
                    }
                    else
                    {
                        svunit.Value = int.Parse(TextBox_Value.Text);
                    }
                    return;
                }
                VariableUnitChangeEventArgs _e = new VariableUnitChangeEventArgs();
                _e.Old = svunit;
                SimulateVariableUnit _svunit = null;
                if (sender == TextBox_Name)
                {
                    _svunit = SimulateVariableUnit.Create(TextBox_Name.Text);
                    if (_svunit == null) return;
                }
                else
                {
                    switch (ComboBox_Type.Text)
                    {
                        case "BIT":
                            _svunit = new SimulateBitUnit();
                            break;
                        case "WORD":
                            _svunit = new SimulateWordUnit();
                            break;
                        case "DWORD":
                            _svunit = new SimulateDWordUnit();
                            break;
                        case "FLOAT":
                            _svunit = new SimulateFloatUnit();
                            break;
                        case "DOUBLE":
                            _svunit = new SimulateDoubleUnit();
                            break;
                        default:
                            return;
                    }
                }
                _svunit.Name = TextBox_Name.Text;
                _svunit.Var = TextBox_Var.Text;
                _e.New = _svunit;

                Setup(_svunit); 

                if (VariableUnitChanged != null)
                {
                    VariableUnitChanged(this, _e);
                }
            }
            public event VariableUnitChangeEventHandler VariableUnitClosed;
           
            private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
            {
                if (Button_Lock.IsLocked)
                {
                    return;
                }
                VariableUnitChangeEventArgs _e = new VariableUnitChangeEventArgs();
                _e.Old = svunit;
                _e.New = null;

                Uninstall();
                if (VariableUnitClosed != null)
                {
                    VariableUnitClosed(this, _e);
                }
            }

            public event VariableUnitChangeEventHandler VariableUnitLocked;
            public event VariableUnitChangeEventHandler VariableUnitUnlocked;

            private void OnLockButtonClicked(object sender, RoutedEventArgs e)
            {
                VariableUnitChangeEventArgs _e = new VariableUnitChangeEventArgs();
                _e.Old = svunit;
                _e.New = svunit;

                if (Button_Lock.IsLocked)
                {
                    if (svunit is SimulateBitUnit)
                    {
                        Button_Value.IsReadOnly = false;
                    }
                    else
                    {
                        TextBox_Value.IsReadOnly = false;
                    }
                    if (VariableUnitLocked != null)
                    {
                        VariableUnitLocked(this, _e);
                    }
                }
                else
                {
                    TextBox_Value.IsReadOnly = true;
                    Button_Value.IsReadOnly = true;
                    if (VariableUnitUnlocked != null)
                    {
                        VariableUnitUnlocked(this, _e);
                    }
                }
            }
            #endregion
        }

        private List<SimulateVariableUnit> svunits;

        private List<RowElement> reles;
        
        public MonitorTable()
        {
            InitializeComponent();
            reles = new List<RowElement>();
        }

        public List<SimulateVariableUnit> SVUnits
        {
            set
            {
                this.svunits = value;
                Update();
            }
        }
        
        public void UpdateValue()
        {
            foreach (RowElement ele in reles)
            {
                ele.Update();
            }
        }

        public void Update()
        {
            IEnumerator<SimulateVariableUnit> iter1 = svunits.GetEnumerator();
            IEnumerator<RowElement> iter2 = reles.GetEnumerator();
            SimulateVariableUnit svunit = null;
            RowElement rele = null;
            int eoi = 0;

            if (!iter1.MoveNext()) eoi |= 1;
            if (!iter2.MoveNext()) eoi |= 2;
            while (eoi == 0)
            {
                svunit = iter1.Current;
                rele = iter2.Current;
                //rele.Update();
                rele.Setup(svunit);
                if (!iter1.MoveNext()) eoi |= 1;
                if (!iter2.MoveNext()) eoi |= 2;
            }
            while ((eoi & 1) == 0)
            {
                svunit = iter1.Current;
                rele = new RowElement(this, reles.Count()+1, svunit);
                //rele.Setup(svunit);
                reles.Add(rele);
                if (!iter1.MoveNext()) eoi |= 1;
            }
            while ((eoi & 2) == 0)
            {
                //svmodel = null;
                rele = iter2.Current;
                if (!iter2.MoveNext()) eoi |= 2;
                rele.Uninstall();
                reles.Remove(rele);
            }
        }

        #region Event Handler
        public event VariableUnitChangeEventHandler VariableUnitChanged;
        private void OnVariableUnitChanged(object sender, VariableUnitChangeEventArgs e)
        {
            int id = svunits.IndexOf(e.Old);
            svunits[id] = e.New;
            if (VariableUnitChanged != null)
            {
                VariableUnitChanged(sender, e);
            }
        }

        public event VariableUnitChangeEventHandler VariableUnitClosed;
        private void OnVariableUnitClosed(object sender, VariableUnitChangeEventArgs e)
        {
            svunits.Remove(e.Old);
            if (VariableUnitClosed != null)
            {
                VariableUnitClosed(sender, e);
            }
        }

        public event VariableUnitChangeEventHandler VariableUnitLocked;
        private void OnVariableUnitLocked(object sender, VariableUnitChangeEventArgs e)
        {
            if (VariableUnitLocked != null)
            {
                VariableUnitLocked(sender, e);
            }
        }

        public event VariableUnitChangeEventHandler VariableUnitUnlocked;
        private void OnVariableUnitUnlocked(object sender, VariableUnitChangeEventArgs e)
        {
            if (VariableUnitUnlocked != null)
            {
                VariableUnitUnlocked(sender, e);
            }
        }
        
        private void MonitorAdd_Click(object sender, RoutedEventArgs e)
        {
            SimulateVInputUnit sviunit = new SimulateVInputUnit();
            svunits.Add(sviunit);
            RowElement rele = null;
            rele = new RowElement(this, reles.Count() + 1, sviunit);
            reles.Add(rele);
        }

        private void MonitorDel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MonitorMulAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
