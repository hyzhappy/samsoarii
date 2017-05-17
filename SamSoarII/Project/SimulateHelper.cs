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
        
        public const int SIMULATE_OK = SimulateDllModel.LOADDLL_OK;
        public const int SIMULATE_LADDER_ERROR = 0x0100;
        public const int SIMULATE_FUNCBLOCK_ERROR = 0x0101;
        public const int CLOSE_OK = 0x00;

        static public int Simulate(ProjectModel _pmodel)
        {
            pmodel = _pmodel;
            smodel = new SimulateModel();
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
            return ret;
        }

        static public int Close()
        {
            if (smodel != null)
            {
                smodel.Dispose();
                smodel = null;   
            }
            pmodel.LadderMode = LadderMode.Edit;
            return CLOSE_OK;
        }
        
        #region Setup
        
        private static void Setup(LadderDiagramViewModel lvdmodel)
        {
            foreach (LadderNetworkViewModel lnvmodel in lvdmodel.GetNetworks())
            {
                Setup(lnvmodel);
            }
        }

        private static void Setup(LadderNetworkViewModel lnvmodel)
        {
            foreach (MoniBaseViewModel mbvmodel in lnvmodel.GetMonitors())
            {
                BaseModel bmodel = mbvmodel.Model;
                if (bmodel == null) continue;
                for (int i = 0; i < bmodel.ParaCount; i++)
                {
                    IValueModel vmodel = bmodel.GetPara(i);
                    SimulateVariableUnit svunit = null;
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
                    }
                    SimuMoniValueModel smvmodel = new SimuMoniValueModel(svunit);
                    mbvmodel.SetValueModel(i, smvmodel);
                }
            }
        }
        
        #endregion
        
    }
}
