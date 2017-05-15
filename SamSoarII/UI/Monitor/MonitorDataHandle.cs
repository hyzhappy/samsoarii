using SamSoarII.Communication.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.UI.Monitor
{
    public class MonitorDataHandle
    {
        private MainMonitor _mainMonitor;
        public Dictionary<int, List<ICommunicationCommand>> ReadCommands { get; set; }
        public Queue<ICommunicationCommand> WriteCommands { get; set; }
        public MonitorDataHandle(MainMonitor mainMonitor)
        {
            _mainMonitor = mainMonitor;
            _mainMonitor.dataHandle = this;
            ReadCommands = new Dictionary<int, List<ICommunicationCommand>>();
            WriteCommands = new Queue<ICommunicationCommand>();
        }
        public void ReadRun()
        {
            
        }
        public void WriteRun()
        {
            
        }
        private void GenerateReadCommands()
        {
            if (_mainMonitor.IsModify)
            {
                foreach (var table in _mainMonitor.tables)
                {
                    if (table.IsModify)
                    {
                        List<ICommunicationCommand> commands = table.GenerateReadCommands();
                        if (ReadCommands.ContainsKey(table.HashCode))
                        {
                            ReadCommands[table.HashCode] = commands;
                        }
                        else
                        {
                            ReadCommands.Add(table.HashCode,commands);
                        }
                    }
                }
                var templist = new List<int>(ReadCommands.Keys);
                foreach (var key in templist)
                {
                    if (!_mainMonitor.tables.ToList().Exists(x => { return x.HashCode == key; }))
                    {
                        ReadCommands.Remove(key);
                    }
                }
                _mainMonitor.IsModify = false;
            }
        }
    }
}
