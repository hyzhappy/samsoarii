using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SamSoarII.UserInterface;
using SamSoarII.LadderInstModel;
using SamSoarII.PLCDevice;
using SamSoarII.ValueModel;
using SamSoarII.LadderInstViewModel.Monitor;

namespace SamSoarII.LadderInstViewModel
{
    public class ShowPropertyDialogEventArgs : EventArgs
    {
        public IPropertyDialog Dialog { get; set; }

        public ShowPropertyDialogEventArgs(IPropertyDialog dialog)
        {
            Dialog = dialog;
        }
    }
    public delegate void ShowPropertyDialogHandler(BaseViewModel sender, ShowPropertyDialogEventArgs e);

    /// <summary>
    /// 梯形图元件基类，抽象类
    /// </summary>
    public abstract class BaseViewModel : UserControl
    {
        public event ShowPropertyDialogHandler ShowPropertyDialogEvent;
        public abstract int X { get; set; }
        public abstract int Y { get; set; }
        public abstract bool IsCommentMode { get; set; }
        public bool CanModify { get; set; }
        public abstract string InstructionName { get; }
        public abstract BaseModel Model { get; protected set; }
        public List<BaseViewModel> NextElements = new List<BaseViewModel>();
        public List<BaseViewModel> SubElements = new List<BaseViewModel>();
        public bool IsSearched { get; set; }
        public virtual ElementType Type { get; }
        public virtual int NetWorkNum { get { return _netWorkNum; } set { _netWorkNum = value; } }
        private int _netWorkNum = -1;
        private string _refLadderName = string.Empty;
        public virtual string RefLadderName { get { return _refLadderName; } set { _refLadderName = value; } }
        public static NullViewModel Null { get { return _nullViewModel; } }
        private static NullViewModel _nullViewModel = new NullViewModel();
        public BaseViewModel()
        {
            MouseDoubleClick += BaseViewModel_MouseDoubleClick;
        }

        private void BaseViewModel_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BeginShowPropertyDialog();
            e.Handled = true;
        }

        public abstract IPropertyDialog PreparePropertyDialog();

        public void BeginShowPropertyDialog()
        {
            if (!CanModify) return;
            var dialog = PreparePropertyDialog();
            if (dialog is ElementPropertyDialog)
            {
                ElementPropertyDialog epdialog = (ElementPropertyDialog)(dialog);
                epdialog.SavePropertyString();
            }
            if (dialog != null)
            {
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                if (ShowPropertyDialogEvent != null)
                {
                    ShowPropertyDialogEvent.Invoke(this, new ShowPropertyDialogEventArgs(dialog));
                }
            }
        }

        public virtual bool Assert()
        {
            return false;
        }

        public virtual void SetSelect(bool e)
        {

        }

        public abstract BaseViewModel Clone();

        public abstract int GetCatalogID();
        /// <summary>
        /// 不包含注释信息的value string, 用于打开工程时使用
        /// </summary>
        /// <param name="valueStrings"></param>
        public abstract void ParseValue(IList<string> valueStrings);
        /// <summary>
        /// 包含注释信息的value string, 一个value，接着一个注释
        /// </summary>
        /// <param name="valueStrings"></param>
        /// <param name="contextDevice"></param>
        public abstract void AcceptNewValues(IList<string> valueStrings, Device contextDevice);

        public abstract IEnumerable<string> GetValueString();
        public abstract IEnumerable<IValueModel> GetValueModels();
        public abstract void UpdateCommentContent();
        public override string ToString()
        {
            return String.Format("(R={0:s})(N={1:d})(X={2:d},Y={3:d})(I={4:s})",
                RefLadderName, NetWorkNum, X, Y, ToInstString());
            //return string.Format("InstructionName:{0}    RoutineName:{1}    NetworkNumber:{2}    X:{3}   Y:{4}", InstructionName,RefLadderName,NetWorkNum,X,Y);
        }
        public string ToInstString()
        {
            if (Model == null) return String.Empty;
            return Model.ToString();
        }

        #region Monitor

        public abstract bool IsMonitorMode { get; set; }

        protected IMoniViewCtrl _ctrl;
        public virtual IMoniViewCtrl ViewCtrl
        {
            get
            {
                return this._ctrl;
            }
            set
            {
                if (_ctrl != null)
                {
                    _ctrl.Started -= OnStart;
                    _ctrl.Aborted -= OnAbort;
                }
                this._ctrl = value;

                if (_ctrl != null)
                {
                    _ctrl.Started += OnStart;
                    _ctrl.Aborted += OnAbort;
                }
            }
        }
        public virtual bool IsRunning
        {
            get
            {
                return _ctrl != null ? _ctrl.IsRunning : false;
            }
        }

        protected IMoniValueModel[] _values = new IMoniValueModel[5];
        public void SetValueModel(int id, IMoniValueModel mvmodel)
        {
            if (_values[id] != null)
            {
                _values[id].ValueChanged -= OnValueChanged;
            }
            _values[id] = mvmodel;
            if (_values[id] != null)
            {
                _values[id].ValueChanged += OnValueChanged;
            }
        }

        protected virtual void OnAbort(object sender, RoutedEventArgs e)
        {

        }
        protected virtual void OnStart(object sender, RoutedEventArgs e)
        {

        }
        protected virtual void OnValueChanged(object sender, RoutedEventArgs e)
        {

        }

        #endregion

    }
}
