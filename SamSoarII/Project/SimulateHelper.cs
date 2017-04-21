using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using SamSoarII.LadderInstViewModel;
using SamSoarII.Simulation;
using SamSoarII.Simulation.Core;
using SamSoarII.Simulation.Shell.ViewModel;
using SamSoarII.Simulation.Shell;
using SamSoarII.Simulation.UI;
using SamSoarII.LadderInstModel;
using SamSoarII.Extend.LadderChartModel;
using System.Windows.Forms;
using SamSoarII.Simulation.Shell.Event;
using System.ComponentModel;
using SamSoarII.Simulation.UI.Base;
using SamSoarII.Simulation.UI.Chart;

namespace SamSoarII.AppMain.Project
{
    public class SimulateHelper
    {
        static private SimulateModel smodel;
        //static private SimulateReportViewModel srmodel;

        static public SimulateModel SModel
        {
            get { return smodel; }
        }
        
        public const int SIMULATE_OK = SimulateDllModel.LOADDLL_OK;
        public const int CLOSE_OK = 0x00;

        static public int Simulate(InteractionFacade ifacade, ReportOutputModel omodel)
        {
            ProjectModel pmodel = ifacade.ProjectModel;
            AppMain.UI.MainTabControl mtctrl = ifacade.MainTabControl;
            smodel = new SimulateModel();
            SetupSimulateModel(pmodel);
            smodel.ReportTextBox = omodel.Report_Simulate;
            int checkresult = smodel.Check();
            switch (checkresult)
            {
                case SimulateDllModel.LOADDLL_OK:
                    smodel.ShowWindow();
                    smodel.TabOpened += OnTabOpened;
                    mtctrl.ShowSimulateItem += OnTabOpened;
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_DLLFILE:
                    MessageBox.Show("Error : 找不到生成的dll文件\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETBIT:
                    MessageBox.Show("Error : 找不到入口GetBit\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETWORD:
                    MessageBox.Show("Error : 找不到入口GetWord\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETDWORD:
                    MessageBox.Show("Error : 找不到入口GetDWord\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETFLOAT:
                    MessageBox.Show("Error : 找不到入口GetFloat\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETDOUBLE:
                    MessageBox.Show("Error : 找不到入口GetDouble\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETBIT:
                    MessageBox.Show("Error : 找不到入口SetBit\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETWORD:
                    MessageBox.Show("Error : 找不到入口SetWord\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETDWORD:
                    MessageBox.Show("Error : 找不到入口SetDWord\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETFLOAT:
                    MessageBox.Show("Error : 找不到入口SetFloat\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETDOUBLE:
                    MessageBox.Show("Error : 找不到入口SetDouble\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_ADDBITDATAPOINT:
                    MessageBox.Show("Error : 找不到入口AddBitDataPoint\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_ADDDOUBLEATAPOINT:
                    MessageBox.Show("Error : 找不到入口AddDoubleDataPoint\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_ADDFLOATDATAPOINT:
                    MessageBox.Show("Error : 找不到入口AddFloatDataPoint\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_ADDWORDDATAPOINT:
                    MessageBox.Show("Error : 找不到入口AddWordDataPoint\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_ADDDWORDDATAPOINT:
                    MessageBox.Show("Error : 找不到入口AddDWordDataPoint\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_REMOVEBITDATAPOINT:
                    MessageBox.Show("Error : 找不到入口RemoveBitDataPoint\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_REMOVEDOUBLEATAPOINT:
                    MessageBox.Show("Error : 找不到入口RemoveDoubleDataPoint\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_REMOVEFLOATDATAPOINT:
                    MessageBox.Show("Error : 找不到入口RemoveFloatDataPoint\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_REMOVEWORDDATAPOINT:
                    MessageBox.Show("Error : 找不到入口RemoveWordDataPoint\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_REMOVEDWORDDATAPOINT:
                    MessageBox.Show("Error : 找不到入口RemoveDWordDataPoint\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_AFTERRUNLADDER:
                    MessageBox.Show("Error : 找不到入口AfterRunLadder\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_BEFORERUNLADDER:
                    MessageBox.Show("Error : 找不到入口BeforeRunLadder\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_ADDVIEWINPUT:
                    MessageBox.Show("Error : 找不到入口AddViewInput\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_ADDVIEWOUTPUT:
                    MessageBox.Show("Error : 找不到入口AddViewOutput\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_REMOVEVIEWINPUT:
                    MessageBox.Show("Error : 找不到入口RemoveViewInput\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUNd_REMOVEVIEWOUTPUT:
                    MessageBox.Show("Error : 找不到入口RemoveViewOutput\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_RUNDATA:
                    MessageBox.Show("Error : 找不到入口RunData\r\n");
                    break;
                default:
                    MessageBox.Show("Error : 发生未知错误\r\n");
                    break;
            }
            omodel.Append(omodel.Report_All, omodel.Report_Simulate.Text);
            return checkresult;
        }

        static public int Close(InteractionFacade ifacade)
        {
            AppMain.UI.MainTabControl mtctrl = ifacade.MainTabControl;
            mtctrl.ShowSimulateItem -= OnTabOpened;
            mtctrl.ReplaceAllTabsToEdit();
            if (smodel != null)
            {
                smodel.Dispose();
                smodel = null;   
            }
            return CLOSE_OK;
        }

        #region Save & Load GlobalSetting
        public static void SaveGlobalSetting()
        {
            SamSoarII.Simulation.Core.Global.GlobalSetting.Save();
        }

        public static void LoadGlobalSetting()
        {
            SamSoarII.Simulation.Core.Global.GlobalSetting.Load();
        }
        #endregion

        #region Setup
        static public void Setup(SimulateModel _smodel)
        {
            smodel = _smodel;
        }

        static public void SetupSimulateModel(ProjectModel pmodel)
        {
            if (smodel == null) return;

            LadderDiagramViewModel lvdmodel = pmodel.MainRoutine;
            SimuViewDiagramModel svdmodel = new SimuViewDiagramModel(smodel, lvdmodel.Name);
            SetupDiagramModel(svdmodel, lvdmodel);
            smodel.MainRoutine = svdmodel;
            foreach (LadderDiagramViewModel _lvdmodel in pmodel.SubRoutines)
            {
                lvdmodel = _lvdmodel;
                svdmodel = new SimuViewDiagramModel(smodel, lvdmodel.Name);
                SetupDiagramModel(svdmodel, lvdmodel);
                smodel.SubRoutines.Add(svdmodel);
            }
            smodel.AllRoutine = new SimuViewAllDiaModel(smodel);

            foreach (FuncBlockViewModel fbvmodel in pmodel.FuncBlocks)
            {
                SimuViewFuncBlockModel svfbmodel = new SimuViewFuncBlockModel("");
                SetupFuncBlockModel(svfbmodel, fbvmodel);
                smodel.FuncBlocks.Add(svfbmodel);
            }
            smodel.AllFuncs = new SimuViewFuncBlockModel("所有函数");
            foreach (FuncBlockViewModel fbvmodel in pmodel.FuncBlocks)
            {
                smodel.AllFuncs.Code += fbvmodel.Code;
            }
        }
        
        private static void SetupDiagramModel(SimuViewDiagramModel svdmodel, LadderDiagramViewModel lvdmodel)
        {
            svdmodel.Name = lvdmodel.ProgramName;
            List<LadderChart> lcharts = new List<LadderChart>();
            foreach (LadderNetworkViewModel lnvmodel in lvdmodel.GetNetworks())
            {
                SimuViewNetworkModel svnmodel = new SimuViewNetworkModel(svdmodel, svdmodel.NetworkCount + 1);
                SetupNetworkModel(svnmodel, lnvmodel, lcharts);
                svdmodel.AppendNetwork(svnmodel);
            }
            //svdmodel.Setup(lcharts.ToArray());
        }

        private static void SetupNetworkModel(SimuViewNetworkModel svnmodel, LadderNetworkViewModel lvnmodel, List<LadderChart> lcharts)
        {
            svnmodel.RowCount = lvnmodel.RowCount;
            svnmodel.NetworkDescription = lvnmodel.NetworkDescription;
            svnmodel.NetworkBrief = lvnmodel.NetworkBrief;
            foreach (BaseViewModel bvmodel in lvnmodel.GetElements())
            {
                SimuViewBaseModel svbmodel = null;
                svbmodel = ConvertViewModel(bvmodel);
                svnmodel.ReplaceElement(svbmodel);
            }
            foreach (VerticalLineViewModel vlmodel in lvnmodel.GetVerticalLines())
            {
                SimuViewVLineModel svvmodel = null;
                svvmodel = (SimuViewVLineModel)(ConvertViewModel(vlmodel));
                svnmodel.ReplaceVerticalLine(svvmodel);
            }
            LadderChart lchart = GenerateHelper.CreateLadderChart(lvnmodel.GetElements().Union(lvnmodel.GetVerticalLines()));
            svnmodel.Setup(lchart);
            lcharts.Add(lchart);
        }

        private static void SetupFuncBlockModel(SimuViewFuncBlockModel svfbmodel, FuncBlockViewModel fbvmodel)
        {
            svfbmodel.FuncBlockName = fbvmodel.FuncBlockName;
            svfbmodel.Code = fbvmodel.Code;
        }

        private static SimuViewBaseModel ConvertViewModel(BaseViewModel bvmodel)
        {
            SimuViewBaseModel svbmodel = null;
            if (bvmodel is HorizontalLineViewModel)
            {
                svbmodel = new SimuViewHLineModel(smodel);
            }
            if (bvmodel is VerticalLineViewModel)
            {
                svbmodel = new SimuViewVLineModel(smodel);
            }
            if (bvmodel is InputBaseViewModel)
            {
                svbmodel = new SimuViewInputModel(smodel);
            }
            if (bvmodel is OutputBaseViewModel)
            {
                svbmodel = new SimuViewOutBitModel(smodel);
            }
            if (bvmodel is OutputRectBaseViewModel)
            {
                svbmodel = new SimuViewOutRecModel(smodel);
            }
            if (bvmodel is SpecialBaseViewModel)
            {
                svbmodel = new SimuViewSpecialModel(smodel);
            }
            if (bvmodel is ALTViewModel)
            {
                svbmodel.Setup(String.Format("ALT {0:s}",
                    ((ALTModel)(bvmodel.Model)).Value.ValueShowString));
            }
            if (bvmodel is ALTPViewModel)
            {
                svbmodel.Setup(String.Format("ALTP {0:s}",
                    ((ALTPModel)(bvmodel.Model)).Value.ValueShowString));
            }
            if (bvmodel is INVViewModel)
            {
                svbmodel.Setup(String.Format("INV"));
            }
            if (bvmodel is LDFViewModel)
            {
                svbmodel.Setup(String.Format("LDF {0:s}",
                    ((LDFModel)(bvmodel.Model)).Value.ValueShowString));
            }
            if (bvmodel is LDIIMViewModel)
            {
                svbmodel.Setup(String.Format("LDIIM {0:s}",
                    ((LDIIMModel)(bvmodel.Model)).Value.ValueShowString));
            }
            if (bvmodel is LDIMViewModel)
            {
                svbmodel.Setup(String.Format("LDIM {0:s}",
                    ((LDIMModel)(bvmodel.Model)).Value.ValueShowString));
            }
            if (bvmodel is LDIViewModel)
            {
                svbmodel.Setup(String.Format("LDI {0:s}",
                    ((LDIModel)(bvmodel.Model)).Value.ValueShowString));
            }
            if (bvmodel is LDViewModel)
            {
                svbmodel.Setup(String.Format("LD {0:s}",
                    ((LDModel)(bvmodel.Model)).Value.ValueShowString));
            }
            if (bvmodel is LDPViewModel)
            {
                svbmodel.Setup(String.Format("LDP {0:s}",
                    ((LDPModel)(bvmodel.Model)).Value.ValueShowString));
            }
            if (bvmodel is MEFViewModel)
            {
                svbmodel.Setup(String.Format("MEF"));
            }
            if (bvmodel is MEPViewModel)
            {
                svbmodel.Setup(String.Format("MEP"));
            }
            if (bvmodel is OUTIMViewModel)
            {
                svbmodel.Setup(String.Format("OUTIM {0:s}",
                    ((OUTIMModel)(bvmodel.Model)).Value.ValueShowString));
            }
            if (bvmodel is OUTViewModel)
            {
                svbmodel.Setup(String.Format("OUT {0:s}",
                    ((OUTModel)(bvmodel.Model)).Value.ValueShowString));
            }
            if (bvmodel is RSTIMViewModel)
            {
                svbmodel.Setup(String.Format("RSTIM {0:s} {1:s}",
                    ((RSTIMModel)(bvmodel.Model)).Value.ValueShowString,
                    ((RSTIMModel)(bvmodel.Model)).Count.ValueShowString));
            }
            if (bvmodel is RSTViewModel)
            {
                svbmodel.Setup(String.Format("RST {0:s} {1:s}",
                    ((RSTModel)(bvmodel.Model)).Value.ValueShowString,
                    ((RSTModel)(bvmodel.Model)).Count.ValueShowString));
            }
            if (bvmodel is SETIMViewModel)
            {
                svbmodel.Setup(String.Format("SETIM {0:s} {1:s}",
                    ((SETIMModel)(bvmodel.Model)).Value.ValueShowString,
                    ((SETIMModel)(bvmodel.Model)).Count.ValueShowString));
            }
            if (bvmodel is SETViewModel)
            {
                svbmodel.Setup(String.Format("SET {0:s} {1:s}",
                    ((SETModel)(bvmodel.Model)).Value.ValueShowString,
                    ((SETModel)(bvmodel.Model)).Count.ValueShowString));
            }
            if (bvmodel is LDDEQViewModel)
            {
                svbmodel.Setup(String.Format("LDDEQ {0:s} {1:s}",
                    ((LDDEQModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDDEQModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDDNEViewModel)
            {
                svbmodel.Setup(String.Format("LDDNE {0:s} {1:s}",
                    ((LDDNEModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDDNEModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDDGEViewModel)
            {
                svbmodel.Setup(String.Format("LDDGE {0:s} {1:s}",
                    ((LDDGEModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDDGEModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDDLEViewModel)
            {
                svbmodel.Setup(String.Format("LDDLE {0:s} {1:s}",
                    ((LDDLEModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDDLEModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDDLViewModel)
            {
                svbmodel.Setup(String.Format("LDDL {0:s} {1:s}",
                    ((LDDLModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDDLModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDDGViewModel)
            {
                svbmodel.Setup(String.Format("LDDG {0:s} {1:s}",
                    ((LDDGModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDDGModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDFEQViewModel)
            {
                svbmodel.Setup(String.Format("LDFEQ {0:s} {1:s}",
                    ((LDFEQModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDFEQModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDFNEViewModel)
            {
                svbmodel.Setup(String.Format("LDFNE {0:s} {1:s}",
                    ((LDFNEModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDFNEModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDFGEViewModel)
            {
                svbmodel.Setup(String.Format("LDFGE {0:s} {1:s}",
                    ((LDFGEModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDFGEModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDFLEViewModel)
            {
                svbmodel.Setup(String.Format("LDFLE {0:s} {1:s}",
                    ((LDFLEModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDFLEModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDFLViewModel)
            {
                svbmodel.Setup(String.Format("LDFL {0:s} {1:s}",
                    ((LDFLModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDFLModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDFGViewModel)
            {
                svbmodel.Setup(String.Format("LDFG {0:s} {1:s}",
                    ((LDFGModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDFGModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDWEQViewModel)
            {
                svbmodel.Setup(String.Format("LDWEQ {0:s} {1:s}",
                    ((LDWEQModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDWEQModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDWNEViewModel)
            {
                svbmodel.Setup(String.Format("LDWNE {0:s} {1:s}",
                    ((LDWNEModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDWNEModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDWGEViewModel)
            {
                svbmodel.Setup(String.Format("LDWGE {0:s} {1:s}",
                    ((LDWGEModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDWGEModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDWLEViewModel)
            {
                svbmodel.Setup(String.Format("LDWLE {0:s} {1:s}",
                    ((LDWLEModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDWLEModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDWLViewModel)
            {
                svbmodel.Setup(String.Format("LDWL {0:s} {1:s}",
                    ((LDWLModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDWLModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is LDWGViewModel)
            {
                svbmodel.Setup(String.Format("LDWG {0:s} {1:s}",
                    ((LDWGModel)(bvmodel.Model)).Value1.ValueShowString,
                    ((LDWGModel)(bvmodel.Model)).Value2.ValueShowString));
            }
            if (bvmodel is BCDViewModel)
            {
                svbmodel.Setup(String.Format("BCD {0:s} {1:s}",
                    ((BCDModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((BCDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is BINViewModel)
            {
                svbmodel.Setup(String.Format("BIN {0:s} {1:s}",
                    ((BINModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((BINModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is DTOFViewModel)
            {
                svbmodel.Setup(String.Format("DTOF {0:s} {1:s}",
                    ((DTOFModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((DTOFModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is DTOWViewModel)
            {
                svbmodel.Setup(String.Format("DTOW {0:s} {1:s}",
                    ((DTOWModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((DTOWModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is ROUNDViewModel)
            {
                svbmodel.Setup(String.Format("ROUND {0:s} {1:s}",
                    ((ROUNDModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((ROUNDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is TRUNCViewModel)
            {
                svbmodel.Setup(String.Format("TRUNC {0:s} {1:s}",
                    ((TRUNCModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((TRUNCModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is WTODViewModel)
            {
                svbmodel.Setup(String.Format("WTOD {0:s} {1:s}",
                    ((WTODModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((WTODModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is ADDFViewModel)
            {
                svbmodel.Setup(String.Format("ADDF {0:s} {1:s} {2:s}",
                    ((ADDFModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((ADDFModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((ADDFModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is SUBFViewModel)
            {
                svbmodel.Setup(String.Format("ADDF {0:s} {1:s} {2:s}",
                    ((SUBFModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((SUBFModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((SUBFModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is MULFViewModel)
            {
                svbmodel.Setup(String.Format("ADDF {0:s} {1:s} {2:s}",
                    ((MULFModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((MULFModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((MULFModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is DIVFViewModel)
            {
                svbmodel.Setup(String.Format("ADDF {0:s} {1:s} {2:s}",
                    ((DIVFModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((DIVFModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((DIVFModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is SINViewModel)
            {
                svbmodel.Setup(String.Format("SIN {0:s} {1:s}",
                    ((SINModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((SINModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is COSViewModel)
            {
                svbmodel.Setup(String.Format("COS {0:s} {1:s}",
                    ((COSModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((COSModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is TANViewModel)
            {
                svbmodel.Setup(String.Format("TAN {0:s} {1:s}",
                    ((TANModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((TANModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is LNViewModel)
            {
                svbmodel.Setup(String.Format("LN {0:s} {1:s}",
                    ((LNModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((LNModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is EXPViewModel)
            {
                svbmodel.Setup(String.Format("EXP {0:s} {1:s}",
                    ((EXPModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((EXPModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is SQRTViewModel)
            {
                svbmodel.Setup(String.Format("SQRT {0:s} {1:s}",
                    ((SQRTModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((SQRTModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is ADDViewModel)
            {
                svbmodel.Setup(String.Format("ADD {0:s} {1:s} {2:s}",
                    ((ADDModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((ADDModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((ADDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is ADDDViewModel)
            {
                svbmodel.Setup(String.Format("ADDD {0:s} {1:s} {2:s}",
                    ((ADDDModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((ADDDModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((ADDDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is SUBViewModel)
            {
                svbmodel.Setup(String.Format("SUB {0:s} {1:s} {2:s}",
                    ((SUBModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((SUBModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((SUBModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is SUBDViewModel)
            {
                svbmodel.Setup(String.Format("SUBD {0:s} {1:s} {2:s}",
                    ((SUBDModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((SUBDModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((SUBDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is MULViewModel)
            {
                svbmodel.Setup(String.Format("MUL {0:s} {1:s} {2:s}",
                    ((MULModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((MULModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((MULModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is MULDViewModel)
            {
                svbmodel.Setup(String.Format("MULD {0:s} {1:s} {2:s}",
                    ((MULDModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((MULDModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((MULDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is MULWViewModel)
            {
                svbmodel.Setup(String.Format("MULW {0:s} {1:s} {2:s}",
                    ((MULWModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((MULWModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((MULWModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is DIVViewModel)
            {
                svbmodel.Setup(String.Format("DIV {0:s} {1:s} {2:s}",
                    ((DIVModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((DIVModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((DIVModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is DIVDViewModel)
            {
                svbmodel.Setup(String.Format("DIVD {0:s} {1:s} {2:s}",
                    ((DIVDModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((DIVDModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((DIVDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is DIVWViewModel)
            {
                svbmodel.Setup(String.Format("DIVW {0:s} {1:s} {2:s}",
                    ((DIVWModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((DIVWModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((DIVWModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is INCViewModel)
            {
                svbmodel.Setup(String.Format("INC {0:s} {1:s}",
                    ((INCModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((INCModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is INCDViewModel)
            {
                svbmodel.Setup(String.Format("INCD {0:s} {1:s}",
                    ((INCDModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((INCDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is DECViewModel)
            {
                svbmodel.Setup(String.Format("DEC {0:s} {1:s}",
                    ((DECModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((DECModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is DECDViewModel)
            {
                svbmodel.Setup(String.Format("DECD {0:s} {1:s}",
                    ((DECDModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((DECDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is ANDWViewModel)
            {
                svbmodel.Setup(String.Format("ANDW {0:s} {1:s} {2:s}",
                       ((ANDWModel)(bvmodel.Model)).InputValue1.ValueShowString,
                       ((ANDWModel)(bvmodel.Model)).InputValue2.ValueShowString,
                       ((ANDWModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is ANDDViewModel)
            {
                svbmodel.Setup(String.Format("ANDD {0:s} {1:s} {2:s}",
                       ((ANDDModel)(bvmodel.Model)).InputValue1.ValueShowString,
                       ((ANDDModel)(bvmodel.Model)).InputValue2.ValueShowString,
                       ((ANDDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is ORWViewModel)
            {
                svbmodel.Setup(String.Format("ORW {0:s} {1:s} {2:s}",
                       ((ORWModel)(bvmodel.Model)).InputValue1.ValueShowString,
                       ((ORWModel)(bvmodel.Model)).InputValue2.ValueShowString,
                       ((ORWModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is ORDViewModel)
            {
                svbmodel.Setup(String.Format("ORD {0:s} {1:s} {2:s}",
                       ((ORDModel)(bvmodel.Model)).InputValue1.ValueShowString,
                       ((ORDModel)(bvmodel.Model)).InputValue2.ValueShowString,
                       ((ORDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is XORWViewModel)
            {
                svbmodel.Setup(String.Format("XORW {0:s} {1:s} {2:s}",
                       ((XORWModel)(bvmodel.Model)).InputValue1.ValueShowString,
                       ((XORWModel)(bvmodel.Model)).InputValue2.ValueShowString,
                       ((XORWModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is XORDViewModel)
            {
                svbmodel.Setup(String.Format("XORD {0:s} {1:s} {2:s}",
                       ((XORDModel)(bvmodel.Model)).InputValue1.ValueShowString,
                       ((XORDModel)(bvmodel.Model)).InputValue2.ValueShowString,
                       ((XORDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is MOVViewModel)
            {
                svbmodel.Setup(String.Format("MOV {0:s} {1:s}",
                       ((MOVModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((MOVModel)(bvmodel.Model)).DestinationValue.ValueShowString));
            }
            if (bvmodel is MOVDViewModel)
            {
                svbmodel.Setup(String.Format("MOVD {0:s} {1:s}",
                       ((MOVDModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((MOVDModel)(bvmodel.Model)).DestinationValue.ValueShowString));
            }
            if (bvmodel is MOVFViewModel)
            {
                svbmodel.Setup(String.Format("MOVF {0:s} {1:s}",
                       ((MOVFModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((MOVFModel)(bvmodel.Model)).DestinationValue.ValueShowString));
            }
            if (bvmodel is MVBLKViewModel)
            {
                svbmodel.Setup(String.Format("MVBLK {0:s} {1:s} {2:s}",
                       ((MVBLKModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((MVBLKModel)(bvmodel.Model)).DestinationValue.ValueShowString,
                       ((MVBLKModel)(bvmodel.Model)).Count.ValueShowString));
            }
            if (bvmodel is MVDBLKViewModel)
            {
                svbmodel.Setup(String.Format("MVDBLK {0:s} {1:s} {2:s}",
                       ((MVDBLKModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((MVDBLKModel)(bvmodel.Model)).DestinationValue.ValueShowString,
                       ((MVDBLKModel)(bvmodel.Model)).Count.ValueShowString));
            }
            if (bvmodel is CALLMViewModel)
            {
                svbmodel.Setup(String.Format("CALLM {0:s}",
                        ((CALLMModel)(bvmodel.Model)).FunctionName));
            }
            if (bvmodel is CALLViewModel)
            {
                svbmodel.Setup(String.Format("CALL {0:s}",
                        ((CALLModel)(bvmodel.Model)).FunctionName));
            }
            if (bvmodel is FORViewModel)
            {
                svbmodel.Setup(String.Format("FOR {0:s}",
                        ((FORModel)(bvmodel.Model)).Count.ValueShowString));
            }
            if (bvmodel is JMPViewModel)
            {
                svbmodel.Setup(String.Format("JMP {0:s}",
                       ((JMPModel)(bvmodel.Model)).LBLIndex.ValueShowString));
            }
            if (bvmodel is LBLViewModel)
            {
                svbmodel.Setup(String.Format("LBL {0:s}",
                       ((LBLModel)(bvmodel.Model)).LBLIndex.ValueShowString));
            }
            if (bvmodel is NEXTViewModel)
            {
                svbmodel.Setup("NEXT");
            }
            if (bvmodel is TRDViewModel)
            {
                svbmodel.Setup(String.Format("TRD {0:s}",
                       ((TRDModel)(bvmodel.Model)).StartValue.ValueShowString));
            }
            if (bvmodel is TWRViewModel)
            {
                svbmodel.Setup(String.Format("TWR {0:s}",
                       ((TWRModel)(bvmodel.Model)).StartValue.ValueShowString));
            }
            if (bvmodel is ROLDViewModel)
            {
                svbmodel.Setup(String.Format("ROLD {0:s} {1:s} {2:s}",
                       ((ROLDModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((ROLDModel)(bvmodel.Model)).DestinationValue.ValueShowString,
                       ((ROLDModel)(bvmodel.Model)).Count.ValueShowString));
            }
            if (bvmodel is ROLViewModel)
            {
                svbmodel.Setup(String.Format("ROL {0:s} {1:s} {2:s}",
                       ((ROLModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((ROLModel)(bvmodel.Model)).DestinationValue.ValueShowString,
                       ((ROLModel)(bvmodel.Model)).Count.ValueShowString));
            }
            if (bvmodel is RORDViewModel)
            {
                svbmodel.Setup(String.Format("RORD {0:s} {1:s} {2:s}",
                       ((RORDModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((RORDModel)(bvmodel.Model)).DestinationValue.ValueShowString,
                       ((RORDModel)(bvmodel.Model)).Count.ValueShowString));
            }
            if (bvmodel is RORViewModel)
            {
                svbmodel.Setup(String.Format("ROR {0:s} {1:s} {2:s}",
                       ((RORModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((RORModel)(bvmodel.Model)).DestinationValue.ValueShowString,
                       ((RORModel)(bvmodel.Model)).Count.ValueShowString));
            }
            if (bvmodel is SHLDViewModel)
            {
                svbmodel.Setup(String.Format("SHLD {0:s} {1:s} {2:s}",
                       ((SHLDModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((SHLDModel)(bvmodel.Model)).DestinationValue.ValueShowString,
                       ((SHLDModel)(bvmodel.Model)).Count.ValueShowString));
            }
            if (bvmodel is SHLViewModel)
            {
                svbmodel.Setup(String.Format("SHL {0:s} {1:s} {2:s}",
                       ((SHLModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((SHLModel)(bvmodel.Model)).DestinationValue.ValueShowString,
                       ((SHLModel)(bvmodel.Model)).Count.ValueShowString));
            }
            if (bvmodel is SHRDViewModel)
            {
                svbmodel.Setup(String.Format("SHRD {0:s} {1:s} {2:s}",
                       ((SHRDModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((SHRDModel)(bvmodel.Model)).DestinationValue.ValueShowString,
                       ((SHRDModel)(bvmodel.Model)).Count.ValueShowString));
            }
            if (bvmodel is SHRViewModel)
            {
                svbmodel.Setup(String.Format("SHR {0:s} {1:s} {2:s}",
                       ((SHRModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((SHRModel)(bvmodel.Model)).DestinationValue.ValueShowString,
                       ((SHRModel)(bvmodel.Model)).Count.ValueShowString));
            }
            svbmodel.X = bvmodel.X;
            svbmodel.Y = bvmodel.Y;
            return svbmodel;
        }
        #endregion

        #region Event Handler

        #region Simulation ViewModel transform to ITabItem
        public class SimulateTabItem : System.Windows.Controls.UserControl, ITabItem
        {
            private string _programName;
            private SimuViewTabModel svtmodel;
            
            public SimulateTabItem(SimuViewTabModel _svtmodel, string programName)
            {
                Content = _svtmodel;
                svtmodel = _svtmodel;
                _programName = programName;
            }

            public string TabHeader
            {
                get
                {
                    return _programName;
                }

                set
                {
                    _programName = value;
                }
            }

            double ITabItem.ActualHeight
            {
                get
                {
                    return svtmodel.ActualHeight;
                }

                set
                {
                    svtmodel.ActualHeight = value;
                }
            }

            double ITabItem.ActualWidth
            {
                get
                {
                    return svtmodel.ActualWidth;
                }

                set
                {
                    svtmodel.ActualWidth = value;
                }
            }
        }

        static public event ShowTabItemEventHandler TabOpen;
        static private void OnTabOpened(object sender, Simulation.Shell.Event.ShowTabItemEventArgs e)
        {
            ShowTabItemEventArgs _e = null;
            SimulateTabItem _stitem = null;
            _e = new ShowTabItemEventArgs(TabType.Simulate);
            if (e.TabName.Equals("所有程序"))
            {
                _stitem = new SimulateTabItem(smodel.AllRoutine, "所有程序");
                if (TabOpen != null)
                {
                    TabOpen(_stitem, _e);
                }            
                return;
            }
            if (e.TabName.Equals("主程序") || e.TabName.Equals("Main"))
            {
                _stitem = new SimulateTabItem(smodel.MainRoutine, "主程序");
                if (TabOpen != null)
                {
                    TabOpen(_stitem, _e);
                }
                return;
            }
            if (e.TabName.Equals("图表"))
            {
                _stitem = new SimulateTabItem(smodel.MainChart, "图表");
                if (TabOpen != null)
                {
                    TabOpen(_stitem, _e);
                }
                return;
            }
            foreach (SimuViewDiagramModel svdmodel in smodel.SubRoutines)
            {
                if (e.TabName.Equals(svdmodel.Name))
                {
                    _stitem = new SimulateTabItem(svdmodel, svdmodel.Name);
                    if (TabOpen != null)
                    {
                        TabOpen(_stitem, _e);
                    }
                    return;
                }
            }
            foreach (SimuViewFuncBlockModel svfmodel in smodel.FuncBlocks)
            {
                if (e.TabName.Equals(svfmodel.Name))
                {
                    _stitem = new SimulateTabItem(svfmodel, svfmodel.Name);
                    if (TabOpen != null)
                    {
                        TabOpen(_stitem, _e);
                    }
                    return;
                }
            }
            foreach (SimuViewXYModel svxmodel in smodel.SubCharts)
            {
                if (e.TabName.Equals(svxmodel.Name))
                {
                    _stitem = new SimulateTabItem(svxmodel, svxmodel.Name);
                    if (TabOpen != null)
                    {
                        TabOpen(_stitem, _e);
                    }
                    return;
                }
            }
        }
        #endregion

        #endregion
    }


}
