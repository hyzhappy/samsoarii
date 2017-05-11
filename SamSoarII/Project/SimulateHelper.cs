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
using SamSoarII.LadderInstViewModel.Interrupt;
using SamSoarII.LadderInstModel.Interrupt;
using SamSoarII.LadderInstModel.Communication;
using SamSoarII.LadderInstViewModel.Pulse;
using SamSoarII.LadderInstModel.Pulse;
using SamSoarII.LadderInstModel.HighCount;
using SamSoarII.LadderInstViewModel.Auxiliar;
using SamSoarII.LadderInstModel.Auxiliar;
using SamSoarII.LadderInstViewModel.Counter;
using SamSoarII.Extend.FuncBlockModel;

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
        public const int SIMULATE_LADDER_ERROR = 0x0100;
        public const int SIMULATE_FUNCBLOCK_ERROR = 0x0101;

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
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETFEQ:
                    MessageBox.Show("Error : 找不到入口GetFeq\r\n");
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
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETFEQ:
                    MessageBox.Show("Error : 找不到入口SetFeq\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_BEFORERUNLADDER:
                    MessageBox.Show("Error : 找不到入口BeforeRunLadder\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_AFTERRUNLADDER:
                    MessageBox.Show("Error : 找不到入口AfterRunLadder\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_INITRUNLADDER:
                    MessageBox.Show("Error : 找不到入口InitRunLadder\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNNT_FOUND_GETCLOCK:
                    MessageBox.Show("Error : 找不到入口GetClock\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_INITCLOCK:
                    MessageBox.Show("Error : 找不到入口InitClock\r\n");
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETCLOCKRATE:
                    MessageBox.Show("Error : 找不到入口SetClockRate\r\n");
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
        
        #region Setup
        static public void Setup(SimulateModel _smodel)
        {
            smodel = _smodel;
        }

        static public void SetupSimulateModel(ProjectModel pmodel)
        {
            if (smodel == null) return;
            
            foreach (FuncBlockViewModel fbvmodel in pmodel.FuncBlocks)
            {
                SimuViewFuncBlockModel svfbmodel = new SimuViewFuncBlockModel("");
                SetupFuncBlockModel(svfbmodel, fbvmodel);
                smodel.FuncBlocks.Add(svfbmodel);
            }
            smodel.AllFuncs = new SimuViewFuncBlockModel("所有函数");
            foreach (SimuViewFuncBlockModel svfbmodel in smodel.FuncBlocks)
            {
                smodel.AllFuncs.Code += svfbmodel.Code;
                smodel.AllFuncs.Headers.AddRange(svfbmodel.Headers);
            }

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

        private static string ConvertCode(string code)
        {
            string ret = code;
            ret = ret.Replace("BIT", "_BIT");
            ret = ret.Replace("WORD", "_WORD");
            ret = ret.Replace("FLOAT", "_FLOAT");
            return ret;
        }

        private static void SetupFuncBlockModel(SimuViewFuncBlockModel svfbmodel, FuncBlockViewModel fbvmodel)
        {
            svfbmodel.FuncBlockName = fbvmodel.FuncBlockName;
            svfbmodel.Code = ConvertCode(fbvmodel.Code);
            foreach (FuncModel fmodel in fbvmodel.Funcs)
            {
                FuncHeaderModel fhmodel = new FuncHeaderModel();
                fhmodel.Name = ConvertCode(fmodel.Name);
                fhmodel.ReturnType = ConvertCode(fmodel.ReturnType);
                fhmodel.ArgCount = fmodel.ArgCount;
                for (int i = 0; i < fhmodel.ArgCount; i++)
                {
                    fhmodel.SetArgName(i, 
                        ConvertCode(
                            fmodel.GetArgName(i)
                        )
                    );
                    fhmodel.SetArgType(i,
                        ConvertCode(
                            fmodel.GetArgType(i)
                        )
                    );
                }
                svfbmodel.Headers.Add(fhmodel);
            }
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
            if (bvmodel is TONViewModel)
            {
                svbmodel.Setup(String.Format("TON {0:s} {1:s}",
                        ((TONModel)(bvmodel.Model)).TimerValue.ValueShowString,
                        ((TONModel)(bvmodel.Model)).EndValue.ValueShowString));
            }
            if (bvmodel is TONRViewModel)
            {
                svbmodel.Setup(String.Format("TONR {0:s} {1:s}",
                        ((TONRModel)(bvmodel.Model)).TimerValue.ValueShowString,
                        ((TONRModel)(bvmodel.Model)).EndValue.ValueShowString));
            }
            if (bvmodel is TOFViewModel)
            {
                svbmodel.Setup(String.Format("TOF {0:s} {1:s}",
                        ((TOFModel)(bvmodel.Model)).TimerValue.ValueShowString,
                        ((TOFModel)(bvmodel.Model)).EndValue.ValueShowString));
            }
            if (bvmodel is CTUViewModel)
            {
                svbmodel.Setup(String.Format("CTU {0:s} {1:s}",
                        ((CTUModel)(bvmodel.Model)).CountValue.ValueShowString,
                        ((CTUModel)(bvmodel.Model)).EndValue.ValueShowString));
            }
            if (bvmodel is CTDViewModel)
            {
                svbmodel.Setup(String.Format("CTU {0:s} {1:s}",
                        ((CTDModel)(bvmodel.Model)).CountValue.ValueShowString,
                        ((CTDModel)(bvmodel.Model)).StartValue.ValueShowString));
            }
            if (bvmodel is CTUDViewModel)
            {
                svbmodel.Setup(String.Format("CTUD {0:s} {1:s}",
                        ((CTUDModel)(bvmodel.Model)).CountValue.ValueShowString,
                        ((CTUDModel)(bvmodel.Model)).EndValue.ValueShowString));
            }
            if (bvmodel is HCNTViewModel)
            {
                svbmodel.Setup(String.Format("HCNT {0:s} {1:s}",
                        ((HCNTModel)(bvmodel.Model)).CountValue.ValueShowString,
                        ((HCNTModel)(bvmodel.Model)).DefineValue.ValueShowString));
            }
            if (bvmodel is CALLMViewModel)
            {
                svbmodel.Setup(String.Format("CALLM {0:s} {1:s} {2:s} {3:s} {4:s}",
                        ((CALLMModel)(bvmodel.Model)).FunctionName,
                        ((CALLMModel)(bvmodel.Model)).Value1.ValueShowString,
                        ((CALLMModel)(bvmodel.Model)).Value2.ValueShowString,
                        ((CALLMModel)(bvmodel.Model)).Value3.ValueShowString,
                        ((CALLMModel)(bvmodel.Model)).Value4.ValueShowString));
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
            if (bvmodel is SHLBViewModel)
            {
                svbmodel.Setup(String.Format("SHLB {0:s} {1:s} {2:s} {3:s}",
                       ((SHLBModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((SHLBModel)(bvmodel.Model)).DestinationValue.ValueShowString,
                       ((SHLBModel)(bvmodel.Model)).CountValue.ValueShowString,
                       ((SHLBModel)(bvmodel.Model)).MoveValue.ValueShowString));
            }
            if (bvmodel is SHRBViewModel)
            {
                svbmodel.Setup(String.Format("SHLB {0:s} {1:s} {2:s} {3:s}",
                       ((SHRBModel)(bvmodel.Model)).SourceValue.ValueShowString,
                       ((SHRBModel)(bvmodel.Model)).DestinationValue.ValueShowString,
                       ((SHRBModel)(bvmodel.Model)).CountValue.ValueShowString,
                       ((SHRBModel)(bvmodel.Model)).MoveValue.ValueShowString));
            }
            if (bvmodel is ATCHViewModel)
            {
                svbmodel.Setup(String.Format("ATCH {0:s} {1:s}",
                    ((ATCHModel)(bvmodel.Model)).IDValue.ValueShowString,
                    ((ATCHModel)(bvmodel.Model)).FuncName));
            }
            if (bvmodel is DTCHViewModel)
            {
                svbmodel.Setup(String.Format("DTCH {0:s}",
                    ((DTCHModel)(bvmodel.Model)).IDValue.ValueShowString));
            }
            if (bvmodel is EIViewModel)
            {
                svbmodel.Setup("EI");
            }
            if (bvmodel is DIViewModel)
            {
                svbmodel.Setup("DI");
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
            if (bvmodel is MBUSViewModel)
            {
                svbmodel.Setup(String.Format("MBUS {0:s} {1:s} {2:s}",
                    ((MBUSModel)(bvmodel.Model)).COMPort.ValueShowString,
                    ((MBUSModel)(bvmodel.Model)).Table,
                    ((MBUSModel)(bvmodel.Model)).Message.ValueShowString));
            }
            if (bvmodel is SENDViewModel)
            {
                svbmodel.Setup(String.Format("SEND {0:s} {1:s} {2:s}",
                    ((SENDModel)(bvmodel.Model)).COMPort.ValueShowString,
                    ((SENDModel)(bvmodel.Model)).BaseValue.ValueShowString,
                    ((SENDModel)(bvmodel.Model)).CountValue.ValueShowString));
            }
            if (bvmodel is REVViewModel)
            {
                svbmodel.Setup(String.Format("REV {0:s} {1:s} {2:s}",
                    ((REVModel)(bvmodel.Model)).COMPort.ValueShowString,
                    ((REVModel)(bvmodel.Model)).BaseValue.ValueShowString,
                    ((REVModel)(bvmodel.Model)).CountValue.ValueShowString));
            }
            if (bvmodel is PLSFViewModel)
            {
                svbmodel.Setup(String.Format("PLSF {0:s} {1:s}",
                    ((PLSFModel)(bvmodel.Model)).FreqValue.ValueShowString,
                    ((PLSFModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is DPLSFViewModel)
            {
                svbmodel.Setup(String.Format("DPLSF {0:s} {1:s}",
                    ((DPLSFModel)(bvmodel.Model)).FreqValue.ValueShowString,
                    ((DPLSFModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is PWMViewModel)
            {
                svbmodel.Setup(String.Format("PWM {0:s} {1:s} {2:s}",
                    ((PWMModel)(bvmodel.Model)).FreqValue.ValueShowString,
                    ((PWMModel)(bvmodel.Model)).DutyCycleValue.ValueShowString,
                    ((PWMModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is DPWMViewModel)
            {
                svbmodel.Setup(String.Format("DPWM {0:s} {1:s} {2:s}",
                    ((DPWMModel)(bvmodel.Model)).FreqValue.ValueShowString,
                    ((DPWMModel)(bvmodel.Model)).DutyCycleValue.ValueShowString,
                    ((DPWMModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is PLSYViewModel)
            {
                svbmodel.Setup(String.Format("PLSY {0:s} {1:s} {2:s}",
                    ((PLSYModel)(bvmodel.Model)).FreqValue.ValueShowString,
                    ((PLSYModel)(bvmodel.Model)).PulseValue.ValueShowString,
                    ((PLSYModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is DPLSYViewModel)
            {
                svbmodel.Setup(String.Format("DPLSY {0:s} {1:s} {2:s}",
                    ((DPLSYModel)(bvmodel.Model)).FreqValue.ValueShowString,
                    ((DPLSYModel)(bvmodel.Model)).PulseValue.ValueShowString,
                    ((DPLSYModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is PLSRViewModel)
            {
                svbmodel.Setup(String.Format("PLSR {0:s} {1:s} {2:s}",
                    ((PLSRModel)(bvmodel.Model)).ArgumentValue.ValueShowString,
                    ((PLSRModel)(bvmodel.Model)).VelocityValue.ValueShowString,
                    ((PLSRModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is DPLSRViewModel)
            {
                svbmodel.Setup(String.Format("DPLSR {0:s} {1:s} {2:s}",
                    ((DPLSRModel)(bvmodel.Model)).ArgumentValue.ValueShowString,
                    ((DPLSRModel)(bvmodel.Model)).VelocityValue.ValueShowString,
                    ((DPLSRModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is PLSRDViewModel)
            {
                svbmodel.Setup(String.Format("PLSRD {0:s} {1:s} {2:s} {3:s}",
                    ((PLSRDModel)(bvmodel.Model)).ArgumentValue.ValueShowString,
                    ((PLSRDModel)(bvmodel.Model)).VelocityValue.ValueShowString,
                    ((PLSRDModel)(bvmodel.Model)).OutputValue1.ValueShowString,
                    ((PLSRDModel)(bvmodel.Model)).OutputValue2.ValueShowString));
            }
            if (bvmodel is DPLSRDViewModel)
            {
                svbmodel.Setup(String.Format("DPLSRD {0:s} {1:s} {2:s} {3:s}",
                    ((DPLSRDModel)(bvmodel.Model)).ArgumentValue.ValueShowString,
                    ((DPLSRDModel)(bvmodel.Model)).VelocityValue.ValueShowString,
                    ((DPLSRDModel)(bvmodel.Model)).OutputValue1.ValueShowString,
                    ((DPLSRDModel)(bvmodel.Model)).OutputValue2.ValueShowString));
            }
            if (bvmodel is PLSNEXTViewModel)
            {
                svbmodel.Setup(String.Format("PLSNEXT {0:s}",
                    ((PLSNEXTModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is PLSSTOPViewModel)
            {
                svbmodel.Setup(String.Format("PLSSTOP {0:s}",
                    ((PLSSTOPModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is ZRNViewModel)
            {
                svbmodel.Setup(String.Format("ZRN {0:s} {1:s} {2:s} {3:s}",
                    ((ZRNModel)(bvmodel.Model)).BackValue.ValueShowString,
                    ((ZRNModel)(bvmodel.Model)).CrawValue.ValueShowString,
                    ((ZRNModel)(bvmodel.Model)).SignalValue.ValueShowString,
                    ((ZRNModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is DZRNViewModel)
            {
                svbmodel.Setup(String.Format("DZRN {0:s} {1:s} {2:s} {3:s}",
                    ((DZRNModel)(bvmodel.Model)).BackValue.ValueShowString,
                    ((DZRNModel)(bvmodel.Model)).CrawValue.ValueShowString,
                    ((DZRNModel)(bvmodel.Model)).SignalValue.ValueShowString,
                    ((DZRNModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is PTOViewModel)
            {
                svbmodel.Setup(String.Format("PTO {0:s} {1:s} {2:s}",
                    ((PTOModel)(bvmodel.Model)).ArgumentValue.ValueShowString,
                    ((PTOModel)(bvmodel.Model)).OutputValue1.ValueShowString,
                    ((PTOModel)(bvmodel.Model)).OutputValue2.ValueShowString));
            }
            if (bvmodel is DRVIViewModel)
            {
                svbmodel.Setup(String.Format("DRVI {0:s} {1:s} {2:s}",
                    ((DRVIModel)(bvmodel.Model)).FreqValue.ValueShowString,
                    ((DRVIModel)(bvmodel.Model)).PulseValue.ValueShowString,
                    ((DRVIModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is DDRVIViewModel)
            {
                svbmodel.Setup(String.Format("DDRVI {0:s} {1:s} {2:s}",
                    ((DDRVIModel)(bvmodel.Model)).FreqValue.ValueShowString,
                    ((DDRVIModel)(bvmodel.Model)).PulseValue.ValueShowString,
                    ((DDRVIModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is HCNTViewModel)
            {
                svbmodel.Setup(String.Format("HCNT {0:s} {1:s} {2:s}",
                    ((HCNTModel)(bvmodel.Model)).DefineValue.ValueShowString,
                    ((HCNTModel)(bvmodel.Model)).CountValue.ValueShowString));
            }
            if (bvmodel is LOGViewModel)
            {
                svbmodel.Setup(String.Format("LOG {0:s} {1:s}",
                    ((LOGModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((LOGModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is POWViewModel)
            {
                svbmodel.Setup(String.Format("POW {0:s} {1:s} {2:s}",
                    ((POWModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((POWModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((POWModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is FACTViewModel)
            {
                svbmodel.Setup(String.Format("FACT {0:s} {1:s}",
                    ((FACTModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((FACTModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is CMPViewModel)
            {
                svbmodel.Setup(String.Format("CMP {0:s} {1:s} {2:s}",
                    ((CMPModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((CMPModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((CMPModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is CMPDViewModel)
            {
                svbmodel.Setup(String.Format("CMPD {0:s} {1:s} {2:s}",
                    ((CMPDModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((CMPDModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((CMPDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is CMPFViewModel)
            {
                svbmodel.Setup(String.Format("CMPF {0:s} {1:s} {2:s}",
                    ((CMPFModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((CMPFModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((CMPFModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is ZCPViewModel)
            {
                svbmodel.Setup(String.Format("ZCP {0:s} {1:s} {2:s} {3:s}",
                    ((ZCPModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((ZCPModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((ZCPModel)(bvmodel.Model)).InputValue3.ValueShowString,
                    ((ZCPModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is ZCPDViewModel)
            {
                svbmodel.Setup(String.Format("ZCPD {0:s} {1:s} {2:s} {3:s}",
                    ((ZCPDModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((ZCPDModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((ZCPDModel)(bvmodel.Model)).InputValue3.ValueShowString,
                    ((ZCPDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is ZCPFViewModel)
            {
                svbmodel.Setup(String.Format("ZCPF {0:s} {1:s} {2:s} {3:s}",
                    ((ZCPFModel)(bvmodel.Model)).InputValue1.ValueShowString,
                    ((ZCPFModel)(bvmodel.Model)).InputValue2.ValueShowString,
                    ((ZCPFModel)(bvmodel.Model)).InputValue3.ValueShowString,
                    ((ZCPFModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is NEGViewModel)
            {
                svbmodel.Setup(String.Format("NEG {0:s} {1:s}",
                    ((NEGModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((NEGModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is NEGDViewModel)
            {
                svbmodel.Setup(String.Format("NEG {0:s} {1:s}",
                    ((NEGDModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((NEGDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is XCHViewModel)
            {
                svbmodel.Setup(String.Format("XCH {0:s} {1:s}",
                    ((XCHModel)(bvmodel.Model)).LeftValue.ValueShowString,
                    ((XCHModel)(bvmodel.Model)).RightValue.ValueShowString));
            }
            if (bvmodel is XCHDViewModel)
            {
                svbmodel.Setup(String.Format("XCHD {0:s} {1:s}",
                    ((XCHDModel)(bvmodel.Model)).LeftValue.ValueShowString,
                    ((XCHDModel)(bvmodel.Model)).RightValue.ValueShowString));
            }
            if (bvmodel is XCHFViewModel)
            {
                svbmodel.Setup(String.Format("XCHF {0:s} {1:s}",
                    ((XCHFModel)(bvmodel.Model)).LeftValue.ValueShowString,
                    ((XCHFModel)(bvmodel.Model)).RightValue.ValueShowString));
            }
            if (bvmodel is CMLViewModel)
            {
                svbmodel.Setup(String.Format("CML {0:s} {1:s}",
                    ((CMLModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((CMLModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is CMLDViewModel)
            {
                svbmodel.Setup(String.Format("CMLD {0:s} {1:s}",
                    ((CMLDModel)(bvmodel.Model)).InputValue.ValueShowString,
                    ((CMLDModel)(bvmodel.Model)).OutputValue.ValueShowString));
            }
            if (bvmodel is FMOVViewModel)
            {
                svbmodel.Setup(String.Format("FMOV {0:s} {1:s} {2:s}",
                    ((FMOVModel)(bvmodel.Model)).SourceValue.ValueShowString,
                    ((FMOVModel)(bvmodel.Model)).CountValue.ValueShowString,
                    ((FMOVModel)(bvmodel.Model)).DestinationValue.ValueShowString));
            }
            if (bvmodel is FMOVDViewModel)
            {
                svbmodel.Setup(String.Format("FMOVD {0:s} {1:s} {2:s}",
                    ((FMOVDModel)(bvmodel.Model)).SourceValue.ValueShowString,
                    ((FMOVDModel)(bvmodel.Model)).CountValue.ValueShowString,
                    ((FMOVDModel)(bvmodel.Model)).DestinationValue.ValueShowString));
            }
            if (bvmodel is SMOVViewModel)
            {
                svbmodel.Setup(String.Format("SMOV {0:s} {1:s} {2:s} {3:s} {4:s}",
                    ((SMOVModel)(bvmodel.Model)).SoruceValue.ValueShowString,
                    ((SMOVModel)(bvmodel.Model)).SourceStart.ValueShowString,
                    ((SMOVModel)(bvmodel.Model)).SourceCount.ValueShowString,
                    ((SMOVModel)(bvmodel.Model)).DestinationValue.ValueShowString,
                    ((SMOVModel)(bvmodel.Model)).DestinationStart.ValueShowString));
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
