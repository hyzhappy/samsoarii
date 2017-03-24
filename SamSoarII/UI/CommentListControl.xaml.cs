using SamSoarII.AppMain.Project;
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
using SamSoarII.ValueModel;
using System.ComponentModel;
using SamSoarII.LadderInstViewModel;
using System.Collections.ObjectModel;

namespace SamSoarII.AppMain.UI
{
    public class ValueComment : INotifyPropertyChanged
    {
        private string _valueString;
        private string _comment;
        public string ValueString
        {
            get
            {
                return _valueString;
            }
            set
            {
                _valueString = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ValueString"));
            }
        }

        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Comment"));
            }
        }

        public ValueComment(string valueString, string comment)
        {
            ValueString = valueString;
            Comment = comment;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
    /// <summary>
    /// CommentControl.xaml 的交互逻辑
    /// </summary>
    public partial class CommentListControl : UserControl, ITabItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public string TabHeader
        {
            get
            {
                return "元件注释表";
            }
            set
            {

            }
        }

        private List<string> _comboboxContent = new List<string>() { "", "X", "Y", "M", "C", "T", "S", "D", "CV", "TV", "AI", "AO", "V", "Z"};
        public IEnumerable<string> ComboboxContent
        {
            get
            {
                return _comboboxContent;
            }
        }
        private string _textSearchHead = string.Empty;
        private string _comboSearchHead = string.Empty;

        private List<ValueComment> _commentCollection = new List<ValueComment>();

        public IEnumerable<ValueComment> CommentCollection
        {
            get
            {
                return _commentCollection.Where(x => x.ValueString.StartsWith(_comboSearchHead, StringComparison.CurrentCultureIgnoreCase)).Where(x => x.ValueString.StartsWith(_textSearchHead, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
        }

        public CommentListControl()
        {
            InitializeComponent();
            this.DataContext = this;
            ValueCommentManager.ValueCommentChanged += ValueCommentManager_ValueCommentChanged;
        }



        public void UpdateComments()
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CommentCollection"));
        }
        #region Event handler
        private void ValueCommentManager_ValueCommentChanged(ValueModel.ValueCommentChangedEventArgs e)
        {
            switch (e.Type)
            {
                case ValueModel.ValueChangedType.Add:
                    if (!ValueParser.IsVariablePattern(e.ValueString))
                    {
                        if (!_commentCollection.Exists(x => { return x.ValueString == e.ValueString; }))
                        {
                            _commentCollection.Add(new ValueComment(e.ValueString, e.Comment));
                        }
                    }
                    break;
                case ValueModel.ValueChangedType.Clear:
                    _commentCollection.Clear();
                    break;
                case ValueModel.ValueChangedType.Remove:
                    try
                    {
                        var com1 = _commentCollection.Where(x => x.ValueString == e.ValueString).First();
                        _commentCollection.Remove(com1);
                    }
                    catch
                    {

                    }
                    break;
                case ValueModel.ValueChangedType.Update:
                    try
                    {
                        var com2 = _commentCollection.Where(x => x.ValueString == e.ValueString).First();
                        com2.Comment = e.Comment;
                    }
                    catch
                    {

                    }
                    break;
            }
            UpdateComments();
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            _textSearchHead = ((TextBox)sender).Text;
            UpdateComments();
        }

        private void OnFilterTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            _comboSearchHead = combo.SelectedItem as string;
            UpdateComments();
        }

        private void OnAddComment(object sender, RoutedEventArgs e)
        {
        }

        private void ValueCommentDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column == CommentColumn)
            {
                ValueComment comment = e.Row.DataContext as ValueComment;
                var textBox = e.EditingElement as TextBox;
                if (comment != null && textBox != null)
                {
                    ValueCommentManager.UpdateComment(comment.ValueString, textBox.Text,false,true);
                    InstructionCommentManager.UpdateCommentContent(comment.ValueString);
                }
            }
        }
        #endregion


    }
}
