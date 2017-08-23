using SamSoarII.Core.Generate;
using SamSoarII.Core.Models;
using SamSoarII.Shell.Models;
using SamSoarII.Shell.Models.Ladders;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII
{
    public abstract class AllResourceManager
    {
        static private ResourceManager<LadderNetworkViewModel> rmgNet;
        static private ResourceManager<InstructionNetworkViewModel> rmgINet;
        static private ResourceManager<InputViewModel> rmgInput;
        static private ResourceManager<OutputViewModel> rmgOutput;
        static private ResourceManager<OutputRectViewModel> rmgOutRec;
        static private ResourceManager<SpecialViewModel> rmgSpecial;
        static private ResourceManager<HLineViewModel> rmgHLine;
        static private ResourceManager<VLineViewModel> rmgVLine;
        static private ResourceManager<InstructionRowViewModel> rmgIRow;
        static private ResourceManager<LadderBrpoViewModel> rmgBrpo;

        static private ResourceManager<InputVisualUnitModel> vuInput;
        static private ResourceManager<OutputVisualUnitModel> vuOutput;
        static private ResourceManager<OutputRectVisualUnitModel> vuOutRec;
        static private ResourceManager<SpecialVisualUnitModel> vuSpecial;
        static private ResourceManager<HLineVisualUnitModel> vuHLine;
        static private ResourceManager<VLineVisualUnitModel> vuVLine;
        static private ResourceManager<LadderBrpoVisualModel> vuBrpo;
        static public void Initialize()
        {
            rmgNet = new ResourceManager<LadderNetworkViewModel>(new LadderNetworkViewModel(null), 20, new object[] { null });
            rmgINet = new ResourceManager<InstructionNetworkViewModel>(new InstructionNetworkViewModel(null), 20, new object[] { null });
            rmgInput = new ResourceManager<InputViewModel>(new InputViewModel(null), 100, new object[] { null });
            rmgOutput = new ResourceManager<OutputViewModel>(new OutputViewModel(null), 25, new object[] { null });
            rmgOutRec = new ResourceManager<OutputRectViewModel>(new OutputRectViewModel(null), 25, new object[] { null });
            rmgSpecial = new ResourceManager<SpecialViewModel>(new SpecialViewModel(null), 25, new object[] { null });
            rmgHLine = new ResourceManager<HLineViewModel>(new HLineViewModel(null), 125, new object[] { null });
            rmgVLine = new ResourceManager<VLineViewModel>(new VLineViewModel(null), 75, new object[] { null });
            rmgIRow = new ResourceManager<InstructionRowViewModel>(new InstructionRowViewModel(null, 0), 25, new object[] { null, 0 });
            rmgBrpo = new ResourceManager<LadderBrpoViewModel>(new LadderBrpoViewModel(null), 20, new object[] { null });

            vuInput = new ResourceManager<InputVisualUnitModel>(new InputVisualUnitModel(null), 100, new object[] { null });
            vuOutput = new ResourceManager<OutputVisualUnitModel>(new OutputVisualUnitModel(null), 25, new object[] { null });
            vuOutRec = new ResourceManager<OutputRectVisualUnitModel>(new OutputRectVisualUnitModel(null), 25, new object[] { null });
            vuSpecial = new ResourceManager<SpecialVisualUnitModel>(new SpecialVisualUnitModel(null), 25, new object[] { null });
            vuHLine = new ResourceManager<HLineVisualUnitModel>(new HLineVisualUnitModel(null), 125, new object[] { null });
            vuVLine = new ResourceManager<VLineVisualUnitModel>(new VLineVisualUnitModel(null), 75, new object[] { null });
            vuBrpo = new ResourceManager<LadderBrpoVisualModel>(new LadderBrpoVisualModel(null), 20, new object[] { null });
        }

        static public LadderNetworkViewModel CreateNet(LadderNetworkModel core)
        {
            return rmgNet.Create(core);
        }
        static public void Dispose(LadderNetworkViewModel net)
        {
            rmgNet.Dispose(net);
        }

        static public InstructionNetworkViewModel CreateINet(InstructionNetworkModel core)
        {
            return rmgINet.Create(core);
        }
        static public void Dispose(InstructionNetworkViewModel inet)
        {
            rmgINet.Dispose(inet);
        }
    
        static public LadderBrpoViewModel CreateBrpo(LadderBrpoModel core)
        {
            return rmgBrpo.Create(core);
        }
        static public void Dispose(LadderBrpoViewModel brpo)
        {
            rmgBrpo.Dispose(brpo);
        }

        static public InstructionRowViewModel CreateInstRow(PLCOriginInst inst, int id)
        {
            return rmgIRow.Create(inst, id);
        }
        static public void Dispose(InstructionRowViewModel irow)
        {
            rmgIRow.Dispose(irow);
        }
        
        static public InputViewModel CreateInput(LadderUnitModel _core)
        {
            return rmgInput.Create(_core);
        }
        static public void Dispose(InputViewModel _view)
        {
            rmgInput.Dispose(_view);
        }

        static public OutputViewModel CreateOutput(LadderUnitModel _core)
        {
            return rmgOutput.Create(_core);
        }
        static public void Dispose(OutputViewModel _view)
        {
            rmgOutput.Dispose(_view);
        }

        static public OutputRectViewModel CreateOutRec(LadderUnitModel _core)
        {
            return rmgOutRec.Create(_core);
        }
        static public void Dispose(OutputRectViewModel _view)
        {
            rmgOutRec.Dispose(_view);
        }

        static public SpecialViewModel CreateSpecial(LadderUnitModel _core)
        {
            return rmgSpecial.Create(_core);
        }
        static public void Dispose(SpecialViewModel _view)
        {
            rmgSpecial.Dispose(_view);
        }

        static public HLineViewModel CreateHLine(LadderUnitModel _core)
        {
            return rmgHLine.Create(_core);
        }
        static public void Dispose(HLineViewModel _view)
        {
            rmgHLine.Dispose(_view);
        }

        static public VLineViewModel CreateVLine(LadderUnitModel _core)
        {
            return rmgVLine.Create(_core);
        }
        static public void Dispose(VLineViewModel _view)
        {
            rmgVLine.Dispose(_view);
        }

        static public InputVisualUnitModel CreateVisualInput(LadderUnitModel _core)
        {
            return vuInput.Create(_core);
        }
        static public void Dispose(InputVisualUnitModel _view)
        {
            vuInput.Dispose(_view);
        }

        static public OutputVisualUnitModel CreateVisualOutput(LadderUnitModel _core)
        {
            return vuOutput.Create(_core);
        }
        static public void Dispose(OutputVisualUnitModel _view)
        {
            vuOutput.Dispose(_view);
        }

        static public OutputRectVisualUnitModel CreateVisualOutRec(LadderUnitModel _core)
        {
            return vuOutRec.Create(_core);
        }
        static public void Dispose(OutputRectVisualUnitModel _view)
        {
            vuOutRec.Dispose(_view);
        }

        static public SpecialVisualUnitModel CreateVisualSpecial(LadderUnitModel _core)
        {
            return vuSpecial.Create(_core);
        }
        static public void Dispose(SpecialVisualUnitModel _view)
        {
            vuSpecial.Dispose(_view);
        }

        static public HLineVisualUnitModel CreateVisualHLine(LadderUnitModel _core)
        {
            return vuHLine.Create(_core);
        }
        static public void Dispose(HLineVisualUnitModel _view)
        {
            vuHLine.Dispose(_view);
        }

        static public VLineVisualUnitModel CreateVisualVLine(LadderUnitModel _core)
        {
            return vuVLine.Create(_core);
        }
        static public void Dispose(VLineVisualUnitModel _view)
        {
            vuVLine.Dispose(_view);
        }
        static public LadderBrpoVisualModel CreateVisualBrpo(LadderBrpoModel core)
        {
            return vuBrpo.Create(core);
        }
        static public void Dispose(LadderBrpoVisualModel brpo)
        {
            vuBrpo.Dispose(brpo);
        }
    }
}
