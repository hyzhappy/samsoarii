using SamSoarII.Core.Models;
using SamSoarII.Global;
using System;
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

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// ElementPropertyDialog_New.xaml 的交互逻辑
    /// </summary>
    public partial class ElementPropertyDialog : Window, INotifyPropertyChanged, IDisposable
    {
        public ElementPropertyDialog(LadderUnitModel _core)
        {
            InitializeComponent();
            DataContext = this;
            KeyDown += ElementPropertyDialog_New_KeyDown;

            core = _core;
            ProjectModel project = _core.Parent.Parent.Parent;
            Details = GlobalSetting.InstrutionNameAndToolTips[Core.InstName];
            switch (Core.InstName)
            {
                case "CALL":
                case "ATCH":
                    SubRoutines = project.Diagrams.Select(diagram => diagram.Name);
                    break;
                case "CALLM":
                    Functions = project.Funcs.Where(f => f.CanCALLM()).Select(f => f.GetMessageList());
                    break;
                case "MBUS":
                    ModbusTables = project.Modbus.Children.Select(modbus => modbus.Name);
                    break;
            }
            switch (core.Shape)
            {
                case LadderUnitModel.Shapes.Input: BPModel = new InputPropModel(core); break;
                case LadderUnitModel.Shapes.Output: BPModel = new OutputPropModel(core); break;
                case LadderUnitModel.Shapes.OutputRect: BPModel = new OutRecPropModel(core); break;
            }
        }

        public void Dispose()
        {
            core = null;
            BPModel = null;
        }

        #region Core

        private LadderUnitModel core;
        public LadderUnitModel Core { get { return this.core; } }
        
        public IList<string> PropertyStrings
        {
            get
            {
                List<string> result = new List<string>();
                for (int i = 0; i < bpmodel.Count; i++)
                {
                    string value = bpmodel.GetValueString(i);
                    if (value.Length == 0)
                    {
                        result.Add("???");
                        result.Add("");
                    }
                    else
                    {
                        result.Add(bpmodel.GetValueString(i));
                        result.Add(bpmodel.GetCommentString(i));
                    }
                }
                return result;
            }
        }

        #endregion

        #region Shell

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
                    bpmodel.PropertyChanged -= OnPropertyChanged;
                    bpmodel.Loaded -= OnPropModelLoaded;
                    GD_Main.Children.Remove(bpmodel);
                    if (bpmodel is OutRecPropModel)
                    {
                        OutRecPropModel orpmodel = (OutRecPropModel)bpmodel;
                        orpmodel.CollectionPopup -= OnShowCollectionPopup;
                    }
                }
                this.bpmodel = value;
                if (bpmodel != null)
                {
                    bpmodel.PropertyChanged += OnPropertyChanged;
                    bpmodel.Loaded += OnPropModelLoaded;
                    GD_Main.Children.Add(bpmodel);
                    if (bpmodel is OutRecPropModel)
                    {
                        OutRecPropModel orpmodel = (OutRecPropModel)bpmodel;
                        orpmodel.CollectionPopup += OnShowCollectionPopup;
                    }
                    if (bpmodel.IsLoaded) OnPropModelLoaded(bpmodel, new RoutedEventArgs());
                }
            }
        }

        private void OnPropModelLoaded(object sender, RoutedEventArgs e)
        {
            bpmodel.SelectedIndex = 0;
        }


        #endregion

        #region Event handler

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bpmodel.SelectedIndex = 0;
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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == bpmodel)
            {
                switch (e.PropertyName)
                {
                    case "SelectedIndex": ReadDetail(); ReadComment(); break;
                    case "SelectedComment": ReadComment(); break;
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
            if (e.Type == CollectionPopupType.FUNCBLOCKS)
            {
                IEnumerable<string[]> fit = Functions.Where(
                    (string[] _msg) => { return bpmodel.GetValueString(0).Equals(_msg[1]); });
                if (fit.Count() > 0)
                {
                    string[] msg = fit.First();
                    OutRecPropModel orpmodel = (OutRecPropModel)bpmodel;
                    orpmodel.Count = (msg.Length - 1) / 2;
                    for (int i = 1; i < orpmodel.Count; i++)
                    {
                        orpmodel.SetPropertyLabel(i, msg[i * 2 + 2]);
                        orpmodel.SetPropertyText(i, "");
                    }
                }
            }
            if (PopupType == CollectionPopupType.FREE)
            {
                e.TB_Value.TextChanged += OnPopupTextBoxTextChanged;
                e.TB_Value.LostFocus += OnPopupTextBoxLostFocus;
                PU_Collection.PlacementTarget = e.TB_Value;
                PopupType = e.Type;
            }
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
            get { return this.details; }
            set { this.details = value; }
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
        
        #region Comment & Detail
        
        private void TB_Comment_TextChanged(object sender, TextChangedEventArgs e)
        {
            WriteComment();
        }

        private void ReadComment()
        {
            if (bpmodel == null || TB_Comment == null) return;
            TB_Comment.Text = bpmodel.SelectedComment;
        }

        private void WriteComment()
        {
            if (bpmodel == null || TB_Comment == null) return;
            bpmodel.SelectedComment = TB_Comment.Text;
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
                     (string[] _msg) => { return _msg[1].Equals(bpmodel.GetValueString(0)); });
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
