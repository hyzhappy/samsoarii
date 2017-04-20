using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.UI.ProjectPropertyWidget.CommunicationInterface
{
    public class COM232 : BaseCommunicationInterface
    {
        private CommunicationInterfaceParams CommunParams232;
        public COM232()
        {
            SetGroup();
            CommunParams232 = (CommunicationInterfaceParams)ProjectPropertyManager.ParamsDic["CommunParams232"];
            SetCommunicationType(CommunParams232.CommuType);
            DataContext = CommunParams232;
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
            Master.GroupName = "232";
            Slave.GroupName = "232";
            FreeButton.GroupName = "232";
        }
        public override void Save()
        {
            CommunParams232.CommuType = GetCommunicationType();
            CommunParams232.BaudRateIndex = Combox1.SelectedIndex;
            CommunParams232.StopBitIndex = Combox3.SelectedIndex;
            CommunParams232.CheckCodeIndex = Combox4.SelectedIndex;
            if (CommunParams232.CommuType == CommunicationType.FreePort)
            {
                CommunParams232.BufferBitIndex = Combox5.SelectedIndex;
            }
            CommunParams232.StationNum = int.Parse(NTextBox.Text);
            CommunParams232.Timeout = int.Parse(TTextBox.Text);
        }
    }
}
