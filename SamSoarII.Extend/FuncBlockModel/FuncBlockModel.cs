using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamSoarII.Extend.FuncBlockModel
{
    public class FuncBlockModel
    {
        protected FuncBlock root;
        public FuncBlock Root
        {
            get { return this.root; }
            set { this.root = value; }
        }

        protected LinkedListNode<FuncBlock> current;
        public FuncBlock Current
        {
            get { return this.current?.Value; }
        }
        public LinkedListNode<FuncBlock> CurrentNode
        {
            get { return this.current; }
            set { this.current = value; }
        }

        protected int index;
        public int CurrentIndex
        {
            get { return this.index; }
            protected set { this.index = value; }
        }

        public Dictionary<string, SortedList<int, FuncBlock_Assignment>> Assigns { get; }
            = new Dictionary<string, SortedList<int, FuncBlock_Assignment>>();
       
        public FuncBlockModel(string text)
        {
            Root = new FuncBlock_Root(this, text);
            this.current = new LinkedListNode<FuncBlock>(Root);
            //Root.Build(text);   
            FuncBlock_Assignment assign = null;
            assign = new FuncBlock_Assignment(this, Root, "uint_32* XBit;");
            assign = new FuncBlock_Assignment(this, Root, "uint_32* YBit;");
            assign = new FuncBlock_Assignment(this, Root, "uint_32* MBit;");
            assign = new FuncBlock_Assignment(this, Root, "uint_32* CBit;");
            assign = new FuncBlock_Assignment(this, Root, "uint_32* TBit;");
            assign = new FuncBlock_Assignment(this, Root, "uint_32* SBit;");
            assign = new FuncBlock_Assignment(this, Root, "uint_16* DWord;");
            assign = new FuncBlock_Assignment(this, Root, "uint_16* CVWord;");
            assign = new FuncBlock_Assignment(this, Root, "uint_32* CVDoubleWord;");
            assign = new FuncBlock_Assignment(this, Root, "uint_16* TVWord;");
        }

        public List<string> GetCodeCompleteNames(string profix)
        {
            List<string> ret = new List<string>();
            if (Current != null && Current is FuncBlock_Comment)
            {
                return ret;
            }
            FuncBlock _current = Current;
            string _profix;
            while (_current != null)
            {
                _profix = String.Format("{0:s}::{1:s}", _current.Namespace, profix);
                if (Assigns.ContainsKey(_profix))
                {
                    SortedList<int, FuncBlock_Assignment> subassigns = Assigns[_profix];
                    foreach (FuncBlock_Assignment fbassign in subassigns.Values)
                    {
                        if (fbassign.IndexEnd < CurrentIndex)
                        {
                            ret.Add(fbassign.Name);
                        }
                    }
                }
                if (_current is FuncBlock_Assignment || _current is FuncBlock_Statement)
                {
                    if (_current.Parent is FuncBlock_Root)
                    {
                        break;
                    }
                    _current = _current.Parent.Parent;
                }
                else
                {
                    _current = _current.Parent;
                }
            }
            ret.Sort();
            return ret;
        }
        
        public void Move(int index)
        {
            CurrentIndex = index;
            while (Current.Parent != null && !Current.Contains(index))
            {
                while (index > Current.IndexEnd)
                {
                    if (current.Next != null && current.Next.Value.IndexStart <= index)
                    {
                        current = current.Next;
                    }
                    else
                    {
                        if (Current.Parent == null)
                        {
                            break;
                        }
                        Current.Parent.Current = current;
                        if (Current.Parent.Parent == null)
                        {
                            current = new LinkedListNode<FuncBlock>(Root);
                        }
                        else
                        {
                            current = Current.Parent.Parent.Current;
                        }
                    }
                }
                while (index < Current.IndexStart)
                {
                    if (current.Previous != null && current.Previous.Value.IndexEnd >= index)
                    {
                        current = current.Previous;
                    }
                    else
                    {
                        if (Current.Parent == null)
                        {
                            break;
                        }
                        Current.Parent.Current = current;
                        if (Current.Parent.Parent == null)
                        {
                            current = new LinkedListNode<FuncBlock>(Root);
                        }
                        else
                        {
                            current = Current.Parent.Parent.Current;
                        }
                    }
                }
            }
            if (Current.Parent == null && !Current.Contains(index))
            {
                return;
            }
            while (Current.Current != null)
            {
                Current.CastOffset();
                while (index < Current.Current.Value.IndexStart)
                {
                    if (Current.Current.Previous == null)
                    {
                        break;
                    }
                    else
                    {
                        Current.Current = Current.Current.Previous;
                    }
                }
                while (index > Current.Current.Value.IndexEnd)
                {
                    if (Current.Current.Next == null)
                    {
                        break;
                    }
                    else
                    {
                        Current.Current = Current.Current.Next;
                    }
                }
                if (Current.Current.Value.Contains(index))
                {
                    current = Current.Current;
                }
                else if (Current.Current.Next != null &&
                    Current.Current.Next.Value.Contains(index))
                {
                    Current.Current = Current.Current.Next;
                    current = Current.Current.Next;
                }
                else
                {
                    break;
                }
            }
        }

        #region Breakpoint System

        private int bpaddrmin;
        private int bpaddrmax;

        public void InitBP()
        {
            InitBP(Root);
        }

        private void InitBP(FuncBlock fblock)
        {
            fblock.IsBreakpoint = false;
            if (fblock is FuncBlock_ForHeader)
            {
                FuncBlock_ForHeader fblockfh = (FuncBlock_ForHeader)fblock;
                InitBP(fblockfh.Start);
                InitBP(fblockfh.Cond);
                InitBP(fblockfh.Next);
            }
            if (fblock is FuncBlock_WhileHeader)
            {
                FuncBlock_WhileHeader fblockwh = (FuncBlock_WhileHeader)fblock;
                InitBP(fblockwh.Cond);
            }
            if (fblock is FuncBlock_IfHeader)
            {
                FuncBlock_IfHeader fblockih = (FuncBlock_IfHeader)fblock;
                InitBP(fblockih.Cond);
            }
            if (fblock is FuncBlock_WhileEnd)
            {
                FuncBlock_WhileEnd fblockwe = (FuncBlock_WhileEnd)fblock;
                InitBP(fblockwe.Cond);
            }
            foreach (FuncBlock child in fblock.Childrens)
            {
                InitBP(child);
            }
        }

        public void GetBPAddrRange()
        {
            bpaddrmin = 0x3fffffff;
            bpaddrmax = -0x3fffffff;
            GetBPAddrRange(Root);
        }

        private void GetBPAddrRange(FuncBlock fblock)
        {
            if (fblock.IsBreakpoint)
            {
                bpaddrmin = Math.Max(bpaddrmin, fblock.BPAddress);
                bpaddrmax = Math.Max(bpaddrmax, fblock.BPAddress);
            }
            if (fblock is FuncBlock_ForHeader)
            {
                FuncBlock_ForHeader fblockfh = (FuncBlock_ForHeader)fblock;
                GetBPAddrRange(fblockfh.Start);
                GetBPAddrRange(fblockfh.Cond);
                GetBPAddrRange(fblockfh.Next);
            }
            if (fblock is FuncBlock_WhileHeader)
            {
                FuncBlock_WhileHeader fblockwh = (FuncBlock_WhileHeader)fblock;
                GetBPAddrRange(fblockwh.Cond);
            }
            if (fblock is FuncBlock_IfHeader)
            {
                FuncBlock_IfHeader fblockih = (FuncBlock_IfHeader)fblock;
                GetBPAddrRange(fblockih.Cond);
            }
            if (fblock is FuncBlock_WhileEnd)
            {
                FuncBlock_WhileEnd fblockwe = (FuncBlock_WhileEnd)fblock;
                GetBPAddrRange(fblockwe.Cond);
            }
            foreach (FuncBlock child in fblock.Childrens)
            {
                GetBPAddrRange(child);
            }    
        }

        public bool ContainBP(int bpaddr)
        {
            return bpaddr >= bpaddrmin && bpaddr <= bpaddrmax;
        }
        #endregion
    }
}
