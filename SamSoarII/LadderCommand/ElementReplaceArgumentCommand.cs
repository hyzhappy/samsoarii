using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.LadderCommand
{
    public class ElementReplaceArgumentCommand : IUndoableCommand
    {
        private BaseViewModel bvmodel;
        private IList<string> pstring_old;
        private IList<string> pstring_new;

        public ElementReplaceArgumentCommand(BaseViewModel _bvmodel, IList<string> _pstring_old, IList<string> _pstring_new)
        {
            bvmodel = _bvmodel;
            pstring_old = _pstring_old;
            pstring_new = _pstring_new;
        }

        public void Execute()
        {
            bvmodel.AcceptNewValues(pstring_new, SamSoarII.PLCDevice.PLCDeviceManager.SelectDevice);
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            bvmodel.AcceptNewValues(pstring_old, SamSoarII.PLCDevice.PLCDeviceManager.SelectDevice);
        }
    }
}
