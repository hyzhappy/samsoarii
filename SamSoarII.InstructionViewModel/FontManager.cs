using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace SamSoarII.LadderInstViewModel
{
    public class FontManager
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        static private FontData Title 
            = new FontData("梯形图标题");
        static private FontData Ladder 
            = new FontData("元件");
        static private FontData Comment 
            = new FontData("注释");
        static private FontData Func 
            = new FontData("函数块");
        static private FontData Inst
            = new FontData("PLC指令");

        static public FontData GetTitle()
        {
            return Title;
        }
        static public FontData GetLadder()
        {
            return Ladder;
        }
        static public FontData GetComment()
        {
            return Comment;
        }
        static public FontData GetFunc()
        {
            return Func;
        }
        static public FontData GetInst()
        {
            return Inst;
        }

        static public void LoadFontDataByXElement(FontData fdat, XElement xele)
        {
            fdat.FontSize = int.Parse(xele.Attribute("FontSize").Value);
            fdat.FontFamily = new FontFamily(xele.Attribute("FontFamily").Value);
            fdat.FontColor = ColorManager.Parse(xele.Attribute("FontColor").Value);
        }
        static public void SaveFontDataToXElement(FontData fdat, XElement xele)
        {
            xele.SetAttributeValue("FontSize", fdat.FontSize);
            xele.SetAttributeValue("FontFamily", fdat.FontFamily);
            xele.SetAttributeValue("FontColor", ColorManager.ToString(fdat.FontColor));
        }
    }

    public class FontData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        private int fontsize;
        public int FontSize
        {
            get
            {
                return this.fontsize;
            }
            set
            {
                this.fontsize = value;
                PropertyChanged(this, new PropertyChangedEventArgs("FontSize"));
            }
        }

        private FontFamily fontfamily;
        public FontFamily FontFamily
        {
            get
            {
                return this.fontfamily;
            }
            set
            {
                this.fontfamily = value;
                PropertyChanged(this, new PropertyChangedEventArgs("FontFamily"));
            }
        }

        private Brush fontcolor;
        public Brush FontColor
        {
            get
            {
                return this.fontcolor;
            }
            set
            {
                this.fontcolor = value;
                PropertyChanged(this, new PropertyChangedEventArgs("FontColor"));
            }
        }

        public FontData(string _name)
        {
            Name = _name;
        }
        
        public void Setup(FontData that)
        {
            this.Name = that.Name;
            this.FontSize = that.FontSize;
            this.FontFamily = that.FontFamily;
            this.FontColor = that.FontColor;
        }
    }
}
