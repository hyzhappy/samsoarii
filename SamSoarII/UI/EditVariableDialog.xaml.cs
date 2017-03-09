using SamSoarII.ValueModel;
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
using System.Windows.Shapes;

namespace SamSoarII.AppMain.UI
{


    /// <summary>
    /// EditVariableDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditVariableDialog : Window
    {
        enum EditingMode
        {
            Modify,
            Add,
        }

        private EditingMode _editMode;

        private string _oldname;

        public event RoutedEventHandler EnsureButtonClick = delegate { };

        public string VariableName { get { return VariableNameTextBox.Text; } }

        public string VariableValueString { get { return VariableValueTextBox.Text; } }

        public LadderValueType VariableValueType
        {
            get
            {
                return (LadderValueType)VariableValueTypeCombobox.SelectedIndex;
            }
        }

        public string VariableComment { get { return VariableCommentTextBox.Text; } }

        public EditVariableDialog()
        {
            InitializeComponent();
            _editMode = EditingMode.Add;
        }

        public EditVariableDialog(LadderVariable variable)
        {
            InitializeComponent();
            this.DataContext = variable;
            _oldname = variable.Name;
            _editMode = EditingMode.Modify;
        }

        public void Commit()
        {
            if(_editMode == EditingMode.Modify)
            {
                if(_oldname == VariableName)
                {
                    try
                    {
                        var newvalue = ValueParser.ParseValue(VariableValueString, VariableValueType);
                        GlobalVariableList.ModifyVariable(VariableName, newvalue, VariableComment);
                    }
                        catch
                    {
                        throw new ArgumentException("非法的输入值");
                    }
                }
                else
                {
                    if (GlobalVariableList.ContainVariable(VariableName))
                    {
                        throw new ArgumentException("已经存在同名变量");
                    }
                    try
                    {
                        var newvalue = ValueParser.ParseValue(VariableValueString, VariableValueType);
                        GlobalVariableList.ModifyVariable(_oldname, VariableName, newvalue, VariableComment);
                    }
                    catch
                    {
                        throw new ArgumentException("非法的输入值");
                    }
                }

            }
            else
            {
                if (GlobalVariableList.ContainVariable(VariableName))
                {
                    throw new ArgumentException("已经存在同名变量");
                }
                try
                {
                    var newvalue = ValueParser.ParseValue(VariableValueString, VariableValueType);
                    GlobalVariableList.AddVariable(VariableName, newvalue, VariableComment);
                }
                catch
                {
                    throw new ArgumentException("非法的输入值");
                }
            }
        }


        #region Event handler
        private void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            EnsureButtonClick.Invoke(sender, e);
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                this.Close();
            }
            if(e.Key == Key.Enter)
            {
                EnsureButtonClick.Invoke(sender, e);
            }
        }
        #endregion


    }
}
