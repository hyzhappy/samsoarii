using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCDevice.DeviceDialog
{
    class FGs16MRDeviceMessageDialog : BaseDeviceMessageDialog
    {
        private FGs16MRDevice _FGs16MRDevice = new FGs16MRDevice();
        public FGs16MRDeviceMessageDialog()
        {
            InitializeComponent();
            X_block.Content = string.Format("X: {0}-{1}", _FGs16MRDevice.XRange.Start, _FGs16MRDevice.XRange.End - 1);
            Y_block.Content = string.Format("Y: {0}-{1}", _FGs16MRDevice.YRange.Start, _FGs16MRDevice.YRange.End - 1);
            M_block.Content = string.Format("M: {0}-{1}", _FGs16MRDevice.MRange.Start, _FGs16MRDevice.MRange.End - 1);
            S_block.Content = string.Format("S: {0}-{1}", _FGs16MRDevice.SRange.Start, _FGs16MRDevice.SRange.End - 1);
            T_block.Content = string.Format("T: {0}-{1}", _FGs16MRDevice.TRange.Start, _FGs16MRDevice.TRange.End - 1);
            C_block.Content = string.Format("C: {0}-{1}", _FGs16MRDevice.CRange.Start, _FGs16MRDevice.CRange.End - 1);
            D_block.Content = string.Format("D: {0}-{1}", _FGs16MRDevice.DRange.Start, _FGs16MRDevice.DRange.End - 1);
            TV_block.Content = string.Format("TV: {0}-{1}", _FGs16MRDevice.TVRange.Start, _FGs16MRDevice.TVRange.End - 1);
            CV_block.Content = string.Format("CV: {0}-{1}", _FGs16MRDevice.CVRange.Start, _FGs16MRDevice.CVRange.End - 1);
        }
    }
}
