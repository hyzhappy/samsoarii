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
//using SamSoarII.GenerateModel;

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
                    lcn.Prototype = bvm;
                    lcn.X = bvm_X;
                    lcn.Y = bvm_Y;
                    lchart.Insert(lcn);
                }
                if (bvm.Type == ElementType.VLine)
                {
                    lcn.Type = String.Empty;
                    lcn.VAccess = true;
                }
                else if (bvm.Type == ElementType.HLine)
                {
                    lcn.Type = String.Empty;
                    lcn.HAccess = true;
                }
                else
                {
                    lcn.Type = bvm.InstructionName;
                    if (bvm.Model is ALTModel)
                    {
                        lcn[1] = ((ALTModel)(bvm.Model)).Value.ValueShowString;
                    }
                    if (bvm.Model is ALTPModel)
                    {
                        lcn[1] = ((ALTPModel)(bvm.Model)).Value.ValueShowString;
                    }
                    if (bvm.Model is INVModel)
                    {
                        // NOTHING TO DO
                    }
                    if (bvm.Model is LDFModel)
                    {
                        lcn[1] = ((LDFModel)(bvm.Model)).Value.ValueShowString;
                    }
                    if (bvm.Model is LDIIMModel)
                    {
                        lcn[1] = ((LDIIMModel)(bvm.Model)).Value.ValueShowString;
                    }
                    if (bvm.Model is LDIMModel)
                    {
                        lcn[1] = ((LDIMModel)(bvm.Model)).Value.ValueShowString;
                    }
                    if (bvm.Model is LDIModel)
                    {
                        lcn[1] = ((LDIModel)(bvm.Model)).Value.ValueShowString;
                    }
                    if (bvm.Model is LDModel)
                    {
                        lcn[1] = ((LDModel)(bvm.Model)).Value.ValueShowString;
                    }
                    if (bvm.Model is LDPModel)
                    {
                        lcn[1] = ((LDPModel)(bvm.Model)).Value.ValueShowString;
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
                        lcn[1] = ((OUTIMModel)(bvm.Model)).Value.ValueShowString;
                    }
                    if (bvm.Model is OUTModel)
                    {
                        lcn[1] = ((OUTModel)(bvm.Model)).Value.ValueShowString;
                    }
                    if (bvm.Model is RSTIMModel)
                    {
                        lcn[1] = ((RSTIMModel)(bvm.Model)).Value.ValueShowString;
                        lcn[2] = ((RSTIMModel)(bvm.Model)).Count.ValueShowString;
                    }
                    if (bvm.Model is RSTModel)
                    {
                        lcn[1] = ((RSTModel)(bvm.Model)).Value.ValueShowString;
                        lcn[2] = ((RSTModel)(bvm.Model)).Count.ValueShowString;
                    }
                    if (bvm.Model is SETIMModel)
                    {
                        lcn[1] = ((SETIMModel)(bvm.Model)).Value.ValueShowString;
                        lcn[2] = ((SETIMModel)(bvm.Model)).Count.ValueShowString;
                    }
                    if (bvm.Model is SETModel)
                    {
                        lcn[1] = ((SETModel)(bvm.Model)).Value.ValueShowString;
                        lcn[2] = ((SETModel)(bvm.Model)).Count.ValueShowString;
                    }
                    if (bvm.Model is LDDEQModel)
                    {
                        lcn[1] = ((LDDEQModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDDEQModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDDNEModel)
                    {
                        lcn[1] = ((LDDNEModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDDNEModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDDGEModel)
                    {
                        lcn[1] = ((LDDGEModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDDGEModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDDLEModel)
                    {
                        lcn[1] = ((LDDLEModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDDLEModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDDLModel)
                    {
                        lcn[1] = ((LDDLModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDDLModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDDGModel)
                    {
                        lcn[1] = ((LDDGModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDDGModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDFEQModel)
                    {
                        lcn[1] = ((LDFEQModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDFEQModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDFNEModel)
                    {
                        lcn[1] = ((LDFNEModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDFNEModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDFGEModel)
                    {
                        lcn[1] = ((LDFGEModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDFGEModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDFLEModel)
                    {
                        lcn[1] = ((LDFLEModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDFLEModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDFLModel)
                    {
                        lcn[1] = ((LDFLModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDFLModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDFGModel)
                    {
                        lcn[1] = ((LDFGModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDFGModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDWEQModel)
                    {
                        lcn[1] = ((LDWEQModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDWEQModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDWNEModel)
                    {
                        lcn[1] = ((LDWNEModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDWNEModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDWGEModel)
                    {
                        lcn[1] = ((LDWGEModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDWGEModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDWLEModel)
                    {
                        lcn[1] = ((LDWLEModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDWLEModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDWLModel)
                    {
                        lcn[1] = ((LDWLModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDWLModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is LDWGModel)
                    {
                        lcn[1] = ((LDWGModel)(bvm.Model)).Value1.ValueShowString;
                        lcn[2] = ((LDWGModel)(bvm.Model)).Value2.ValueShowString;
                    }
                    if (bvm.Model is BCDModel)
                    {
                        lcn[1] = ((BCDModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((BCDModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is BINModel)
                    {
                        lcn[1] = ((BINModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((BINModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is DTOFModel)
                    {
                        lcn[1] = ((DTOFModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((DTOFModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is DTOWModel)
                    {
                        lcn[1] = ((DTOWModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((DTOWModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is ROUNDModel)
                    {
                        lcn[1] = ((ROUNDModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((ROUNDModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is TRUNCModel)
                    {
                        lcn[1] = ((TRUNCModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((TRUNCModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is WTODModel)
                    {
                        lcn[1] = ((WTODModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((WTODModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is ADDFModel)
                    {
                        lcn[1] = ((ADDFModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((ADDFModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((ADDFModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is SUBFModel)
                    {
                        lcn[1] = ((SUBFModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((SUBFModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((SUBFModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is MULFModel)
                    {
                        lcn[1] = ((MULFModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((MULFModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((MULFModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is DIVFModel)
                    {
                        lcn[1] = ((DIVFModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((DIVFModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((DIVFModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is SINModel)
                    {
                        lcn[1] = ((SINModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((SINModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is COSModel)
                    {
                        lcn[1] = ((COSModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((COSModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is TANModel)
                    {
                        lcn[1] = ((TANModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((TANModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is LNModel)
                    {
                        lcn[1] = ((LNModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((LNModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is EXPModel)
                    {
                        lcn[1] = ((EXPModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((EXPModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is SQRTModel)
                    {
                        lcn[1] = ((SQRTModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((SQRTModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is ADDModel)
                    {
                        lcn[1] = ((ADDModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((ADDModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((ADDModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is ADDDModel)
                    {
                        lcn[1] = ((ADDDModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((ADDDModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((ADDDModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is SUBModel)
                    {
                        lcn[1] = ((SUBModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((SUBModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((SUBModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is SUBDModel)
                    {
                        lcn[1] = ((SUBDModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((SUBDModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((SUBDModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is MULModel)
                    {
                        lcn[1] = ((MULModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((MULModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((MULModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is MULDModel)
                    {
                        lcn[1] = ((MULDModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((MULDModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((MULDModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is MULWModel)
                    {
                        lcn[1] = ((MULWModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((MULWModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((MULWModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is DIVModel)
                    {
                        lcn[1] = ((DIVModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((DIVModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((DIVModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is DIVDModel)
                    {
                        lcn[1] = ((DIVDModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((DIVDModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((DIVDModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is DIVWModel)
                    {
                        lcn[1] = ((DIVWModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((DIVWModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((DIVWModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is INCModel)
                    {
                        lcn[1] = ((INCModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((INCModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is INCDModel)
                    {
                        lcn[1] = ((INCDModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((INCDModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is DECModel)
                    {
                        lcn[1] = ((DECModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((DECModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is DECDModel)
                    {
                        lcn[1] = ((DECDModel)(bvm.Model)).InputValue.ValueShowString;
                        lcn[2] = ((DECDModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is ANDWModel)
                    {
                        lcn[1] = ((ANDWModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((ANDWModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((ANDWModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is ANDDModel)
                    {
                        lcn[1] = ((ANDDModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((ANDDModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((ANDDModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is ORWModel)
                    {
                        lcn[1] = ((ORWModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((ORWModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((ORWModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is ORDModel)
                    {
                        lcn[1] = ((ORDModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((ORDModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((ORDModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is XORWModel)
                    {
                        lcn[1] = ((XORWModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((XORWModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((XORWModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is XORDModel)
                    {
                        lcn[1] = ((XORDModel)(bvm.Model)).InputValue1.ValueShowString;
                        lcn[2] = ((XORDModel)(bvm.Model)).InputValue2.ValueShowString;
                        lcn[3] = ((XORDModel)(bvm.Model)).OutputValue.ValueShowString;
                    }
                    if (bvm.Model is MOVModel)
                    {
                        lcn[1] = ((MOVModel)(bvm.Model)).SourceValue.ValueShowString;
                        lcn[2] = ((MOVModel)(bvm.Model)).DestinationValue.ValueShowString;
                    }
                    if (bvm.Model is MOVDModel)
                    {
                        lcn[1] = ((MOVDModel)(bvm.Model)).SourceValue.ValueShowString;
                        lcn[2] = ((MOVDModel)(bvm.Model)).DestinationValue.ValueShowString;
                    }
                    if (bvm.Model is MOVFModel)
                    {
                        lcn[1] = ((MOVFModel)(bvm.Model)).SourceValue.ValueShowString;
                        lcn[2] = ((MOVFModel)(bvm.Model)).DestinationValue.ValueShowString;
                    }
                    if (bvm.Model is MVBLKModel)
                    {
                        lcn[1] = ((MVBLKModel)(bvm.Model)).SourceValue.ValueShowString;
                        lcn[2] = ((MVBLKModel)(bvm.Model)).DestinationValue.ValueShowString;
                        lcn[3] = ((MVBLKModel)(bvm.Model)).Count.ValueShowString;
                    }
                    if (bvm.Model is MVDBLKModel)
                    {
                        lcn[1] = ((MVDBLKModel)(bvm.Model)).SourceValue.ValueShowString;
                        lcn[2] = ((MVDBLKModel)(bvm.Model)).DestinationValue.ValueShowString;
                        lcn[3] = ((MVDBLKModel)(bvm.Model)).Count.ValueShowString;
                    }
                    if (bvm.Model is CALLMModel)
                    {
                        lcn[1] = ((CALLMModel)(bvm.Model)).FunctionName;
                    }
                    if (bvm.Model is CALLModel)
                    {
                        lcn[1] = ((CALLModel)(bvm.Model)).FunctionName;
                    }
                    if (bvm.Model is FORModel)
                    {
                        lcn[1] = ((FORModel)(bvm.Model)).Count.ValueShowString;
                    }
                    if (bvm.Model is JMPModel)
                    {
                        lcn[1] = ((JMPModel)(bvm.Model)).LBLIndex.ValueShowString;
                    }
                    if (bvm.Model is LBLModel)
                    {
                        lcn[1] = ((LBLModel)(bvm.Model)).LBLIndex.ValueShowString;
                    }
                    if (bvm.Model is NEXTModel)
                    {
                        //svbmodel.Setup("NEXT");
                    }
                    if (bvm.Model is TRDModel)
                    {
                        lcn[1] = ((TRDModel)(bvm.Model)).StartValue.ValueShowString;
                    }
                    if (bvm.Model is TWRModel)
                    {
                        lcn[1] = ((TWRModel)(bvm.Model)).StartValue.ValueShowString;
                    }
                    if (bvm.Model is ROLDModel)
                    {
                        lcn[1] = ((ROLDModel)(bvm.Model)).SourceValue.ValueShowString;
                        lcn[2] = ((ROLDModel)(bvm.Model)).DestinationValue.ValueShowString;
                        lcn[3] = ((ROLDModel)(bvm.Model)).Count.ValueShowString;
                    }
                    if (bvm.Model is ROLModel)
                    {
                        lcn[1] = ((ROLModel)(bvm.Model)).SourceValue.ValueShowString;
                        lcn[2] = ((ROLModel)(bvm.Model)).DestinationValue.ValueShowString;
                        lcn[3] = ((ROLModel)(bvm.Model)).Count.ValueShowString;
                    }
                    if (bvm.Model is RORDModel)
                    {
                        lcn[1] = ((RORDModel)(bvm.Model)).SourceValue.ValueShowString;
                        lcn[2] = ((RORDModel)(bvm.Model)).DestinationValue.ValueShowString;
                        lcn[3] = ((RORDModel)(bvm.Model)).Count.ValueShowString;
                    }
                    if (bvm.Model is RORModel)
                    {
                        lcn[1] = ((RORModel)(bvm.Model)).SourceValue.ValueShowString;
                        lcn[2] = ((RORModel)(bvm.Model)).DestinationValue.ValueShowString;
                        lcn[3] = ((RORModel)(bvm.Model)).Count.ValueShowString;
                    }
                    if (bvm.Model is SHLDModel)
                    {
                        lcn[1] = ((SHLDModel)(bvm.Model)).SourceValue.ValueShowString;
                        lcn[2] = ((SHLDModel)(bvm.Model)).DestinationValue.ValueShowString;
                        lcn[3] = ((SHLDModel)(bvm.Model)).Count.ValueShowString;
                    }
                    if (bvm.Model is SHLModel)
                    {
                        lcn[1] = ((SHLModel)(bvm.Model)).SourceValue.ValueShowString;
                        lcn[2] = ((SHLModel)(bvm.Model)).DestinationValue.ValueShowString;
                        lcn[3] = ((SHLModel)(bvm.Model)).Count.ValueShowString;
                    }
                    if (bvm.Model is SHRDModel)
                    {
                        lcn[1] = ((SHRDModel)(bvm.Model)).SourceValue.ValueShowString;
                        lcn[2] = ((SHRDModel)(bvm.Model)).DestinationValue.ValueShowString;
                        lcn[3] = ((SHRDModel)(bvm.Model)).Count.ValueShowString;
                    }
                    if (bvm.Model is SHRModel)
                    {
                        lcn[1] = ((SHRModel)(bvm.Model)).SourceValue.ValueShowString;
                        lcn[2] = ((SHRModel)(bvm.Model)).DestinationValue.ValueShowString;
                        lcn[3] = ((SHRModel)(bvm.Model)).Count.ValueShowString;
                    }
                    lcn.HAccess = true;
                }
            }
            return lchart;
        }
    }
}
