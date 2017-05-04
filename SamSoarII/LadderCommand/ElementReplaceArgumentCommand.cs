using SamSoarII.LadderInstViewModel;
using SamSoarII.ValueModel;
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
            if (bvmodel.InstructionName.Equals("CALLM"))
            {
                int argcount = (pstring_new.Count() - 2) / 4;
                ArgumentValue[] _values = new ArgumentValue[argcount];
                for (int i = 0; i < argcount; i++)
                {
                    _values[i] = ArgumentValue.Create(
                        pstring_new[i * 4 + 3], pstring_new[i * 4 + 2], pstring_new[i * 4 + 4],
                        PLCDevice.PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
                    _values[i].Comment = pstring_new[i * 4 + 5];
                }
                ((CALLMViewModel)(bvmodel)).AcceptNewValues(pstring_new[0], _values);
            }
            else
            {
                bvmodel.AcceptNewValues(pstring_new, PLCDevice.PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
            }
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            if (bvmodel.InstructionName.Equals("CALLM"))
            {
                int argcount = (pstring_old.Count() - 2) / 4;
                ArgumentValue[] _values = new ArgumentValue[argcount];
                for (int i = 0; i < argcount; i++)
                {
                    _values[i] = ArgumentValue.Create(
                        pstring_old[i * 4 + 2], pstring_old[i * 4 + 3], pstring_old[i * 4 + 4],
                        PLCDevice.PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
                    _values[i].Comment = pstring_old[i * 4 + 5];
                }
                   ((CALLMViewModel)(bvmodel)).AcceptNewValues(pstring_old[0], _values);
            }
            else
            {
                bvmodel.AcceptNewValues(pstring_old, PLCDevice.PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
            }
        }
    }
}
