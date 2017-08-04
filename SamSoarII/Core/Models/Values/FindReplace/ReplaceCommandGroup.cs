using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Models
{
    /// <summary> 替换命令的集合 </summary>
    public class ReplaceCommandGroup : IDisposable
    {
        public ReplaceCommandGroup()
        {
            commands = new List<ReplaceCommand>();
        }

        public void Dispose()
        {
            foreach (ReplaceCommand cmd in commands) cmd.Dispose();
            commands.Clear();
        }

        private List<ReplaceCommand> commands;

        public void Add(ReplaceCommand cmd)
        {
            commands.Add(cmd);
        }

        public void Undo()
        {
            foreach (ReplaceCommand cmd in commands) cmd.Undo();
        }

        public void Redo()
        {
            foreach (ReplaceCommand cmd in commands) cmd.Redo();
        }

    }
}
