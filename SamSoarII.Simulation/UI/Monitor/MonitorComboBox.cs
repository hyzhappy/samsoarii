using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;

using SamSoarII.Simulation.Core.VariableModel;
using System.Windows;
using System.Windows.Input;

namespace SamSoarII.Simulation.UI.Monitor
{
    public class MonitorComboBox : ComboBox
    {
        public const int TYPE_NAME = 0x01;
        public const int TYPE_DATATYPE = 0x02;
        public const int TYPE_VAR = 0x03;
        public const int TYPE_VALUE = 0x04;

        private int type;
        private SimulateVariableUnit svunit;
       
        public event RoutedEventHandler TextLegalChanged;

        public SimulateVariableUnit SVUnit
        {
            set
            {
                if (value == null)
                {
                    this.svunit = value;
                    Items.Clear();
                    return;
                }
                this.Dispatcher.Invoke(new Utility.Delegates.Execute(() =>
                {
                    this.svunit = value;
                    Items.Clear();
                    if (svunit is SimulateVInputUnit)
                    {
                        return;
                    }
                    if (svunit.Name[0] == 'D' && !(svunit is SimulateSpecialUnit))
                    {
                        Items.Add("WORD");
                        Items.Add("DWORD");
                        Items.Add("FLOAT");
                        if (svunit is SimulateUnitSeries)
                        {
                            SimulateUnitSeries ssunit = (SimulateUnitSeries)(svunit);
                            switch (ssunit.DataType)
                            {
                                case "BIT":
                                    Text = "BIT";
                                    break;
                                case "WORD":
                                    Text = "WORD";
                                    break;
                                case "DWORD":
                                    Text = "DWORD";
                                    break;
                                case "FLOAT":
                                    Text = "FLOAT";
                                    break;
                            }
                        }
                        else
                        {
                            Text = svunit.Type;
                        }
                    }
                    else
                    {
                        if (svunit is SimulateUnitSeries)
                        {
                            SimulateUnitSeries ssunit = (SimulateUnitSeries)(svunit);
                            Items.Add(ssunit.DataType);
                            Text = ssunit.DataType;
                        }
                        else
                        {
                            Items.Add(svunit.Type);
                            Text = svunit.Type;
                            switch (svunit.Name)
                            {
                                case "Y0": case "Y1": case "Y2": case "Y3":
                                    Items.Add("PULSE");
                                    break;
                            }
                        }
                    }
                }));
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

        #region KeyBoard Control

        /// <summary>
        /// 当焦点上移时，触发这个代理
        /// </summary>
        public event RoutedEventHandler FocusUp = delegate { };
        /// <summary>
        /// 当焦点下移时，触发这个代理
        /// </summary>
        public event RoutedEventHandler FocusDown = delegate { };
        /// <summary>
        /// 当焦点左移时，触发这个代理
        /// </summary>
        public event RoutedEventHandler FocusLeft = delegate { };
        /// <summary>
        /// 当焦点右移时，触发这个代理
        /// </summary>
        public event RoutedEventHandler FocusRight = delegate { };
        /// <summary>
        /// 当按下键盘时发生
        /// </summary>
        /// <param name="e">键盘事件</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key != Key.Left && e.Key != Key.Right)
            {
                base.OnPreviewKeyDown(e);
            }
            // 根据按键执行相应操作
            switch (e.Key)
            {
                // 键入Enter时打开多选框
                case Key.Enter:
                    break;
                // 键入Up时选择上移
                case Key.Up:
                    break;
                // 键入Down时选择下移
                case Key.Down:
                    break;
                // 键入Left时焦点左移
                case Key.Left:
                    FocusLeft(this, new RoutedEventArgs());
                    break;
                // 键入Right时焦点右移
                case Key.Right:
                    FocusRight(this, new RoutedEventArgs());
                    break;
            }
        }
        /// <summary>
        /// 当松开键盘时发生
        /// </summary>
        /// <param name="e">键盘事件</param>
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if (e.Key != Key.Left && e.Key != Key.Right)
            {
                base.OnPreviewKeyUp(e);
            }
        }

        #endregion
    }
}
