using SamSoarII.PLCCOM.Memory;
using SamSoarII.PLCCOM.USB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCCOM
{
    public class PLCCOMModel
    {
        #region Number

        private PLCDevice.Device device;

        private USBModel usbmodel;

        private MonitorMemoryModel mmmodel;

        #endregion

        #region Upload & Download
        public void Download(string file_bin, string file_pro)
        {
            usbmodel.Download(file_bin, file_pro);
        }

        public void Upload(string file_bin, string file_pro)
        {
            usbmodel.Upload(file_bin, file_pro);
        }
        #endregion

        #region View Mode
        
        public void StartView()
        {

        }

        public void StopView()
        {

        }

        public UnitMemoryModel AddView(string name, string type)
        {
            UnitMemoryModel ret = UnitMemoryModel.Create(name, type, device);
            return ret;
        }

        public UnitMemoryModel RemoveView(string name, string type)
        {
            UnitMemoryModel ret = UnitMemoryModel.Create(name, type, device);
            return ret;
        }

        #endregion

    }
}
