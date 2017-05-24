using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace SamSoarII.LadderInstViewModel
{
    public class ColorManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        static private ColorData LadderScreen
            = new ColorData("梯形图背景");
        static private ColorData Ladder
            = new ColorData("元件");
        static private ColorData Comment
            = new ColorData("元件注释");
        static private ColorData DiagramTitle
            = new ColorData("梯形图标题");
        static private ColorData NetworkTitle
            = new ColorData("网络标题");
        static private ColorData FuncScreen
            = new ColorData("函数块背景");
        static private ColorData InstScreen
            = new ColorData("PLC指令背景");
        static private ColorData Inst
            = new ColorData("PLC指令单元格");

        static public ColorData GetLadderScreen()
        {
            return LadderScreen;
        }
        static public ColorData GetLadder()
        {
            return Ladder;
        }
        static public ColorData GetComment()
        {
            return Comment;
        }
        static public ColorData GetDiagramTitle()
        {
            return DiagramTitle;
        }
        static public ColorData GetNetworkTitle()
        {
            return NetworkTitle;
        }
        static public ColorData GetFuncScreen()
        {
            return FuncScreen;
        }
        static public ColorData GetInstScreen()
        {
            return InstScreen;
        }
        static public ColorData GetInst()
        {
            return Inst;
        }

        static public Brush Parse(string text)
        {
            string[] args = text.Split(' ');
            Color color = new Color();
            color.A = byte.Parse(args[0]);
            color.R = byte.Parse(args[1]);
            color.G = byte.Parse(args[2]);
            color.B = byte.Parse(args[3]);
            return new SolidColorBrush(color);
        }

        static public string ToString(Brush brush)
        {
            if (brush is SolidColorBrush)
            {
                SolidColorBrush scbrush = (SolidColorBrush)brush;
                Color color = scbrush.Color;
                return String.Format("{0:d} {1:d} {2:d} {3:d}",
                    color.A, color.R, color.G, color.B);
            }
            return String.Empty;
        }

        static public void LoadColorDataByXElement(ColorData cdat, XElement xele)
        {
            cdat.Background = Parse(xele.Attribute("Background").Value);
            cdat.Foreground = Parse(xele.Attribute("Foreground").Value);
        }

        static public void SaveColorDataToXElement(ColorData cdat, XElement xele)
        {
            xele.SetAttributeValue("Background", ToString(cdat.Background));
            xele.SetAttributeValue("Foreground", ToString(cdat.Foreground));
        }

    }
    
    public class DemoColorManager
    {
        static private ColorData LadderScreen
            = new ColorData("梯形图背景");
        static private ColorData Ladder
            = new ColorData("元件");
        static private ColorData Comment
            = new ColorData("元件注释");
        static private ColorData DiagramTitle
            = new ColorData("梯形图标题");
        static private ColorData NetworkTitle
            = new ColorData("网络标题");
        static private ColorData FuncScreen
            = new ColorData("函数块背景");
        static private ColorData InstScreen
            = new ColorData("PLC指令背景");
        static private ColorData Inst
            = new ColorData("PLC指令单元格");

        static public ColorData GetLadderScreen()
        {
            return LadderScreen;
        }
        static public ColorData GetLadder()
        {
            return Ladder;
        }
        static public ColorData GetComment()
        {
            return Comment;
        }
        static public ColorData GetDiagramTitle()
        {
            return DiagramTitle;
        }
        static public ColorData GetNetworkTitle()
        {
            return NetworkTitle;
        }
        static public ColorData GetFuncScreen()
        {
            return FuncScreen;
        }
        static public ColorData GetInstScreen()
        {
            return InstScreen;
        }
        static public ColorData GetInst()
        {
            return Inst;
        }
    }

    public class ColorData : INotifyPropertyChanged
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

        private Brush background;
        public Brush Background
        {
            get
            {
                return this.background;
            }
            set
            {
                this.background = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Background"));
            }
        }

        private Brush foreground;
        public Brush Foreground
        {
            get
            {
                return this.foreground;
            }
            set
            {
                this.foreground = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Stroke"));
            }
        }
        
        public ColorData(string _name)
        {
            Name = _name;
        }
        
        public void Setup(ColorData that)
        {
            this.Name = that.Name;
            this.Foreground = that.Foreground;
            this.Background = that.Background;
        }
    }
}
