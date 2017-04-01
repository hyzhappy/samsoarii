using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SamSoarII.Simulation.UI.Base
{
    public class SimuViewTabModel : UserControl
    {
        protected double _actualWidth;
        protected double _actualHeight;
        
        public new double ActualWidth
        {
            get
            {
                return this._actualWidth;
            }
            set
            {
                this._actualWidth = value;
                OnActualWidthChanged();
            }
        }

        public new double ActualHeight
        {
            get
            {
                return this._actualHeight;
            }
            set
            {
                this._actualHeight = value;
                OnActualHeightChanged();
            }
        }

        protected virtual void OnActualWidthChanged()
        {

        }
        protected virtual void OnActualHeightChanged()
        {

        }
        
    }
}
