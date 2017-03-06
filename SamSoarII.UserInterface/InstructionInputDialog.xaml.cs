using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SamSoarII.UserInterface
{
    /// <summary>
    /// InstructionInputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InstructionInputDialog : Window
    {
        public event RoutedEventHandler EnsureButtonClick = delegate { };
        private System.Windows.Forms.AutoCompleteStringCollection instructionNames = new System.Windows.Forms.AutoCompleteStringCollection();
        private System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();
        private Dictionary<string, string> instructionNameAndToolTips;
        private string oldText;
        public string InstructionInput
        {
            get
            {
                return InstructionInputTextBox.Text;
            }
            set
            {
                InstructionInputTextBox.Text = value;
            }
        }

        public InstructionInputDialog(string initialString, Dictionary<string, string> instructionNameAndToolTips)
        {
            InitializeComponent();
            InstructionInput = initialString;
            Font font = new Font("Arial", 12);
            InstructionInputTextBox.Font = font;
            Loaded += (sender, e) =>
            {
                InstructionInputTextBox.Focus();
                InstructionInputTextBox.Select(InstructionInputTextBox.Text.Length, 0);
                this.instructionNameAndToolTips = instructionNameAndToolTips;
                instructionNames.AddRange(this.instructionNameAndToolTips.Keys.ToArray());
                InstructionInputTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
                InstructionInputTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
                InstructionInputTextBox.AutoCompleteCustomSource = instructionNames;
            };
            toolTip.OwnerDraw = true;
            toolTip.IsBalloon = false;
            toolTip.Active = false;
            toolTip.InitialDelay = 1000;
            toolTip.ForeColor = System.Drawing.Color.Black;
            toolTip.BackColor = ColorTranslator.FromHtml("#FFFFF0");
            toolTip.Draw += ToolTip_Draw;
            toolTip.Popup += ToolTip_Popup;
        }
        private void ToolTip_Popup(object sender, System.Windows.Forms.PopupEventArgs e)
        {
            e.ToolTipSize = System.Windows.Forms.TextRenderer.MeasureText(string.Format("HELLO    WORLD\nHELLO    WORLD"), new Font("Arial", 20.0f));
        }

        private void ToolTip_Draw(object sender, System.Windows.Forms.DrawToolTipEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(toolTip.BackColor), e.Graphics.ClipBounds);
            e.Graphics.DrawLines(SystemPens.ActiveBorder, new System.Drawing.Point[] { new System.Drawing.Point(0, e.Bounds.Height - 1), new System.Drawing.Point(0, 0), new System.Drawing.Point(e.Bounds.Width - 1, 0), new System.Drawing.Point(e.Bounds.Width - 1, e.Bounds.Height - 1) });
            //e.Graphics.DrawLines(SystemPens.ControlDarkDark, new System.Drawing.Point[] { new System.Drawing.Point(0, e.Bounds.Height - 1), new System.Drawing.Point(e.Bounds.Width - 1, e.Bounds.Height - 1), new System.Drawing.Point(e.Bounds.Width - 1, 0), new System.Drawing.Point(0, 0) });
            System.Windows.Forms.TextFormatFlags sf = System.Windows.Forms.TextFormatFlags.Default | System.Windows.Forms.TextFormatFlags.LeftAndRightPadding;
            e.DrawText(sf);
        }
        public void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            toolTip.Dispose();
            EnsureButtonClick.Invoke(sender, e);
        }


        private void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                this.Close();
            }
            if(e.Key == Key.Enter)
            {
                EnsureButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
        private bool CheckInstructionName(string InstructionName)
        {
            return instructionNames.Contains(InstructionName);
        }
        private void OnInstructionTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            toolTip.Dispose();
        }
        private void ShowToolTip(string instructionTooltip)
        {
            toolTip.Active = true;
            toolTip.Show(instructionTooltip, InstructionInputTextBox, 200, -210);
        }
        private void OnLocationChanged(object sender, EventArgs e)
        {
            if (toolTip.Active)
            {
                toolTip.Active = false;
                toolTip.Hide(InstructionInputTextBox);
                string instructionToolTip;
                if (instructionNameAndToolTips.TryGetValue(oldText, out instructionToolTip))
                {
                    ShowToolTip(instructionToolTip);
                }
            }
        }

        private void OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Space)
            {
                string instructionName = InstructionInputTextBox.Text.Trim().ToUpper();
                if (CheckInstructionName(instructionName))
                {
                    oldText = instructionName;
                    string instructionToolTip;
                    if (instructionNameAndToolTips.TryGetValue(instructionName, out instructionToolTip))
                    {
                        ShowToolTip(instructionToolTip);
                    }
                }
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Delete)
            {
                if (toolTip.Active)
                {
                    string instructionName = InstructionInputTextBox.Text.Trim().ToUpper();
                    if (!instructionName.Contains(oldText))
                    {
                        toolTip.Active = false;
                        toolTip.Hide(InstructionInputTextBox);
                    }
                }
            }
        }
    }
}
