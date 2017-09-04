using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Utility.DXF
{
    public class DXFSection : DXFEntity
    {
        public DXFSection(string name, DXFReader reader,DXFModel parent) : base(reader, parent)
        {
            Type = EntityType.Section;
            Name = name;
            Entities = new List<DXFEntity>();
            ReadEntities();
        }
        public override void ReadEntities()
        {
            switch (Name)
            {
                case "BLOCKS":
                    ReadBlocks();
                    break;
                case "ENTITIES":
                    while (true)
                    {
                        //这里为0时表示下一个图元,因此不移动
                        if (Reader.CurrentCode != 0)
                            Reader.MoveNext();
                        if (Reader.CurrentValue == "ENDSEC")
                            break;
                        switch (Reader.CurrentCode)
                        {
                            case 0:
                                switch (Reader.CurrentValue)
                                {
                                    case "LINE":
                                        Entities.Add(new DXFLine(Reader.CurrentValue, Reader, Parent));
                                        break;
                                    case "ARC":
                                        Entities.Add(new DXFArc(Reader.CurrentValue, Reader, Parent));
                                        break;
                                    case "ELLIPSE":
                                        Entities.Add(new DXFEllipse(Reader.CurrentValue, Reader, Parent));
                                        break;
                                    case "CIRCLE":
                                        Entities.Add(new DXFCircle(Reader.CurrentValue, Reader, Parent));
                                        break;
                                    default:
                                        Reader.MoveNext();
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
            }
        }
        public void ReadBlocks()
        {
            while (true)
            {
                Reader.MoveNext();
                if (Reader.CurrentValue == "ENDSEC") break;
            }
        }
    }
}
