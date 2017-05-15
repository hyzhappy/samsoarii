using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SamSoarII.UserInterface;
using SamSoarII.Utility;
using SamSoarII.LadderInstModel;
using SamSoarII.PLCCompiler;
using SamSoarII.LadderInstViewModel;
using SamSoarII.Extend.LadderChartModel;
using SamSoarII.Extend.LogicGraph;
using SamSoarII.Extend.Utility;

namespace SamSoarII.AppMain.Project
{
    public class ExtendHelper
    {
        public static List<PLCInstruction> GeneratePLCInsts(IEnumerable<BaseViewModel> bvms)
        {
            LadderChart lchart = GenerateHelper.CreateLadderChart(bvms);
            LGraph lgraph = lchart.Generate();
            List<PLCInstruction> insts = lgraph.GenInst();
            return insts;
        }
    }
}
