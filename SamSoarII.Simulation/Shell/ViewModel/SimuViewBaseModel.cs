using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using SamSoarII.Simulation.Core.VariableModel;
using SamSoarII.UserInterface;
using SamSoarII.Simulation.Core.Event;

namespace SamSoarII.Simulation.Shell.ViewModel
{
    public abstract class SimuViewBaseModel : UserControl
    {
        protected SimulateModel _parent;
        protected int _x;
        protected int _y;
        protected string _iname;
        protected SimulateVariableUnit _args1;
        protected SimulateVariableUnit _args2;
        protected SimulateVariableUnit _args3;
        protected SimulateVariableUnit _args4;
        protected SimulateVariableUnit _args5;
        protected SimuArgsDialog dialog;

        public SimulateModel SimuParent
        {
            get { return this._parent; }
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
                return this._iname;
            }
            set
            {
                this._iname = value;
            }
        }

        public virtual SimulateVariableUnit this[int id]
        {
            get
            {
                switch (id)
                {
                    case 1: return this._args1;
                    case 2: return this._args2;
                    case 3: return this._args3;
                    case 4: return this._args4;
                    case 5: return this._args5;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (id)
                {
                    case 1:
                        if (this._args1 != null)
                        {
                            this._args1.ValueChanged -= OnValueChanged;
                            this._args1.LockChanged -= OnLockChanged;
                        }
                        this._args1 = value;
                        this._args1.ValueChanged += OnValueChanged;
                        this._args1.LockChanged += OnLockChanged;
                        break;
                    case 2:
                        if (this._args2 != null)
                        {
                            this._args2.ValueChanged -= OnValueChanged;
                            this._args2.LockChanged -= OnLockChanged;
                        }
                        this._args2 = value;
                        this._args2.ValueChanged += OnValueChanged;
                        this._args2.LockChanged += OnLockChanged;
                        break;
                    case 3:
                        if (this._args3 != null)
                        {
                            this._args3.ValueChanged -= OnValueChanged;
                            this._args3.LockChanged -= OnLockChanged;
                        }
                        this._args3 = value;
                        this._args3.ValueChanged += OnValueChanged;
                        this._args3.LockChanged += OnLockChanged;
                        break;
                    case 4:
                        if (this._args4 != null)
                        {
                            this._args4.ValueChanged -= OnValueChanged;
                            this._args4.LockChanged -= OnLockChanged;
                        }
                        this._args4 = value;
                        this._args4.ValueChanged += OnValueChanged;
                        this._args4.LockChanged += OnLockChanged;
                        break;
                    case 5:
                        if (this._args5 != null)
                        {
                            this._args5.ValueChanged -= OnValueChanged;
                            this._args5.LockChanged -= OnLockChanged;
                        }
                        this._args5 = value;
                        this._args5.ValueChanged += OnValueChanged;
                        this._args5.LockChanged += OnLockChanged;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
       
        public SimuViewBaseModel(SimulateModel parent)
        {
            this._parent = parent;
            _parent.BuildRouted(this);    
        }
        
        public abstract void Setup(string text);

        public abstract void Update();

        static public SimuViewBaseModel Create(SimulateModel parent, string text)
        {
            string[] texts = text.Split(' ');
            string inst = texts[0];
            SimuViewBaseModel svbmodel = null;

            switch (inst)
            {
                case "LD": case "LDI": case "LDIM": case "LDIIM": case "LDP": case "LDF":
                    svbmodel = new SimuViewInputModel(parent);
                    break;
                case "ALT": case "ALTP": case "OUT": case "OUTIM": case "RST": case "RSTIM": case "SET": case "SETIM":
                    svbmodel = new SimuViewOutBitModel(parent);
                    break;
                default:
                    svbmodel = new SimuViewOutRecModel(parent);
                    break;
            }

            svbmodel.Setup(text);
            return svbmodel;
        }

        protected void _SetDialogProperty(string[] labels, string[] values, string[] types)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = String.Format("{0:s}:{1:s}",
                    this[i + 1].Var.Length > 0 
                        ? this[i + 1].Var 
                        : this[i + 1].Name,
                    labels[i]);
                values[i] = (this[i + 1].Islocked || !this[i + 1].CanLock) 
                    ? this[i + 1].Value.ToString() 
                    : String.Empty;
                types[i] = (this[i + 1].CanLock)
                    ? this[i + 1].Type
                    : "READONLY";
            }
        }

        #region Event Handler
        protected virtual void OnValueChanged(object sender, RoutedEventArgs e)
        {
            Update();
        }

        protected virtual void OnLockChanged(object sender, RoutedEventArgs e)
        {
            Update();
        }

        public event VariableUnitChangeEventHandler VariableUnitLocked = delegate { };
        public event VariableUnitChangeEventHandler VariableUnitUnlocked = delegate { };
        public event VariableUnitChangeEventHandler VariableUnitValueChanged = delegate { };

        protected virtual void OnDialogEnsureClicked(object sender, SimuArgsDialogValuesArgs e)
        {
            if (sender == dialog)
            {
                VariableUnitChangeEventArgs _e = new VariableUnitChangeEventArgs();
                for (int i = 0; i < 5; i++)
                {
                    if (this[i + 1] != null)
                    {
                        if (!(e.Values[i] is SimuArgsDialogUnlockValue))
                        {
                            this[i + 1].Islocked = true;
                            this[i + 1].Value = e.Values[i];
                            _e.Old = _e.New = this[i + 1];
                            if (e.IsLocks[i])
                            {
                                VariableUnitLocked(this, _e);
                            }
                            else
                            {
                                VariableUnitValueChanged(this, _e);
                            }
                            Update();              
                        }
                        if (e.Values[i] is SimuArgsDialogUnlockValue && this[i + 1].Islocked)
                        {
                            this[i + 1].Islocked = false;
                            _e.Old = _e.New = this[i + 1];
                            VariableUnitUnlocked(this, _e);
                            Update();
                        }
                    }
                }
                dialog.Close();
                dialog = null;
                Update();
            }
        }

        protected virtual void OnDialogCancelClicked(object sender, SimuArgsDialogValuesArgs e)
        {
            if (sender == dialog)
            {
                dialog.Close();
                dialog = null;
            }
        }
        #endregion
    }
}
