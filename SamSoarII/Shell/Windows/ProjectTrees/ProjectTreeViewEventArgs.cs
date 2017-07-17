using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Shell.Windows
{
    public class ProjectTreeViewEventArgs : EventArgs, IWindowEventArgs
    {
        public const int TYPE_ROOT = 0x0;
        public const int TYPE_ROUTINEFLODER = 0x1;
        public const int TYPE_NETWORKFLODER = 0x2;
        public const int TYPE_FUNCBLOCKFLODER = 0x3;
        public const int TYPE_MODBUSFLODER = 0x4;
        public const int TYPE_ROUTINE = 0x5;
        public const int TYPE_NETWORK = 0x6;
        public const int TYPE_FUNCBLOCK = 0x7;
        public const int TYPE_FUNC = 0x8;
        public const int TYPE_MODBUS = 0x9;
        public const int TYPE_PROGRAM = 0xa;
        public const int TYPE_LADDERS = 0xb;
        public const int TYPE_INSTRUCTION = 0xc;
        public const int TYPE_CONST = 0xd;
        public const int TYPE_ELEMENTLIST = 0xe;
        public const int TYPE_ELEMENTINITIALIZE = 0xf;

        public const int FLAG_DOUBLECLICK = 0x10;
        public const int FLAG_CONFIG = 0x20;
        
        public int Flags { get; private set; }
        public object RelativeObject { get; set; }
        public object TargetedObject { get; set; }

        public ProjectTreeViewEventArgs
        (
            int _flags,
            object _relativeObject,
            object _targetedObject
        )
        {
            Flags = _flags;
            RelativeObject = _relativeObject;
            TargetedObject = _targetedObject;
        }
    }
    
}
