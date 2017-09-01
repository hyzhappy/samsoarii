using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace SamSoarII.Utility.DXF
{
    public class DXFEntity
    {
        public DXFEntity(DXFReader reader)
        {
            Reader = reader;
        }
        public string Name { get; set; }
        public EntityType Type { get; set; }
        public List<DXFEntity> Entities { get; set; }

        protected DXFReader Reader;
        public virtual void ReadEntities() { }
        public virtual void ReadProperties() { }
        public virtual void Render(DrawingContext context) { }
        
    }
}
