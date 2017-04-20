using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.UI.ProjectPropertyWidget.CommunicationInterface
{
    public class COM485 : BaseCommunicationInterface
    {
        private CommunicationInterfaceParams CommunParams485;
        public COM485()
        {
            SetGroup();
            CommunParams485 = (CommunicationInterfaceParams)ProjectPropertyManager.ParamsDic["CommunParams485"];
            SetCommunicationType(CommunParams485.CommuType);
            DataContext = CommunParams485;
        }
        public override void SetCommunicationType(CommunicationType type)
        {
            base.SetCommunicationType(type);
        }
        public override CommunicationType GetCommunicationType()
        {
            return base.GetCommunicationType();
        }
        private void SetGroup()
        {
            Master.GroupName = "485";
            Slave.GroupName = "485";
            FreeButton.GroupName = "485";
        }
        public override void Save()
        {
            CommunParams485.CommuType = GetCommunicationType();
            CommunParams485.BaudRateIndex = Combox1.SelectedIndex;
            CommunParams485.StopBitIndex = Combox3.SelectedIndex;
            CommunParams485.CheckCodeIndex = Combox4.SelectedIndex;
            if (CommunParams485.CommuType == CommunicationType.FreePort)
            {
                CommunParams485.BufferBitIndex = Combox5.SelectedIndex;
            }
            CommunParams485.StationNum = int.Parse(NTextBox.Text);
            CommunParams485.Timeout = int.Parse(TTextBox.Text);
        }
    }
}
