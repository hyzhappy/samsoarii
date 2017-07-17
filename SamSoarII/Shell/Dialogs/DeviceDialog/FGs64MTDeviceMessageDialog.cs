using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCDevice.DeviceDialog
{
    class FGs64MTDeviceMessageDialog : BaseDeviceMessageDialog
    {
        private FGs64MTDevice _FGs64MTDevice = new FGs64MTDevice();
        public FGs64MTDeviceMessageDialog()
        {
            InitializeComponent();
            X_block.Content = string.Format("X: {0}-{1}", _FGs64MTDevice.XRange.Start, _FGs64MTDevice.XRange.End - 1);
            Y_block.Content = string.Format("Y: {0}-{1}", _FGs64MTDevice.YRange.Start, _FGs64MTDevice.YRange.End - 1);
            M_block.Content = string.Format("M: {0}-{1}", _FGs64MTDevice.MRange.Start, _FGs64MTDevice.MRange.End - 1);
            S_block.Content = string.Format("S: {0}-{1}", _FGs64MTDevice.SRange.Start, _FGs64MTDevice.SRange.End - 1);
            T_block.Content = string.Format("T: {0}-{1}", _FGs64MTDevice.TRange.Start, _FGs64MTDevice.TRange.End - 1);
            C_block.Content = string.Format("C: {0}-{1}", _FGs64MTDevice.CRange.Start, _FGs64MTDevice.CRange.End - 1);
            D_block.Content = string.Format("D: {0}-{1}", _FGs64MTDevice.DRange.Start, _FGs64MTDevice.DRange.End - 1);
            TV_block.Content = string.Format("TV: {0}-{1}", _FGs64MTDevice.TVRange.Start, _FGs64MTDevice.TVRange.End - 1);
            CV_block.Content = string.Format("CV: {0}-{1}", _FGs64MTDevice.CVRange.Start, _FGs64MTDevice.CVRange.End - 1);
        }
    }
}
