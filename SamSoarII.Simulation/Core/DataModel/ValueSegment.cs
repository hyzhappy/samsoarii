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
    
}
