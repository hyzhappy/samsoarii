using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.ComponentModel;
using SamSoarII.InstructionViewModel;
namespace SamSoarII.AppMain.Project
{
    public class ProjectModel
    {

        public string ProjectName { get; set; }

        public List<LadderDiagramViewModel> SubRoutines = new List<LadderDiagramViewModel>();

        public LadderDiagramViewModel MainRoutine;

        public List<FuncBlockViewModel> FuncBlocks = new List<FuncBlockViewModel>();

        private ProjectModel()
        {

        }

        public ProjectModel(string projectname)
        {
            ProjectName = projectname;
            MainRoutine = new LadderDiagramViewModel("Main");
            MainRoutine.IsMainLadder = true;
        }

        public void SetMainRoutine(LadderDiagramViewModel ldmodel)
        { 
            ldmodel.IsMainLadder = true;
            MainRoutine = ldmodel;
        }

        /// <summary>
        /// Find Routine by name from current project, if not found, return null
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public LadderDiagramViewModel GetRoutineByName(string name)
        {
            if(MainRoutine != null)
            {
                if(MainRoutine.LadderName == name)
                    return MainRoutine;
            }
            foreach(var routine in SubRoutines)
            {
                if(routine.LadderName == name)
                {
                    return routine;
                }
            }
            return null;
        }
        /// <summary>
        /// Find FunctionBlock by name from current project, if not found, return null
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public FuncBlockViewModel GetFuncBlockByName(string name)
        {
            foreach(var funcb in FuncBlocks)
            {
                if(funcb.FuncBlockName == name)
                {
                    return funcb;
                }
            }
            return null;
        }
        /// <summary>
        /// Find if any Routine has the name Name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool ContainSubRoutine(string name)
        {
            if(MainRoutine.LadderName == name)
            {
                return true;
            }
            return SubRoutines.Any(x => { return x.LadderName == name; });
        }
        /// <summary>
        /// Find if any FunctionBlock has the name Name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool ContainFuncBlock(string name)
        {
            return FuncBlocks.Any(x => { return x.Name == name; });
        }
        /// <summary>
        /// Add SubRoutine 
        /// </summary>
        /// <param name="ldmodel"></param>
        public void AddSubRoutine(LadderDiagramViewModel ldmodel)
        {           
            SubRoutines.Add(ldmodel);
        }
        /// <summary>
        /// Add FunctionBlock 
        /// </summary>
        /// <param name="fbmodel"></param>
        public void AddFuncBlock(FuncBlockViewModel fbmodel)
        {
            FuncBlocks.Add(fbmodel);
        }

        public void RemoveSubRoutineByName(string name)
        {
            var routine = GetRoutineByName(name);
            if(routine != null)
            {
                SubRoutines.Remove(routine);
            }
        }
        public void RemoveFuncBlockByName(string name)
        {
            var fbmodel = GetFuncBlockByName(name);
            if (fbmodel != null)
            {
                FuncBlocks.Remove(fbmodel);
            }
        }

        /// <summary>
        /// Save the ProjectModel to a xml format file
        /// </summary>
        /// <param name="filepath"></param>
        public void Save(string filepath)
        {
            XmlDocument xmldoc = new XmlDocument();
            // Declaration
            var dec = xmldoc.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
            xmldoc.AppendChild(dec);
            // Root Node 
            var root = xmldoc.CreateElement("Project");
            root.SetAttribute("Name", this.ProjectName);
            xmldoc.AppendChild(root);
            // Configuartion node 
            var settingNode = xmldoc.CreateElement("Setting");
            root.AppendChild(settingNode);
            // MainRoutine Node 
            root.AppendChild(SaveLadder(xmldoc, MainRoutine));
            foreach (var ladderModel in SubRoutines)
            {
                root.AppendChild(SaveLadder(xmldoc, ladderModel));
            }
            foreach (var fbmodel in FuncBlocks)
            {
                root.AppendChild(SaveFuncBlock(xmldoc, fbmodel));
            }
            xmldoc.Save(filepath);
        }
   
        public bool Open(string filepath)
        {
            //try
            //{
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(filepath);
                var rootNode = xmldoc.SelectSingleNode("Project");
                ProjectName = rootNode.Attributes["Name"].Value;
                // Open Ladder Model
                SubRoutines.Clear();
                FuncBlocks.Clear();
                var ldnodes = rootNode.SelectNodes("Ladder");
                foreach (XmlNode ldnode in ldnodes)
                {
                    var ldmodel = OpenLadder(ldnode);
                    if (ldmodel.IsMainLadder)
                    {
                        MainRoutine = ldmodel;
                    }
                    else
                    {
                        SubRoutines.Add(ldmodel);
                    }
                }
                // Open FunctionBlock
                var fbnodes = rootNode.SelectNodes("FuncBlock");
                foreach (XmlNode fbnode in fbnodes)
                {
                    var fbmodel = OpenFuncBlock(fbnode);
                    FuncBlocks.Add(fbmodel);
                }
                return true;
            //}
            //catch(Exception exception)
            //{
            //    return false;
            //}
        }

        #region Static Save and Open method
        public static ProjectModel Load(string filepath)
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
        private static XmlNode SaveLadder(XmlDocument xmldoc, LadderDiagramViewModel ldmodel)
        {
            var node = xmldoc.CreateElement("Ladder");
            node.SetAttribute("Name", ldmodel.LadderName);
            if (ldmodel.IsMainLadder)
            {
                node.SetAttribute("IsMain", "True");
            }
            foreach (var network in ldmodel.GetNetworks())
            {
                var networkNode = SaveNetwork(xmldoc, network);
                node.AppendChild(networkNode);
            }
            return node;
        }
        private static XmlNode SaveNetwork(XmlDocument xmldoc, LadderNetworkViewModel network)
        {
            var node = xmldoc.CreateElement("Network");
            node.SetAttribute("Number", network.NetworkNumber.ToString());
            node.SetAttribute("RowCount", network.RowCount.ToString());
            var briefNode = xmldoc.CreateElement("Brief");
            var descriptionNode = xmldoc.CreateElement("Description");
            briefNode.InnerText = network.NetworkBrief;
            node.AppendChild(briefNode);
            descriptionNode.InnerText = network.NetworkDescription;
            node.AppendChild(descriptionNode);
            foreach (var instEle in network.GetElements())
            {
                var enode = xmldoc.CreateElement("InstEle");
                enode.SetAttribute("X", instEle.X.ToString());
                enode.SetAttribute("Y", instEle.Y.ToString());
                enode.SetAttribute("CatalogID", instEle.GetCatalogID().ToString());
                foreach (string str in instEle.GetValueString())
                {
                    var valuenode = xmldoc.CreateElement("Value");
                    valuenode.InnerText = str;
                    enode.AppendChild(valuenode);
                }
                node.AppendChild(enode);
            }

            foreach (var vline in network.GetVerticalLines())
            {
                var vnode = xmldoc.CreateElement("VerticalLine");
                vnode.SetAttribute("X", vline.X.ToString());
                vnode.SetAttribute("Y", vline.Y.ToString());
                vnode.SetAttribute("CatalogID", vline.GetCatalogID().ToString());
                node.AppendChild(vnode);
            }
            return node;
        }
        private static XmlNode SaveFuncBlock(XmlDocument xmldoc, FuncBlockViewModel fbmodel)
        {
            var node = xmldoc.CreateElement("FuncBlock");
            node.SetAttribute("Name", fbmodel.FuncBlockName);
            node.InnerText = fbmodel.Code;
            return node;
        }
        private static LadderDiagramViewModel OpenLadder(XmlNode ldnode)
        {
            var ldmodel = new LadderDiagramViewModel(ldnode.Attributes["Name"].Value);
            if (ldnode.Attributes != null && ldnode.Attributes["IsMain"] != null && ldnode.Attributes["IsMain"].Value == "True")
            {
                ldmodel.IsMainLadder = true;
            }
            else
            {
                ldmodel.IsMainLadder = false;
            }
            ldmodel.RemoveAllNetworks();
            foreach (XmlNode netNode in ldnode.SelectNodes("Network"))
            {
                var network = OpenNetwrok(netNode, ldmodel);
                ldmodel.AppendNetwork(network);
            }
            return ldmodel;
        }
        private static LadderNetworkViewModel OpenNetwrok(XmlNode networkNode, LadderDiagramViewModel ldmodel)
        {
            int number = int.Parse(networkNode.Attributes["Number"].Value);
            var network = new LadderNetworkViewModel(ldmodel, number);
            network.RowCount = int.Parse(networkNode.Attributes["RowCount"].Value);
            network.NetworkBrief = networkNode.SelectSingleNode("Brief").InnerText;
            network.NetworkDescription = networkNode.SelectSingleNode("Description").InnerText;
            foreach (XmlNode instEleNode in networkNode.SelectNodes("InstEle"))
            {
                int catalogId = int.Parse(instEleNode.Attributes["CatalogID"].Value);
                var viewmodel = InstructionViewModelPrototype.Clone(catalogId);
                int x = int.Parse(instEleNode.Attributes["X"].Value);
                int y = int.Parse(instEleNode.Attributes["Y"].Value);
                viewmodel.X = x;
                viewmodel.Y = y;
                List<string> valueStrings = new List<string>();
                foreach (XmlNode valuenode in instEleNode.SelectNodes("Value"))
                {
                    valueStrings.Add(valuenode.InnerText);
                }
                viewmodel.ParseValue(valueStrings);
                network.ReplaceElement(viewmodel);
            }
            foreach (XmlNode vlineNode in networkNode.SelectNodes("VerticalLine"))
            {
                var vline = new VerticalLineViewModel();
                vline.X = int.Parse(vlineNode.Attributes["X"].Value);
                vline.Y = int.Parse(vlineNode.Attributes["Y"].Value);
                network.AddVerticalLine(vline);
            }
            return network;
        }
        private static FuncBlockViewModel OpenFuncBlock(XmlNode fbnode)
        {
            var fbmodel = new FuncBlockViewModel(fbnode.Attributes["Name"].Value);
            fbmodel.Code = fbnode.InnerText;
            return fbmodel;
        }
        #endregion

        public void Compile()
        {

        }

    }
}
