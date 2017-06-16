using SamSoarII.AppMain.Project;
using SamSoarII.AppMain.UI;
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
        private LadderNetworkViewModel lnvmodel;
        private BaseViewModel bvmodel;
        private IList<string> pstring_old;
        private IList<string> pstring_new;

        public ElementReplaceArgumentCommand(
            LadderNetworkViewModel _lnvmodel, BaseViewModel _bvmodel, 
            IList<string> _pstring_old, IList<string> _pstring_new)
        {
            lnvmodel = _lnvmodel;
            bvmodel = _bvmodel;
            pstring_old = _pstring_old;
            pstring_new = _pstring_new;
        }

        public void Execute()
        {
            if (bvmodel.InstructionName.Equals("CALLM"))
            {
                if (pstring_new.Count() == 0)
                {
                    throw new ValueParseException(
                         String.Format(Properties.Resources.Message_Invalid_Function_Name));
                }
                int argcount = (pstring_new.Count() - 2) / 4;
                ArgumentValue[] _values = new ArgumentValue[argcount];
                for (int i = 0; i < argcount; i++)
                {
                    _values[i] = ArgumentValue.Create(
                        pstring_new[i * 4 + 3], pstring_new[i * 4 + 2], pstring_new[i * 4 + 4],
                        PLCDevice.PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
                    _values[i].Comment = pstring_new[i * 4 + 5];
                }
                ((CALLMViewModel)(bvmodel)).AcceptNewValues(
                    pstring_new[0], pstring_new[1], _values);
            }
            else
            {
                bvmodel.AcceptNewValues(pstring_new, PLCDevice.PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
            }
            //lnvmodel.INVModel.Setup(lnvmodel);
            // 导航到修改参数的元件
            lnvmodel.AcquireSelectRect();
            LadderDiagramViewModel ldvmodel = lnvmodel.LDVModel;
            ldvmodel.SelectionRect.X = bvmodel.X;
            ldvmodel.SelectionRect.Y = bvmodel.Y;
            ldvmodel.ProjectModel.IFacade.NavigateToNetwork(
                new NavigateToNetworkEventArgs(
                    lnvmodel.NetworkNumber,
                    ldvmodel.ProgramName,
                    ldvmodel.SelectionRect.X,
                    ldvmodel.SelectionRect.Y));
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
                        pstring_old[i * 4 + 3], pstring_old[i * 4 + 2], pstring_old[i * 4 + 4],
                        PLCDevice.PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
                    _values[i].Comment = pstring_old[i * 4 + 5];
                }
                ((CALLMViewModel)(bvmodel)).AcceptNewValues(
                    pstring_old[0], pstring_old[1], _values);
            }
            else
            {
                bvmodel.AcceptNewValues(pstring_old, PLCDevice.PLCDeviceManager.GetPLCDeviceManager().SelectDevice);
            }
            //lnvmodel.INVModel.Setup(lnvmodel);
            // 导航到修改参数的元件
            lnvmodel.AcquireSelectRect();
            LadderDiagramViewModel ldvmodel = lnvmodel.LDVModel;
            ldvmodel.SelectionRect.X = bvmodel.X;
            ldvmodel.SelectionRect.Y = bvmodel.Y;
            ldvmodel.ProjectModel.IFacade.NavigateToNetwork(
                new NavigateToNetworkEventArgs(
                    lnvmodel.NetworkNumber,
                    ldvmodel.ProgramName,
                    ldvmodel.SelectionRect.X,
                    ldvmodel.SelectionRect.Y));
        }
    }
}
