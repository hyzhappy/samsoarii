using SamSoarII.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;


namespace SamSoarII.AppMain.UI
{
    public class FuncBlockTabItem : BaseTabItem
    {
        private System.Windows.Forms.Integration.WindowsFormsHost _windowsFormsHost = new System.Windows.Forms.Integration.WindowsFormsHost();
        private System.Windows.Forms.RichTextBox _richTextBox = new System.Windows.Forms.RichTextBox();
        private FuncBlockModel _funcBlockModel;
        public override string TabName { get { return Header.ToString(); } }

        public FuncBlockTabItem(FuncBlockModel fbmodel)
        {
            MinWidth = 100;
            _funcBlockModel = fbmodel;
            BindingOperations.SetBinding(this, HeaderProperty, new Binding() { Source = _funcBlockModel, Path = new System.Windows.PropertyPath("Name"), Mode = BindingMode.OneWay });
            _richTextBox.Font = new System.Drawing.Font("Consolas", 18);
            Content = _windowsFormsHost;
            _windowsFormsHost.Child = _richTextBox;
            _richTextBox.AcceptsTab = true;
            _richTextBox.Text = fbmodel.Code;
            _richTextBox.TextChanged += _richTextBox_TextChanged;
            //_richTextBox.KeyDown += _richTextBox_KeyDown;
        }

        private void _richTextBox_TextChanged(object sender, EventArgs e)
        {
            _funcBlockModel.Code = _richTextBox.Text;
        }

        public override void ClearElements()
        {
            // Nothing to do
        }

        public override void OnItemSelected()
        {
            _richTextBox.Focus();
        }
    }
}
