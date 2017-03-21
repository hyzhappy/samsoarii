using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using SamSoarII.Simulation.Core.VariableModel;

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

        public SimuViewBaseModel(SimulateModel parent)
        {
            this._parent = parent;
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
    }
}
