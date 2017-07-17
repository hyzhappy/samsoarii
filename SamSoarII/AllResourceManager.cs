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

        static public void Initialize()
        {
            rmgInput = new ResourceManager<InputViewModel>(new InputViewModel(new LadderUnitModel(null, LadderUnitModel.Types.LD)));
            rmgOutput = new ResourceManager<OutputViewModel>(new OutputViewModel(new LadderUnitModel(null, LadderUnitModel.Types.OUT)));
            rmgOutRec = new ResourceManager<OutputRectViewModel>(new OutputRectViewModel(new LadderUnitModel(null, LadderUnitModel.Types.ADD)));
            rmgSpecial = new ResourceManager<SpecialViewModel>(new SpecialViewModel(new LadderUnitModel(null, LadderUnitModel.Types.INV)));
            rmgHLine = new ResourceManager<HLineViewModel>(new HLineViewModel(new LadderUnitModel(null, LadderUnitModel.Types.HLINE)));
            rmgVLine = new ResourceManager<VLineViewModel>(new VLineViewModel(new LadderUnitModel(null, LadderUnitModel.Types.VLINE)));
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
