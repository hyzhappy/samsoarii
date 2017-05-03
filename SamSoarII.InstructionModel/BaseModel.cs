using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public abstract class BaseModel
    {
        public string ImportVaribleName { get; set; }
        public string ExportVaribleName { get; set; }    
        protected List<string> VaribleNameList = new List<string>();
        public int TotalVaribleCount { get; protected set; }
        public void ResetVaribleName()
        {
            VaribleNameList.Clear();
        }
        public void AddVaribleName(string name)
        {
            VaribleNameList.Add(name);
        }
        public abstract string GenerateCode();
        
        public abstract int ParaCount { get; }
        public abstract IValueModel GetPara(int id);
        public abstract void SetPara(int id, IValueModel value);
    }
}
