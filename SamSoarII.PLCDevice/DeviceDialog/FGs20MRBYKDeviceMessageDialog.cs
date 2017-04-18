using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCDevice.DeviceDialog
{
    class FGs20MRBYKDeviceMessageDialog : BaseDeviceMessageDialog
    {
        private FGs20MR_BYKDevice _FGs20MR_BYKDevice = new FGs20MR_BYKDevice();
        public FGs20MRBYKDeviceMessageDialog()
        {
            InitializeComponent();
            X_block.Content = string.Format("X: {0}-{1}", _FGs20MR_BYKDevice.XRange.Start, _FGs20MR_BYKDevice.XRange.End - 1);
            Y_block.Content = string.Format("Y: {0}-{1}", _FGs20MR_BYKDevice.YRange.Start, _FGs20MR_BYKDevice.YRange.End - 1);
            M_block.Content = string.Format("M: {0}-{1}", _FGs20MR_BYKDevice.MRange.Start, _FGs20MR_BYKDevice.MRange.End - 1);
            S_block.Content = string.Format("S: {0}-{1}", _FGs20MR_BYKDevice.SRange.Start, _FGs20MR_BYKDevice.SRange.End - 1);
            T_block.Content = string.Format("T: {0}-{1}", _FGs20MR_BYKDevice.TRange.Start, _FGs20MR_BYKDevice.TRange.End - 1);
            C_block.Content = string.Format("C: {0}-{1}", _FGs20MR_BYKDevice.CRange.Start, _FGs20MR_BYKDevice.CRange.End - 1);
            D_block.Content = string.Format("D: {0}-{1}", _FGs20MR_BYKDevice.DRange.Start, _FGs20MR_BYKDevice.DRange.End - 1);
            TV_block.Content = string.Format("TV: {0}-{1}", _FGs20MR_BYKDevice.TVRange.Start, _FGs20MR_BYKDevice.TVRange.End - 1);
            CV_block.Content = string.Format("CV: {0}-{1}", _FGs20MR_BYKDevice.CVRange.Start, _FGs20MR_BYKDevice.CVRange.End - 1);
        }
    }
}
