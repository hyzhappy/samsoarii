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

namespace SamSoarII.LadderInstViewModel
{
    /// <summary>
    /// BreakpointRect.xaml 的交互逻辑
    /// </summary>
    public partial class BreakpointRect : UserControl, IPosition,IDisposable
    {
        public BreakpointRect(string _label)
        {
            InitializeComponent();
            Label = _label;
        }

        public string Label
        {
            get
            {
                return TB_Label.Text;
            }
            set
            {
                TB_Label.Text = value;
            }
        }
        
        private BaseViewModel bvmodel;
        public BaseViewModel BVModel
        {
            get
            {
                return this.bvmodel;
            }
            set
            {
                BaseViewModel _bvmodel = bvmodel;
                this.bvmodel = value;
                if (_bvmodel != null)
                {
                    _bvmodel.BPRect = null;
                }
                if (bvmodel != null && bvmodel.BPRect != this)
                {
                    bvmodel.BPRect = this;
                }
                
            }
        }
        public int X
        {
            get
            {
                return bvmodel == null ? 0 : bvmodel.X;
            }
        }

        public int Y
        {
            get
            {
                return bvmodel == null ? 0 : bvmodel.Y;
            }
        }

        public void Dispose()
        {
            BVModel = null;
        }
    }
}
