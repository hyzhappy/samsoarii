using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Models
{
    public class LadderUnitFormat
    {
        public LadderUnitFormat(int _catalogid, string _name, 
            LadderUnitModel.Types _type, LadderUnitModel.Outlines _outline, LadderUnitModel.Shapes _shape,
            string _describe, string _detail_ch, string _detail_en, ValueFormat[] _formats)
        {
            catalogid = _catalogid;
            name = _name;
            type = _type;
            outline = _outline;
            shape = _shape;
            describe = _describe;
            detail_ch = _detail_ch;
            detail_en = _detail_en;
            formats = _formats;
        }

        #region Number

        private int catalogid;
        public int CatalogID { get { return this.catalogid; } }

        private string name;
        public string Name { get { return this.name; } }

        private string fullname_ch;
        private string fullname_en;
        public string Fullname { get { return App.CultureIsZH_CH() ? fullname_ch : fullname_en; } }

        private LadderUnitModel.Types type;
        public LadderUnitModel.Types Type { get { return this.type; } }
        
        private LadderUnitModel.Outlines outline;
        public LadderUnitModel.Outlines Outline { get { return this.outline; } }

        private LadderUnitModel.Shapes shape;
        public LadderUnitModel.Shapes Shape { get { return this.shape; } }
        
        private string describe;
        public string Describe { get { return this.describe; } }

        private string detail_ch;
        private string detail_en;
        public string Detail { get { return App.CultureIsZH_CH() ? detail_ch : detail_en; } }

        private ValueFormat[] formats;
        public IList<ValueFormat> Formats { get { return this.formats; } }

        #endregion
    }
}
