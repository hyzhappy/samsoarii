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

        public LadderDiagramModel GetRoutineByName(string name)
        {
            if(MainRoutine != null)
            {
                if(MainRoutine.Name == name)
                    return MainRoutine;
            }
            foreach(var routine in SubRoutines)
            {
                if(routine.Name == name)
                {
                    return routine;
                }
            }
            return null;
        }

        /// <summary>
        /// find if any Routine has the name Name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool Contain(string Name)
        {
            if(MainRoutine.Name == Name)
            {
                return true;
            }
            return SubRoutines.Any(x => { return x.Name == Name; });
        }

        public void AddSubRoutine(LadderDiagramModel ldmodel)
        {           
            SubRoutines.Add(ldmodel);
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
            root.SetAttribute("Name", Name);
            xmldoc.AppendChild(root);
            // Configuartion node 
            var configNode = xmldoc.CreateElement("Configuration");
            root.AppendChild(configNode);
            // MainRoutine Node 
            root.AppendChild(SaveLadder(xmldoc, MainRoutine));
            foreach (var ladderModel in SubRoutines)
            {
                root.AppendChild(SaveLadder(xmldoc, ladderModel));
            }
            xmldoc.Save(filepath);
        }

        public void Open(string filepath)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filepath);          
            var rootNode = xmldoc.SelectSingleNode("Project");
            Name = rootNode.Attributes["Name"].Value;
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

        public void Compile()
        {
            

        }

    }
}
