using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCDevice.DeviceDialog
{
    class FGs32MRDeviceMessageDialog : BaseDeviceMessageDialog
    {
        private FGs32MRDevice _FGs32MRDevice = new FGs32MRDevice();
        public FGs32MRDeviceMessageDialog()
        {
            InitializeComponent();
            X_block.Content = string.Format("X: {0}-{1}", _FGs32MRDevice.XRange.Start, _FGs32MRDevice.XRange.End - 1);
            Y_block.Content = string.Format("Y: {0}-{1}", _FGs32MRDevice.YRange.Start, _FGs32MRDevice.YRange.End - 1);
            M_block.Content = string.Format("M: {0}-{1}", _FGs32MRDevice.MRange.Start, _FGs32MRDevice.MRange.End - 1);
            S_block.Content = string.Format("S: {0}-{1}", _FGs32MRDevice.SRange.Start, _FGs32MRDevice.SRange.End - 1);
            T_block.Content = string.Format("T: {0}-{1}", _FGs32MRDevice.TRange.Start, _FGs32MRDevice.TRange.End - 1);
            C_block.Content = string.Format("C: {0}-{1}", _FGs32MRDevice.CRange.Start, _FGs32MRDevice.CRange.End - 1);
            D_block.Content = string.Format("D: {0}-{1}", _FGs32MRDevice.DRange.Start, _FGs32MRDevice.DRange.End - 1);
            TV_block.Content = string.Format("TV: {0}-{1}", _FGs32MRDevice.TVRange.Start, _FGs32MRDevice.TVRange.End - 1);
            CV_block.Content = string.Format("CV: {0}-{1}", _FGs32MRDevice.CVRange.Start, _FGs32MRDevice.CVRange.End - 1);
        }
    }
}
