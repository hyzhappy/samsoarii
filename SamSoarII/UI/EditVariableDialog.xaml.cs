using SamSoarII.LadderInstViewModel;
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

        private IVariableValue _variable;

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

        public EditVariableDialog(IVariableValue variable)
        {
            InitializeComponent();
            this.DataContext = variable;
            _oldname = variable.VarName;
            _editMode = EditingMode.Modify;
            _variable = variable;
        }

        public void Commit()
        {
            if (VariableName == string.Empty)
            {
                throw new ValueParseException(string.Format("VariableName can not be empty!"));
            }
            else
            {
                if (_editMode == EditingMode.Modify)
                {
                    if (VariableName != _variable.VarName)
                    {
                        if (VariableManager.ContainVariable(VariableName))
                        {
                            throw new ValueParseException(string.Format("Variable {0} is exist", VariableName));
                        }
                    }
                    var variable = ValueParser.CreateVariableValue(VariableName, VariableValueString, VariableValueType, VariableComment);
                    VariableManager.ReplactVarialbe(_variable, variable);
                    InstructionCommentManager.UpdateCommentContent(variable.ValueString);
                }
                else
                {
                    if (VariableManager.ContainVariable(VariableName))
                    {
                        throw new ValueParseException(string.Format("Variable {0} is exist", VariableName));
                    }
                    var variable = ValueParser.CreateVariableValue(VariableName, VariableValueString, VariableValueType, VariableComment);
                    VariableManager.AddVariable(variable);
                    InstructionCommentManager.UpdateCommentContent(variable.ValueString);
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
