using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.Simulation.Core.VariableModel
{
    public class SimulateUnitSeries : SimulateVariableUnit
    {

        public SimulateUnitSeries()
        {
        }

        public SimulateUnitSeries(SimulateVariableModel _svmodel)
        {
            this.model = _svmodel;
        }

        public override string Type
        {
            get
            {
                return "Series";
            }
        }

        public string DataType
        {
            get
            {
                if (model == null)
                    return "null";
                if (model is SimulateBitModel)
                    return "BIT";
                if (model is SimulateWordModel)
                    return "WORD";
                if (model is SimulateDWordModel)
                    return "DWORD";
                if (model is SimulateFloatModel)
                    return "FLOAT";
                return String.Empty;
            }
        }

        private SimulateVariableModel model;

        public SimulateVariableModel Model
        {
            get { return this.model; }
        }
        
        public string Base
        {
            get { return model.Base; }
        }

        public int Offset
        {
            get { return model.Offset; }
        }
        
        public override object Value
        {
            get
            { 
                if (model != null)
                {
                    return model.Values;
                }
                else
                {
                    return new SimulateVariableUnit[0];
                }
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        protected override bool _Check_Name(string _name)
        {
            Match m1 = Regex.Match(_name, @"^\w+\[\d+\.\.\d+\]$");
            if (!m1.Success) return false;
            return true;
        }

        public override event RoutedEventHandler ValueChanged = delegate { };

        public override void Set(SimulateDllModel dllmodel)
        {
            model.Set(dllmodel);
        }
        
        public override void Update(SimulateDllModel dllmodel)
        {
            model.Update(dllmodel);
        }

        public bool IsExpand { get; set; }

        public void CreateExpand()
        {
            Match mn = Regex.Match(Name, @"\w+");
            Match mi = Regex.Match(Name, @"\d+");
            string basename = mn.Value;
            int low = int.Parse(mi.Value);
            mi = mi.NextMatch();
            int high = int.Parse(mi.Value);
            string _name = String.Format("{0:s}{1:d}", basename, low);
            int _size = high - low + 1;
            model = SimulateVariableModel.Create(_name, _size);
        }

        public SimulateUnitSeries ChangeDataType(string _datatype)
        {
            int factor = 1;
            switch (model.Base)
            {
                case "D":
                    if (model is SimulateDWordModel)
                        factor = 2;
                    if (model is SimulateFloatModel)
                        factor = 2;
                    break;
                default:
                    factor = 1;
                    break;
            }
            SimulateVariableModel svmodel = SimulateVariableModel.Create(model.Name, model.Size*factor, _datatype);
            if (svmodel == null) return null;
            SimulateUnitSeries ret = new SimulateUnitSeries(svmodel);
            ret.Name = Name;
            ret.Var = Var;
            return ret;
        }

    }
}
