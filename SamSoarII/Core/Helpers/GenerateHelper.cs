using SamSoarII.Core.Generate;
using SamSoarII.Core.Models;
using SamSoarII.Core.Simulate;
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
            Generate(project.MainDiagram, nets);
            foreach (LadderDiagramModel diagram in project.Diagrams)
                if (!diagram.IsMainLadder) Generate(diagram, nets);
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
            BreakpointManager.Initialize();
            InstHelper.InstToSimuCode(sw, nets.ToArray());
            sw.Write("void InitUserRegisters()\r\n{\r\n");
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
                            sw.Write("*((int32_t*)(&{0:s})) = {1:s};\r\n",
                                varname, imodel.ShowValue);
                            break;
                        case 2:
                            sw.Write("*((uint32_t*)(&{0:s})) = {1:s};\r\n",
                                varname, imodel.ShowValue);
                            break;
                        case 3:
                            sw.Write("*((int64_t*)(&{0:s})) = {1:s};\r\n",
                                varname, imodel.ShowValue);
                            break;
                        case 4:
                            sw.Write("*((uint64_t*)(&{0:s})) = {1:s};\r\n",
                                varname, imodel.ShowValue);
                            break;
                        case 5:
                            sw.Write("{0:s} = _BCD_to_WORD({1:s});\r\n",
                                varname, imodel.ShowValue);
                            break;
                        case 6:
                            sw.Write("*((double*)(&{0:s})) = {1:s};\r\n",
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
            sw.Write("typedef int32_t _BIT;\r\n");
            sw.Write("typedef int32_t _WORD;\r\n");
            sw.Write("typedef int64_t D_WORD;\r\n");
            sw.Write("typedef double _FLOAT;\r\n");
            foreach (FuncBlockModel fbmodel in project.FuncBlocks)
                GenerateCHeader(fbmodel, sw);
            sw.Close();
            // 生成用户函数的c语言
            sw = new StreamWriter(funcBlockCFile);
            sw.Write("#include <math.h>\r\n");
            sw.Write("#include \"simuf.h\"\r\n");
            foreach (FuncBlockModel fbmodel in project.FuncBlocks)
                GenerateCCode(fbmodel, sw);
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
            File.Delete(ladderHFile);
            File.Delete(ladderCFile);
            File.Delete(simulibHFile);
            File.Delete(simulibCFile);
            File.Delete(funcBlockHFile);
            File.Delete(funcBlockCFile);
            return SimulateDllModel.LoadDll(outputDllFile);
        }

        public static void GenerateFinal(ProjectModel project, string plclibAFile)
        {
            List<InstHelper.PLCInstNetwork> nets =
                new List<InstHelper.PLCInstNetwork>();
            Generate(project.MainDiagram, nets);
            foreach (LadderDiagramModel diagram in project.Diagrams)
            {
                if (!diagram.IsMainLadder) Generate(diagram, nets);
            }
            // 建立仿真的c环境的路径
            string currentPath = Utility.FileHelper.AppRootPath;
            string ladderHFile = String.Format(@"{0:s}\downg\downc.h", currentPath);
            string ladderCFile = String.Format(@"{0:s}\downg\downc.c", currentPath);
            string funcBlockHFile = String.Format(@"{0:s}\downg\downf.h", currentPath);
            string funcBlockCFile = String.Format(@"{0:s}\downg\downf.c", currentPath);
            string downlibOFile = String.Format(@"{0:s}\downg\downlib.o", currentPath);
            string memLDFile = String.Format(@"{0:s}\downg\mem.ld", currentPath);
            string libsLDFile = String.Format(@"{0:s}\downg\libs.ld", currentPath);
            string sectionsLDFile = String.Format(@"{0:s}\downg\sections.ld", currentPath);
            string outputElfFile = String.Format(@"{0:s}\downc.elf", currentPath);
            string outputBinFile = String.Format(@"{0:s}\downc.bin", currentPath);
            string aaMapFile = String.Format(@"{0:s}\downg\aa.map", currentPath);
            plclibAFile = String.Format(@"{0:s}\downg\{1:s}", currentPath, plclibAFile);
            // 生成梯形图的c语言
            StreamWriter sw = new StreamWriter(ladderCFile);
            InstHelper.InstToDownCode(sw, nets.ToArray());
            sw.Close();
            // 生成用户函数的头文件
            sw = new StreamWriter(funcBlockHFile);
            sw.Write("#include <stdint.h>\n");
            sw.Write("typedef int32_t _BIT;\n");
            sw.Write("typedef int16_t _WORD;\n");
            sw.Write("typedef int32_t D_WORD;\n");
            sw.Write("typedef float _FLOAT;\n");
            foreach (FuncBlockModel fbmodel in project.FuncBlocks)
            {
                GenerateCHeader(fbmodel, sw);
            }
            sw.Close();
            // 生成用户函数的c语言
            sw = new StreamWriter(funcBlockCFile);
            sw.Write("#include \"downf.h\"\n");
            sw.Write("#include <math.h>\n");
            foreach (FuncBlockModel fbmodel in project.FuncBlocks)
            {
                GenerateCCode(fbmodel, sw);
            }
            sw.Close();
            Process cmd = null;
            cmd = new Process();
            cmd.StartInfo.WorkingDirectory = String.Format(@"{0:s}\downg\.", currentPath);
            cmd.StartInfo.FileName = String.Format(@"{0:s}\downg\make.cmd", currentPath);
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.Start();
            cmd.WaitForExit();
            cmd = new Process();
            cmd.StartInfo.WorkingDirectory = String.Format(@"{0:s}\downg\.", currentPath);
            cmd.StartInfo.FileName = String.Format(@"{0:s}\downg\clean.cmd", currentPath);
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.Start();
            cmd.WaitForExit();
        }

        private static void Generate(
            LadderDiagramModel diagram,
            List<InstHelper.PLCInstNetwork> nets)
        {
            foreach (LadderNetworkModel network in diagram.Children)
            {
                Generate(network, nets);
            }
        }

        private static void Generate(
            LadderNetworkModel network,
            List<InstHelper.PLCInstNetwork> nets)
        {
            InstHelper.PLCInstNetwork net = new InstHelper.PLCInstNetwork(
                network.Parent.Name,
                network.Inst.Insts.Select(i => i.Inst).ToArray());
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

        private static void GenerateCCode(
            FuncBlockModel fbvmodel,
            StreamWriter sw)
        {
            sw.Write(GenerateCType(fbvmodel.View != null ? fbvmodel.View.Code : fbvmodel.Code));
        }
    }
}
