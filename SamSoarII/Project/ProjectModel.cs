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

namespace SamSoarII.AppMain.Project
{
    public enum ChangeType
    {
        Add,
        Remove,
        Modify,
        Clear
    }
    public class RefNetworksBriefChangedEventArgs : EventArgs
    {
        public ChangeType Type { get; set; }
        public LadderDiagramViewModel Routine { get; set; }
        public RefNetworksBriefChangedEventArgs(ChangeType type, LadderDiagramViewModel routine)
        {
            Type = type;
            Routine = routine;
        }
    }
    public delegate void RefNetworksBriefChangedEventHandler(RefNetworksBriefChangedEventArgs e);
    public class ProjectModel
    {
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
        public event RefNetworksBriefChangedEventHandler RefNetworksBriefChanged = delegate { };

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
        public LadderDiagramViewModel MainRoutine { get; set; }
        public ObservableCollection<LadderDiagramViewModel> SubRoutines { get; set; } = new ObservableCollection<LadderDiagramViewModel>();
        public ObservableCollection<TreeViewItem> SubRoutineTreeViewItems { get; set; } = new ObservableCollection<TreeViewItem>();
        public Dictionary<LadderDiagramViewModel, ObservableCollection<string>> RefNetworksBrief { get; set; } = new Dictionary<LadderDiagramViewModel, ObservableCollection<string>>();
        public ObservableCollection<FuncBlockViewModel> FuncBlocks { get; set; } = new ObservableCollection<FuncBlockViewModel>();
        public ModbusTableViewModel MTVModel { get; set; }
        public ReportOutputModel OModel { get; set; }

        private PLCDevice.Device currentDevice;
        public PLCDevice.Device CurrentDevice
        {
            get { return this.currentDevice; }
            set
            {
                this.currentDevice = value;
                if (MTVModel != null)
                    MTVModel.PLCDevice = value;
            }
        }

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

        public void UpdateNetworkBriefs(LadderDiagramViewModel Routine, ChangeType Type)
        {
            switch (Type)
            {
                case ChangeType.Add:
                    ObservableCollection<string> networksBrief = new ObservableCollection<string>();
                    foreach (var network in Routine.LadderNetworks.OrderBy(x => { return x.NetworkNumber; }))
                    {
                        networksBrief.Add(string.Format("{0}-{1}", network.NetworkNumber, network.NetworkBrief));
                    }
                    RefNetworksBrief.Add(Routine, networksBrief);
                    break;
                case ChangeType.Remove:
                    RefNetworksBrief.Remove(Routine);
                    break;
                case ChangeType.Modify:
                    networksBrief = new ObservableCollection<string>();
                    foreach (var network in Routine.LadderNetworks.OrderBy(x => { return x.NetworkNumber; }))
                    {
                        networksBrief.Add(string.Format("{0}-{1}", network.NetworkNumber, network.NetworkBrief));
                    }
                    RefNetworksBrief[Routine] = networksBrief;
                    break;
                case ChangeType.Clear:
                    RefNetworksBrief.Clear();
                    break;
                default:
                    break;
            }
            RefNetworksBriefChanged.Invoke(new RefNetworksBriefChangedEventArgs(Type, Routine));
        }

        public ProjectModel()
        {
        }

        public ProjectModel(string projectname, ReportOutputModel _outputmodel)
        {
            ProjectName = projectname;
            MainRoutine = new LadderDiagramViewModel("Main", this);
            MainRoutine.IsMainLadder = true;
            MTVModel = new ModbusTableViewModel();
            MTVModel.PLCDevice = CurrentDevice;
            OModel = _outputmodel;
        }

        public void MainRoutine_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LadderNetworks")
            {
                UpdateNetworkBriefs(sender as LadderDiagramViewModel, ChangeType.Modify);
            }
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
            if (MainRoutine != null)
            {
                if (MainRoutine.ProgramName == name)
                    return MainRoutine;
            }
            foreach (var routine in SubRoutines)
            {
                if (routine.ProgramName == name)
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
            foreach (var funcb in FuncBlocks)
            {
                if (funcb.ProgramName == name)
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
            TreeViewItem item = new TreeViewItem();
            item.Header = ldmodel;
            SubRoutineTreeViewItems.Add(item);
            UpdateNetworkBriefs(ldmodel, ChangeType.Add);
            ldmodel.PropertyChanged += MainRoutine_PropertyChanged;
        }
        /// <summary>
        /// Add FunctionBlock 
        /// </summary>
        /// <param name="fbmodel"></param>
        public void AddFuncBlock(FuncBlockViewModel fbmodel)
        {
            fbmodel.OModel = OModel;
            FuncBlocks.Add(fbmodel);
        }

        public void RemoveSubRoutine(LadderDiagramViewModel ldmodel)
        {
            SubRoutines.Remove(ldmodel);
            foreach (var item in SubRoutineTreeViewItems)
            {
                if (item.Header == ldmodel)
                {
                    SubRoutineTreeViewItems.Remove(item);
                    break;
                }
            }
            UpdateNetworkBriefs(ldmodel, ChangeType.Remove);
            ldmodel.PropertyChanged -= MainRoutine_PropertyChanged;
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
            foreach (var item in SubRoutines)
            {
                item.PropertyChanged -= MainRoutine_PropertyChanged;
            }
            SubRoutines.Clear();
            SubRoutineTreeViewItems.Clear();
            UpdateNetworkBriefs(null, ChangeType.Clear);
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
                    SubRoutineTreeViewItems.Add(item);
                    ldmodel.PropertyChanged += MainRoutine_PropertyChanged;
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
            var mtmodel = new ModbusTableViewModel();
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

        public void CompileFuncBlock(string name)
        {
            FuncBlockViewModel fbvmodel = null;
            foreach (FuncBlockViewModel _fbvmodel in FuncBlocks)
            {
                if (_fbvmodel.ProgramName.Equals(name))
                {
                    fbvmodel = _fbvmodel;
                    break;
                }
            }
            if (fbvmodel == null)
            {
                return;
            }

            string fbfile = SamSoarII.Utility.FileHelper.GetTempFile(".c");
            string oofile = SamSoarII.Utility.FileHelper.GetTempFile(".o");
            File.WriteAllText(fbfile, fbvmodel.Code);
            Process cmd = new Process();
            cmd.StartInfo.FileName = "i686-w64-mingw32-gcc";
            cmd.StartInfo.Arguments = string.Format("{0} -o {1}", fbfile, oofile);
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

