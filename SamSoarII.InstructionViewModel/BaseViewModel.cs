﻿using System;
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
using SamSoarII.Utility;

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
    public abstract class BaseViewModel : UserControl, IPosition,IDisposable
    {
        public event ShowPropertyDialogHandler ShowPropertyDialogEvent;
        public abstract int X { get; set; }
        public abstract int Y { get; set; }
        public abstract IntPoint IntPos { get; set; }
        public abstract bool IsCommentMode { get; set; }
        public bool CanModify { get; set; } = true;
        public abstract string InstructionName { get; }
        public abstract BaseModel Model { get; protected set; }
        public List<BaseViewModel> NextElements = new List<BaseViewModel>();
        public List<BaseViewModel> SubElements = new List<BaseViewModel>();
        public bool IsSearched { get; set; }
        public bool IsPushed { get; set; }
        public virtual ElementType Type { get; }
        public virtual int NetWorkNum { get { return _netWorkNum; } set { _netWorkNum = value; } }
        private int _netWorkNum = -1;
        private string _refLadderName = string.Empty;
        public virtual string RefLadderName { get { return _refLadderName; } set { _refLadderName = value; } }
        public static NullViewModel Null { get { return _nullViewModel; } }
        private static NullViewModel _nullViewModel = new NullViewModel();
        public int BPAddress { get; set; }
        private BreakpointRect bprect;
        public BreakpointRect BPRect
        {
            get
            {
                return this.bprect;
            }
            set
            {
                BreakpointRect _bprect = bprect;
                this.bprect = value;
                if (_bprect != null)
                {
                    _bprect.BVModel = null;
                }
                if (bprect != null && bprect.BVModel != this)
                {
                    bprect.BVModel = this;
                }
                if (bprect != null)
                {
                    double cx = Canvas.GetLeft(this);
                    double cy = Canvas.GetTop(this);
                    Canvas.SetLeft(bprect, cx);
                    Canvas.SetTop(bprect, cy);
                }
            }
        }

        public BaseViewModel()
        {
            MouseDoubleClick += BaseViewModel_MouseDoubleClick;
        }

        private void BaseViewModel_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BeginShowPropertyDialog();
            e.Handled = true;
        }

        public virtual IPropertyDialog PreparePropertyDialog()
        {
            BasePropModel bpmodel = null;
            if (this is InputBaseViewModel)
                bpmodel = new InputPropModel();
            if (this is OutputBaseViewModel)
                bpmodel = new OutputPropModel();
            if (this is OutputRectBaseViewModel)
                bpmodel = new OutRecPropModel();
            if (bpmodel == null)
                return null;
            bpmodel.InstructionName = InstructionName;
            bpmodel.Count = Model.ParaCount;
            if (bpmodel.Count >= 1)
                bpmodel.ValueString1 = Model.GetPara(0).ValueString;
            if (bpmodel.Count >= 2)
                bpmodel.ValueString2 = Model.GetPara(1).ValueString;
            if (bpmodel.Count >= 3)
                bpmodel.ValueString3 = Model.GetPara(2).ValueString;
            if (bpmodel.Count >= 4)
                bpmodel.ValueString4 = Model.GetPara(3).ValueString;
            if (bpmodel.Count >= 5)
                bpmodel.ValueString5 = Model.GetPara(4).ValueString;
            ElementPropertyDialog_New epdialog = new ElementPropertyDialog_New();
            epdialog.BPModel = bpmodel;
            return epdialog;
        }

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
        public IMoniValueModel GetValueModel(int id)
        {
            return _values[id];
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

        public void Dispose()
        {
            for (int i = 0; i < 5; i++)
            {
                SetValueModel(i,null);
            }
            ViewCtrl = null;
            BPRect = null;
            //Model = null;
            MouseDoubleClick -= BaseViewModel_MouseDoubleClick;
            NextElements.Clear();
            SubElements.Clear();
        }
    }
}
