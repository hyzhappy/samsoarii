using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Utility.DXF
{
    public class DXFSection : DXFEntity
    {
        public DXFSection(string name,DXFModel parent) : base(parent)
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
                        if (Parent.Reader.CurrentCode != 0)
                            Parent.Reader.MoveNext();
                        if (Parent.Reader.CurrentValue == "ENDSEC")
                            break;
                        switch (Parent.Reader.CurrentCode)
                        {
                            case 0:
                                switch (Parent.Reader.CurrentValue)
                                {
                                    case "LINE":
                                        Entities.Add(new DXFLine(Parent.Reader.CurrentValue, Parent));
                                        break;
                                    case "ARC":
                                        Entities.Add(new DXFArc(Parent.Reader.CurrentValue, Parent));
                                        break;
                                    case "ELLIPSE":
                                        Entities.Add(new DXFEllipse(Parent.Reader.CurrentValue, Parent));
                                        break;
                                    case "CIRCLE":
                                        Entities.Add(new DXFCircle(Parent.Reader.CurrentValue, Parent));
                                        break;
                                    default:
                                        Parent.Reader.MoveNext();
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
                Parent.Reader.MoveNext();
                if (Parent.Reader.CurrentValue == "ENDSEC") break;
            }
        }
    }
}
