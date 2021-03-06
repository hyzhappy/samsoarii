﻿using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SamSoarII.LadderInstViewModel.Monitor
{
    public interface IMoniValueModel
    {
        string Value { get; }
        event RoutedEventHandler ValueChanged;
    }

    public interface IMoniViewCtrl
    {
        bool IsRunning { get; }
        event RoutedEventHandler Started;
        event RoutedEventHandler Aborted;
    }

    public abstract class MoniBaseViewModel : UserControl
    {
        protected BaseModel _model;
        protected int _x;
        protected int _y;
        protected IMoniViewCtrl _ctrl;
        protected IMoniValueModel[] _values = new IMoniValueModel[5];

        public virtual BaseModel Model
        {
            get { return this._model; }
            protected set
            {
                this._model = value;
                Update();
            }
        }
        public virtual int X
        {
            get
            {
                return this._x;
            }
            set
            {
                this._x = value;
                Canvas.SetLeft(this, X * 300);
            }
        }
        public virtual int Y
        {
            get
            {
                return this._y;
            }
            set
            {
                this._y = value;
                Canvas.SetTop(this, Y * 300);
            }
        }
        
        public virtual string Inst
        {
            get
            {
                return _model.InstructionName;
            }
        }

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
        
        public abstract void Update();

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
            Update();
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

        static public MoniBaseViewModel Create(BaseModel bmodel)
        {
            MoniBaseViewModel svbmodel = null;
            switch (bmodel.InstructionName)
            {
                case "LD":
                case "LDI":
                case "LDIM":
                case "LDIIM":
                case "LDP":
                case "LDF":
                case "LDWEQ":
                case "LDWNE":
                case "LDWLE":
                case "LDWGE":
                case "LDWL":
                case "LDWG":
                case "LDDEQ":
                case "LDDNE":
                case "LDDLE":
                case "LDDGE":
                case "LDDL":
                case "LDDG":
                case "LDFEQ":
                case "LDFNE":
                case "LDFLE":
                case "LDFGE":
                case "LDFL":
                case "LDFG":
                    svbmodel = new MoniInputViewModel(bmodel);
                    break;
                case "INV":
                case "MEP":
                case "MEF":
                    svbmodel = new MoniSpecViewModel(bmodel);
                    break;
                case "ALT":
                case "ALTP":
                case "OUT":
                case "OUTIM":
                case "RST":
                case "RSTIM":
                case "SET":
                case "SETIM":
                    svbmodel = new MoniOutBitViewModel(bmodel);
                    break;
                case "HLINE":
                    svbmodel = new MoniVLineViewModel();
                    break;
                case "VLINE":
                    svbmodel = new MoniHLineViewModel();
                    break;
                default:
                    svbmodel = new MoniOutRecViewModel(bmodel);
                    break;
            }
            return svbmodel;
        }
    }
}
