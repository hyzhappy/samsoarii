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
using SamSoarII.GenerateModel;

namespace SamSoarII.AppMain.Project
{
    public static class GenerateHelper
    {
        public static LadderChart CreateLadderChart(IEnumerable<BaseViewModel> bvms)
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
                        lcn[1] = InstHelper.RegAddr(((ALTModel)(bvm.Model)).Value.ToShowString());
                    }
                    if (bvm.Model is ALTPModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((ALTPModel)(bvm.Model)).Value.ToShowString());
                    }
                    if (bvm.Model is INVModel)
                    {
                        // NOTHING TO DO
                    }
                    if (bvm.Model is LDFModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDFModel)(bvm.Model)).Value.ToShowString());
                    }
                    if (bvm.Model is LDIIMModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDIIMModel)(bvm.Model)).Value.ToShowString());
                    }
                    if (bvm.Model is LDIMModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDIMModel)(bvm.Model)).Value.ToShowString());
                    }
                    if (bvm.Model is LDIModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDIModel)(bvm.Model)).Value.ToShowString());
                    }
                    if (bvm.Model is LDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDModel)(bvm.Model)).Value.ToShowString());
                    }
                    if (bvm.Model is LDPModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDPModel)(bvm.Model)).Value.ToShowString());
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
                        lcn[1] = InstHelper.RegAddr(((OUTIMModel)(bvm.Model)).Value.ToShowString());
                    }
                    if (bvm.Model is OUTModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((OUTModel)(bvm.Model)).Value.ToShowString());
                    }
                    if (bvm.Model is RSTIMModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((RSTIMModel)(bvm.Model)).Value.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((RSTIMModel)(bvm.Model)).Count.ToShowString());
                    }
                    if (bvm.Model is RSTModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((RSTModel)(bvm.Model)).Value.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((RSTModel)(bvm.Model)).Count.ToShowString());
                    }
                    if (bvm.Model is SETIMModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((SETIMModel)(bvm.Model)).Value.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((SETIMModel)(bvm.Model)).Count.ToShowString());
                    }
                    if (bvm.Model is SETModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((SETModel)(bvm.Model)).Value.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((SETModel)(bvm.Model)).Count.ToShowString());
                    }
                    if (bvm.Model is LDDEQModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDDEQModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDDEQModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDDNEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDDNEModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDDNEModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDDGEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDDGEModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDDGEModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDDLEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDDLEModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDDLEModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDDLModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDDLModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDDLModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDDGModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDDGModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDDGModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDFEQModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDFEQModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDFEQModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDFNEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDFNEModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDFNEModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDFGEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDFGEModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDFGEModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDFLEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDFLEModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDFLEModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDFLModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDFLModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDFLModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDFGModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDFGModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDFGModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDWEQModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDWEQModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDWEQModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDWNEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDWNEModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDWNEModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDWGEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDWGEModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDWGEModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDWLEModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDWLEModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDWLEModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDWLModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDWLModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDWLModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is LDWGModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LDWGModel)(bvm.Model)).Value1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LDWGModel)(bvm.Model)).Value2.ToShowString());
                    }
                    if (bvm.Model is BCDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((BCDModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((BCDModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is BINModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((BINModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((BINModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is DTOFModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((DTOFModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((DTOFModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is DTOWModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((DTOWModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((DTOWModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is ROUNDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((ROUNDModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((ROUNDModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is TRUNCModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((TRUNCModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((TRUNCModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is WTODModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((WTODModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((WTODModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is ADDFModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((ADDFModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((ADDFModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((ADDFModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is SUBFModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((SUBFModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((SUBFModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((SUBFModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is MULFModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((MULFModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((MULFModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((MULFModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is DIVFModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((DIVFModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((DIVFModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((DIVFModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is SINModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((SINModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((SINModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is COSModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((COSModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((COSModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is TANModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((TANModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((TANModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is LNModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LNModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((LNModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is EXPModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((EXPModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((EXPModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is SQRTModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((SQRTModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((SQRTModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is ADDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((ADDModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((ADDModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((ADDModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is ADDDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((ADDDModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((ADDDModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((ADDDModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is SUBModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((SUBModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((SUBModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((SUBModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is SUBDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((SUBDModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((SUBDModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((SUBDModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is MULModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((MULModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((MULModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((MULModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is MULDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((MULDModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((MULDModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((MULDModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is MULWModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((MULWModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((MULWModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((MULWModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is DIVModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((DIVModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((DIVModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((DIVModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is DIVDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((DIVDModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((DIVDModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((DIVDModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is DIVWModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((DIVWModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((DIVWModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((DIVWModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is INCModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((INCModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((INCModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is INCDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((INCDModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((INCDModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is DECModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((DECModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((DECModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is DECDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((DECDModel)(bvm.Model)).InputValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((DECDModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is ANDWModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((ANDWModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((ANDWModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((ANDWModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is ANDDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((ANDDModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((ANDDModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((ANDDModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is ORWModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((ORWModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((ORWModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((ORWModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is ORDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((ORDModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((ORDModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((ORDModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is XORWModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((XORWModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((XORWModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((XORWModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is XORDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((XORDModel)(bvm.Model)).InputValue1.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((XORDModel)(bvm.Model)).InputValue2.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((XORDModel)(bvm.Model)).OutputValue.ToShowString());
                    }
                    if (bvm.Model is MOVModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((MOVModel)(bvm.Model)).SourceValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((MOVModel)(bvm.Model)).DestinationValue.ToShowString());
                    }
                    if (bvm.Model is MOVDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((MOVDModel)(bvm.Model)).SourceValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((MOVDModel)(bvm.Model)).DestinationValue.ToShowString());
                    }
                    if (bvm.Model is MOVFModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((MOVFModel)(bvm.Model)).SourceValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((MOVFModel)(bvm.Model)).DestinationValue.ToShowString());
                    }
                    if (bvm.Model is MVBLKModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((MVBLKModel)(bvm.Model)).SourceValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((MVBLKModel)(bvm.Model)).DestinationValue.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((MVBLKModel)(bvm.Model)).Count.ToShowString());
                    }
                    if (bvm.Model is MVDBLKModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((MVDBLKModel)(bvm.Model)).SourceValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((MVDBLKModel)(bvm.Model)).DestinationValue.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((MVDBLKModel)(bvm.Model)).Count.ToShowString());
                    }
                    if (bvm.Model is CALLMModel)
                    {
                        lcn[1] = lcn.StringID(((CALLMModel)(bvm.Model)).FunctionName);
                    }
                    if (bvm.Model is CALLModel)
                    {
                        lcn[1] = lcn.StringID(((CALLModel)(bvm.Model)).FunctionName);
                    }
                    if (bvm.Model is FORModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((FORModel)(bvm.Model)).Count.ToShowString());
                    }
                    if (bvm.Model is JMPModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((JMPModel)(bvm.Model)).LBLIndex.ToShowString());
                    }
                    if (bvm.Model is LBLModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((LBLModel)(bvm.Model)).LBLIndex.ToShowString());
                    }
                    if (bvm.Model is NEXTModel)
                    {
                        //svbmodel.Setup("NEXT");
                    }
                    if (bvm.Model is TRDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((TRDModel)(bvm.Model)).StartValue.ToShowString());
                    }
                    if (bvm.Model is TWRModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((TWRModel)(bvm.Model)).StartValue.ToShowString());
                    }
                    if (bvm.Model is ROLDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((ROLDModel)(bvm.Model)).SourceValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((ROLDModel)(bvm.Model)).DestinationValue.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((ROLDModel)(bvm.Model)).Count.ToShowString());
                    }
                    if (bvm.Model is ROLModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((ROLModel)(bvm.Model)).SourceValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((ROLModel)(bvm.Model)).DestinationValue.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((ROLModel)(bvm.Model)).Count.ToShowString());
                    }
                    if (bvm.Model is RORDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((RORDModel)(bvm.Model)).SourceValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((RORDModel)(bvm.Model)).DestinationValue.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((RORDModel)(bvm.Model)).Count.ToShowString());
                    }
                    if (bvm.Model is RORModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((RORModel)(bvm.Model)).SourceValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((RORModel)(bvm.Model)).DestinationValue.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((RORModel)(bvm.Model)).Count.ToShowString());
                    }
                    if (bvm.Model is SHLDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((SHLDModel)(bvm.Model)).SourceValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((SHLDModel)(bvm.Model)).DestinationValue.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((SHLDModel)(bvm.Model)).Count.ToShowString());
                    }
                    if (bvm.Model is SHLModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((SHLModel)(bvm.Model)).SourceValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((SHLModel)(bvm.Model)).DestinationValue.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((SHLModel)(bvm.Model)).Count.ToShowString());
                    }

                    if (bvm.Model is SHRDModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((SHRDModel)(bvm.Model)).SourceValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((SHRDModel)(bvm.Model)).DestinationValue.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((SHRDModel)(bvm.Model)).Count.ToShowString());
                    }
                    if (bvm.Model is SHRModel)
                    {
                        lcn[1] = InstHelper.RegAddr(((SHRModel)(bvm.Model)).SourceValue.ToShowString());
                        lcn[2] = InstHelper.RegAddr(((SHRModel)(bvm.Model)).DestinationValue.ToShowString());
                        lcn[3] = InstHelper.RegAddr(((SHRModel)(bvm.Model)).Count.ToShowString());
                    }
                    lcn.HAccess = true;
                }
            }
            return lchart;
        }
    }
}
