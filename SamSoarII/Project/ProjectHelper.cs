using SamSoarII.LadderInstViewModel;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace SamSoarII.AppMain.Project
{
    public static class ProjectHelper
    {
        public static XElement CreateXElementByLadderDiagram(LadderDiagramViewModel ldmodel)
        {
            XElement result = new XElement("Ladder");
            result.SetAttributeValue("Name", ldmodel.LadderName);
            if(ldmodel.IsMainLadder)
            {
                result.SetAttributeValue("IsMain", "True");
            }
            foreach(var net in ldmodel.GetNetworks())
            {
                var netEle = CreateXElemnetByLadderNetwork(net);
                result.Add(netEle);
            }
            return result;
        }

        public static XElement CreateXElemnetByLadderNetwork(LadderNetworkViewModel netmodel)
        {
            XElement result = new XElement("Network");
            result.SetAttributeValue("Number", netmodel.NetworkNumber);
            result.SetAttributeValue("RowCount", netmodel.RowCount);
            result.SetAttributeValue("IsMasked", netmodel.IsMasked);
            XElement briefNode = new XElement("Brief");
            XElement descNode = new XElement("Description");
            briefNode.SetValue(netmodel.NetworkBrief);
            descNode.SetValue(netmodel.NetworkDescription);
            result.Add(briefNode);
            result.Add(descNode);
            XElement contentNode = CreateXElementByLadderElementsAndVertialLines(netmodel.GetElements(), netmodel.GetVerticalLines());
            result.Add(contentNode);
            return result;
        }

        public static XElement CreateXElementByLadderElementsAndVertialLines(IEnumerable<BaseViewModel> instEles, IEnumerable<VerticalLineViewModel> vlines)
        {
            XElement result = new XElement("LadderContent");
            foreach (var instEle in instEles)
            {
                XElement instNode = new XElement("InstEle");
                instNode.SetAttributeValue("X", instEle.X);
                instNode.SetAttributeValue("Y", instEle.Y);
                instNode.SetAttributeValue("CatalogID", instEle.GetCatalogID());
                foreach (var valuestring in instEle.GetValueString())
                {
                    XElement valueNode = new XElement("Value");
                    valueNode.SetValue(valuestring);
                    instNode.Add(valueNode);
                }
                result.Add(instNode);
            }
            foreach (var vline in vlines)
            {
                XElement vNode = new XElement("VerticalLine");
                vNode.SetAttributeValue("X", vline.X);
                vNode.SetAttributeValue("Y", vline.Y);
                vNode.SetAttributeValue("CatalogID", vline.GetCatalogID());
                result.Add(vNode);
            }
            return result;
        }

        public static XElement CreateXElementByLadderElementsAndVertialLines(IEnumerable<BaseViewModel> instEles, IEnumerable<VerticalLineViewModel> vlines, int xBegin, int yBegin, int width, int height)
        {
            XElement result = CreateXElementByLadderElementsAndVertialLines(instEles, vlines);
            result.SetAttributeValue("XBegin", xBegin);
            result.SetAttributeValue("YBegin", yBegin);
            result.SetAttributeValue("Width", width);
            result.SetAttributeValue("Height", height);
            return result;
        }

        public static XElement CreateXElementByFuncBlock(FuncBlockViewModel fbmodel)
        {
            XElement result = new XElement("FuncBlock");
            result.SetAttributeValue("Name", fbmodel.FuncBlockName);
            result.Value = fbmodel.Code;
            return result;
        }

        public static LadderDiagramViewModel CreateLadderDiagramByXElement(XElement xEle)
        {
            LadderDiagramViewModel result = new LadderDiagramViewModel(xEle.Attribute("Name").Value);
            if(xEle.Attribute("IsMain") != null && xEle.Attribute("IsMain").Value == "True")
            {
                result.IsMainLadder = true;
            }
            else
            {
                result.IsMainLadder = false;
            }
            result.InitNetworks();
            foreach (XElement netNode in xEle.Elements("Network"))
            {
                var net = CreateLadderNetworkByXElement(netNode, result);
                result.AppendNetwork(net);
            }
            return result;
        } 

        public static LadderNetworkViewModel CreateLadderNetworkByXElement(XElement xEle, LadderDiagramViewModel parent)
        {
            LadderNetworkViewModel result = new LadderNetworkViewModel(parent, int.Parse(xEle.Attribute("Number").Value));
            result.RowCount = int.Parse(xEle.Attribute("RowCount").Value);
            if(xEle.Attribute("IsMasked") != null)
            {
                result.IsMasked = bool.Parse(xEle.Attribute("IsMasked").Value);
            }
            else
            {
                result.IsMasked = false;
            }
            result.NetworkBrief = xEle.Element("Brief").Value;
            result.NetworkDescription = xEle.Element("Description").Value;
           
            XElement contentNode = xEle.Element("LadderContent");
            foreach (var instEle in CreateLadderElementsByXElement(contentNode))
            { 
                result.ReplaceElement(instEle);
            }
            foreach (var vline in CreateLadderVertialLineByXElement(contentNode))
            {
                result.ReplaceVerticalLine(vline);
            }
            return result;
        }

        public static IEnumerable<BaseViewModel> CreateLadderElementsByXElement(XElement xEle)
        {
            List<BaseViewModel> result = new List<BaseViewModel>();
            foreach(XElement instNode in xEle.Elements("InstEle"))
            {
                int catalogId = int.Parse(instNode.Attribute("CatalogID").Value);
                var viewmodel = LadderInstViewModelPrototype.Clone(catalogId);
                int x = int.Parse(instNode.Attribute("X").Value);
                int y = int.Parse(instNode.Attribute("Y").Value);
                viewmodel.X = x;
                viewmodel.Y = y;
                List<string> valueStrings = new List<string>();
                foreach (XElement valuenode in instNode.Elements("Value"))
                {
                    valueStrings.Add(valuenode.Value);
                }
                viewmodel.ParseValue(valueStrings);
                result.Add(viewmodel);
            }
            return result;
        }

        public static IEnumerable<VerticalLineViewModel> CreateLadderVertialLineByXElement(XElement xEle)
        {
            List<VerticalLineViewModel> result = new List<VerticalLineViewModel>();
            foreach (XElement vNode in xEle.Elements("VerticalLine"))
            {
                var vline = new VerticalLineViewModel();
                vline.X = int.Parse(vNode.Attribute("X").Value);
                vline.Y = int.Parse(vNode.Attribute("Y").Value);
                result.Add(vline);
            }
            return result;
        }

        public static FuncBlockViewModel CreateFuncBlockByXElement(XElement xEle)
        {
            FuncBlockViewModel result = new FuncBlockViewModel(xEle.Attribute("Name").Value);
            result.Code = xEle.Value;
            return result;
        }

        public static XElement CreateXElementByLadderVariable(LadderVariable variable)
        {
            XElement result = new XElement("Variable");
            result.SetAttributeValue("Name", variable.Name);
            result.SetElementValue("Value", variable.ValueModel.ToString());
            result.SetElementValue("ValueType", (int)variable.ValueType);
            result.SetElementValue("Comment", variable.Comment);
            return result;
        }

        public static LadderVariable CreateLadderVariableByXElement(XElement xEle)
        {
            string name = xEle.Attribute("Name").Value;
            string valueString = xEle.Element("Value").Value;
            LadderValueType valuetype = (LadderValueType)(int.Parse(xEle.Element("ValueType").Value));
            string comment = xEle.Element("Comment").Value;
            IValueModel valueModel = ValueParser.ParseValue(valueString, valuetype);
            return new LadderVariable(name, valueModel, comment);
        }

        public static ProjectModel LoadProject(string filepath)
        {
            ProjectModel model = new ProjectModel();
            if (model.Open(filepath))
            {
                return model;
            }
            else
            {
                return null;
            }
        }

        public static void LoadGlobalVariableListByXElement(XElement xEle)
        {
            if(xEle != null)
            {
                foreach (XElement xele in xEle.Elements("Variable"))
                {
                    var variable = CreateLadderVariableByXElement(xele);
                    GlobalVariableList.AddVariable(variable);
                }
            }
        }

        public static XElement CreateXElementByGlobalVariableList()
        {
            XElement xEle = new XElement("GlobalVariableList");
            foreach(var variable in GlobalVariableList.VariableCollection)
            {
                xEle.Add(CreateXElementByLadderVariable(variable));
            }
            return xEle;
        }
    }
}
