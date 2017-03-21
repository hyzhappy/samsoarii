﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using SamSoarII.Simulation.Core.VariableModel;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows;

namespace SamSoarII.Simulation.UI.Monitor
{
    public class MonitorTextBox : TextBox
    {
        public const int TYPE_NAME = 0x01;
        public const int TYPE_VAR = 0x03;
        public const int TYPE_VALUE = 0x04;

        private int type;
        private SimulateVariableUnit svunit;

        public SimulateVariableUnit SVUnit
        {
            set
            {
                this.svunit = value;
                SetText();
            }
        }

        public MonitorTextBox(int _type)
        {
            type = _type;
        }
        
        public void SetText()
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (type)
                {
                    case TYPE_NAME:
                        Text = svunit.Name;
                        break;
                    case TYPE_VAR:
                        Text = svunit.Var;
                        break;
                    case TYPE_VALUE:
                        Text = svunit.Value.ToString();
                        break;
                    default:
                        break;
                }
            });
        }

        public event RoutedEventHandler TextLegalChanged;

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            try
            {
                switch (type)
                {
                    case TYPE_NAME:
                        if (Regex.Match(Text, @"^\w+\d+$").Length == 0)
                            return;
                        //svunit.Name = Text;
                        break;
                    case TYPE_VAR:
                        svunit.Var = Text;
                        break;
                    case TYPE_VALUE:
                        this.Background = Brushes.Red;
                        switch (svunit.Type)
                        {
                            case "BIT":
                                if (Regex.Match(Text, @"^[01]$").Length == 0)
                                    return;
                                svunit.Value = int.Parse(Text);
                                break;
                            case "WORD":
                            case "DWORD":
                                if (Regex.Match(Text, @"^\d+$").Length == 0)
                                    return;
                                svunit.Value = int.Parse(Text);
                                break;
                            case "FLOAT":
                                if (Regex.Match(Text, @"\d+\.\d+").Length == 0)
                                    return;
                                svunit.Value = float.Parse(Text);
                                break;
                            case "DOUBLE":
                                if (Regex.Match(Text, @"\d+\.\d+").Length == 0)
                                    return;
                                svunit.Value = double.Parse(Text);
                                break;
                            default:
                                break;
                        }
                        this.Background = Brushes.White;
                        break;
                    default:
                        break;
                }
            }
            catch (FormatException)
            {
                return;
            }
            if (TextLegalChanged != null)
            {
                TextLegalChanged(this, new RoutedEventArgs());
            }
        }
    }
}
