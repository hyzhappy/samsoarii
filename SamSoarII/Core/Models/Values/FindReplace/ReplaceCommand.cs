using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Models
{
    /// <summary> 替换命令 </summary>
    public class ReplaceCommand : IDisposable
    {
        public ReplaceCommand(LadderUnitModel _unit, string _output)
        {
            oldunit = _unit;
            network = oldunit.Parent;
            string[] _newtexts = _output.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            LadderUnitModel.Types newtype = LadderUnitModel.TypeOfNames[_newtexts[0]];
            string[] newargs = new string[_newtexts.Length - 1];
            Array.Copy(_newtexts, 1, newargs, 0, newargs.Length);
            newunit = new LadderUnitModel(null, newtype) { X = oldunit.X, Y = oldunit.Y };
            newunit.Parse(newargs);
        }

        public void Dispose()
        {
            oldunit = null;
            newunit = null;
        }

        #region Number

        private LadderNetworkModel network;
        private LadderUnitModel oldunit;
        private LadderUnitModel newunit;

        #endregion

        #region Undo & Redo

        public void Redo()
        {
            if (network?.Parent?.View != null)
                network.Parent.View.IsViewModified = true;
            network.Remove(oldunit);
            try
            {
                network.Add(newunit);
            }
            catch (LadderUnitChangedEventException exce)
            {
                network.Add(oldunit);
                throw exce;
            }
        }

        public void Undo()
        {
            if (network?.Parent?.View != null)
                network.Parent.View.IsViewModified = true;
            network.Remove(newunit);
            try
            {
                network.Add(oldunit);
            }
            catch (LadderUnitChangedEventException exce)
            {
                network.Add(newunit);
                throw exce;
            }
        }

        #endregion
    }
}
