using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCDevice.DeviceDialog
{
    class FGs64MRDeviceMessageDialog : BaseDeviceMessageDialog
    {
        private FGs64MRDevice _FGs64MRDevice = new FGs64MRDevice();
        public FGs64MRDeviceMessageDialog()
        {
            InitializeComponent();
            X_block.Content = string.Format("X: {0}-{1}", _FGs64MRDevice.XRange.Start, _FGs64MRDevice.XRange.End - 1);
            Y_block.Content = string.Format("Y: {0}-{1}", _FGs64MRDevice.YRange.Start, _FGs64MRDevice.YRange.End - 1);
            M_block.Content = string.Format("M: {0}-{1}", _FGs64MRDevice.MRange.Start, _FGs64MRDevice.MRange.End - 1);
            S_block.Content = string.Format("S: {0}-{1}", _FGs64MRDevice.SRange.Start, _FGs64MRDevice.SRange.End - 1);
            T_block.Content = string.Format("T: {0}-{1}", _FGs64MRDevice.TRange.Start, _FGs64MRDevice.TRange.End - 1);
            C_block.Content = string.Format("C: {0}-{1}", _FGs64MRDevice.CRange.Start, _FGs64MRDevice.CRange.End - 1);
            D_block.Content = string.Format("D: {0}-{1}", _FGs64MRDevice.DRange.Start, _FGs64MRDevice.DRange.End - 1);
            TV_block.Content = string.Format("TV: {0}-{1}", _FGs64MRDevice.TVRange.Start, _FGs64MRDevice.TVRange.End - 1);
            CV_block.Content = string.Format("CV: {0}-{1}", _FGs64MRDevice.CVRange.Start, _FGs64MRDevice.CVRange.End - 1);
        }
    }
}
