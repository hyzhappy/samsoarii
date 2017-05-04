using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SamSoarII.Simulation.Core.VariableModel;

namespace SamSoarII.Simulation.Core.DataModel
{
    public class SimulateDataModel
    {
        private SimulateVariableUnit svunit;

        private List<ValueSegment> values;

        public SimulateVariableUnit SVUnit
        {
            get { return this.svunit; }
            set { this.svunit = value; }
        }

        public IEnumerable<ValueSegment> Values
        {
            get { return this.values; }
        }
        
        public string Name
        {
            get { return this.svunit == null ? string.Empty : this.svunit.Name; }
        }

        public string Var
        {
            get { return this.svunit == null ? string.Empty : this.svunit.Var; }
        }

        public string Type
        {
            get { return this.svunit == null ? string.Empty : this.svunit.Type; }
        }

        public bool IsLock
        {
            get; set;
        }

        public bool IsView
        {
            get; set;
        }
        
        public int TimeStart
        {
            get
            {
                return values.First().TimeStart;
            }
        }

        public int TimeEnd
        {
            get
            {
                return values.Last().TimeEnd;
            }
        }
        
        public SimulateDataModel()
        {
            values = new List<ValueSegment>();
        }

        public SimulateDataModel(SimulateVariableUnit _svunit)
        {
            SVUnit = _svunit;
            values = new List<ValueSegment>();
        }

        public void Init(int starttime)
        {
            values.Clear();
            ValueSegment vs;
            switch (Type)
            {
                case "BIT": 
                    vs = new BitSegment();
                    vs.Value = 0;
                    break;
                case "WORD":
                    vs = new WordSegment();
                    vs.Value = 0;
                    break;
                case "DWORD":
                    vs = new DWordSegment();
                    vs.Value = 0;
                    break;
                case "FLOAT":
                    vs = new FloatSegment();
                    vs.Value = 0.0;
                    break;
                default:
                    throw new ArgumentException();
            }
            vs.TimeStart = starttime;
            vs.TimeEnd = starttime;
            values.Add(vs);
        }

        public void Add(ValueSegment vs)
        {
            values.Add(vs);
        }

        public void Remove(ValueSegment vs)
        {
            values.Remove(vs);
        }
        
        public void SortByTime()
        {
            values.Sort(new ValueSegmentTimeComparer());
        }

        public SimulateDataModel Clone()
        {
            SimulateDataModel sdmodel = new SimulateDataModel();
            sdmodel.SVUnit = SVUnit;
            foreach (ValueSegment vs in Values)
            {
                sdmodel.Add(vs.Clone());
            }
            return sdmodel;
        }

        public override string ToString()
        {
            string text = "";
            foreach (ValueSegment vs in Values)
            {
                text += vs.ToString();
            }
            return text;
        }
        
        public static SimulateDataModel Create(string name)
        {
            SimulateVariableUnit svunit = SimulateVariableUnit.Create(name);
            if (svunit == null) return null;
            return new SimulateDataModel(svunit);
        }


        #region Set & Update

        private int CurrentIndex { get; set; }

        private ValueSegment CurrentSegment
        {
            get
            {
                if (CurrentIndex < 0)
                    return null;
                if (CurrentIndex >= values.Count())
                    return null;
                return values[CurrentIndex];
            }
        }

        public void Move(int time)
        {
            CurrentIndex = Math.Max(CurrentIndex, 0);
            while (CurrentSegment != null
                && CurrentSegment.TimeEnd < time)
            {
                CurrentIndex++;
            }
            CurrentIndex = Math.Min(CurrentIndex, values.Count() - 1);
            while (CurrentSegment != null
                && CurrentSegment.TimeStart > time)
            {
                CurrentIndex--;
            }
        }

        public void Set(SimulateDllModel dllmodel, int time)
        {
            Move(time);
            if (CurrentSegment != null
             && CurrentSegment.TimeStart <= time
             && CurrentSegment.TimeEnd >= time)
            {
                SVUnit.Value = CurrentSegment.Value;
                SVUnit.Set(dllmodel);
            }
        }

        public void Update(SimulateDllModel dllmodel, int time)
        {
            SVUnit.Update(dllmodel);
            Move(time);
            ValueSegment nseg = null;
            if (CurrentSegment != null)
            {
                nseg = CurrentSegment;
                if (time > nseg.TimeEnd)
                {
                    if (!SVUnit.Value.Equals(nseg.Value))
                    {
                        nseg = nseg.Clone();
                        nseg.Value = SVUnit.Value;
                        nseg.TimeStart = nseg.TimeEnd + 1;
                        nseg.TimeEnd = time;
                        values.Insert(CurrentIndex + 1, nseg);
                    }
                    else
                    {
                        nseg.TimeEnd = time;
                    }
                }
                else if (time >= nseg.TimeStart && time <= nseg.TimeEnd)
                {
                    if (!SVUnit.Value.Equals(nseg.Value))
                    {
                        values.Remove(nseg);
                        ValueSegment lseg = nseg.Clone();
                        ValueSegment rseg = nseg.Clone();
                        nseg = nseg.Clone();
                        lseg.TimeEnd = time - 1;
                        rseg.TimeStart = time + 1;
                        nseg.TimeStart = nseg.TimeEnd = time;
                        nseg.Value = SVUnit.Value;
                        if (rseg.TimeEnd >= rseg.TimeStart)
                            values.Insert(CurrentIndex, rseg);
                        values.Insert(CurrentIndex, nseg);
                        if (lseg.TimeEnd >= lseg.TimeStart)
                            values.Insert(CurrentIndex, lseg);
                    }
                }
                else
                {
                    throw new ArgumentException(String.Format("Cannot get value segment related to time {0:d}", time));
                }
            }
            else if (values.Count() == 0)
            {
                Init(time);
                CurrentIndex = 0;
                CurrentSegment.Value = SVUnit.Value;
            }
            else if (CurrentIndex < 0)
            {
                nseg = values.First().Clone();
                nseg.Value = SVUnit.Value;
                nseg.TimeStart = nseg.TimeEnd = time;
                values.Insert(0, nseg);
            }
            else
            {
                throw new ArgumentException(String.Format("Cannot get value segment related to time {0:d}", time));
            }
        }

        #endregion
        
        #region XML
        public void SaveXml(XElement node_SDModel, int timestart, int timeend)
        {
            node_SDModel.SetAttributeValue("Name", Name);
            node_SDModel.SetAttributeValue("Type", Type);
            node_SDModel.SetAttributeValue("Var", Var);
            foreach (ValueSegment vs in Values)
            {
                int vtimestart = vs.TimeStart;
                int vtimeend = vs.TimeEnd;
                if (vtimestart < timestart)
                    vtimestart = timestart;
                if (vtimeend > timeend)
                    vtimeend = timeend;
                if (vtimestart > vtimeend)
                    continue;
                XElement node_VS = new XElement("ValueSegment");
                node_VS.SetAttributeValue("TimeStart", vtimestart);
                node_VS.SetAttributeValue("TimeEnd", vtimeend);
                node_VS.SetAttributeValue("Value", vs.Value);
                node_SDModel.Add(node_VS);
            }
            node_SDModel.SetAttributeValue("IsLock", IsLock);
            node_SDModel.SetAttributeValue("IsView", IsView);
        }

        public void SaveXml(XElement node_SDModel)
        {
            node_SDModel.SetAttributeValue("Name", Name);
            node_SDModel.SetAttributeValue("Type", Type);
            node_SDModel.SetAttributeValue("Var", Var);
            foreach (ValueSegment vs in Values)
            {
                XElement node_VS = new XElement("ValueSegment");
                node_VS.SetAttributeValue("TimeStart", vs.TimeStart);
                node_VS.SetAttributeValue("TimeEnd", vs.TimeEnd);
                node_VS.SetAttributeValue("Value", vs.Value);
                node_SDModel.Add(node_VS);
            }
            node_SDModel.SetAttributeValue("IsLock", IsLock);
            node_SDModel.SetAttributeValue("IsView", IsView);
        }

        public void LoadXml(XElement node_SDModel, int timestart, int timeend)
        {
            string name = (string)(node_SDModel.Attribute("Name"));
            string type = (string)(node_SDModel.Attribute("Type"));
            string var = (string)(node_SDModel.Attribute("var"));
            SVUnit = SimulateVariableUnit.Create(name, type);
            SVUnit.Var = var;
            IEnumerable<XElement> node_VSs = node_SDModel.Elements("ValueSegment");
            //values = new List<ValueSegment>();
            foreach (XElement node_VS in node_VSs)
            {
                ValueSegment vs;
                switch (Type)
                {
                    case "BIT": 
                        vs = new BitSegment();
                        break;
                    case "WORD":
                        vs = new WordSegment();
                        break;
                    case "DWORD":
                        vs = new DWordSegment();
                        break;
                    case "FLOAT":
                        vs = new FloatSegment();
                        break;
                    default:
                        throw new FormatException();
                }
                vs.Value = node_VS.Attribute("Value");
                vs.TimeStart = (int)(node_VS.Attribute("TimeStart"));
                vs.TimeEnd = (int)(node_VS.Attribute("TimeEnd"));
                values.Add(vs);
            }
            IsLock = (bool)(node_SDModel.Attribute("IsLock"));
            IsView = (bool)(node_SDModel.Attribute("IsView"));
        }

        public void LoadXml(XElement node_SDModel)
        {
            string name = (string)(node_SDModel.Attribute("Name"));
            string type = (string)(node_SDModel.Attribute("Type"));
            string var = (string)(node_SDModel.Attribute("var"));
            SVUnit = SimulateVariableUnit.Create(name, type);
            SVUnit.Var = var;
            IEnumerable<XElement> node_VSs = node_SDModel.Elements("ValueSegment");
            values = new List<ValueSegment>();
            foreach (XElement node_VS in node_VSs)
            {
                ValueSegment vs;
                switch (Type)
                {
                    case "BIT":
                        vs = new BitSegment();
                        vs.Value = (Int32)node_VS.Attribute("Value");
                        break;
                    case "WORD":
                        vs = new WordSegment();
                        vs.Value = (Int32)node_VS.Attribute("Value");
                        break;
                    case "DWORD":
                        vs = new DWordSegment();
                        vs.Value = (Int64)node_VS.Attribute("Value");
                        break;
                    case "FLOAT":
                        vs = new FloatSegment();
                        vs.Value = (double)node_VS.Attribute("Value");
                        break;
                    default:
                        throw new FormatException();
                }
                vs.TimeStart = (int)(node_VS.Attribute("TimeStart"));
                vs.TimeEnd = (int)(node_VS.Attribute("TimeEnd"));
                values.Add(vs);
            }
            IsLock = (bool)(node_SDModel.Attribute("IsLock"));
            IsView = (bool)(node_SDModel.Attribute("IsView"));
        }

        #endregion
    }
}
