using SamSoarII.AppMain.Project;
using SamSoarII.AppMain.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.AppMain.LadderCommand
{
    public class NetworkChangeElementArea
    {
        public SelectStatus SU_Select { get; set; }
        public CrossNetworkState SU_Cross { get; set; }
        public int NetworkNumberStart { get; set; }
        public int NetworkNumberEnd { get; set; }
        public int X1 { get; set; }
        public int X2 { get; set; }
        public int Y1 { get; set; }
        public int Y2 { get; set; }

        public void Select(LadderNetworkViewModel lnvmodel)
        {
            LadderDiagramViewModel ldvmodel = lnvmodel.LDVModel;
            ProjectModel pmodel = ldvmodel.ProjectModel;
            InteractionFacade ifacade = pmodel.IFacade;
            ldvmodel.AcquireArea(this);
            ifacade.MainTabControl.ShowItem(ldvmodel);
        }

        public void Save(XElement xele)
        {
            xele.SetAttributeValue("SelectStatus",       (int)SU_Select);
            xele.SetAttributeValue("CrossNetworkState",  (int)SU_Cross);
            xele.SetAttributeValue("NetworkNumberStart", NetworkNumberStart);
            xele.SetAttributeValue("NetworkNumberEnd",   NetworkNumberEnd);
            xele.SetAttributeValue("X1",                 X1);
            xele.SetAttributeValue("X2",                 X2);
            xele.SetAttributeValue("Y1",                 Y1);
            xele.SetAttributeValue("Y2",                 Y2);
        }

        public void Load(XElement xele)
        {
            SU_Select = (SelectStatus)(int.Parse(xele.Attribute("SelectStatus").Value));
            SU_Cross = (CrossNetworkState)(int.Parse(xele.Attribute("CrossNetworkState").Value));
            NetworkNumberStart = int.Parse(xele.Attribute("NetworkNumberStart").Value);
            NetworkNumberEnd = int.Parse(xele.Attribute("NetworkNumberEnd").Value);
            X1 = int.Parse(xele.Attribute("X1").Value);
            X2 = int.Parse(xele.Attribute("X2").Value);
            Y1 = int.Parse(xele.Attribute("Y1").Value);
            Y2 = int.Parse(xele.Attribute("Y2").Value);
        }
        
    }
}
