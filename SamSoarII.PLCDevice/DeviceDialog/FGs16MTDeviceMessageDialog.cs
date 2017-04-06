using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCDevice.DeviceDialog
{
    class FGs16MTDeviceMessageDialog : BaseDeviceMessageDialog
    {
        private FGs16MTDevice _FGs16MTDevice = new FGs16MTDevice();
        public FGs16MTDeviceMessageDialog()
        {
            InitializeComponent();
            X_block.Content = string.Format("X: {0}-{1}", _FGs16MTDevice.XRange.Start, _FGs16MTDevice.XRange.End - 1);
            Y_block.Content = string.Format("Y: {0}-{1}", _FGs16MTDevice.YRange.Start, _FGs16MTDevice.YRange.End - 1);
            M_block.Content = string.Format("M: {0}-{1}", _FGs16MTDevice.MRange.Start, _FGs16MTDevice.MRange.End - 1);
            S_block.Content = string.Format("S: {0}-{1}", _FGs16MTDevice.SRange.Start, _FGs16MTDevice.SRange.End - 1);
            T_block.Content = string.Format("T: {0}-{1}", _FGs16MTDevice.TRange.Start, _FGs16MTDevice.TRange.End - 1);
            C_block.Content = string.Format("C: {0}-{1}", _FGs16MTDevice.CRange.Start, _FGs16MTDevice.CRange.End - 1);
            D_block.Content = string.Format("D: {0}-{1}", _FGs16MTDevice.DRange.Start, _FGs16MTDevice.DRange.End - 1);
            TV_block.Content = string.Format("TV: {0}-{1}", _FGs16MTDevice.TVRange.Start, _FGs16MTDevice.TVRange.End - 1);
            CV_block.Content = string.Format("CV: {0}-{1}", _FGs16MTDevice.CVRange.Start, _FGs16MTDevice.CVRange.End - 1);
        }
    }
}
