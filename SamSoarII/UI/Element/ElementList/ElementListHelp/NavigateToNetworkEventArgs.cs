using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.UI
{
    public delegate void NavigateToNetworkEventHandler(NavigateToNetworkEventArgs e);
    public class NavigateToNetworkEventArgs : EventArgs
    {
        private int _x;
        private int _y;
        private string _refLadderName;
        private int _networknum;
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }
        public string RefLadderName
        {
            get
            {
                return _refLadderName;
            }
            set
            {
                _refLadderName = value;
            }
        }
        public int NetworkNum
        {
            get
            {
                return _networknum;
            }
            set
            {
                _networknum = value;
            }
        }
        public NavigateToNetworkEventArgs(int networkNum,string refLadderName,int x,int y)
        {
            NetworkNum = networkNum;
            RefLadderName = refLadderName;
            X = x;
            Y = y;
        }
    }
}
