using SamSoarII.Core.Generate;
using SamSoarII.Core.Models;
using SamSoarII.Core.Simulate;
using SamSoarII.PLCDevice;
using SamSoarII.Shell.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Helpers
{
    public abstract class GenerateHelper : IHelper
    {
        public static int GenerateSimu(ProjectModel project)
        {
            List<InstHelper.PLCInstNetwork> nets =
                new List<InstHelper.PLCInstNetwork>();
            project.MainDiagram.CName = "_RunLadder";
            int diagramid = 0;
            foreach (LadderDiagramModel diagram in project.Diagrams)
                if (!diagram.IsMainLadder)
                    diagram.CName = String.Format("_SBR_{0:d}", diagramid++);
            Generate(project.MainDiagram, nets);
            foreach (LadderDiagramModel diagram in project.Diagrams)
                if (!diagram.IsMainLadder)
                    Generate(diagram, nets);
            // 建立仿真的c环境的路径
            string currentPath = Utility.FileHelper.AppRootPath;
            string ladderHFile = String.Format(@"{0:s}\simug\simuc.h", currentPath);
            string ladderCFile = String.Format(@"{0:s}\simug\simuc.c", currentPath);
            string funcBlockHFile = String.Format(@"{0:s}\simug\simuf.h", currentPath);
            string funcBlockCFile = String.Format(@"{0:s}\simug\simuf.c", currentPath);
            string simulibHFile = String.Format(@"{0:s}\simug\simulib.h", currentPath);
            string simulibCFile = String.Format(@"{0:s}\simug\simulib.c", currentPath);
            string outputDllFile = String.Format(@"{0:s}\simuc.dll", currentPath);
            // 生成梯形图的c语言
            StreamWriter sw = new StreamWriter(ladderCFile);
            InstHelper.InstToSimuCode(sw, nets.ToArray());
            sw.Write("void InitUserRegisters()\r\n{\r\n");
            sw.Write("ClearEdge();\r\n");
            ValueManager ValueManager = project.Parent.MNGValue;
            foreach (ValueInfo vinfo in ValueManager)
            {
                if (vinfo.InitModel != null)
                {
                    IElementInitializeModel imodel = vinfo.InitModel;
                    string varname = String.Empty;
                    switch (imodel.Base)
                    {
                        case "X":
                        case "Y":
                        case "S":
                        case "M":
                        case "T":
                        case "C":
                            varname = String.Format("{0:s}Bit[{1:d}]",
                                imodel.Base, imodel.Offset);
                            break;
                        case "D":
                        case "TV":
                        case "AI":
                        case "AO":
                        case "V":
                        case "Z":
                            varname = String.Format("{0:s}Word[{1:d}]",
                                imodel.Base, imodel.Offset);
                            break;
                        case "CV":
                            varname = String.Format("{0:s}{1:s}[{2:d}]",
                                imodel.Base,
                                (imodel.DataType == 3 || imodel.DataType == 4)
                                    ? "DoubleWord" : "Word",
                                (imodel.DataType == 3 || imodel.DataType == 4)
                                    ? (imodel.Offset - 200) : imodel.Offset);
                            break;
                    }
                    switch (imodel.DataType)
                    {
                        case 0:
                            sw.Write("{0:s} = {1:d};\r\n",
                                varname, imodel.ShowValue.Equals("ON") ? 1 : 0);
                            break;
                        case 1:
                            sw.Write("*((_WORD)(&{0:s})) = {1:s};\r\n",
                                varname, imodel.ShowValue);
                            break;
                        case 2:
                            sw.Write("*((U_WORD)(&{0:s})) = {1:s};\r\n",
                                varname, imodel.ShowValue);
                            break;
                        case 3:
                            sw.Write("*((D_WORD)(&{0:s})) = {1:s};\r\n",
                                varname, imodel.ShowValue);
                            break;
                        case 4:
                            sw.Write("*((UD_WORD)(&{0:s})) = {1:s};\r\n",
                                varname, imodel.ShowValue);
                            break;
                        case 5:
                            sw.Write("*((_WORD)(&{0:s})) = _BCD_to_WORD({1:s});\r\n",
                                varname, imodel.ShowValue);
                            break;
                        case 6:
                            sw.Write("*((_FLOAT)(&{0:s})) = {1:s};\r\n",
                                varname, imodel.ShowValue);
                            break;
                    }
                }
            }
            sw.Write("}\r\n");
            sw.Close();
            // 生成用户函数的c语言头
            sw = new StreamWriter(funcBlockHFile);
            sw.Write("#include<stdint.h>\r\n");
            sw.Write("typedef int8_t* _BIT;\r\n");
            sw.Write("typedef int16_t* _WORD;\r\n");
            sw.Write("typedef uint16_t* U_WORD;\r\n");
            sw.Write("typedef int32_t* D_WORD;\r\n");
            sw.Write("typedef uint32_t* UD_WORD;\r\n");
            sw.Write("typedef float* _FLOAT;\r\n");
            sw.Write("#define DW ((D_WORD)W)\r\n");
            sw.Write("#define FW ((_FLOAT)W)\r\n");
            sw.Write("extern void callinfo();\r\n");
            sw.Write("extern void callleave();\r\n");
            sw.Write("extern void bpcycle(int addr);\r\n");
            foreach (FuncBlockModel fbmodel in project.FuncBlocks)
                GenerateCHeader(fbmodel, sw);
            sw.Close();
            // 生成用户函数的c语言
            sw = new StreamWriter(funcBlockCFile);
            sw.Write("#include <math.h>\r\n");
            sw.Write("#include \"simuf.h\"\r\n");
            sw.Write("extern _BIT XBit;\r\n");
            sw.Write("extern _BIT YBit;\r\n");
            sw.Write("extern _BIT SBit;\r\n");
            sw.Write("extern _BIT MBit;\r\n");
            sw.Write("extern _BIT CBit;\r\n");
            sw.Write("extern _BIT TBit;\r\n");
            sw.Write("extern _WORD DWord;\r\n");
            sw.Write("extern _WORD TVWord;\r\n");
            sw.Write("extern _WORD CVWord;\r\n");
            sw.Write("extern _WORD VWord;\r\n");
            sw.Write("extern _WORD ZWord;\r\n");
            sw.Write("extern _WORD AIWord;\r\n");
            sw.Write("extern _WORD AOWord;\r\n");
            sw.Write("extern D_WORD CVDoubleWord;\r\n");
            foreach (FuncBlockModel fbmodel in project.FuncBlocks)
                GenerateCCode(fbmodel, sw, true);
            sw.Close();
            SimulateDllModel.CreateSource();
            Process cmd = null;
            cmd = new Process();
            cmd.StartInfo.FileName
                = String.Format(@"{0:s}\Compiler\tcc\tcc", currentPath);
            cmd.StartInfo.Arguments
                = String.Format("\"{0:s}\" \"{1:s}\" \"{2:s}\" -o \"{3:s}\" -shared -DBUILD_DLL",
                    simulibCFile, ladderCFile, funcBlockCFile, outputDllFile);
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.Start();
            cmd.WaitForExit();
#if RELEASE
            File.Delete(ladderHFile);
            File.Delete(ladderCFile);
            File.Delete(simulibHFile);
            File.Delete(simulibCFile);
            File.Delete(funcBlockHFile);
            File.Delete(funcBlockCFile);
#endif
            return SimulateDllModel.LoadDll(outputDllFile);
        }

        public static bool GenerateFinal(ProjectModel project)
        {
            List<InstHelper.PLCInstNetwork> nets =
                new List<InstHelper.PLCInstNetwork>();
            project.MainDiagram.CName = "RunLadder";
            int diagramid = 0;
            foreach (LadderDiagramModel diagram in project.Diagrams)
                if (!diagram.IsMainLadder)
                    diagram.CName = String.Format("_SBR_{0:d}", diagramid++);
            Generate(project.MainDiagram, nets);
            foreach (LadderDiagramModel diagram in project.Diagrams)
                if (!diagram.IsMainLadder)
                    Generate(diagram, nets);
            // 建立仿真的c环境的路径
            string currentPath = Utility.FileHelper.AppRootPath;
            string ladderHFile = String.Format(@"{0:s}\downg\downc.h", currentPath);
            string ladderCFile = String.Format(@"{0:s}\downg\downc.c", currentPath);
            string funcBlockHFile = String.Format(@"{0:s}\downg\downf.h", currentPath);
            string funcBlockCFile = String.Format(@"{0:s}\downg\downf.c", currentPath);
            string downlibOFile = String.Format(@"{0:s}\downg\downlib.o", currentPath);
            
            // 生成梯形图的c语言
            StreamWriter sw = new StreamWriter(ladderCFile);
            sw.Write("void InitUserRegisters();\r\n");
            InstHelper.InstToDownCode(sw, nets.ToArray());
            sw.Write("void InitUserRegisters()\r\n{\r\n");
            if (DownloadHelper.IsDownloadInitialize)
            {
                ValueManager ValueManager = project.Parent.MNGValue;
                foreach (ValueInfo vinfo in ValueManager)
                {
                    if (vinfo.InitModel != null)
                    {
                        IElementInitializeModel imodel = vinfo.InitModel;
                        string varname = String.Empty;
                        switch (imodel.Base)
                        {
                            case "X":
                            case "Y":
                            case "S":
                            case "M":
                            case "T":
                            case "C":
                                varname = String.Format("{0:s}Bit[{1:d}]",
                                    imodel.Base, imodel.Offset);
                                break;
                            case "D":
                            case "TV":
                            case "AI":
                            case "AO":
                            case "V":
                            case "Z":
                                varname = String.Format("{0:s}Word[{1:d}]",
                                    imodel.Base, imodel.Offset);
                                break;
                            case "CV":
                                varname = String.Format("{0:s}{1:s}[{2:d}]",
                                    imodel.Base,
                                    (imodel.DataType == 3 || imodel.DataType == 4)
                                        ? "32DoubleWord" : "Word",
                                    (imodel.DataType == 3 || imodel.DataType == 4)
                                        ? (imodel.Offset - 200) : imodel.Offset);
                                break;
                        }
                        switch (imodel.DataType)
                        {
                            case 0:
                                sw.Write("{0:s} = {1:d};\r\n",
                                    varname, imodel.ShowValue.Equals("ON") ? 1 : 0);
                                break;
                            case 1:
                                sw.Write("*((_WORD)(&{0:s})) = {1:s};\r\n",
                                    varname, imodel.ShowValue);
                                break;
                            case 2:
                                sw.Write("*((U_WORD)(&{0:s})) = {1:s};\r\n",
                                    varname, imodel.ShowValue);
                                break;
                            case 3:
                                sw.Write("*((D_WORD)(&{0:s})) = {1:s};\r\n",
                                    varname, imodel.ShowValue);
                                break;
                            case 4:
                                sw.Write("*((UD_WORD)(&{0:s})) = {1:s};\r\n",
                                    varname, imodel.ShowValue);
                                break;
                            case 5:
                                sw.Write("*((_WORD)(&{0:s})) = _BCD_to_WORD({1:s});\r\n",
                                    varname, imodel.ShowValue);
                                break;
                            case 6:
                                sw.Write("*((_FLOAT)(&{0:s})) = {1:s};\r\n",
                                    varname, imodel.ShowValue);
                                break;
                        }
                    }
                }
            }
            sw.Write("}\r\n");
            sw.Close();
            // 生成用户函数的头文件
            sw = new StreamWriter(funcBlockHFile);
            sw.Write("#include <stdint.h>\n");
            sw.Write("typedef int32_t* _BIT;\n");
            sw.Write("typedef int16_t* _WORD;\n");
            sw.Write("typedef uint16_t* U_WORD;\n");
            sw.Write("typedef int32_t* D_WORD;\n");
            sw.Write("typedef uint32_t* UD_WORD;\n");
            sw.Write("typedef float* _FLOAT;\n");
            sw.Write("#define DW ((D_WORD)W)\r\n");
            sw.Write("#define FW ((_FLOAT)W)\r\n");
            foreach (FuncBlockModel fbmodel in project.FuncBlocks)
                GenerateCHeader(fbmodel, sw);
            sw.Close();
            // 生成用户函数的c语言
            sw = new StreamWriter(funcBlockCFile);
            sw.Write("#include \"downf.h\"\n");
            sw.Write("#include <math.h>\n");
            sw.Write("extern _BIT XBit;\r\n");
            sw.Write("extern _BIT YBit;\r\n");
            sw.Write("extern _BIT SBit;\r\n");
            sw.Write("extern _BIT MBit;\r\n");
            sw.Write("extern _BIT CBit;\r\n");
            sw.Write("extern _BIT TBit;\r\n");
            sw.Write("extern _WORD DWord;\r\n");
            sw.Write("extern _WORD TVWord;\r\n");
            sw.Write("extern _WORD CVWord;\r\n");
            sw.Write("extern _WORD VWord;\r\n");
            sw.Write("extern _WORD ZWord;\r\n");
            sw.Write("extern _WORD AIWord;\r\n");
            sw.Write("extern _WORD AOWord;\r\n");
            sw.Write("extern D_WORD CVDoubleWord;\r\n");
            foreach (FuncBlockModel fbmodel in project.FuncBlocks)
                GenerateCCode(fbmodel, sw);
            sw.Close();
            Process cmd = null;
            cmd = new Process();
            cmd.StartInfo.WorkingDirectory = String.Format(@"{0:s}\downg\.", currentPath);
            //由于暂不支持多脉冲，故默认为(PLC_FGs_Type)
            if (project.Parent.MNGComu.PLCMessage.PLCType is PLC_FGs_Type)
            {
                switch ((PLC_FGs_Type)project.Parent.MNGComu.PLCMessage.PLCType)
                {
                    case PLC_FGs_Type.FGs_16MR_A:
                    case PLC_FGs_Type.FGs_16MR_D:
                    case PLC_FGs_Type.FGs_16MT_A:
                    case PLC_FGs_Type.FGs_16MT_D:
                    case PLC_FGs_Type.FGs_32MR_A:
                    case PLC_FGs_Type.FGs_32MR_D:
                    case PLC_FGs_Type.FGs_32MT_A:
                    case PLC_FGs_Type.FGs_32MT_D:
                        cmd.StartInfo.FileName = String.Format(@"{0:s}\downg\make32.cmd", currentPath);
                        break;
                    case PLC_FGs_Type.FGs_64MR_A:
                    case PLC_FGs_Type.FGs_64MR_D:
                    case PLC_FGs_Type.FGs_64MT_A:
                    case PLC_FGs_Type.FGs_64MT_D:
                        cmd.StartInfo.FileName = String.Format(@"{0:s}\downg\make64.cmd", currentPath);
                        break;
                    default:
                        cmd.StartInfo.FileName = String.Format(@"{0:s}\downg\make.cmd", currentPath);
                        break;
                }
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                //cmd.StartInfo.RedirectStandardOutput = true;
                //cmd.StartInfo.RedirectStandardError = true;
                cmd.Start();
                cmd.WaitForExit();
                return true;
            }
            else return false;
#if RELEASE
            cmd = new Process();
            cmd.StartInfo.WorkingDirectory = String.Format(@"{0:s}\downg\.", currentPath);
            cmd.StartInfo.FileName = String.Format(@"{0:s}\downg\clean.cmd", currentPath);
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            //cmd.StartInfo.RedirectStandardOutput = true;
            //cmd.StartInfo.RedirectStandardError = true;
            cmd.Start();
            cmd.WaitForExit();
#endif
        }

        private static void Generate(
            LadderDiagramModel diagram,
            List<InstHelper.PLCInstNetwork> nets)
        {
            foreach (LadderNetworkModel network in diagram.Children.Where(n => !n.IsMasked))
                Generate(network, nets);
        }

        private static void Generate(
            LadderNetworkModel network,
            List<InstHelper.PLCInstNetwork> nets)
        {
            ProjectModel project = network.Parent.Parent;
            PLCInstruction[] insts = network.Inst.Insts.Select(i => i.Inst).ToArray();
            foreach (PLCInstruction inst in insts)
            {
                if (inst[0].Equals("CALL") || inst[0].Equals("ATCH"))
                {
                    LadderDiagramModel diagram = project.Diagrams.FirstOrDefault(d => d.Name.Equals(inst[1]));
                    if (diagram != null) inst[1] = diagram.CName;
                }
                if (inst[0].Equals("MBUS"))
                {
                    ModbusModel modbus = project.Modbus.Children.FirstOrDefault(m => m.Name.Equals(inst[2]));
                    if (modbus != null) inst[2] = modbus.Parent.Children.IndexOf(modbus).ToString(); 
                }
            }
            InstHelper.PLCInstNetwork net = new InstHelper.PLCInstNetwork(
                network.Parent.CName, insts);
            nets.Add(net);
        }
        
        private static string GenerateCType(string type)
        {
            type = type.Replace("BIT", "_BIT");
            type = type.Replace("WORD", "_WORD");
            type = type.Replace("FLOAT", "_FLOAT");
            return type;
        }

        private static void GenerateCHeader(
            FuncBlockModel fbvmodel,
            StreamWriter sw)
        {
            foreach (FuncModel fmodel in fbvmodel.Funcs)
            {
                if (fmodel.ArgCount == 0)
                {
                    sw.Write("{0:s} {1:s}();",
                        GenerateCType(fmodel.ReturnType),
                        fmodel.Name);
                }
                else
                {
                    sw.Write("{0:s} {1:s}({2:s} {3:s}",
                        GenerateCType(fmodel.ReturnType),
                        fmodel.Name,
                        GenerateCType(fmodel.GetArgType(0)),
                        fmodel.GetArgName(0));
                    for (int i = 1; i < fmodel.ArgCount; i++)
                    {
                        sw.Write(",{0:s} {1:s}",
                            GenerateCType(fmodel.GetArgType(i)),
                            fmodel.GetArgName(i));
                    }
                    sw.Write(");\r\n");
                }
            }
        }

        private static List<FuncBlock_Local> funcs = new List<FuncBlock_Local>();
        private static List<FuncBlock> bps = new List<FuncBlock>();
        private static void GenerateCCode(FuncBlockModel fbmodel, StreamWriter sw, bool simu = false)
        {
            if (!simu)
            {
                sw.Write(GenerateCType(fbmodel.Code));
                return;
            }
            funcs.Clear();
            bps.Clear();
            FindInFuncBlock(fbmodel.Root);
            StringBuilder code = new StringBuilder();
            int start = 0, len = 0;
            if (funcs.Count() == 0)
            {
                sw.Write(GenerateCType(fbmodel.Code));
                return;
            }

            for (int i1 = 0, i2 = 0; i1 < funcs.Count(); i1++)
            {
                start = i1 == 0 ? 0 : funcs[i1 - 1].IndexEnd + 1;
                len = funcs[i1].IndexStart - start - 1;
                if (len > 0) code.Append(fbmodel.Code.Substring(start, len));
                code.Append("{callinto();");

                bool firstbp = true;
                for (; i2 < bps.Count() && bps[i2].IndexStart < funcs[i1].IndexEnd; i2++)
                {
                    start = firstbp ? funcs[i1].IndexStart + 1 : bps[i2 - 1].IndexEnd + 1;
                    len = bps[i2].IndexStart - start;
                    if (len > 0) code.Append(fbmodel.Code.Substring(start, len));
                    
                    if (bps[i2] is FuncBlock_Statement) code.Append("{");
                    start = bps[i2].IndexStart;
                    len = bps[i2].IndexEnd - start + 1;
                    code.Append(String.Format("bpcycle({0:d});", bps[i2].BPAddress));
                    if (bps[i2] is FuncBlock_Return) code.Append("callleave();");
                    if (len > 0) code.Append(fbmodel.Code.Substring(start, len));
                    if (bps[i2] is FuncBlock_Statement) code.Append("}");
                    firstbp = false;
                }

                start = firstbp ? funcs[i1].IndexStart + 1 : bps[i2 - 1].IndexEnd + 1;
                len = funcs[i1].IndexEnd - start;
                if (len > 0) code.Append(fbmodel.Code.Substring(start, len));
                code.Append("callleave();}");
            }
            
            code.Append(fbmodel.Code.Substring(funcs.Last().IndexEnd + 1));
            sw.Write(GenerateCType(code.ToString()));
        }

        private static void FindInFuncBlock(FuncBlock fblock)
        {
            if (fblock is FuncBlock_Local)
            {
                FuncBlock_Local local = (FuncBlock_Local)fblock;
                if (local.Header != null) funcs.Add(local);
            }
            if (fblock.Breakpoint != null)
                bps.Add(fblock);
            foreach (FuncBlock sub in fblock.Childrens)
                FindInFuncBlock(sub);
        }
    }
}
