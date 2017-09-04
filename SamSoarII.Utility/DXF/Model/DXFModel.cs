using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SamSoarII.Utility.DXF
{
    public enum EntityType
    {
        Line,
        Arc,
        Ellipse,
        Circle,
        Section
    }
    public class DXFModel
    {
        public DXFGraph Graph { get; set; }
        private DXFReader converter;
        private List<DXFEntity> sections;
        public List<DXFEntity> Sections { get { return sections; } }
        public DXFModel()
        {
            sections = new List<DXFEntity>();
            Graph = new DXFGraph();
        }

        public void Convert(string filename)
        {
            try
            {
                converter = new DXFReader(filename);
                do
                {
                    converter.MoveNext();
                    if (converter.CurrentValue == "EOF") break;
                    //这里只记录BLOCKS和ENTITIES,其他块不影响图元的读取
                    switch (converter.CurrentValue)
                    {
                        case "BLOCKS":
                            sections.Add(new DXFSection("BLOCKS", converter, this));
                            break;
                        case "ENTITIES":
                            sections.Add(new DXFSection("ENTITIES", converter, this));
                            break;
                    }
                } while (true);
                converter.Close();
            }
            catch (Exception)
            {
                converter.Close();
            }
        }
    }
}
