using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Controls;

using SamSoarII.GenerateModel;
using SamSoarII.AppMain.UI;
using SamSoarII.Extend.Utility;
using System.Diagnostics;
using System.Windows;

namespace SamSoarII.AppMain.Project
{
    class GenerateModel
    {
        private string _name;

        private GenerateViewModel _generateViewModel;

        private ProjectModel _projectModel;

        private GenerateTreeItem _treeItemCC;
        private GenerateTreeItem _treeItemGI;
        private GenerateTreeItem _treeItemPM;
        private GenerateTreeItem _treeItemGB;
        private GenerateTreeItem _treeItemDL;

        private TextBox report;

        private int _netnum;
        private int _netnum_done;
      
        public GenerateModel()
        {
            //_generateViewModel = new GenerateViewModel();
        }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public GenerateViewModel GVModel
        {
            get { return this._generateViewModel; }
        }

        public ProjectModel ProjModel
        {
            get { return this._projectModel; }
            set
            {
                this._projectModel = value;
                //CreateViewModel_FromProjectModel(value);
            }
        }

        public GenerateTreeItem TreeItemCC
        {
            set { this._treeItemCC = value; }
        }
        public GenerateTreeItem TreeItemGI
        {
            set { this._treeItemGI = value; }
        }
        public GenerateTreeItem TreeItemPM
        {
            set { this._treeItemPM = value; }
        }
        public GenerateTreeItem TreeItemGB
        {
            set { this._treeItemGB = value; }
        }
        public GenerateTreeItem TreeItemDL
        {
            set { this._treeItemDL = value; }
        }
        public TextBox ReportTextBox
        {
            set { this.report = value; }
        }


        #region Create a new GenerateModel
        public void CreateViewModel_FromProjectModel(ProjectModel pmodel)
        {
            _projectModel = pmodel;
            Name = pmodel.ProjectName;

            _generateViewModel = new GenerateViewModel();
            _generateViewModel.MRModel = new GenerateRoutineModel();
            _generateViewModel.SRModels = new List<GenerateRoutineModel>();
            _generateViewModel.FBModel = new GenerateFuncBlockModel();

            _netnum = 0;

            CreateRoutineModel_FromDiagramModel(_generateViewModel.MRModel, pmodel.MainRoutine);
            foreach (LadderDiagramViewModel submodel in pmodel.SubRoutines)
            {
                GenerateRoutineModel grmodel = new GenerateRoutineModel();
                CreateRoutineModel_FromDiagramModel(grmodel, submodel);
                _generateViewModel.SRModels.Add(grmodel);
            }

            foreach (FuncBlockViewModel fbvmodel in pmodel.FuncBlocks)
                CreateFuncBlockModel(fbvmodel);
        }

        public void CreateRoutineModel_FromDiagramModel(GenerateRoutineModel grmodel, LadderDiagramViewModel ldvmodel)
        {
            ldvmodel.Dispatcher.Invoke(() => {grmodel.Name = ldvmodel.Name;});
            
            grmodel.NetworkModels = new LinkedList<GenerateNetworkModel>();
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                GenerateNetworkModel gnmodel = new GenerateNetworkModel();
                gnmodel.ID = grmodel.NetworkModels.Count() + 1;
                gnmodel.Name = "NETWORK" + gnmodel.ID;
                CreateNetworkModel_FromNetworkModel(gnmodel, lnvmodel);
                grmodel.NetworkModels.AddLast(gnmodel);
            }
        }

        public void CreateNetworkModel_FromNetworkModel(GenerateNetworkModel gnmodel, LadderNetworkViewModel lnvmodel)
        {
            _netnum++;
            gnmodel.LChart = GenerateHelper.CreateLadderChart(lnvmodel.GetAllLadderViewModels());
        }

        public void CreateFuncBlockModel(FuncBlockViewModel fbvmodel)
        {
            if (_generateViewModel.FBModel == null)
                _generateViewModel.FBModel = new GenerateFuncBlockModel();
            _generateViewModel.FBModel.Append(fbvmodel.Code);
        }

        #endregion

        #region Generate Function

        #region Check Circuit
        public int CheckCircuit()
        {
            int ret = 0;
            report.Dispatcher.Invoke(() => { report.Text += String.Format("开始检查工程{0:s}...\r\n", Name); });
            _netnum_done = 0;
            _treeItemCC.Value = 0;
            ret += CheckCircuit_ForRoutine(_generateViewModel.MRModel);
            foreach (GenerateRoutineModel grmodel in _generateViewModel.SRModels)
                ret += CheckCircuit_ForRoutine(grmodel);
            return ret;
        }

        public int CheckCircuit_ForRoutine(GenerateRoutineModel grmodel)
        {
            int ret = 0;
            report.Dispatcher.Invoke(() => { report.Text += String.Format("开始检查程序{0:s}...\r\n", grmodel.Name); });
            foreach (GenerateNetworkModel gnmodel in grmodel.NetworkModels)
                ret += CheckCircuit_ForNetwork(gnmodel);
            return ret;
        }

        public int CheckCircuit_ForNetwork(GenerateNetworkModel gnmodel)
        {
            int ret = 0;
            report.Dispatcher.Invoke(() => { report.Text += String.Format("开始检查程序{0:s}的线路{1:d}...\r\n", gnmodel.Name, gnmodel.ID); });
            gnmodel.LGraph = gnmodel.LChart.Generate();
            if (gnmodel.LGraph.checkOpenCircuit())
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("程序{0:s}的线路{1:d}发生断路错误！\r\n", gnmodel.Name, gnmodel.ID); });
                ret++;
            }
            if (ret > 0) return ret;
            _treeItemCC.Value = (_netnum_done + 0.333) / _netnum;
            if (gnmodel.LGraph.checkShortCircuit())
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("程序{0:s}的线路{1:d}发生短路错误！\r\n", gnmodel.Name, gnmodel.ID); });
                ret++;
            }
            if (ret > 0) return ret;
            _treeItemCC.Value = (_netnum_done + 0.666) / _netnum;
            if (gnmodel.LGraph.CheckFusionCircuit())
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("程序{0:s}的线路{1:d}发生混联错误！\r\n", gnmodel.Name, gnmodel.ID); });
                ret++;
            }
            _treeItemCC.Value = (_netnum_done + 1.0) / _netnum;
            _netnum_done++;
            return ret;
        }
        #endregion

        #region Generate PLC Instructions

        public int GeneratePLCInsts()
        {
            int ret = 0;
            report.Dispatcher.Invoke(() => { report.Text += String.Format("开始生成PLC指令程序...\r\n"); });
            _netnum_done = 0;
            _treeItemGI.Value = 0.0;
            ret += GeneratePLCInsts_ForRoutine(_generateViewModel.MRModel);
            foreach (GenerateRoutineModel grmodel in _generateViewModel.SRModels)
            {
                ret += GeneratePLCInsts_ForRoutine(grmodel);
            }
            _treeItemGI.Value = 1.0;
            return ret;
        }

        public int GeneratePLCInsts_ForRoutine(GenerateRoutineModel grmodel)
        {
            int ret = 0;
            report.Dispatcher.Invoke(() => { report.Text += String.Format("开始生成程序{0:s}的指令程序...\r\n", grmodel.Name); });
            foreach (GenerateNetworkModel gnmodel in grmodel.NetworkModels)
            {
                ret += GeneratePLCInsts_ForNetwork(gnmodel);
            }
            return ret;
        }

        public int GeneratePLCInsts_ForNetwork(GenerateNetworkModel gnmodel)
        {
            int ret = 0;
            report.Dispatcher.Invoke(() => { report.Text += String.Format("开始生成程序{0:s}的线路{1:d}的指令程序...\r\n", gnmodel.Name, gnmodel.ID); });
            gnmodel.PLCInsts = gnmodel.LGraph.GenInst();
            _netnum_done++;
            _treeItemGI.Value = _netnum_done / _netnum;
            return ret;
        }

        #endregion

        #region Premeasurement
        public int Premeasurement()
        {
            int ret = 0;
            _treeItemPM.Value = 0.0;
            // DO SOMETHING
            _treeItemPM.Value = 1.0;
            return ret;
        }
        #endregion

        #region generate BINARY execute program
        public int GenerateBin()
        {
            int ret = 0;
            List<InstHelper.PLCInstNetwork> networks = new List<InstHelper.PLCInstNetwork>();
            ret += GenerateBin_ForRoutine(networks, _generateViewModel.MRModel);
            foreach (GenerateRoutineModel grmodel in _generateViewModel.SRModels)
            {
                ret += GenerateBin_ForRoutine(networks, grmodel);
            }
            if (ret > 0)
                return ret;
            
            string ladderFile = SamSoarII.Utility.FileHelper.GetTempFile(".c");
            string funcBlockFile = SamSoarII.Utility.FileHelper.GetTempFile(".c");
            string outputBinaryFile = SamSoarII.Utility.FileHelper.GetTempFile(".bin");
            string tempPath = Path.GetTempPath();
            string currentPath = Environment.CurrentDirectory;

            StreamWriter sw = new StreamWriter(ladderFile);
            InstHelper.InstToCCode(sw, networks.ToArray());
            File.WriteAllText(funcBlockFile, _generateViewModel.FBModel.Text);

            ladderFile = String.Format("/cygdrive/c/{0}", ladderFile.Substring(3));
            funcBlockFile = String.Format("/cygdrive/c/{0}", funcBlockFile.Substring(3));
            outputBinaryFile = String.Format("/cygdrive/c/{0}", outputBinaryFile.Substring(3));
            tempPath = String.Format("/cygdrive/c/{0}", tempPath.Substring(3));
            currentPath = String.Format("/cygdrive/c/{0}", currentPath.Substring(3));

            ladderFile = ladderFile.Replace('\\', '/');
            funcBlockFile = funcBlockFile.Replace('\\', '/');
            outputBinaryFile = outputBinaryFile.Replace('\\', '/');
            tempPath = tempPath.Replace('\\', '/');
            currentPath = currentPath.Replace('\\', '/');

            Process cmd = new Process();
            cmd.StartInfo.FileName = String.Format(@"{0}\cygwin64\Cygwin.bat", Environment.CurrentDirectory);
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            
            if (cmd.Start())
            {
                cmd.StandardInput.Write("export PATH={0:s}/arm/bin:$PATH\r\n", currentPath);
                cmd.StandardInput.Write("cp -r {0:s}/F407PLC {1:s}\r\n", currentPath, tempPath);
                cmd.StandardInput.Write("cp {0:s} {1:s}F407PLC/src/application/main.c\r\n", ladderFile, tempPath);
                cmd.StandardInput.Write("cp {0:s} {1:s}F407PLC/src/application/func.c\r\n", funcBlockFile, tempPath);
                cmd.StandardInput.Write("make -C {0:s}F407PLC\r\n", tempPath);
                cmd.StandardInput.Write("cp {0:s}F407PLC/libF407PLC.a {1:s}\r\n", tempPath, currentPath);
                cmd.StandardInput.Write("rm -r {0:s}F407PLC\r\n", tempPath);
                cmd.StandardInput.Write("exit\r\n");
            }
            cmd.WaitForExit();
            string s = string.Format("stdout : {0}\r\nstderr: {1}\r\n", cmd.StandardOutput.ReadToEnd(), cmd.StandardError.ReadToEnd());
            //MessageBox.Show(s);
            report.Dispatcher.Invoke(() => { report.Text += s; });

            return ret;
        }

        public int GenerateBin_ForRoutine(List<InstHelper.PLCInstNetwork> nets, GenerateRoutineModel grmodel)
        {
            int ret = 0;
            foreach (GenerateNetworkModel gnmodel in grmodel.NetworkModels)
            {
                ret += GenerateBin_ForNetwork(nets, gnmodel);
            }
            return ret;
        }

        public int GenerateBin_ForNetwork(List<InstHelper.PLCInstNetwork> nets, GenerateNetworkModel gnmodel)
        {
            int ret = 0;
            InstHelper.PLCInstNetwork net = new InstHelper.PLCInstNetwork(
                gnmodel.Name, gnmodel.PLCInsts.ToArray());
            nets.Add(net);
            return ret;
        }
        #endregion

        #region Download to the PLC device
        public int Download()
        {
            int ret = 0;
            return ret;
        }
        #endregion

        #endregion

        #region Generate Process
        
        private enum STATUS_POSITION
        {
            START, CHECKCIRCUIT, PLCINSTRUCTION, PREMEASUREMENT, GENERATEBIN, DOWNLOAD
        };
        private enum STATUS_RESULT
        {
            RUNNING, WORK, ERROR, UNKNOWN
        };
        private STATUS_POSITION _status_position;
        private STATUS_RESULT _status_result;
        private STATUS_POSITION StatusPosition
        {
            get { return this._status_position; }
            set
            {
                this._status_position = value;
                int treeItemStatus = 0;
                switch (this._status_result)
                {
                    case STATUS_RESULT.RUNNING:
                        treeItemStatus = GenerateTreeItem.STATUS_RUNNING; break;
                    case STATUS_RESULT.WORK:
                        treeItemStatus = GenerateTreeItem.STATUS_FINISH; break;
                    case STATUS_RESULT.ERROR:
                        treeItemStatus = GenerateTreeItem.STATUS_ERROR; break;
                    case STATUS_RESULT.UNKNOWN:
                        treeItemStatus = GenerateTreeItem.STATUS_NOTRUN; break;
                }
                switch (this._status_position)
                {
                    case STATUS_POSITION.START:
                        treeItemStatus = GenerateTreeItem.STATUS_NOTRUN;
                        _treeItemCC.Status = treeItemStatus;
                        _treeItemGI.Status = treeItemStatus;
                        _treeItemPM.Status = treeItemStatus;
                        _treeItemGB.Status = treeItemStatus;
                        _treeItemDL.Status = treeItemStatus;
                        break;
                    case STATUS_POSITION.CHECKCIRCUIT:
                        _treeItemCC.Status = treeItemStatus; break;
                    case STATUS_POSITION.PLCINSTRUCTION:
                        _treeItemGI.Status = treeItemStatus; break;
                    case STATUS_POSITION.PREMEASUREMENT:
                        _treeItemPM.Status = treeItemStatus; break;
                    case STATUS_POSITION.GENERATEBIN:
                        _treeItemGB.Status = treeItemStatus; break;
                    case STATUS_POSITION.DOWNLOAD:
                        _treeItemDL.Status = treeItemStatus; break;
                }
            }
        }
        private STATUS_RESULT StatusResult
        {
            get { return this._status_result; }
            set { this._status_result = value; }
        }

        public void GenerateProcess_Init()
        {
            StatusPosition = STATUS_POSITION.START;
            report.Text = String.Empty;
        }

        public int GenerateProcess_CheckCircuit()
        {
            if (StatusPosition == STATUS_POSITION.CHECKCIRCUIT &&
                StatusResult == STATUS_RESULT.WORK)
            {
                return 0;
            }
            CreateViewModel_FromProjectModel(_projectModel);
            StatusResult = STATUS_RESULT.RUNNING;
            StatusPosition = STATUS_POSITION.CHECKCIRCUIT;
            int ret = CheckCircuit();
            if (ret > 0)
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("总共{0:d}处错误，生成终止。\r\n", ret); });
                StatusResult = STATUS_RESULT.ERROR;
                StatusPosition = STATUS_POSITION.CHECKCIRCUIT;
            }
            else
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("工程检查完毕。\r\n"); });
                StatusResult = STATUS_RESULT.WORK;
                StatusPosition = STATUS_POSITION.CHECKCIRCUIT;
            }
            return ret;
        }

        public int GenerateProcess_GeneratePLCInstruction()
        {
            int ret = 0;
            if (StatusPosition == STATUS_POSITION.PLCINSTRUCTION &&
                StatusResult == STATUS_RESULT.WORK)
            {
                return ret;
            }
            if (StatusPosition < STATUS_POSITION.PLCINSTRUCTION)
            {
                ret += GenerateProcess_CheckCircuit();
                if (ret > 0)
                    return ret;
            }
            StatusResult = STATUS_RESULT.RUNNING;
            StatusPosition = STATUS_POSITION.PLCINSTRUCTION;
            ret += GeneratePLCInsts();
            if (ret > 0)
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("总共{0:d}处错误，生成终止。\r\n", ret); });
                StatusResult = STATUS_RESULT.ERROR;
                StatusPosition = STATUS_POSITION.PLCINSTRUCTION;
            }
            else
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("PLC指令生成完毕。\r\n"); });
                StatusResult = STATUS_RESULT.WORK;
                StatusPosition = STATUS_POSITION.PLCINSTRUCTION;
            }
            return ret;
        }

        public int GenerateProcess_Premeasurement()
        {
            int ret = 0;
            if (StatusPosition == STATUS_POSITION.PREMEASUREMENT &&
                StatusResult == STATUS_RESULT.WORK)
            {
                return ret;
            }
            if (StatusPosition < STATUS_POSITION.PREMEASUREMENT)
            {
                ret += GenerateProcess_GeneratePLCInstruction();
                if (ret > 0)
                    return ret;
            }
            StatusResult = STATUS_RESULT.RUNNING;
            StatusPosition = STATUS_POSITION.PREMEASUREMENT;
            ret += Premeasurement();
            if (ret > 0)
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("总共{0:d}处错误，生成终止。\r\n", ret); });
                StatusResult = STATUS_RESULT.ERROR;
                StatusPosition = STATUS_POSITION.PREMEASUREMENT;
            }
            else
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("性能测评完毕。\r\n"); });
                StatusResult = STATUS_RESULT.WORK;
                StatusPosition = STATUS_POSITION.PREMEASUREMENT;
            }
            return ret;
        }

        public int GenerateProcess_GenerateBin()
        {
            int ret = 0;
            if (StatusPosition == STATUS_POSITION.GENERATEBIN &&
                StatusResult == STATUS_RESULT.WORK)
            {
                return ret;
            }
            if (StatusPosition < STATUS_POSITION.GENERATEBIN)
            {
                ret += GenerateProcess_Premeasurement();
                if (ret > 0)
                    return ret;
            }
            StatusResult = STATUS_RESULT.RUNNING;
            StatusPosition = STATUS_POSITION.GENERATEBIN;
            ret += GenerateBin();
            if (ret > 0)
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("总共{0:d}处错误，生成终止。\r\n", ret); });
                StatusResult = STATUS_RESULT.ERROR;
                StatusPosition = STATUS_POSITION.GENERATEBIN;
            }
            else
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("设备写入程序生成完毕。\r\n"); });
                StatusResult = STATUS_RESULT.WORK;
                StatusPosition = STATUS_POSITION.GENERATEBIN;
            }
            return ret;
        }

        public int GenerateProcess_Download()
        {
            int ret = 0;
            if (StatusPosition == STATUS_POSITION.DOWNLOAD &&
                 StatusResult == STATUS_RESULT.WORK)
            {
                return ret;
            }
            if (StatusPosition < STATUS_POSITION.DOWNLOAD)
            {
                ret += GenerateProcess_GenerateBin();
                if (ret > 0)
                    return ret;
            }
            StatusResult = STATUS_RESULT.RUNNING;
            StatusPosition = STATUS_POSITION.DOWNLOAD;
            ret += Download();
            if (ret > 0)
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("总共{0:d}处错误，生成终止。\r\n", ret); });
                StatusResult = STATUS_RESULT.ERROR;
                StatusPosition = STATUS_POSITION.DOWNLOAD;
            }
            else
            {
                report.Dispatcher.Invoke(() => { report.Text += String.Format("下载完毕，PLC置于运行(WORK)状态！\r\n"); });
                StatusResult = STATUS_RESULT.WORK;
                StatusPosition = STATUS_POSITION.DOWNLOAD;
            }
            return ret;
        }

        #endregion

        
    }
}
