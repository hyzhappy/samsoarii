﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// ElementPropertyDialog_New.xaml 的交互逻辑
    /// </summary>
    public partial class ElementPropertyDialog_New : Window, IPropertyDialog, INotifyPropertyChanged
    {
        public ElementPropertyDialog_New()
        {
            InitializeComponent();
            DataContext = this;
            KeyDown += ElementPropertyDialog_New_KeyDown;
        }
        
        private void ElementPropertyDialog_New_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EnsureButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else
            {
                if (e.Key == Key.Escape)
                {
                    CancelButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
        }

        private BasePropModel bpmodel;
        public BasePropModel BPModel
        {
            get
            {
                return this.bpmodel;
            }
            set
            {
                if (bpmodel != null)
                {
                    GD_Main.Children.Remove(bpmodel);
                }
                this.bpmodel = value;
                bpmodel.Dialog = this;
                bpmodel.PropertyChanged += OnPropertyChanged;
                if (bpmodel is OutRecPropModel)
                {
                    OutRecPropModel orpmodel = (OutRecPropModel)bpmodel;
                    orpmodel.CollectionPopup += OnShowCollectionPopup;
                }
                if (bpmodel != null)
                {
                    Grid.SetRow(bpmodel, 0);
                    Grid.SetColumn(bpmodel, 0);
                    GD_Main.Children.Add(bpmodel);
                }
                
            }
        }
        
        #region Event handler
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == bpmodel)
            {
                switch (e.PropertyName)
                {
                    case "SelectedIndex":
                        ReadComment();
                        ReadDetail();
                        break;
                    case "CommentString1":
                    case "CommentString2":
                    case "CommentString3":
                    case "CommentString4":
                    case "CommentString5":
                        ReadComment();
                        break;
                }
            }
        }
        public RoutedEventHandler Ensure = delegate { };
        private void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            Ensure(this, new RoutedEventArgs());
            //Close();
        }

        public RoutedEventHandler Cancel = delegate { };
        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Cancel(this, new RoutedEventArgs());
            Close();
        }

        public event RoutedEventHandler Commit = delegate { };
        
        #endregion

        #region Collection

        private CollectionPopupType popuptype;
        public CollectionPopupType PopupType
        {
            get
            {
                return this.popuptype;
            }
            set
            {
                this.popuptype = value;
                if (popuptype == CollectionPopupType.FREE
                 && PU_Collection.PlacementTarget is TextBox)
                {
                    TextBox tbox = (TextBox)(PU_Collection.PlacementTarget);
                    tbox.TextChanged -= OnPopupTextBoxTextChanged;
                    tbox.LostFocus -= OnPopupTextBoxLostFocus;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("CollectionSource"));
            }
        }

        public IEnumerable<Label> CollectionSource
        {
            get
            {
                TextBox tbox = null;
                if (PU_Collection.PlacementTarget is TextBox)
                    tbox = (TextBox)(PU_Collection.PlacementTarget);
                if (tbox == null)
                    return new List<Label>();
                switch (PopupType)
                {
                    case CollectionPopupType.FREE:
                        return new List<Label>();
                    case CollectionPopupType.SUBROUTINES:
                        return SubRoutineLabels.Where((Label label) =>
                        {
                            return label.Content.ToString().StartsWith(tbox.Text)
                                && !label.Content.ToString().Equals(tbox.Text);
                        });
                    case CollectionPopupType.FUNCBLOCKS:
                        return FunctionLabels.Where((Label label) =>
                        {
                            return label.Content.ToString().StartsWith(tbox.Text)
                                && !label.Content.ToString().Equals(tbox.Text);
                        });
                    case CollectionPopupType.MODBUSES:
                        return ModbusLabels.Where((Label label) =>
                        {
                            return label.Content.ToString().StartsWith(tbox.Text)
                                && !label.Content.ToString().Equals(tbox.Text);
                        });
                    default:
                        return new List<Label>();
                }
            }
        }
        
        private void OnShowCollectionPopup(object sender, CollectionPopupEventArgs e)
        {
            if (PopupType != CollectionPopupType.FREE)
                return;
            e.TB_Value.TextChanged += OnPopupTextBoxTextChanged;
            e.TB_Value.LostFocus += OnPopupTextBoxLostFocus;
            PU_Collection.PlacementTarget = e.TB_Value;
            PopupType = e.Type;
        }

        private void OnPopupTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            PopupType = CollectionPopupType.FREE;
        }

        private void OnPopupTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            PropertyChanged(this, new PropertyChangedEventArgs("CollectionSource"));
        }

        private void DrawFree(Label label)
        {
            label.Background = Brushes.White;
            label.Foreground = Brushes.Black;
            label.FontWeight = FontWeights.Normal;
        }

        private void DrawCursor(Label label)
        {
            label.Background = Brushes.Black;
            label.Foreground = Brushes.White;
            label.FontWeight = FontWeights.Heavy;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Label)
            {
                Label label = sender as Label;
                DrawCursor(label);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Label)
            {
                Label label = sender as Label;
                DrawFree(label);
            }
        }

        private void OnLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label)
            {
                Label label = sender as Label;
                if (label.Content is string
                 && PU_Collection.PlacementTarget is TextBox)
                {
                    TextBox tbox = (TextBox)(PU_Collection.PlacementTarget);
                    string text = (string)(label.Content);
                    tbox.Text = text;
                    PopupType = CollectionPopupType.FREE;
                }
            }
        }

        #endregion

        #region Extra Message

        private IList<string> details;
        public IList<string> Details
        {
            get
            {
                return this.details;
            }
            set
            {
                this.details = value;
                if (bpmodel != null)
                {
                    bpmodel.SelectedIndex = 0;
                }
            }
        }

        private Label _StringToLabel(string text)
        {
            Label result = new Label();
            result.Content = text;
            return result;
        }

        private IEnumerable<string> subroutines = new List<string>();
        private IEnumerable<Label> subroutinelabels = new List<Label>();
        public IEnumerable<string> SubRoutines
        {
            get { return this.subroutines; }
            set
            {
                this.subroutines = value;
                subroutinelabels = value.Select(_StringToLabel);
            }
        }
        public IEnumerable<Label> SubRoutineLabels
        {
            get { return this.subroutinelabels; }
        }

        private IEnumerable<string[]> functions = new List<string[]>();
        private IEnumerable<Label> functionlabels = new List<Label>();
        public IEnumerable<string[]> Functions
        {
            get { return this.functions; }
            set
            {
                this.functions = value;
                functionlabels = value.Select(msgs => { return _StringToLabel(msgs[1]); });
                if (bpmodel is OutRecPropModel)
                    ((OutRecPropModel)bpmodel).UpdateCALLM();
            }
        }
        public IEnumerable<Label> FunctionLabels
        {
            get { return this.functionlabels; }
        }

        private IEnumerable<string> modbuss = new List<string>();
        private IEnumerable<Label> modbuslabels = new List<Label>();
        public IEnumerable<string> ModbusTables
        {
            get { return this.modbuss; }
            set
            {
                this.modbuss = value;
                this.modbuslabels = value.Select(_StringToLabel);
            }
        }
        public IEnumerable<Label> ModbusLabels
        {
            get { return this.modbuslabels; }
        }

        #endregion
        
        public IList<string> PropertyStrings
        {
            get
            {
                List<string> result = new List<string>();
                if (bpmodel.InstructionName.Equals("CALLM"))
                {
                    IEnumerable<string[]> fit = Functions.Where(
                        (string[] msgs) => { return msgs[1].Equals(bpmodel.ValueString1); });
                    if (fit.Count() > 0)
                    {
                        string[] msg = fit.First();
                        result.Add(bpmodel.ValueString1);
                        result.Add(msg[2]);
                        if (bpmodel.Count >= 2)
                        {
                            result.Add(msg[3]);
                            result.Add(msg[4]);
                            result.Add(bpmodel.ValueString2);
                            result.Add(bpmodel.CommentString2);
                        }
                        if (bpmodel.Count >= 3)
                        {
                            result.Add(msg[5]);
                            result.Add(msg[6]);
                            result.Add(bpmodel.ValueString3);
                            result.Add(bpmodel.CommentString3);
                        }
                        if (bpmodel.Count >= 4)
                        {
                            result.Add(msg[7]);
                            result.Add(msg[8]);
                            result.Add(bpmodel.ValueString4);
                            result.Add(bpmodel.CommentString4);
                        }
                        if (bpmodel.Count >= 5)
                        {
                            result.Add(msg[9]);
                            result.Add(msg[10]);
                            result.Add(bpmodel.ValueString5);
                            result.Add(bpmodel.CommentString5);
                        }
                    }
                }
                else
                {
                    if (bpmodel.Count >= 1)
                    {
                        result.Add(bpmodel.ValueString1);
                        result.Add(bpmodel.CommentString1);
                    }
                    if (bpmodel.Count >= 2)
                    {
                        result.Add(bpmodel.ValueString2);
                        result.Add(bpmodel.CommentString2);
                    }
                    if (bpmodel.Count >= 3)
                    {
                        result.Add(bpmodel.ValueString3);
                        result.Add(bpmodel.CommentString3);
                    }
                    if (bpmodel.Count >= 4)
                    {
                        result.Add(bpmodel.ValueString4);
                        result.Add(bpmodel.CommentString4);
                    }
                    if (bpmodel.Count >= 5)
                    {
                        result.Add(bpmodel.ValueString5);
                        result.Add(bpmodel.CommentString5);
                    }
                }
                return result;
            }
        }

        void IPropertyDialog.ShowDialog()
        {
            ShowDialog();
        }

        void IPropertyDialog.Close()
        {
            Close();
        }

        #region Comment & Detail
        
        private void TB_Comment_TextChanged(object sender, TextChangedEventArgs e)
        {
            WriteComment();
        }

        private void ReadComment()
        {
            if (bpmodel == null) return;
            if (bpmodel.InstructionName.Equals("CALLM")
             && bpmodel.SelectedIndex == 0)
            {
                IEnumerable<string[]> fit = Functions.Where(
                     (string[] _msg) => { return _msg[1].Equals(bpmodel.ValueString1); });
                if (fit.Count() > 0)
                {
                    TB_Comment.Text = fit.First()[2];
                    TB_Comment.IsReadOnly = true;
                }
            }
            else
            {
                switch (bpmodel.SelectedIndex)
                {
                    case 0:
                        TB_Comment.Text = bpmodel.CommentString1;
                        TB_Comment.IsReadOnly = false;
                        break;
                    case 1:
                        TB_Comment.Text = bpmodel.CommentString2;
                        TB_Comment.IsReadOnly = false;
                        break;
                    case 2:
                        TB_Comment.Text = bpmodel.CommentString3;
                        TB_Comment.IsReadOnly = false;
                        break;
                    case 3:
                        TB_Comment.Text = bpmodel.CommentString4;
                        TB_Comment.IsReadOnly = false;
                        break;
                    case 4:
                        TB_Comment.Text = bpmodel.CommentString5;
                        TB_Comment.IsReadOnly = false;
                        break;
                    default:
                        TB_Comment.Text = String.Empty;
                        TB_Comment.IsReadOnly = true;
                        break;
                }
            }
        }

        private void WriteComment()
        {
            if (bpmodel == null) return;
            switch (bpmodel.SelectedIndex)
            {
                case 0: bpmodel.CommentString1 = TB_Comment.Text; break;
                case 1: bpmodel.CommentString2 = TB_Comment.Text; break;
                case 2: bpmodel.CommentString3 = TB_Comment.Text; break;
                case 3: bpmodel.CommentString4 = TB_Comment.Text; break;
                case 4: bpmodel.CommentString5 = TB_Comment.Text; break;
            }
        }
        
        private void ReadDetail()
        {
            if (bpmodel == null) return;
            if (bpmodel.InstructionName.Equals("CALLM"))
            {
                if (bpmodel.SelectedIndex == 0)
                {
                    TB_Detail.Text = String.Empty;
                }
                IEnumerable<string[]> fit = Functions.Where(
                     (string[] _msg) => { return _msg[1].Equals(bpmodel.ValueString1); });
                if (fit.Count() > 0 && bpmodel.SelectedIndex > 0)
                {
                    string[] msg = fit.First();
                    string argtype = msg[bpmodel.SelectedIndex * 2 + 1];
                    string argname = msg[bpmodel.SelectedIndex * 2 + 2];
                    switch (argtype)
                    {
                        case "BIT*":
                            TB_Detail.Text = String.Format("[位]{0:s}(X/Y/M/C/T/S)", argname);
                            break;
                        case "WORD*":
                            TB_Detail.Text = String.Format("[单字]{0:s}(D/CV/TV)", argname);
                            break;
                        case "DWORD*":
                            TB_Detail.Text = String.Format("[双字]{0:s}(D)", argname);
                            break;
                        case "FLOAT*":
                            TB_Detail.Text = String.Format("[浮点]{0:s}(D)", argname);
                            break;
                        default:
                            TB_Detail.Text = String.Format("[{1:s}]{0:s}", argname, argtype);
                            break;
                    }
                }
            }
            else
            {
                TB_Detail.Text = Details[bpmodel.SelectedIndex + 1];
            }
        }

        #endregion
    }

    public enum CollectionPopupType
    {
        FREE, SUBROUTINES, FUNCBLOCKS, MODBUSES
    }

    public class CollectionPopupEventArgs : EventArgs
    {
        public CollectionPopupType Type { get; private set; }
        public TextBox TB_Value { get; private set; }
        public CollectionPopupEventArgs(CollectionPopupType _type, TextBox _textbox)
        {
            Type = _type;
            TB_Value = _textbox;
        }
    }

    public delegate void CollectionPopupEventHandler(object sender, CollectionPopupEventArgs e);
}
