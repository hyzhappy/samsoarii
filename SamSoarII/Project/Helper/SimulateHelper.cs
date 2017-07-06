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
using SamSoarII.LadderInstViewModel.Monitor;
using SamSoarII.ValueModel;
using SamSoarII.Simulation.Core.VariableModel;
using SamSoarII.AppMain.UI.Monitor;
using SamSoarII.Simulation.UI.Breakpoint;
using SamSoarII.UserInterface;

namespace SamSoarII.AppMain.Project
{
    public class SimulateHelper
    {
        static private ProjectModel pmodel;
        static public ProjectModel PModel
        {
            get { return pmodel; }
        }
        static private SimulateModel smodel;
        static public SimulateModel SModel
        {
            get { return smodel; }
        }
        static private SimulateMonitorManager smmanager;
        static public SimulateMonitorManager SMManager
        {
            get { return smmanager; }
        }
        
        public const int SIMULATE_OK = SimulateDllModel.LOADDLL_OK;
        public const int SIMULATE_LADDER_ERROR = 0x0100;
        public const int SIMULATE_FUNCBLOCK_ERROR = 0x0101;
        public const int CLOSE_OK = 0x00;

        static public int Simulate(ProjectModel _pmodel)
        {
            pmodel = _pmodel;
            smodel = new SimulateModel();
            smmanager = new SimulateMonitorManager(_pmodel.MMonitorManager.MMWindow, smodel);
            Setup(pmodel.MainRoutine);
            foreach (LadderDiagramViewModel lvdmodel in pmodel.SubRoutines)
            {
                Setup(lvdmodel);
            }
            int ret = GenerateHelper.GenerateSimu(pmodel);
            switch (ret)
            {
                case SimulateDllModel.LOADDLL_OK:
                    smodel.ShowWindow();
                    pmodel.LadderMode = LadderMode.Simulate;
                    smmanager.Initialize();
                    SimuBrpoWindow bpwindow = pmodel.IFacade.BPWindow;
                    bpwindow.Route(pmodel);
                    bpwindow.SModel = smodel;
                    bpwindow.SManager = smodel.SManager;
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_DLLFILE:
                    LocalizedMessageBox.Show("Error : 找不到生成的dll文件\r\n",LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETBIT:
                    LocalizedMessageBox.Show("Error : 找不到入口GetBit\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETWORD:
                    LocalizedMessageBox.Show("Error : 找不到入口GetWord\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETDWORD:
                    LocalizedMessageBox.Show("Error : 找不到入口GetDWord\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETFLOAT:
                    LocalizedMessageBox.Show("Error : 找不到入口GetFloat\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_GETFEQ:
                    LocalizedMessageBox.Show("Error : 找不到入口GetFeq\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETBIT:
                    LocalizedMessageBox.Show("Error : 找不到入口SetBit\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETWORD:
                    LocalizedMessageBox.Show("Error : 找不到入口SetWord\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETDWORD:
                    LocalizedMessageBox.Show("Error : 找不到入口SetDWord\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETFLOAT:
                    LocalizedMessageBox.Show("Error : 找不到入口SetFloat\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETFEQ:
                    LocalizedMessageBox.Show("Error : 找不到入口SetFeq\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_BEFORERUNLADDER:
                    LocalizedMessageBox.Show("Error : 找不到入口BeforeRunLadder\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_AFTERRUNLADDER:
                    LocalizedMessageBox.Show("Error : 找不到入口AfterRunLadder\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_INITRUNLADDER:
                    LocalizedMessageBox.Show("Error : 找不到入口InitRunLadder\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNNT_FOUND_GETCLOCK:
                    LocalizedMessageBox.Show("Error : 找不到入口GetClock\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_INITCLOCK:
                    LocalizedMessageBox.Show("Error : 找不到入口InitClock\r\n", LocalizedMessageIcon.Error);
                    break;
                case SimulateDllModel.LOADDLL_CANNOT_FOUND_SETCLOCKRATE:
                    LocalizedMessageBox.Show("Error : 找不到入口SetClockRate\r\n", LocalizedMessageIcon.Error);
                    break;
                default:
                    LocalizedMessageBox.Show("Error : 发生未知错误\r\n", LocalizedMessageIcon.Error);
                    break;
            }
            return ret;
        }

        static public int Close()
        {
            if (smodel != null)
            {
                smodel.Dispose();
                smodel = null;
            }
            if (smmanager != null)
            {
                smmanager.Abort();
                smmanager = null;
            }
            if (pmodel != null)
            {
                pmodel.LadderMode = LadderMode.Edit;
                SimuBrpoWindow bpwindow = pmodel.IFacade.BPWindow;
                bpwindow.Unroute(pmodel);
                bpwindow.SModel = null;
                bpwindow.SManager = null;
                pmodel = null;
            }
            return CLOSE_OK;
        }
        
        #region Setup
        
        private static void Setup(LadderDiagramViewModel ldvmodel)
        {
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                Setup(lnvmodel);
            }
        }

        private static void Setup(LadderNetworkViewModel lnvmodel)
        {
            foreach (BaseViewModel bvmodel in lnvmodel.GetElements())
            {
                bvmodel.ViewCtrl = smmanager;
                BaseModel bmodel = bvmodel.Model;
                if (bmodel == null) continue;
                for (int i = 0; i < bmodel.ParaCount; i++)
                {
                    IValueModel vmodel = bmodel.GetPara(i);
                    SimulateVariableUnit svunit = null;
                    if (vmodel.ValueString.Equals(String.Empty))
                    {
                        continue;
                    }
                    switch (vmodel.Type)
                    {
                        case LadderValueType.Bool:
                            svunit = smodel.GetVariableUnit(vmodel.ValueString, "BIT");
                            break;
                        case LadderValueType.Word:
                            svunit = smodel.GetVariableUnit(vmodel.ValueString, "WORD");
                            break;
                        case LadderValueType.DoubleWord:
                            svunit = smodel.GetVariableUnit(vmodel.ValueString, "DWORD");
                            break;
                        case LadderValueType.Float:
                            svunit = smodel.GetVariableUnit(vmodel.ValueString, "FLOAT");
                            break;
                        case LadderValueType.String:
                            svunit = new SimulateStringUnit(vmodel.ValueString);
                            break;
                    }
                    svunit.CanClose = false;
                    SimuMoniValueModel smvmodel = new SimuMoniValueModel(svunit, smodel);
                    bvmodel.SetValueModel(i, smvmodel);
                }
            }
        }
        
        #endregion
        
    }
}
