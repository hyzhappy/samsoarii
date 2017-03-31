using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Simulation.Core.DataModel
{
    abstract public class ValueSegment
    {
        protected int tstart;
        protected int tend;
        
        public int TimeStart
        {
            get
            {
                return this.tstart;
            }
            set
            {
                this.tstart = value;
            }
        }

        public int TimeEnd
        {
            get
            {
                return this.tend;
            }
            set
            {
                this.tend = value;
            }
        }

        abstract public object Value
        {
            get; set;
        }

        abstract public ValueSegment Clone();

        protected ValueSegment _Clone(ValueSegment vseg)
        {
            vseg.TimeStart = TimeStart;
            vseg.TimeEnd = TimeEnd;
            return vseg;
        }

        public override string ToString()
        {
            return String.Format("{0}[{1}..{2}]", Value, TimeStart, TimeEnd);
        }

    }

    public class IntSegment : ValueSegment
    {
        protected int value;
        override public object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = (int)(value);
            }
        }
        public override ValueSegment Clone()
        {
            IntSegment iseg = new IntSegment();
            iseg.Value = Value;
            return _Clone(iseg);
        }
    }

    public class FloatSegment : ValueSegment
    {
        protected float value;
        override public object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = (float)(value);
            }
        }
        public override ValueSegment Clone()
        {
            FloatSegment fseg = new FloatSegment();
            fseg.Value = Value;
            return _Clone(fseg);
        }
    }

    public class DoubleSegment : ValueSegment
    {
        protected double value;
        override public object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = (double)(value);
            }
        }

        public override ValueSegment Clone()
        {
            DoubleSegment dseg = new DoubleSegment();
            dseg.Value = Value;
            return _Clone(dseg);
        }
    }

    public class BitSegment : IntSegment
    {

    }

    public class WordSegment : IntSegment
    {

    }

    public class DWordSegment : IntSegment
    {

    }

    public class ValueSegmentTimeComparer : IComparer<ValueSegment>
    {
        public int Compare(ValueSegment x, ValueSegment y)
        {
            return x.TimeStart.CompareTo(y.TimeStart);
        }
    }

}
