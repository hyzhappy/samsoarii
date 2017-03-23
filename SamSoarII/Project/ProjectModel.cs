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

namespace SamSoarII.AppMain.Project
{
    public class ProjectModel
    {
        private bool _isCommentMode;
        public bool IsCommentMode
        {
            get { return _isCommentMode; }
            set
            {
                _isCommentMode = value;
                MainRoutine.IsCommendMode = _isCommentMode;
                foreach(var ldmodel in SubRoutines)
                {
                    ldmodel.IsCommendMode = _isCommentMode;
                }
            }
        }
        public string ProjectName { get; set; }

        public LadderDiagramViewModel MainRoutine { get; set; }

        public ObservableCollection<LadderDiagramViewModel> SubRoutines { get; set; } = new ObservableCollection<LadderDiagramViewModel>();

        public ObservableCollection<FuncBlockViewModel> FuncBlocks { get; set; } = new ObservableCollection<FuncBlockViewModel>();

        public PLCDevice.Device CurrentDevice { get; set; }
        public ProjectModel()
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
                if(MainRoutine.ProgramName == name)
                    return MainRoutine;
            }
            foreach(var routine in SubRoutines)
            {
                if(routine.ProgramName == name)
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
                if(funcb.ProgramName == name)
                {
                    return funcb;
                }
            }
            return null;
        }


        public bool ContainProgram(string name)
        {
            return SubRoutines.Any(x => x.ProgramName == name) | FuncBlocks.Any(x => x.ProgramName == name);
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

        public void RemoveSubRoutine(LadderDiagramViewModel ldmodel)
        {
            SubRoutines.Remove(ldmodel);
        }

        public void RemoveFuncBlock(FuncBlockViewModel fbmodel)
        {
            FuncBlocks.Remove(fbmodel);
        }

        /// <summary>
        /// Save the ProjectModel to a xml format file
        /// </summary>
        /// <param name="filepath"></param>
        public void Save(string filepath)
        {
            XDocument xdoc = new XDocument();
            //XDeclaration decNode = new XDeclaration("1.0", "utf-8", null);
            //xdoc.Add(decNode);
            var rootNode = new XElement("Project");
            rootNode.SetAttributeValue("Name", this.ProjectName);
            xdoc.Add(rootNode);
            var settingNode = new XElement("Setting");
            rootNode.Add(settingNode);
            rootNode.Add(ProjectHelper.CreateXElementByValueComments());
            rootNode.Add(ProjectHelper.CreateXElementByGlobalVariableList());
            rootNode.Add(ProjectHelper.CreateXElementByLadderDiagram(MainRoutine));
            foreach(var ldmodel in SubRoutines)
            {
                rootNode.Add(ProjectHelper.CreateXElementByLadderDiagram(ldmodel));
            }
            foreach(var fbmodel in FuncBlocks)
            {
                rootNode.Add(ProjectHelper.CreateXElementByFuncBlock(fbmodel));
            }
            xdoc.Save(filepath);
        }
   
        public bool Open(string filepath)
        {
            //try
            //{
                XDocument xmldoc = XDocument.Load(filepath);
                XElement rootNode = xmldoc.Element("Project");
                ProjectName = rootNode.Attribute("Name").Value;
                // Open Ladder Model
                SubRoutines.Clear();
                FuncBlocks.Clear();
                VariableManager.Clear();
                ValueCommentManager.Clear();
                InstructionCommentManager.Clear();
                ProjectHelper.LoadValueCommentsByXElement(rootNode.Element("ValueComments"));
                ProjectHelper.LoadGlobalVariableListByXElement(rootNode.Element("GlobalVariableList"));   
                var ldnodes = rootNode.Elements("Ladder");
                foreach (XElement ldnode in ldnodes)
                {
                    var ldmodel = ProjectHelper.CreateLadderDiagramByXElement(ldnode);
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
                var fbnodes = rootNode.Elements("FuncBlock");
                foreach (XElement fbnode in fbnodes)
                {
                    var fbmodel = ProjectHelper.CreateFuncBlockByXElement(fbnode);
                    FuncBlocks.Add(fbmodel);
                }
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
            string s = string.Format("stdout : {0}\r\nstderr: {1}\r\n",cmd.StandardOutput.ReadToEnd(), cmd.StandardError.ReadToEnd());
            MessageBox.Show(s);
        }

        private string GenerateCodeFromLadder()
        {
            string code = string.Empty;
            code += string.Format("#include \"plc.h\"\r\n");
            code += MainRoutine.GenerateDeclarationCode("RunLadder");
            foreach(var sub in SubRoutines)
            {
                code += sub.GenerateDeclarationCode(sub.ProgramName);
            }
            code += MainRoutine.GenerateCode("RunLadder");
            foreach(var sub in SubRoutines)
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
