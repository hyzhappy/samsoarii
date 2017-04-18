using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCDevice.DeviceDialog
{
    class FGs32MRYTJDeviceMessageDialog : BaseDeviceMessageDialog
    {
        private FGs32MR_YTJDevice _FGs32MR_YTJDevice = new FGs32MR_YTJDevice();
        public FGs32MRYTJDeviceMessageDialog()
        {
            InitializeComponent();
            X_block.Content = string.Format("X: {0}-{1}", _FGs32MR_YTJDevice.XRange.Start, _FGs32MR_YTJDevice.XRange.End - 1);
            Y_block.Content = string.Format("Y: {0}-{1}", _FGs32MR_YTJDevice.YRange.Start, _FGs32MR_YTJDevice.YRange.End - 1);
            M_block.Content = string.Format("M: {0}-{1}", _FGs32MR_YTJDevice.MRange.Start, _FGs32MR_YTJDevice.MRange.End - 1);
            S_block.Content = string.Format("S: {0}-{1}", _FGs32MR_YTJDevice.SRange.Start, _FGs32MR_YTJDevice.SRange.End - 1);
            T_block.Content = string.Format("T: {0}-{1}", _FGs32MR_YTJDevice.TRange.Start, _FGs32MR_YTJDevice.TRange.End - 1);
            C_block.Content = string.Format("C: {0}-{1}", _FGs32MR_YTJDevice.CRange.Start, _FGs32MR_YTJDevice.CRange.End - 1);
            D_block.Content = string.Format("D: {0}-{1}", _FGs32MR_YTJDevice.DRange.Start, _FGs32MR_YTJDevice.DRange.End - 1);
            TV_block.Content = string.Format("TV: {0}-{1}", _FGs32MR_YTJDevice.TVRange.Start, _FGs32MR_YTJDevice.TVRange.End - 1);
            CV_block.Content = string.Format("CV: {0}-{1}", _FGs32MR_YTJDevice.CVRange.Start, _FGs32MR_YTJDevice.CVRange.End - 1);
        }
    }
}
