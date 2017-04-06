using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCDevice.DeviceDialog
{
    class FGs32MTDeviceMessageDialog : BaseDeviceMessageDialog
    {
        private FGs32MTDevice _FGs32MTDevice = new FGs32MTDevice();
        public FGs32MTDeviceMessageDialog()
        {
            InitializeComponent();
            X_block.Content = string.Format("X: {0}-{1}", _FGs32MTDevice.XRange.Start, _FGs32MTDevice.XRange.End - 1);
            Y_block.Content = string.Format("Y: {0}-{1}", _FGs32MTDevice.YRange.Start, _FGs32MTDevice.YRange.End - 1);
            M_block.Content = string.Format("M: {0}-{1}", _FGs32MTDevice.MRange.Start, _FGs32MTDevice.MRange.End - 1);
            S_block.Content = string.Format("S: {0}-{1}", _FGs32MTDevice.SRange.Start, _FGs32MTDevice.SRange.End - 1);
            T_block.Content = string.Format("T: {0}-{1}", _FGs32MTDevice.TRange.Start, _FGs32MTDevice.TRange.End - 1);
            C_block.Content = string.Format("C: {0}-{1}", _FGs32MTDevice.CRange.Start, _FGs32MTDevice.CRange.End - 1);
            D_block.Content = string.Format("D: {0}-{1}", _FGs32MTDevice.DRange.Start, _FGs32MTDevice.DRange.End - 1);
            TV_block.Content = string.Format("TV: {0}-{1}", _FGs32MTDevice.TVRange.Start, _FGs32MTDevice.TVRange.End - 1);
            CV_block.Content = string.Format("CV: {0}-{1}", _FGs32MTDevice.CVRange.Start, _FGs32MTDevice.CVRange.End - 1);
        }
    }
}
