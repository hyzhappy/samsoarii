using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SamSoarII.LadderInstViewModel;
using SamSoarII.Simulation;
using SamSoarII.Simulation.Shell.ViewModel;
using SamSoarII.Simulation.Shell;
using SamSoarII.Simulation.UI;
using SamSoarII.LadderInstModel;
using SamSoarII.Extend.LadderChartModel;

namespace SamSoarII.AppMain.Project
{
    public class SimulateHelper
    {
        static private SimulateModel smodel;

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
            svdmodel.Name = lvdmodel.LadderName;
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
            if (bvmodel is ALTViewModel)
            {
                svbmodel.Setup(String.Format("ALT {0:s}",
                    ((ALTModel)(bvmodel.Model)).Value.ToShowString()));
            }
            if (bvmodel is ALTPViewModel)
            {
                svbmodel.Setup(String.Format("ALTP {0:s}",
                    ((ALTPModel)(bvmodel.Model)).Value.ToShowString()));
            }
            if (bvmodel is INVViewModel)
            {
                svbmodel.Setup(String.Format("INV"));
            }
            if (bvmodel is LDFViewModel)
            {
                svbmodel.Setup(String.Format("LDF {0:s}",
                    ((LDFModel)(bvmodel.Model)).Value.ToShowString()));
            }
            if (bvmodel is LDIIMViewModel)
            {
                svbmodel.Setup(String.Format("LDIIM {0:s}",
                    ((LDIIMModel)(bvmodel.Model)).Value.ToShowString()));
            }
            if (bvmodel is LDIMViewModel)
            {
                svbmodel.Setup(String.Format("LDIM {0:s}",
                    ((LDIMModel)(bvmodel.Model)).Value.ToShowString()));
            }
            if (bvmodel is LDIViewModel)
            {
                svbmodel.Setup(String.Format("LDI {0:s}",
                    ((LDIModel)(bvmodel.Model)).Value.ToShowString()));
            }
            if (bvmodel is LDViewModel)
            {
                svbmodel.Setup(String.Format("LD {0:s}",
                    ((LDModel)(bvmodel.Model)).Value.ToShowString()));
            }
            if (bvmodel is LDPViewModel)
            {
                svbmodel.Setup(String.Format("LDP {0:s}",
                    ((LDPModel)(bvmodel.Model)).Value.ToShowString()));
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
                    ((OUTIMModel)(bvmodel.Model)).Value.ToShowString()));
            }
            if (bvmodel is OUTViewModel)
            {
                svbmodel.Setup(String.Format("OUT {0:s}",
                    ((OUTModel)(bvmodel.Model)).Value.ToShowString()));
            }
            if (bvmodel is RSTIMViewModel)
            {
                svbmodel.Setup(String.Format("RSTIM {0:s} {1:s}",
                    ((RSTIMModel)(bvmodel.Model)).Value.ToShowString(),
                    ((RSTIMModel)(bvmodel.Model)).Count.ToShowString()));
            }
            if (bvmodel is RSTViewModel)
            {
                svbmodel.Setup(String.Format("RST {0:s} {1:s}",
                    ((RSTModel)(bvmodel.Model)).Value.ToShowString(),
                    ((RSTModel)(bvmodel.Model)).Count.ToShowString()));
            }
            if (bvmodel is SETIMViewModel)
            {
                svbmodel.Setup(String.Format("SETIM {0:s} {1:s}",
                    ((SETIMModel)(bvmodel.Model)).Value.ToShowString(),
                    ((SETIMModel)(bvmodel.Model)).Count.ToShowString()));
            }
            if (bvmodel is SETViewModel)
            {
                svbmodel.Setup(String.Format("SET {0:s} {1:s}",
                    ((SETModel)(bvmodel.Model)).Value.ToShowString(),
                    ((SETModel)(bvmodel.Model)).Count.ToShowString()));
            }
            if (bvmodel is LDDEQViewModel)
            {
                svbmodel.Setup(String.Format("LDDEQ {0:s} {1:s}",
                    ((LDDEQModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDDEQModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDDNEViewModel)
            {
                svbmodel.Setup(String.Format("LDDNE {0:s} {1:s}",
                    ((LDDNEModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDDNEModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDDGEViewModel)
            {
                svbmodel.Setup(String.Format("LDDGE {0:s} {1:s}",
                    ((LDDGEModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDDGEModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDDLEViewModel)
            {
                svbmodel.Setup(String.Format("LDDLE {0:s} {1:s}",
                    ((LDDLEModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDDLEModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDDLViewModel)
            {
                svbmodel.Setup(String.Format("LDDL {0:s} {1:s}",
                    ((LDDLModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDDLModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDDGViewModel)
            {
                svbmodel.Setup(String.Format("LDDG {0:s} {1:s}",
                    ((LDDGModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDDGModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDFEQViewModel)
            {
                svbmodel.Setup(String.Format("LDFEQ {0:s} {1:s}",
                    ((LDFEQModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDFEQModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDFNEViewModel)
            {
                svbmodel.Setup(String.Format("LDFNE {0:s} {1:s}",
                    ((LDFNEModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDFNEModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDFGEViewModel)
            {
                svbmodel.Setup(String.Format("LDFGE {0:s} {1:s}",
                    ((LDFGEModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDFGEModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDFLEViewModel)
            {
                svbmodel.Setup(String.Format("LDFLE {0:s} {1:s}",
                    ((LDFLEModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDFLEModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDFLViewModel)
            {
                svbmodel.Setup(String.Format("LDFL {0:s} {1:s}",
                    ((LDFLModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDFLModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDFGViewModel)
            {
                svbmodel.Setup(String.Format("LDFG {0:s} {1:s}",
                    ((LDFGModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDFGModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDWEQViewModel)
            {
                svbmodel.Setup(String.Format("LDWEQ {0:s} {1:s}",
                    ((LDWEQModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDWEQModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDWNEViewModel)
            {
                svbmodel.Setup(String.Format("LDWNE {0:s} {1:s}",
                    ((LDWNEModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDWNEModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDWGEViewModel)
            {
                svbmodel.Setup(String.Format("LDWGE {0:s} {1:s}",
                    ((LDWGEModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDWGEModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDWLEViewModel)
            {
                svbmodel.Setup(String.Format("LDWLE {0:s} {1:s}",
                    ((LDWLEModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDWLEModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDWLViewModel)
            {
                svbmodel.Setup(String.Format("LDWL {0:s} {1:s}",
                    ((LDWLModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDWLModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is LDWGViewModel)
            {
                svbmodel.Setup(String.Format("LDWG {0:s} {1:s}",
                    ((LDWGModel)(bvmodel.Model)).Value1.ToShowString(),
                    ((LDWGModel)(bvmodel.Model)).Value2.ToShowString()));
            }
            if (bvmodel is BCDViewModel)
            {
                svbmodel.Setup(String.Format("BCD {0:s} {1:s}",
                    ((BCDModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((BCDModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is BINViewModel)
            {
                svbmodel.Setup(String.Format("BIN {0:s} {1:s}",
                    ((BINModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((BINModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is DTOFViewModel)
            {
                svbmodel.Setup(String.Format("DTOF {0:s} {1:s}",
                    ((DTOFModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((DTOFModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is DTOWViewModel)
            {
                svbmodel.Setup(String.Format("DTOW {0:s} {1:s}",
                    ((DTOWModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((DTOWModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is ROUNDViewModel)
            {
                svbmodel.Setup(String.Format("ROUND {0:s} {1:s}",
                    ((ROUNDModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((ROUNDModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is TRUNCViewModel)
            {
                svbmodel.Setup(String.Format("TRUNC {0:s} {1:s}",
                    ((TRUNCModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((TRUNCModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is WTODViewModel)
            {
                svbmodel.Setup(String.Format("WTOD {0:s} {1:s}",
                    ((WTODModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((WTODModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is ADDFViewModel)
            {
                svbmodel.Setup(String.Format("ADDF {0:s} {1:s} {2:s}",
                    ((ADDFModel)(bvmodel.Model)).InputValue1.ToShowString(),
                    ((ADDFModel)(bvmodel.Model)).InputValue2.ToShowString(),
                    ((ADDFModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is SUBFViewModel)
            {
                svbmodel.Setup(String.Format("ADDF {0:s} {1:s} {2:s}",
                    ((SUBFModel)(bvmodel.Model)).InputValue1.ToShowString(),
                    ((SUBFModel)(bvmodel.Model)).InputValue2.ToShowString(),
                    ((SUBFModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is MULFViewModel)
            {
                svbmodel.Setup(String.Format("ADDF {0:s} {1:s} {2:s}",
                    ((MULFModel)(bvmodel.Model)).InputValue1.ToShowString(),
                    ((MULFModel)(bvmodel.Model)).InputValue2.ToShowString(),
                    ((MULFModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is DIVFViewModel)
            {
                svbmodel.Setup(String.Format("ADDF {0:s} {1:s} {2:s}",
                    ((DIVFModel)(bvmodel.Model)).InputValue1.ToShowString(),
                    ((DIVFModel)(bvmodel.Model)).InputValue2.ToShowString(),
                    ((DIVFModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is SINViewModel)
            {
                svbmodel.Setup(String.Format("SIN {0:s} {1:s}",
                    ((SINModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((SINModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is COSViewModel)
            {
                svbmodel.Setup(String.Format("COS {0:s} {1:s}",
                    ((COSModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((COSModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is TANViewModel)
            {
                svbmodel.Setup(String.Format("TAN {0:s} {1:s}",
                    ((TANModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((TANModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is LNViewModel)
            {
                svbmodel.Setup(String.Format("LN {0:s} {1:s}",
                    ((LNModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((LNModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is EXPViewModel)
            {
                svbmodel.Setup(String.Format("EXP {0:s} {1:s}",
                    ((EXPModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((EXPModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is SQRTViewModel)
            {
                svbmodel.Setup(String.Format("SQRT {0:s} {1:s}",
                    ((SQRTModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((SQRTModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is ADDViewModel)
            {
                svbmodel.Setup(String.Format("ADD {0:s} {1:s} {2:s}",
                    ((ADDModel)(bvmodel.Model)).InputValue1.ToShowString(),
                    ((ADDModel)(bvmodel.Model)).InputValue2.ToShowString(),
                    ((ADDModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is ADDDViewModel)
            {
                svbmodel.Setup(String.Format("ADDD {0:s} {1:s} {2:s}",
                    ((ADDDModel)(bvmodel.Model)).InputValue1.ToShowString(),
                    ((ADDDModel)(bvmodel.Model)).InputValue2.ToShowString(),
                    ((ADDDModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is SUBViewModel)
            {
                svbmodel.Setup(String.Format("SUB {0:s} {1:s} {2:s}",
                    ((SUBModel)(bvmodel.Model)).InputValue1.ToShowString(),
                    ((SUBModel)(bvmodel.Model)).InputValue2.ToShowString(),
                    ((SUBModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is SUBDViewModel)
            {
                svbmodel.Setup(String.Format("SUBD {0:s} {1:s} {2:s}",
                    ((SUBDModel)(bvmodel.Model)).InputValue1.ToShowString(),
                    ((SUBDModel)(bvmodel.Model)).InputValue2.ToShowString(),
                    ((SUBDModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is MULViewModel)
            {
                svbmodel.Setup(String.Format("MUL {0:s} {1:s} {2:s}",
                    ((MULModel)(bvmodel.Model)).InputValue1.ToShowString(),
                    ((MULModel)(bvmodel.Model)).InputValue2.ToShowString(),
                    ((MULModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is MULDViewModel)
            {
                svbmodel.Setup(String.Format("MULD {0:s} {1:s} {2:s}",
                    ((MULDModel)(bvmodel.Model)).InputValue1.ToShowString(),
                    ((MULDModel)(bvmodel.Model)).InputValue2.ToShowString(),
                    ((MULDModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is MULWViewModel)
            {
                svbmodel.Setup(String.Format("MULW {0:s} {1:s} {2:s}",
                    ((MULWModel)(bvmodel.Model)).InputValue1.ToShowString(),
                    ((MULWModel)(bvmodel.Model)).InputValue2.ToShowString(),
                    ((MULWModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is DIVViewModel)
            {
                svbmodel.Setup(String.Format("DIV {0:s} {1:s} {2:s}",
                    ((DIVModel)(bvmodel.Model)).InputValue1.ToShowString(),
                    ((DIVModel)(bvmodel.Model)).InputValue2.ToShowString(),
                    ((DIVModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is DIVDViewModel)
            {
                svbmodel.Setup(String.Format("DIVD {0:s} {1:s} {2:s}",
                    ((DIVDModel)(bvmodel.Model)).InputValue1.ToShowString(),
                    ((DIVDModel)(bvmodel.Model)).InputValue2.ToShowString(),
                    ((DIVDModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is DIVWViewModel)
            {
                svbmodel.Setup(String.Format("DIVW {0:s} {1:s} {2:s}",
                    ((DIVWModel)(bvmodel.Model)).InputValue1.ToShowString(),
                    ((DIVWModel)(bvmodel.Model)).InputValue2.ToShowString(),
                    ((DIVWModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is INCViewModel)
            {
                svbmodel.Setup(String.Format("INC {0:s} {1:s}",
                    ((INCModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((INCModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is INCDViewModel)
            {
                svbmodel.Setup(String.Format("INCD {0:s} {1:s}",
                    ((INCDModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((INCDModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is DECViewModel)
            {
                svbmodel.Setup(String.Format("DEC {0:s} {1:s}",
                    ((DECModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((DECModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is DECDViewModel)
            {
                svbmodel.Setup(String.Format("DECD {0:s} {1:s}",
                    ((DECDModel)(bvmodel.Model)).InputValue.ToShowString(),
                    ((DECDModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is ANDWViewModel)
            {
                svbmodel.Setup(String.Format("ANDW {0:s} {1:s} {2:s}",
                       ((ANDWModel)(bvmodel.Model)).InputValue1.ToShowString(),
                       ((ANDWModel)(bvmodel.Model)).InputValue2.ToShowString(),
                       ((ANDWModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is ANDDViewModel)
            {
                svbmodel.Setup(String.Format("ANDD {0:s} {1:s} {2:s}",
                       ((ANDDModel)(bvmodel.Model)).InputValue1.ToShowString(),
                       ((ANDDModel)(bvmodel.Model)).InputValue2.ToShowString(),
                       ((ANDDModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is ORWViewModel)
            {
                svbmodel.Setup(String.Format("ORW {0:s} {1:s} {2:s}",
                       ((ORWModel)(bvmodel.Model)).InputValue1.ToShowString(),
                       ((ORWModel)(bvmodel.Model)).InputValue2.ToShowString(),
                       ((ORWModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is ORDViewModel)
            {
                svbmodel.Setup(String.Format("ORD {0:s} {1:s} {2:s}",
                       ((ORDModel)(bvmodel.Model)).InputValue1.ToShowString(),
                       ((ORDModel)(bvmodel.Model)).InputValue2.ToShowString(),
                       ((ORDModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is XORWViewModel)
            {
                svbmodel.Setup(String.Format("XORW {0:s} {1:s} {2:s}",
                       ((XORWModel)(bvmodel.Model)).InputValue1.ToShowString(),
                       ((XORWModel)(bvmodel.Model)).InputValue2.ToShowString(),
                       ((XORWModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is XORDViewModel)
            {
                svbmodel.Setup(String.Format("XORD {0:s} {1:s} {2:s}",
                       ((XORDModel)(bvmodel.Model)).InputValue1.ToShowString(),
                       ((XORDModel)(bvmodel.Model)).InputValue2.ToShowString(),
                       ((XORDModel)(bvmodel.Model)).OutputValue.ToShowString()));
            }
            if (bvmodel is MOVViewModel)
            {
                svbmodel.Setup(String.Format("MOV {0:s} {1:s}",
                       ((MOVModel)(bvmodel.Model)).SourceValue.ToShowString(),
                       ((MOVModel)(bvmodel.Model)).DestinationValue.ToShowString()));
            }
            if (bvmodel is MOVDViewModel)
            {
                svbmodel.Setup(String.Format("MOVD {0:s} {1:s}",
                       ((MOVDModel)(bvmodel.Model)).SourceValue.ToShowString(),
                       ((MOVDModel)(bvmodel.Model)).DestinationValue.ToShowString()));
            }
            if (bvmodel is MOVFViewModel)
            {
                svbmodel.Setup(String.Format("MOVF {0:s} {1:s}",
                       ((MOVFModel)(bvmodel.Model)).SourceValue.ToShowString(),
                       ((MOVFModel)(bvmodel.Model)).DestinationValue.ToShowString()));
            }
            if (bvmodel is MVBLKViewModel)
            {
                svbmodel.Setup(String.Format("MVBLK {0:s} {1:s} {2:s}",
                       ((MVBLKModel)(bvmodel.Model)).SourceValue.ToShowString(),
                       ((MVBLKModel)(bvmodel.Model)).DestinationValue.ToShowString(),
                       ((MVBLKModel)(bvmodel.Model)).Count.ToShowString()));
            }
            if (bvmodel is MVDBLKViewModel)
            {
                svbmodel.Setup(String.Format("MVDBLK {0:s} {1:s} {2:s}",
                       ((MVDBLKModel)(bvmodel.Model)).SourceValue.ToShowString(),
                       ((MVDBLKModel)(bvmodel.Model)).DestinationValue.ToShowString(),
                       ((MVDBLKModel)(bvmodel.Model)).Count.ToShowString()));
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
                        ((FORModel)(bvmodel.Model)).Count.ToShowString()));
            }
            if (bvmodel is JMPViewModel)
            {
                svbmodel.Setup(String.Format("JMP {0:s}",
                       ((JMPModel)(bvmodel.Model)).LBLIndex.ToShowString()));
            }
            if (bvmodel is LBLViewModel)
            {
                svbmodel.Setup(String.Format("LBL {0:s}",
                       ((LBLModel)(bvmodel.Model)).LBLIndex.ToShowString()));
            }
            if (bvmodel is NEXTViewModel)
            {
                svbmodel.Setup("NEXT");
            }
            if (bvmodel is TRDViewModel)
            {
                svbmodel.Setup(String.Format("TRD {0:s}",
                       ((TRDModel)(bvmodel.Model)).StartValue.ToShowString()));
            }
            if (bvmodel is TWRViewModel)
            {
                svbmodel.Setup(String.Format("TWR {0:s}",
                       ((TWRModel)(bvmodel.Model)).StartValue.ToShowString()));
            }
            if (bvmodel is ROLDViewModel)
            {
                svbmodel.Setup(String.Format("ROLD {0:s} {1:s} {2:s}",
                       ((ROLDModel)(bvmodel.Model)).SourceValue.ToShowString(),
                       ((ROLDModel)(bvmodel.Model)).DestinationValue.ToShowString(),
                       ((ROLDModel)(bvmodel.Model)).Count.ToShowString()));
            }
            if (bvmodel is ROLViewModel)
            {
                svbmodel.Setup(String.Format("ROL {0:s} {1:s} {2:s}",
                       ((ROLModel)(bvmodel.Model)).SourceValue.ToShowString(),
                       ((ROLModel)(bvmodel.Model)).DestinationValue.ToShowString(),
                       ((ROLModel)(bvmodel.Model)).Count.ToShowString()));
            }
            if (bvmodel is RORDViewModel)
            {
                svbmodel.Setup(String.Format("RORD {0:s} {1:s} {2:s}",
                       ((RORDModel)(bvmodel.Model)).SourceValue.ToShowString(),
                       ((RORDModel)(bvmodel.Model)).DestinationValue.ToShowString(),
                       ((RORDModel)(bvmodel.Model)).Count.ToShowString()));
            }
            if (bvmodel is RORViewModel)
            {
                svbmodel.Setup(String.Format("ROR {0:s} {1:s} {2:s}",
                       ((RORModel)(bvmodel.Model)).SourceValue.ToShowString(),
                       ((RORModel)(bvmodel.Model)).DestinationValue.ToShowString(),
                       ((RORModel)(bvmodel.Model)).Count.ToShowString()));
            }
            if (bvmodel is SHLDViewModel)
            {
                svbmodel.Setup(String.Format("SHLD {0:s} {1:s} {2:s}",
                       ((SHLDModel)(bvmodel.Model)).SourceValue.ToShowString(),
                       ((SHLDModel)(bvmodel.Model)).DestinationValue.ToShowString(),
                       ((SHLDModel)(bvmodel.Model)).Count.ToShowString()));
            }
            if (bvmodel is SHLViewModel)
            {
                svbmodel.Setup(String.Format("SHL {0:s} {1:s} {2:s}",
                       ((SHLModel)(bvmodel.Model)).SourceValue.ToShowString(),
                       ((SHLModel)(bvmodel.Model)).DestinationValue.ToShowString(),
                       ((SHLModel)(bvmodel.Model)).Count.ToShowString()));
            }
            if (bvmodel is SHRDViewModel)
            {
                svbmodel.Setup(String.Format("SHRD {0:s} {1:s} {2:s}",
                       ((SHRDModel)(bvmodel.Model)).SourceValue.ToShowString(),
                       ((SHRDModel)(bvmodel.Model)).DestinationValue.ToShowString(),
                       ((SHRDModel)(bvmodel.Model)).Count.ToShowString()));
            }
            if (bvmodel is SHRViewModel)
            {
                svbmodel.Setup(String.Format("SHR {0:s} {1:s} {2:s}",
                       ((SHRModel)(bvmodel.Model)).SourceValue.ToShowString(),
                       ((SHRModel)(bvmodel.Model)).DestinationValue.ToShowString(),
                       ((SHRModel)(bvmodel.Model)).Count.ToShowString()));
            }
            svbmodel.X = bvmodel.X;
            svbmodel.Y = bvmodel.Y;
            return svbmodel;
        }
        #endregion
        
    }


}
