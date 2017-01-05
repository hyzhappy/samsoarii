using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.ComponentModel;
using SamSoarII.InstructionViewModel;
namespace SamSoarII.Project
{
    public class ProjectModel : INotifyPropertyChanged
    {

        public string Name { get; set; }

        public List<LadderDiagramModel> SubRoutines = new List<LadderDiagramModel>();

        public LadderDiagramModel MainRoutine;

        public List<FuncBlockModel> FuncBlocks = new List<FuncBlockModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ProjectModel()
        {
            
        }

        public ProjectModel(string name)
        {          
            Name = name;
        }

        public void SetMainRoutine(LadderDiagramModel ldmodel)
        { 
            ldmodel.IsMainLadder = true;
            MainRoutine = ldmodel;
        }

        /// <summary>
        /// Find Routine by name from current project, if not found, return null
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public LadderDiagramModel GetRoutineByName(string Name)
        {
            if(MainRoutine != null)
            {
                if(MainRoutine.Name == Name)
                    return MainRoutine;
            }
            foreach(var routine in SubRoutines)
            {
                if(routine.Name == Name)
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
        public FuncBlockModel GetFuncBlockByName(string Name)
        {
            foreach(var funcb in FuncBlocks)
            {
                if(funcb.Name == Name)
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
        public bool ContainSubRoutine(string Name)
        {
            if(MainRoutine.Name == Name)
            {
                return true;
            }
            return SubRoutines.Any(x => { return x.Name == Name; });
        }
        /// <summary>
        /// Find if any FunctionBlock has the name Name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool ContainFuncBlock(string Name)
        {
            return FuncBlocks.Any(x => { return x.Name == Name; });
        }
        /// <summary>
        /// Add SubRoutine 
        /// </summary>
        /// <param name="ldmodel"></param>
        public void AddSubRoutine(LadderDiagramModel ldmodel)
        {           
            SubRoutines.Add(ldmodel);
        }
        /// <summary>
        /// Add FunctionBlock 
        /// </summary>
        /// <param name="fbmodel"></param>
        public void AddFuncBlock(FuncBlockModel fbmodel)
        {
            FuncBlocks.Add(fbmodel);
        }

        public void RemoveSubRoutineByName(string Name)
        {
            var routine = GetRoutineByName(Name);
            if(routine != null)
            {
                SubRoutines.Remove(routine);
            }
        }
        public void RemoveFuncBlockByName(string name)
        {
            var fbmodel = GetFuncBlockByName(Name);
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
            var dec = xmldoc.CreateXmlDeclaration("1.0", string.Empty, string.Empty);
            xmldoc.AppendChild(dec);
            // Root Node 
            var root = xmldoc.CreateElement("Project");
            root.SetAttribute("Name", this.Name);
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

        public void Open(string filepath)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filepath);          
            var rootNode = xmldoc.SelectSingleNode("Project");
            Name = rootNode.Attributes["Name"].Value;
            // Open Ladder Model
            var ldnodes = xmldoc.GetElementsByTagName("LadderModel");
            SubRoutines.Clear();
            foreach (XmlNode ldnode in ldnodes)
            {
                var ldmodel = OpenLadder(ldnode);
                if(ldmodel.IsMainLadder)
                {
                    MainRoutine = ldmodel;
                }
                else
                {
                    SubRoutines.Add(ldmodel);
                }     
            }
            // Open FunctionBlock
            var fbnodes = xmldoc.GetElementsByTagName("FuncBlock");
            FuncBlocks.Clear();
            foreach (XmlNode fbnode in fbnodes)
            {
                var fbmodel = OpenFuncBlock(fbnode);
                FuncBlocks.Add(fbmodel);
            }
        }

        private XmlNode SaveLadder(XmlDocument xmldoc, LadderDiagramModel ldmodel)
        {
            var node = xmldoc.CreateElement("LadderModel");
            node.SetAttribute("Name", ldmodel.Name);
            if (ldmodel.IsMainLadder)
            {
                node.SetAttribute("IsMain", "True");
            }
            foreach (var viewmodel in ldmodel.GetElements())
            {
                var enode = xmldoc.CreateElement("ViewModel");
                enode.SetAttribute("X", viewmodel.X.ToString());
                enode.SetAttribute("Y", viewmodel.Y.ToString());
                enode.SetAttribute("CatalogID", viewmodel.GetCatalogID().ToString());
                foreach (string str in viewmodel.GetValueString())
                {
                    var valuenode = xmldoc.CreateElement("Value");
                    valuenode.InnerText = str;
                    enode.AppendChild(valuenode);
                }
                node.AppendChild(enode);
            }
            foreach (var vline in ldmodel.GetVerticalLines())
            {
                var vnode = xmldoc.CreateElement("VerticalLine");
                vnode.SetAttribute("X", vline.X.ToString());
                vnode.SetAttribute("Y", vline.Y.ToString());
                vnode.SetAttribute("CatalogID", vline.GetCatalogID().ToString());
                node.AppendChild(vnode);
            }
            return node;
        }

        private XmlNode SaveFuncBlock(XmlDocument xmldoc, FuncBlockModel fbmodel)
        {
            var node = xmldoc.CreateElement("FuncBlock");
            node.SetAttribute("Name", fbmodel.Name);
            node.InnerText = fbmodel.Code;
            return node;
        }

        private LadderDiagramModel OpenLadder(XmlNode ldnode)
        {
            var ldmodel = new LadderDiagramModel(ldnode.Attributes["Name"].Value);
            if (ldnode.Attributes != null && ldnode.Attributes["IsMain"] != null && ldnode.Attributes["IsMain"].Value == "True")
            {
                ldmodel.IsMainLadder = true;
            }
            else
            {
                ldmodel.IsMainLadder = false;
            }
            foreach (XmlNode vmnode in ldnode.SelectNodes("ViewModel"))
            {
                int catalogId = int.Parse(vmnode.Attributes["CatalogID"].Value);
                var viewmodel = InstructionViewModelPrototype.Clone(catalogId);
                int x = int.Parse(vmnode.Attributes["X"].Value);
                int y = int.Parse(vmnode.Attributes["Y"].Value);
                viewmodel.X = x;
                viewmodel.Y = y;
                List<string> valueStrings = new List<string>();
                foreach (XmlNode valuenode in vmnode.ChildNodes)
                {
                    valueStrings.Add(valuenode.InnerText);
                }
                viewmodel.ParseValue(valueStrings);
                ldmodel.AddElement(viewmodel);
            }
            foreach (XmlNode vlinenode in ldnode.SelectNodes("VerticalLine"))
            {
                var vline = new VerticalLineViewModel();
                int x = int.Parse(vlinenode.Attributes["X"].Value);
                int y = int.Parse(vlinenode.Attributes["Y"].Value);
                vline.X = x;
                vline.Y = y;
                ldmodel.AddVerticalLine(vline);
            }
            return ldmodel;
        }

        private FuncBlockModel OpenFuncBlock(XmlNode fbnode)
        {
            var fbmodel = new FuncBlockModel(fbnode.Attributes["Name"].Value);
            fbmodel.Code = fbnode.InnerText;
            return fbmodel;
        }

        public void Compile()
        {
            

        }

    }
}
