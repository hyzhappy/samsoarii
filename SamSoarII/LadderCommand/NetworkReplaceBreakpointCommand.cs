using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.LadderCommand
{
    public class NetworkReplaceBreakpointCommand : IUndoableCommand
    {
        private LadderNetworkViewModel _network;
        private HashSet<BreakpointRect> _oldbreakpoints;
        private HashSet<BreakpointRect> _newbreakpoints;
        private NetworkChangeElementArea _oldarea;
        private NetworkChangeElementArea _newarea;

        public NetworkReplaceBreakpointCommand(
            LadderNetworkViewModel network,
            IEnumerable<BreakpointRect> oldbreakpoints,
            IEnumerable<BreakpointRect> newbreakpoints,
            NetworkChangeElementArea oldarea = null,
            NetworkChangeElementArea newarea = null)
        {
            _network = network;
            _oldbreakpoints = new HashSet<BreakpointRect>(oldbreakpoints);
            _newbreakpoints = new HashSet<BreakpointRect>(newbreakpoints);
            _oldarea = oldarea;
            _newarea = newarea;
        }

        public void Execute()
        {
            foreach (var bp in _oldbreakpoints)
            {
                _network.RemoveBreakpoint(bp);
            }
            foreach (var bp in _newbreakpoints)
            {
                _network.ReplaceBreakpoint(bp);
            }
            if (_newarea != null)
            {
                _newarea.Select(_network);
            }
            
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            foreach (var bp in _newbreakpoints)
            {
                _network.RemoveBreakpoint(bp);
            }
            foreach (var bp in _oldbreakpoints)
            {
                _network.ReplaceBreakpoint(bp);
            }
            if (_oldarea != null)
            {
                _oldarea.Select(_network);
            }
        }
    }
    
}
