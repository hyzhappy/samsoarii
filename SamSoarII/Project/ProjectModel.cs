using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.ComponentModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.ValueModel;
using System.Diagnostics;
using System.Windows;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SamSoarII.AppMain.UI;
using System.Windows.Controls;
using SamSoarII.Extend.FuncBlockModel;
using SamSoarII.PLCDevice;
using SamSoarII.UserInterface;

namespace SamSoarII.AppMain.Project
{
    public class ProjectModel
    {
        #region Numbers & Interfaces
        
        public bool IsModify
        {
            get
            {
                bool ret = false;
                ret |= MainRoutine.IsModify;
                foreach (LadderDiagramViewModel ldvmodel in SubRoutines)
                {
                    ret |= ldvmodel.IsModify;
                }
                return ret;
            }
            set
            {
                MainRoutine.IsModify = value;
                foreach (LadderDiagramViewModel ldvmodel in SubRoutines)
                {
                    ldvmodel.IsModify = value;
                }
            }
        }

        private bool _isCommentMode;
        public bool IsCommentMode
        {
            get { return _isCommentMode; }
            set
            {
                _isCommentMode = value;
                MainRoutine.IsCommendMode = _isCommentMode;
                foreach (var ldmodel in SubRoutines)
                {
                    ldmodel.IsCommendMode = _isCommentMode;
                }
            }
        }

        public string ProjectName { get; set; }

        public InteractionFacade IFacade { get; set; }

        public LadderDiagramViewModel MainRoutine { get; set; }

        public ObservableCollection<LadderDiagramViewModel> SubRoutines { get; set; } 
            = new ObservableCollection<LadderDiagramViewModel>();
        
        public ObservableCollection<FuncBlockViewModel> FuncBlocks { get; set; } 
            = new ObservableCollection<FuncBlockViewModel>();
        
        public IEnumerable<FuncModel> Funcs
        {
            get
            {
                List<FuncModel> result = new List<FuncModel>();
                foreach (FuncBlockViewModel fbvmodel in FuncBlocks)
                {
                    result.AddRange(fbvmodel.Funcs);
                }
                return result;
            }
        }

        public ModbusTableViewModel MTVModel { get; set; }

        public ReportOutputModel OModel { get; set; }

        public Device CurrentDevice
        {
            get { return PLCDeviceManager.GetPLCDeviceManager().SelectDevice; }
            set
            {
                PLCDeviceManager.GetPLCDeviceManager().SetSelectDeviceType(value.Type);
            }
        }

        #endregion

        #region Initialize

        public ProjectModel()
        {

        }

        public ProjectModel(string projectname, ReportOutputModel _outputmodel)
        {
            ProjectName = projectname;
            MainRoutine = new LadderDiagramViewModel("Main", this);
            MainRoutine.IsMainLadder = true;
            MTVModel = new ModbusTableViewModel(this);
            OModel = _outputmodel;
        }

        #endregion

        #region Modification
        
        public bool ContainProgram(string name)
        {
            return SubRoutines.Any(x => x.ProgramName == name) | FuncBlocks.Any(x => x.ProgramName == name);
        }

        public void Add(LadderDiagramViewModel ldmodel)
        {
            if (!SubRoutines.Contains(ldmodel))
                SubRoutines.Add(ldmodel);
        }

        public void Add(FuncBlockViewModel fbmodel)
        {
            if (!FuncBlocks.Contains(fbmodel))
            {
                fbmodel.OModel = OModel;
                FuncBlocks.Add(fbmodel);
            }
        }

        public void Remove(LadderDiagramViewModel ldmodel)
        {
            if (SubRoutines.Contains(ldmodel))
                SubRoutines.Remove(ldmodel);
        }

        public void Remove(FuncBlockViewModel fbmodel)
        {
            if (FuncBlocks.Contains(fbmodel))
                FuncBlocks.Remove(fbmodel);
        }

        #endregion

        /// <summary>
        /// Save the ProjectModel to a xml format file
        /// </summary>
        /// <param name="filepath"></param>
        public void Save(XElement rootNode)
        {
            //XDocument xdoc = new XDocument();
            //XDeclaration decNode = new XDeclaration("1.0", "utf-8", null);
            //xdoc.Add(decNode);
            //var rootNode = new XElement("Project");
            rootNode.SetAttributeValue("Name", ProjectName);
            rootNode.SetAttributeValue("DeviceType", CurrentDevice.Type);
            //xdoc.Add(rootNode);
            var settingNode = new XElement("Setting");
            rootNode.Add(settingNode);
            rootNode.Add(ProjectHelper.CreateXElementByValueComments());
            rootNode.Add(ProjectHelper.CreateXElementByValueAlias());
            rootNode.Add(ProjectPropertyManager.CreateProjectPropertyXElement());
            //rootNode.Add(ProjectHelper.CreateXElementByGlobalVariableList());
            rootNode.Add(ProjectHelper.CreateXElementByLadderDiagram(MainRoutine));
            foreach (var ldmodel in SubRoutines)
            {
                rootNode.Add(ProjectHelper.CreateXElementByLadderDiagram(ldmodel));
            }
            foreach (var fbmodel in FuncBlocks)
            {
                rootNode.Add(ProjectHelper.CreateXElementByFuncBlock(fbmodel));
            }
            var mtnode = new XElement("Modbus");
            MTVModel.Save(mtnode);
            rootNode.Add(mtnode);
            //xdoc.Save(filepath);
        }

        public bool Open(XElement rootNode)
        {
            //try
            //{
            //XDocument xmldoc = XDocument.Load(filepath);
            //XElement rootNode = xmldoc.Element("Project");
            ProjectName = rootNode.Attribute("Name").Value;
            PLCDeviceManager.GetPLCDeviceManager().SetSelectDeviceType((PLCDeviceType)Enum.Parse(typeof(PLCDeviceType),rootNode.Attribute("DeviceType").Value));
            SubRoutines.Clear();
            FuncBlocks.Clear();
            //VariableManager.Clear();
            ValueAliasManager.Clear();
            ValueCommentManager.Clear();
            InstructionCommentManager.Clear();
            ProjectHelper.LoadValueCommentsByXElement(rootNode.Element("ValueComments"));
            ProjectHelper.LoadValueAliasByXElement(rootNode.Element("ValueAlias"));
            ProjectPropertyManager.LoadProjectPropertyByXElement(rootNode.Element("ProjectPropertyParams"));
            //ProjectHelper.LoadGlobalVariableListByXElement(rootNode.Element("GlobalVariableList"));
            var ldnodes = rootNode.Elements("Ladder");
            foreach (XElement ldnode in ldnodes)
            {
                var ldmodel = ProjectHelper.CreateLadderDiagramByXElement(ldnode, this);
                if (ldmodel.IsMainLadder)
                {
                    MainRoutine = ldmodel;
                }
                else
                {
                    SubRoutines.Add(ldmodel);
                    TreeViewItem item = new TreeViewItem();
                    item.Header = ldmodel;
                }
            }
            // Open FunctionBlock
            var fbnodes = rootNode.Elements("FuncBlock");
            foreach (XElement fbnode in fbnodes)
            {
                var fbmodel = ProjectHelper.CreateFuncBlockByXElement(fbnode);
                fbmodel.OModel = OModel;
                FuncBlocks.Add(fbmodel);
            }
            var mtnodes = rootNode.Element("Modbus");
            var mtmodel = new ModbusTableViewModel(this);
            mtmodel.Load(mtnodes);
            MTVModel = mtmodel;
            return true;
            //}
            //catch (Exception exception)
            //{
            //    return false;
            //}
        }

        public void Compile()
        {
            string ladderFile = SamSoarII.Utility.FileHelper.GetTempFile(".c");
            string funcBlockFile = SamSoarII.Utility.FileHelper.GetTempFile(".c");
            string outputBinaryFile = SamSoarII.Utility.FileHelper.GetTempFile(".bin");
            File.WriteAllText(ladderFile, GenerateCodeFromLadder());
            File.WriteAllText(funcBlockFile, GenerateCodeFromFuncBlock());
            Process cmd = new Process();
            cmd.StartInfo.FileName = string.Format(@"{0}\plcc.exe", Environment.CurrentDirectory);
            cmd.StartInfo.Arguments = string.Format(" {0} {1} {2} {3}", "FGs16MR", ladderFile, funcBlockFile, "aa.bin");
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.Start();
            cmd.WaitForExit();
            string s = string.Format("stdout : {0}\r\nstderr: {1}\r\n", cmd.StandardOutput.ReadToEnd(), cmd.StandardError.ReadToEnd());
            OModel.Write(OModel.Report_Complie, s);
            //MessageBox.Show(s);
        }

        private string GenerateCodeFromLadder()
        {
            string code = string.Empty;
            code += string.Format("#include \"plc.h\"\r\n");
            code += MainRoutine.GenerateDeclarationCode("RunLadder");
            foreach (var sub in SubRoutines)
            {
                code += sub.GenerateDeclarationCode(sub.ProgramName);
            }
            code += MainRoutine.GenerateCode("RunLadder");
            foreach (var sub in SubRoutines)
            {
                code += sub.GenerateCode(sub.ProgramName);
            }
            return code;
        }

        private string GenerateCodeFromFuncBlock()
        {
            string result = string.Empty;
            return result;
        }

    }
}

