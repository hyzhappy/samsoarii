using SamSoarII.Core.Generate;
using SamSoarII.Core.Models;
using SamSoarII.Shell.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII
{
    public abstract class AllResourceManager
    {
        static private ResourceManager<InputViewModel> rmgInput;
        static private ResourceManager<OutputViewModel> rmgOutput;
        static private ResourceManager<OutputRectViewModel> rmgOutRec;
        static private ResourceManager<SpecialViewModel> rmgSpecial;
        static private ResourceManager<HLineViewModel> rmgHLine;
        static private ResourceManager<VLineViewModel> rmgVLine;
        static private ResourceManager<InstructionRowViewModel> rmgIRow;

        static public void Initialize()
        {
            rmgInput = new ResourceManager<InputViewModel>(new InputViewModel(null), 100, new object[] { null });
            rmgOutput = new ResourceManager<OutputViewModel>(new OutputViewModel(null), 25, new object[] { null });
            rmgOutRec = new ResourceManager<OutputRectViewModel>(new OutputRectViewModel(null), 25, new object[] { null });
            rmgSpecial = new ResourceManager<SpecialViewModel>(new SpecialViewModel(null), 25, new object[] { null });
            rmgHLine = new ResourceManager<HLineViewModel>(new HLineViewModel(null), 125, new object[] { null });
            rmgVLine = new ResourceManager<VLineViewModel>(new VLineViewModel(null), 75, new object[] { null });
            rmgIRow = new ResourceManager<InstructionRowViewModel>(new InstructionRowViewModel(null, 0), 25, new object[] { null, 0 });
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
    }
}
