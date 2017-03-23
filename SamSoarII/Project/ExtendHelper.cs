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
            // 按照位置坐标来排序
            Comparison<BaseViewModel> sortByPosition = delegate (BaseViewModel v1, BaseViewModel v2)
            {
                int v1_X = v1.X;
                int v1_Y = v1.Y;
                int v2_X = v2.X;
                int v2_Y = v2.Y;
                if (v1.Type == ElementType.VLine)
                    v1_X++;
                if (v2.Type == ElementType.VLine)
                    v2_X++;
                if (v1_Y < v2_Y || v1_Y == v2_Y && v1_X < v2_X)
                    return -1;
                if (v1_Y > v2_Y || v1_Y == v2_Y && v1_X > v2_X)
                    return 1;
                if (v1_Y == v2_Y && v1_X == v2_X)
                    return 0;
                return 0;
            };
            List<BaseViewModel> _bvms = bvms.ToList();
            _bvms.Sort(sortByPosition);

            LadderChart lchart = new LadderChart();
            LCNode lcn = new LCNode(-1);
            lcn.X = -1;
            lcn.Y = -1;
            int lcnCount = 0;
           
            foreach (BaseViewModel bvm in _bvms)
            {
                int bvm_X = bvm.X;
                int bvm_Y = bvm.Y;
                if (bvm.Type == ElementType.VLine)
                    bvm_X++;
                if (bvm_X != lcn.X || bvm_Y != lcn.Y)
                {
                    lcn = new LCNode(++lcnCount);
                    lcn.X = bvm_X;
                    lcn.Y = bvm_Y;
                    lchart.Insert(lcn);
                }
                if (bvm.Type == ElementType.VLine)
                {
                    lcn.VAccess = true;
                }
                else if (bvm.Type == ElementType.HLine)
                {
                    lcn.HAccess = true;
                }
                else
                {
                    lcn.Type = InstHelper.InstID(bvm.InstructionName);
                    if (bvm.Model is ALTModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((ALTModel)(bvm.Model)).Value.ValueShowString);    
                    }
                    if (bvm.Model is ALTPModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((ALTPModel)(bvm.Model)).Value.ValueShowString);
                    }
                    if (bvm.Model is INVModel)
                    {
                        // NOTHING TO DO
                    }
                    if (bvm.Model is LDFModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDFModel)(bvm.Model)).Value.ValueShowString);
                    }
                    if (bvm.Model is LDIIMModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDIIMModel)(bvm.Model)).Value.ValueShowString);
                    }
                    if (bvm.Model is LDIMModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDIMModel)(bvm.Model)).Value.ValueShowString);
                    }
                    if (bvm.Model is LDIModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDIModel)(bvm.Model)).Value.ValueShowString);
                    }
                    if (bvm.Model is LDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDModel)(bvm.Model)).Value.ValueShowString);
                    }
                    if (bvm.Model is LDPModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDPModel)(bvm.Model)).Value.ValueShowString);
                    }
                    if (bvm.Model is MEFModel)
                    {
                        // NOTHING TO DO
                    }
                    if (bvm.Model is MEPModel)
                    {
                        // NOTHING TO DO
                    }
                    if (bvm.Model is OUTIMModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((OUTIMModel)(bvm.Model)).Value.ValueShowString);
                    }
                    if (bvm.Model is OUTModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((OUTModel)(bvm.Model)).Value.ValueShowString);
                    }
                    if (bvm.Model is RSTIMModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((RSTIMModel)(bvm.Model)).Value.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((RSTIMModel)(bvm.Model)).Count.ValueShowString);
                    }
                    if (bvm.Model is RSTModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((RSTModel)(bvm.Model)).Value.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((RSTModel)(bvm.Model)).Count.ValueShowString);
                    }
                    if (bvm.Model is SETIMModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((SETIMModel)(bvm.Model)).Value.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((SETIMModel)(bvm.Model)).Count.ValueShowString);
                    }
                    if (bvm.Model is SETModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((SETModel)(bvm.Model)).Value.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((SETModel)(bvm.Model)).Count.ValueShowString);
                    }
                    if (bvm.Model is LDDEQModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDDEQModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDDEQModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDDNEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDDNEModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDDNEModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDDGEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDDGEModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDDGEModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDDLEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDDLEModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDDLEModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDDLModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDDLModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDDLModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDDGModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDDGModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDDGModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDFEQModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDFEQModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDFEQModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDFNEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDFNEModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDFNEModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDFGEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDFGEModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDFGEModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDFLEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDFLEModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDFLEModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDFLModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDFLModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDFLModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDFGModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDFGModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDFGModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDWEQModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDWEQModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDWEQModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDWNEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDWNEModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDWNEModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDWGEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDWGEModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDWGEModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDWLEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDWLEModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDWLEModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDWLModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDWLModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDWLModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is LDWGModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDWGModel)(bvm.Model)).Value1.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((LDWGModel)(bvm.Model)).Value2.ValueShowString);
                    }
                    if (bvm.Model is BCDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((BCDModel)(bvm.Model)).InputValue.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((BCDModel)(bvm.Model)).OutputValue.ValueShowString);
                    }
                    if (bvm.Model is BINModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((BINModel)(bvm.Model)).InputValue.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((BINModel)(bvm.Model)).OutputValue.ValueShowString);
                    }
                    if (bvm.Model is DTOFModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((DTOFModel)(bvm.Model)).InputValue.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((DTOFModel)(bvm.Model)).OutputValue.ValueShowString);
                    }
                    if (bvm.Model is DTOWModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((DTOWModel)(bvm.Model)).InputValue.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((DTOWModel)(bvm.Model)).OutputValue.ValueShowString);
                    }
                    if (bvm.Model is ROUNDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((ROUNDModel)(bvm.Model)).InputValue.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((ROUNDModel)(bvm.Model)).OutputValue.ValueShowString);
                    }
                    if (bvm.Model is TRUNCModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((TRUNCModel)(bvm.Model)).InputValue.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((TRUNCModel)(bvm.Model)).OutputValue.ValueShowString);
                    }
                    if (bvm.Model is WTODModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((WTODModel)(bvm.Model)).InputValue.ValueShowString);
                        lcn[2] = InstHelper.RegAddr(((WTODModel)(bvm.Model)).OutputValue.ValueShowString);
                    }
                    lcn.HAccess = true;
                }
            }

            LGraph lgraph = lchart.Generate();
            List<PLCInstruction> insts = lgraph.GenInst();
            return insts;

        }
    }
}
