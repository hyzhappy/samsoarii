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
            private int focus;

            public MonitorTextBox TextBox_Name;
            public MonitorComboBox ComboBox_Type;
            public MonitorTextBox TextBox_Var;
            public MonitorTextBox TextBox_Value;
            public MonitorBitButton Button_Value;
            public MonitorLockButton Button_Lock;
            public MonitorExpandButton Button_Expand;
            public MonitorCloseButton Button_Close;

            public int ID
            {
                get
                {
                    return this.id;
                }
                set
                {
                    this.id = value;
                    Grid.SetRow(TextBox_Name, id);
                    Grid.SetRow(ComboBox_Type, id);
                    Grid.SetRow(TextBox_Var, id);
                    Grid.SetRow(TextBox_Value, id);
                    Grid.SetRow(Button_Value, id);
                    Grid.SetRow(Button_Lock, id);
                    Grid.SetRow(Button_Close, id);
                    if (Button_Expand != null)
                        Grid.SetRow(Button_Expand, id);
                }
            }

            public const int FOCUS_NULL = 0x00;
            public const int FOCUS_NAME = 0x01;
            public const int FOCUS_TYPE = 0x02;
            public const int FOCUS_VAR = 0x03;
            public const int FOCUS_VALUE = 0x04;
            public int Focus
            {
                get
                {
                    return this.focus;
                }
                set
                {
                    this.focus = value;
                    switch (focus)
                    {
                        case FOCUS_NAME:
                            TextBox_Name.Focus();
                            break;
                        case FOCUS_TYPE:
                            ComboBox_Type.Focus();
                            break;
                        case FOCUS_VAR:
                            TextBox_Var.Focus();
                            break;
                        case FOCUS_VALUE:
                            TextBox_Value.Focus();
                            break;
                    }
                }
            }

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
                if (svunit is SimulateUnitSeries)
                {
                    if (parent.MainGrid.Children.Contains(Button_Lock))
                    {
                        parent.MainGrid.Children.Remove(Button_Lock);
                    }
                    if (Button_Expand != null)
                    {
                        if (parent.MainGrid.Children.Contains(Button_Expand))
                        {
                            parent.MainGrid.Children.Remove(Button_Expand);
                        }
                    }
                    SimulateUnitSeries ssunit = (SimulateUnitSeries)(svunit);
                    Button_Expand = new MonitorExpandButton(ssunit);
                    Button_Expand.IsExpanded = ssunit.IsExpand;
                    Button_Expand.MouseUp += OnExpandButtonClicked;
                    Grid.SetRow(Button_Expand, id);
                    Grid.SetColumn(Button_Expand, 4);
                    parent.MainGrid.Children.Add(Button_Expand);
                }
                else
                {
                    if (Button_Expand != null)
                    {
                        if (parent.MainGrid.Children.Contains(Button_Expand))
                        {
                            parent.MainGrid.Children.Remove(Button_Expand);
                        }
                        Button_Expand = null;
                    }
                    if (!parent.MainGrid.Children.Contains(Button_Lock))
                    {
                        parent.MainGrid.Children.Add(Button_Lock);
                    }
                    Button_Lock.IsLocked = svunit.Islocked;
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

                TextBox_Name.TextLegalChanged += OnTextLegalChanged;
                TextBox_Name.InsertRowElementBehindHere += OnInsertRowBehindHere;
                TextBox_Name.GotFocus += OnChildrenGotFocus;
                TextBox_Name.LostFocus += OnChildrenLostFocus;
                TextBox_Name.FocusUp += OnFocusUp;
                TextBox_Name.FocusDown += OnFocusDown;
                ComboBox_Type.TextLegalChanged += OnTextLegalChanged;
                ComboBox_Type.GotFocus += OnChildrenGotFocus;
                ComboBox_Type.LostFocus += OnChildrenLostFocus;
                ComboBox_Type.FocusUp += OnFocusUp;
                ComboBox_Type.FocusDown += OnFocusDown;
                TextBox_Var.TextLegalChanged += OnTextLegalChanged;
                TextBox_Var.GotFocus += OnChildrenGotFocus;
                TextBox_Var.LostFocus += OnChildrenLostFocus;
                TextBox_Var.FocusUp += OnFocusUp;
                TextBox_Var.FocusDown += OnFocusDown;
                TextBox_Value.TextLegalChanged += OnTextLegalChanged;
                TextBox_Value.GotFocus += OnChildrenGotFocus;
                TextBox_Value.LostFocus += OnChildrenLostFocus;
                TextBox_Value.FocusUp += OnFocusUp;
                TextBox_Value.FocusDown += OnFocusDown;
                Button_Value.TextLegalChanged += OnTextLegalChanged;
                Button_Lock.MouseUp += OnLockButtonClicked;
                Button_Close.MouseUp += OnCloseButtonClicked;
                VariableUnitChanged += parent.OnVariableUnitChanged;
                VariableUnitClosed += parent.OnVariableUnitClosed;
                VariableUnitLocked += parent.OnVariableUnitLocked;
                VariableUnitUnlocked += parent.OnVariableUnitUnlocked;
                VariableUnitExpanded += parent.OnVariableUnitExpanded;
                InsertRowElementBehindHere += parent.OnInsertRowBehindHere;
                FocusUp += parent.OnFocusUp;
                FocusDown += parent.OnFocusDown;

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
                VariableUnitChangeEventArgs _e = new VariableUnitChangeEventArgs();
                if (svunit is SimulateUnitSeries)
                {
                    SimulateUnitSeries ssunit = (SimulateUnitSeries)(svunit);
                    if (sender == ComboBox_Type)
                    {
                        _e.Old = svunit;
                        _e.New = ssunit.ChangeDataType(ComboBox_Type.Text);

                        Setup(_e.New);

                        if (VariableUnitChanged != null)
                        {
                            VariableUnitChanged(this, _e);
                        }

                        return;
                    }
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

            public event RoutedEventHandler VariableUnitExpanded;
            private void OnExpandButtonClicked(object sender, RoutedEventArgs e)
            {
                if (VariableUnitExpanded != null)
                {
                    VariableUnitExpanded(this, e);
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
            
            private void OnChildrenGotFocus(object sender, RoutedEventArgs e)
            {
                if (sender == TextBox_Name)
                {
                    this.focus = FOCUS_NAME;
                }
                if (sender == ComboBox_Type)
                {
                    this.focus = FOCUS_TYPE;
                }
                if (sender == TextBox_Var)
                {
                    this.focus = FOCUS_VAR;
                }
                if (sender == TextBox_Value)
                {
                    this.focus = FOCUS_VALUE;
                }
            }

            private void OnChildrenLostFocus(object sender, RoutedEventArgs e)
            {
                this.focus = FOCUS_NULL;
            }

            public event RoutedEventHandler InsertRowElementBehindHere;
            private void OnInsertRowBehindHere(object sender, RoutedEventArgs e)
            {
                if (InsertRowElementBehindHere != null)
                {
                    InsertRowElementBehindHere(this, e);
                }
            }

            public event RoutedEventHandler FocusUp;
            private void OnFocusUp(object sender, RoutedEventArgs e)
            {
                if (FocusUp != null)
                {
                    FocusUp(this, e);
                }
            }
            public event RoutedEventHandler FocusDown;
            private void OnFocusDown(object sender, RoutedEventArgs e)
            {
                if (FocusDown != null)
                {
                    FocusDown(this, e);
                }
            }
            public event RoutedEventHandler FocusLeft;
            private void OnFocusLeft(object sender, RoutedEventArgs e)
            {
                if (this.focus > FOCUS_NAME)
                {
                    this.Focus--;
                }
                if (FocusLeft != null)
                {
                    FocusLeft(this, e);
                }
            }
            public event RoutedEventHandler FocusRight;
            private void OnFocusRight(object sender, RoutedEventArgs e)
            {
                if (this.Focus < FOCUS_VALUE)
                {
                    this.Focus++;
                }
                if (FocusRight != null)
                {
                    FocusRight(this, e);
                }
            }
            #endregion
        }
        
        private List<SimulateVariableUnit> svunits;

        private LinkedList<RowElement> reles;
        
        public MonitorTable()
        {
            InitializeComponent();
            reles = new LinkedList<RowElement>();
        }

        public List<SimulateVariableUnit> SVUnits
        {
            set
            {
                this.svunits = value;
                Update();
            }
            get
            {
                List<SimulateVariableUnit> ret = new List<SimulateVariableUnit>();
                foreach (SimulateVariableUnit svunit in svunits)
                {
                    ret.Add(svunit);
                    if (svunit is SimulateUnitSeries)
                    {
                        SimulateUnitSeries sus = svunit as SimulateUnitSeries;
                        foreach (SimulateVariableUnit ssvunit in (SimulateVariableUnit[])(sus.Value))
                        {
                            ret.Add(ssvunit);
                        }
                    }
                }
                return ret;
            }
        }
        
        public void UpdateValue()
        {
            lock (reles)
            {
                System.Threading.Monitor.Enter(reles);
                foreach (RowElement ele in reles)
                {
                    ele.Update();
                }
                System.Threading.Monitor.Exit(reles);
            }
        }

        public void Update()
        {

            lock (reles)
            {
                System.Threading.Monitor.Enter(reles);

                IEnumerator<SimulateVariableUnit> iter1 = SVUnits.GetEnumerator();
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
                    rele = new RowElement(this, reles.Count() + 1, svunit);
                    //rele.Setup(svunit);
                    reles.AddLast(rele);
                    if (!iter1.MoveNext()) eoi |= 1;
                }
                List<RowElement> relesDel = new List<RowElement>();
                while ((eoi & 2) == 0)
                {
                    //svmodel = null;
                    rele = iter2.Current;
                    relesDel.Add(rele);
                    if (!iter2.MoveNext()) eoi |= 2;
                    //rele.Uninstall();
                    //reles.Remove(rele);
                }
                foreach (RowElement releDel in relesDel)
                {
                    releDel.Uninstall();
                    reles.Remove(releDel);
                }
                
                System.Threading.Monitor.Exit(reles);
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

        public event RoutedEventHandler VariableUnitExpanded;
        private void OnVariableUnitExpanded(object sender, RoutedEventArgs e)
        {
            Update();
            if (VariableUnitExpanded != null)
            {
                VariableUnitExpanded(sender, e);
            }
        }

        private void OnInsertRowBehindHere(object sender, RoutedEventArgs e)
        {
            if (sender is RowElement)
            {
                RowElement rele = (RowElement)(sender);
                if (reles.Contains(rele))
                {
                    //reles.Add(rele);
                    SimulateVariableUnit _svunit = new SimulateVInputUnit();
                    svunits.Add(_svunit);
                    RowElement _rele = new RowElement(this, rele.ID + 1, _svunit);
                    /*
                    VariableUnitChangeEventArgs _e = new VariableUnitChangeEventArgs();
                    _e.Old = null;
                    _e.New = _svunit;
                    VariableUnitChanged(this, _e);
                    */
                    LinkedListNode<RowElement> node = reles.Find(rele).Next;
                    while (node != null)
                    {
                        node.Value.ID++;
                        node = node.Next;
                    }
                    node = reles.Find(rele);
                    reles.AddAfter(node, _rele);

                    _rele.Focus = rele.Focus;
                }
            }
        }

        private void OnFocusUp(object sender, RoutedEventArgs e)
        {
            if (sender is RowElement)
            {
                RowElement rele = (RowElement)(sender);
                LinkedListNode<RowElement> node = reles.Find(rele);
                LinkedListNode<RowElement> nodep = node.Previous;
                if (nodep != null)
                {
                    RowElement relep = nodep.Value;
                    relep.Focus = rele.Focus;
                }
            }
        }

        private void OnFocusDown(object sender, RoutedEventArgs e)
        {
            if (sender is RowElement)
            {
                RowElement rele = (RowElement)(sender);
                LinkedListNode<RowElement> node = reles.Find(rele);
                LinkedListNode<RowElement> noden = node.Next;
                if (noden != null)
                {
                    RowElement relen = noden.Value;
                    relen.Focus = rele.Focus;
                }
            }
        }

        private void MonitorAdd_Click(object sender, RoutedEventArgs e)
        {
            SimulateVInputUnit sviunit = new SimulateVInputUnit();
            svunits.Add(sviunit);
            RowElement rele = null;
            rele = new RowElement(this, reles.Count() + 1, sviunit);
            reles.AddLast(rele);
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
