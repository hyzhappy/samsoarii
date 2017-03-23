using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                if (model is SimulateDoubleModel)
                    return "DOUBLE";
                return String.Empty;
            }
        }

        private SimulateVariableModel model;

        public override object Value
        {
            get
            { 
                if (IsExpand && model != null)
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
            Match m = Regex.Match(_name, @"^\w+\[\d+\.\.\d+\]$");
            if (!m.Success)
            {
                return false;
            }
            return true;
        }

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
                    if (model is SimulateDoubleModel)
                        factor = 4;
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
