using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SamSoarII.LadderInstViewModel.Monitor
{
    public abstract class MoniBaseViewModel : UserControl
    {
        protected int _x;
        protected int _y;
        protected string _iname;
        protected string[] _labels = new string[5]
            {String.Empty,String.Empty,String.Empty,String.Empty,String.Empty,};
        protected string[] _values = new string[5]
            {String.Empty,String.Empty,String.Empty,String.Empty,String.Empty,};

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

        public abstract void Setup(string text);

        public abstract void Update();
        
        public virtual void SetValue(int id, string value)
        {
            _values[id] = value;
        }

        static public MoniBaseViewModel Create(string text)
        {
            string[] texts = text.Split(' ');
            string inst = texts[0];
            MoniBaseViewModel svbmodel = null;
            switch (inst)
            {
                case "LD":
                case "LDI":
                case "LDIM":
                case "LDIIM":
                case "LDP":
                case "LDF":
                    //svbmodel = new MoniInputViewModel(text);
                    break;
                case "ALT":
                case "ALTP":
                case "OUT":
                case "OUTIM":
                case "RST":
                case "RSTIM":
                case "SET":
                case "SETIM":
                    //svbmodel = new MoniOutBitViewModel(text);
                    break;
                case "HLINE":
                    break;
                case "VLINE":
                    break;
                default:
                    //svbmodel = new MoniOutRecViewModel(text);
                    break;
            }
            return svbmodel;
        }

    }
    
}
