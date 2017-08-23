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
        static private ResourceManager<LadderUnitViewModel> rmgUnit;
        static private ResourceManager<LadderNetworkViewModel> rmgNet;
        static private ResourceManager<InstructionNetworkViewModel> rmgINet;
        static private ResourceManager<InstructionRowViewModel> rmgIRow;
        static private ResourceManager<LadderBrpoViewModel> rmgBrpo;
        
        static public void Initialize()
        {
            rmgUnit = new ResourceManager<LadderUnitViewModel>(new LadderUnitViewModel(null), 200, new object[] { null });
            rmgNet = new ResourceManager<LadderNetworkViewModel>(new LadderNetworkViewModel(null), 20, new object[] { null });
            rmgINet = new ResourceManager<InstructionNetworkViewModel>(new InstructionNetworkViewModel(null), 20, new object[] { null });
            rmgIRow = new ResourceManager<InstructionRowViewModel>(new InstructionRowViewModel(null), 25, new object[] { null});
            rmgBrpo = new ResourceManager<LadderBrpoViewModel>(new LadderBrpoViewModel(null), 20, new object[] { null });
        }

        static public LadderUnitViewModel CreateUnit(LadderUnitModel core)
        {
            return rmgUnit.Create(core);
        }
        static public void Dispose(LadderUnitViewModel unit)
        {
            rmgUnit.Dispose(unit);
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

        static public InstructionRowViewModel CreateInstRow(PLCOriginInst inst)
        {
            return rmgIRow.Create(inst);
        }
        static public void Dispose(InstructionRowViewModel irow)
        {
            rmgIRow.Dispose(irow);
        }
        
    }
}
