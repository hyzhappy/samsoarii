using SamSoarII.Core.Models;
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

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// ElementValueModifyPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ValueModifyPanel : UserControl, IDisposable
    {
        public ValueModifyPanel(object _core)
        {
            InitializeComponent();
            core = _core;
            TB_Name.Text = Store.Name;
            TB_Value.Text = Store.ShowValue;
            switch (Store.Parent.Prototype.Base)
            {
                case ValueModel.Bases.X:
                case ValueModel.Bases.Y:
                    InitializeForce();
                    break;
                case ValueModel.Bases.M:
                case ValueModel.Bases.S:
                case ValueModel.Bases.C:
                case ValueModel.Bases.T:
                    InitializeBit();
                    break;
                default:
                    InitializeWord();
                    break;
            }
        }

        public void Dispose()
        {
            core = null;
        }

        #region Number

        private object core;
        public ValueModel VMCore { get { return core is ValueModel ? (ValueModel)core : null; } }
        public MonitorElement MECore { get { return core is MonitorElement ? (MonitorElement)core : null; } }
        public ValueStore Store
        {
            get
            {
                if (core is ValueStore) return (ValueStore)core;
                if (VMCore != null) return VMCore.Store;
                if (MECore != null) return MECore.Store;
                return null;
            }
        }

        public ValueModel.Types SelectedType
        {
            get { return (ValueModel.Types)Enum.Parse(typeof(ValueModel.Types), CB_Type.SelectedItem.ToString()); }
        }

        #endregion
        
        private void InitializeForce()
        {
            TB_Value.IsReadOnly = true;
            CB_Type.Items.Add("BOOL");
            CB_Type.SelectedIndex = 0;
            BT_FON.Visibility = BT_FOFF.Visibility = BT_CF.Visibility = BT_CFA.Visibility
                = Visibility.Visible;
        }
        private void InitializeBit()
        {
            TB_Value.IsReadOnly = true;
            CB_Type.Items.Add("BOOL");
            CB_Type.SelectedIndex = 0;
            BT_WON.Visibility = BT_WOFF.Visibility
                = Visibility.Visible;
        }
        private void InitializeWord()
        {
            if (VMCore != null)
            {
                switch (VMCore.Type)
                {
                    case ValueModel.Types.WORD:
                        CB_Type.Items.Add("WORD");
                        CB_Type.Items.Add("UWORD");
                        CB_Type.Items.Add("BCD");
                        break;
                    case ValueModel.Types.DWORD:
                        CB_Type.Items.Add("DWORD");
                        CB_Type.Items.Add("UDWORD");
                        break;
                    case ValueModel.Types.FLOAT:
                        CB_Type.Items.Add("FLOAT");
                        break;
                }
                CB_Type.SelectedIndex = 0;
            }
            if (MECore != null)
            {
                CB_Type.Items.Add("WORD");
                CB_Type.Items.Add("UWORD");
                CB_Type.Items.Add("DWORD");
                CB_Type.Items.Add("UDWORD");
                CB_Type.Items.Add("BCD");
                CB_Type.Items.Add("FLOAT");
                CB_Type.SelectedIndex = MECore.SelectIndex;
            }
            BT_Write.Visibility = Visibility.Visible;
        }

        public event RoutedEventHandler Closing = delegate { };

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (TB_Value.Background == Brushes.Red)
            {
                return;
            }
            if (sender == BT_FON)
            {
                Store.Write("ON", true);
                TB_Value.Text = "ON";
            }
            if (sender == BT_FOFF)
            {
                Store.Write("OFF", true);
                TB_Value.Text = "OFF";
            }
            if (sender == BT_CF)
            {
                Store.Unlock();
            }
            if (sender == BT_CFA)
            {
                Store.Unlock(true);
            }
            if (sender == BT_WON)
            {
                Store.Write("ON");
                TB_Value.Text = "ON";
            }
            if (sender == BT_WOFF)
            {
                Store.Write("OFF");
                TB_Value.Text = "OFF";
            }
            if (sender == BT_Write)
            {
                Store.Write(TB_Value.Text, false);
            }
        }

        private void CB_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MECore != null) MECore.SelectIndex = CB_Type.SelectedIndex;
            CheckValue();
        }

        private void TB_Value_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValue();
        }

        private void TB_Value_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (BT_Write.Visibility == Visibility.Visible
                     && TB_Value.Background != Brushes.Red)
                    {
                        OnButtonClick(BT_Write, new RoutedEventArgs());
                        Closing(this, new RoutedEventArgs());
                    }
                    break;
                case Key.Escape:
                    Closing(this, new RoutedEventArgs());
                    break;
            }
        }

        private void CheckValue()
        {
            try
            {
                switch (CB_Type.Items[CB_Type.SelectedIndex].ToString())
                {
                    case "BOOL":
                        switch (TB_Value.Text)
                        {
                            case "ON":
                            case "OFF":
                                break;
                            default:
                                TB_Value.Background = Brushes.Red;
                                return;
                        }
                        break;
                    case "WORD":
                        Int16.Parse(TB_Value.Text);
                        break;
                    case "UWORD":
                        UInt16.Parse(TB_Value.Text);
                        break;
                    case "DWORD":
                        Int32.Parse(TB_Value.Text);
                        break;
                    case "UDWORD":
                        UInt32.Parse(TB_Value.Text);
                        break;
                    case "BCD":
                        if (TB_Value.Text.Length > 4)
                        {
                            TB_Value.Background = Brushes.Red;
                            return;
                        }
                        UInt16 value = UInt16.Parse(TB_Value.Text);
                        if (value > 9999) throw new FormatException();
                        break;
                    case "FLOAT":
                        float.Parse(TB_Value.Text);
                        break;
                }
                TB_Value.Background = Brushes.White;
            }
            catch (Exception)
            {
                TB_Value.Background = Brushes.Red;
            }
        }
    }
}
