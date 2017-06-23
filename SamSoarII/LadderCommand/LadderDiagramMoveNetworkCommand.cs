using SamSoarII.AppMain.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.LadderCommand
{
    class LadderDiagramMoveNetworkCommand : IUndoableCommand
    {
        private LadderDiagramViewModel ldvmodel;
        private LadderNetworkViewModel lnvmodel;
        private int number_old;
        private int number_new;
        private NetworkChangeElementArea area_old;
        private NetworkChangeElementArea area_new;

        public LadderDiagramMoveNetworkCommand(
            LadderDiagramViewModel _ldvmodel, LadderNetworkViewModel _lnvmodel, int _number)
        {
            ldvmodel = _ldvmodel;
            lnvmodel = _lnvmodel;
            number_old = lnvmodel.NetworkNumber;
            number_new = _number;

            area_old = new NetworkChangeElementArea();
            area_old.SU_Select = SelectStatus.MultiSelected;
            area_old.SU_Cross = CrossNetworkState.NoCross;
            area_old.NetworkNumberStart = number_old;
            area_old.NetworkNumberEnd = number_old;
            area_old.X1 = 0;
            area_old.Y1 = 0;
            area_old.X2 = GlobalSetting.LadderXCapacity - 1;
            area_old.Y2 = lnvmodel.RowCount - 1;
            area_new = area_old.Clone();
            area_new.NetworkNumberStart = number_new - (number_new > number_old ? 1 : 0);
            area_new.NetworkNumberEnd = area_new.NetworkNumberStart;
        }

        public void Execute()
        {
            ldvmodel.RemoveNetwork(lnvmodel);
            ldvmodel.AddNetwork(lnvmodel, number_new - (number_new > number_old ? 1 : 0));
            ldvmodel.IDVModel.Setup(ldvmodel);
            ldvmodel.UpdateModelMessageByNetwork();
            if (area_new != null) area_new.Select(lnvmodel);
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            ldvmodel.RemoveNetwork(lnvmodel);
            ldvmodel.AddNetwork(lnvmodel, number_old);
            ldvmodel.IDVModel.Setup(ldvmodel);
            ldvmodel.UpdateModelMessageByNetwork();
            if (area_old != null) area_old.Select(lnvmodel);
        }
    }
}
