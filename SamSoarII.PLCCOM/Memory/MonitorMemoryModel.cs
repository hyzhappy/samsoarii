using SamSoarII.PLCCOM.USB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCCOM.Memory
{
    public class MonitorMemoryModel
    {
        #region Numbers

        private Dictionary<string, BlockMemoryModel> rbdict =
            new Dictionary<string, BlockMemoryModel>();

        private Dictionary<string, UnitMemoryModel> rudict =
            new Dictionary<string, UnitMemoryModel>();

        private ObservableCollection<BlockMemoryModel> blocks =
            new ObservableCollection<BlockMemoryModel>();

        #endregion
        
        public UnitMemoryModel Add(UnitMemoryModel unit)
        {
            if (rudict.ContainsKey(unit.Name))
            {
                return rudict[unit.Name];
            }
            UnitMemoryModel prev = unit.Prev();
            UnitMemoryModel next = unit.Next();
            BlockMemoryModel bprev = null;
            BlockMemoryModel bnext = null;
            BlockMemoryModel bmerge = null; 
            bool cprev = rbdict.ContainsKey(prev.Name);
            bool cnext = rbdict.ContainsKey(next.Name);
            if (cprev && cnext)
            {
                bprev = rbdict[prev.Name];
                bnext = rbdict[next.Name];
                bmerge = BlockMemoryModel.Merge(bprev, bnext);
                foreach (UnitMemoryModel _unit in bprev.Units)
                {
                    rbdict[_unit.Name] = bmerge;
                }
                foreach (UnitMemoryModel _unit in bnext.Units)
                {
                    rbdict[_unit.Name] = bmerge;
                }
                blocks.Remove(bprev);
                blocks.Remove(bnext);
                blocks.Add(bmerge);
            }
            else if (cprev)
            {
                bprev = rbdict[prev.Name];
                bmerge = bprev;
            }
            else if (cnext)
            {
                bnext = rbdict[next.Name];
                bmerge = bnext;
            }
            else
            {
                bmerge = BlockMemoryModel.Create(unit);
                blocks.Add(bmerge);
            }
            bmerge.Add(unit);
            rudict[unit.Name] = unit;
            rbdict[unit.Name] = bmerge;
            return unit;
        }

        public UnitMemoryModel Remove(UnitMemoryModel unit)
        {
            if (rudict.ContainsKey(unit.Name))
            {
                unit = rudict[unit.Name];
            }
            else
            {
                return unit;
            }
            BlockMemoryModel brm = rbdict[unit.Name];
            brm.Remove(unit);
            rbdict.Remove(unit.Name);
            rudict.Remove(unit.Name);
            return unit;
        }

        public void Update(USBModel usbmodel)
        {
            foreach (BlockMemoryModel block in blocks)
            {
                block.Update(usbmodel);
            }
        }

        public void Set(USBModel usbmodel, UnitMemoryModel unit)
        {
            object value = unit.Value;
            if (rudict.ContainsKey(unit.Name))
            {
                unit = rudict[unit.Name];
            }
            unit.Value = value;
            rbdict[unit.Name].Set(usbmodel, unit.Offset, unit.Offset + unit.Length);
        }
        
    }
}
