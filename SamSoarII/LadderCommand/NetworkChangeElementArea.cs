using SamSoarII.AppMain.Project;
using SamSoarII.AppMain.UI;
using SamSoarII.LadderInstViewModel;
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
        static public NetworkChangeElementArea Empty { get; private set; }
            = new NetworkChangeElementArea();

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
            if (this == Empty) return;
            LadderDiagramViewModel ldvmodel = lnvmodel.LDVModel;
            ProjectModel pmodel = ldvmodel.ProjectModel;
            InteractionFacade ifacade = pmodel.IFacade;
            ldvmodel.AcquireArea(this);
            ifacade.MainTabControl.ShowItem(ldvmodel);
        }

        static public NetworkChangeElementArea Create(
            LadderNetworkViewModel lnvmodel,
            IEnumerable<BaseViewModel> elements,
            IEnumerable<VerticalLineViewModel> vlines)
        {
            NetworkChangeElementArea area = null;
            foreach (BaseViewModel bvmodel in elements)
            {
                Analyze(lnvmodel, bvmodel, ref area);
            }
            foreach (VerticalLineViewModel vlvmodel in vlines)
            {
                Analyze(lnvmodel, vlvmodel, ref area);
            }
            return area;
        }

        static public NetworkChangeElementArea Create(
            LadderDiagramViewModel ldvmodel,
            IEnumerable<LadderNetworkViewModel> lnvmodels)
        {
            NetworkChangeElementArea area = null;
            foreach (LadderNetworkViewModel lnvmodel in lnvmodels)
            {
                Analyze(ldvmodel, lnvmodel, ref area);
            }
            return area;
        }
        
        static private void Analyze(
            LadderNetworkViewModel lnvmodel,
            BaseViewModel bvmodel, 
            ref NetworkChangeElementArea area)
        {
            int x = bvmodel.X;
            int y = bvmodel.Y;
            if (bvmodel is VerticalLineViewModel) x++;
            if (area == null)
            {
                area = new NetworkChangeElementArea();
                area.SU_Select = SelectStatus.SingleSelected;
                area.SU_Cross = CrossNetworkState.NoCross;
                area.NetworkNumberStart = lnvmodel.NetworkNumber;
                area.NetworkNumberEnd = lnvmodel.NetworkNumber;
                area.X1 = area.X2 = x;
                area.Y1 = area.Y2 = y;
                 
            }
            else
            {
                area.SU_Select = SelectStatus.MultiSelected;
                area.X1 = Math.Min(area.X1, x);
                area.X2 = Math.Max(area.X2, x);
                area.Y1 = Math.Min(area.Y1, y);
                area.Y2 = Math.Max(area.Y2, y);
            }
        }

        static private void Analyze(
            LadderDiagramViewModel ldvmodel,
            LadderNetworkViewModel lnvmodel,
            ref NetworkChangeElementArea area)
        {
            if (area == null)
            {
                area = new NetworkChangeElementArea();
                area.SU_Select = SelectStatus.MultiSelected;
                area.SU_Cross = CrossNetworkState.CrossDown;
                area.NetworkNumberStart = lnvmodel.NetworkNumber;
                area.NetworkNumberEnd = lnvmodel.NetworkNumber;
            }
            else
            {
                area.NetworkNumberStart = Math.Min(
                    area.NetworkNumberStart, 
                    lnvmodel.NetworkNumber);
                area.NetworkNumberEnd = Math.Min(
                    area.NetworkNumberEnd,
                    lnvmodel.NetworkNumber);
            }
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
